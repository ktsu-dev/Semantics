// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Colors;

using System.Collections.Generic;

using ktsu.Semantics.Color;

/// <summary>
/// Tests for named colors matching CSS (#449), contrast seeing alpha (#393), contrast adjustment reaching the
/// target for saturated colors (#351), and Oklab results staying in gamut for later HSL work (#355).
/// </summary>
[TestClass]
public class ColorGamutAndAlphaTests
{
	private static readonly Color White = NamedColors.White;

	// CSS Color Module Level 4, section 6.1, for every name the table holds.
	private static readonly Dictionary<string, string> CssValues = new()
	{
		["black"] = "#000000",
		["white"] = "#FFFFFF",
		["red"] = "#FF0000",
		["green"] = "#008000",
		["lime"] = "#00FF00",
		["blue"] = "#0000FF",
		["yellow"] = "#FFFF00",
		["cyan"] = "#00FFFF",
		["magenta"] = "#FF00FF",
		["gray"] = "#808080",
		["grey"] = "#808080",
		["orange"] = "#FFA500",
		["purple"] = "#800080",
	};

	[TestMethod]
	public void NamedColors_MatchCss()
	{
		foreach (KeyValuePair<string, string> entry in CssValues)
		{
			Assert.IsTrue(NamedColors.TryGet(entry.Key, out Color color), entry.Key);
			Assert.AreEqual(entry.Value, color.ToHex(), entry.Key);
		}

		Assert.AreEqual("#008000", NamedColors.Green.ToHex());
		Assert.AreEqual("#00FF00", NamedColors.Lime.ToHex());
	}

	[TestMethod]
	public void NamedColors_EveryOpaqueEntryIsPinned()
	{
		foreach (string name in NamedColors.All.Keys)
		{
			Assert.IsTrue(name == "transparent" || CssValues.ContainsKey(name), $"'{name}' has no CSS value in this test.");
		}
	}

	[TestMethod]
	public void ContrastRatio_OfTransparentIsOne()
	{
		Assert.AreEqual(1.0, NamedColors.Transparent.ContrastRatio(White), 1e-9);
		Assert.AreEqual(AccessibilityLevel.Fail, NamedColors.Transparent.AccessibilityLevelAgainst(White));
	}

	[TestMethod]
	public void ContrastRatio_OfAFaintColorIsMeasuredAsBlended()
	{
		Color faint = Color.FromLinear(0, 0, 0, 0.05);

		Assert.AreEqual(AccessibilityLevel.Fail, faint.AccessibilityLevelAgainst(White));
		Assert.IsLessThan(1.2, faint.ContrastRatio(White));
	}

	[TestMethod]
	public void ContrastRatio_OfOpaqueColorsIsUnchanged()
	{
		Assert.AreEqual(21.0, NamedColors.Black.ContrastRatio(White), 1e-9);
		Assert.AreEqual(21.0, White.ContrastRatio(NamedColors.Black), 1e-9);
		Assert.AreEqual(NamedColors.Gray.ContrastRatio(White), White.ContrastRatio(NamedColors.Gray), 1e-12);
	}

	[TestMethod]
	public void CompositeOver_BlendsInSrgbLikeABrowser()
	{
		Color halfBlack = Color.FromLinear(0, 0, 0, 0.5);
		Color blended = halfBlack.CompositeOver(White);

		// 50% black over white is sRGB 0.5 in every channel, #808080 rounded as browsers render it.
		Assert.AreEqual(0.5, blended.ToSrgb().R, 1e-9);
		Assert.AreEqual(1.0, blended.A, 1e-12);
		Assert.AreEqual(White, NamedColors.Transparent.CompositeOver(White));
	}

	[TestMethod]
	public void AdjustForContrast_FixesATranslucentColorWhenItCan()
	{
		Color halfGray = Color.FromHex("#888888").WithAlpha(0.6);
		Color adjusted = halfGray.AdjustForContrast(White, AccessibilityLevel.AA);

		Assert.AreEqual(0.6, adjusted.A, 1e-12);
		Assert.IsGreaterThanOrEqualTo(4.5, adjusted.ContrastRatio(White));
	}

	[TestMethod]
	public void AdjustForContrast_ReturnsATooFaintColorUnchanged()
	{
		Color faint = Color.FromLinear(0, 0, 0, 0.05);
		Assert.AreEqual(faint, faint.AdjustForContrast(White, AccessibilityLevel.AAA));
	}

	[TestMethod]
	[DataRow("#0000FF", "#777777", AccessibilityLevel.AA)]
	[DataRow("#8000FF", "#777777", AccessibilityLevel.AA)]
	[DataRow("#0000AA", "#777777", AccessibilityLevel.AA)]
	[DataRow("#FF0000", "#0000FF", AccessibilityLevel.AAA)]
	[DataRow("#FF00FF", "#0000FF", AccessibilityLevel.AAA)]
	public void AdjustForContrast_ReachesTheTargetForSaturatedColors(string foreground, string background, AccessibilityLevel level)
	{
		Color fg = Color.FromHex(foreground);
		Color bg = Color.FromHex(background);
		double required = level == AccessibilityLevel.AAA ? 7.0 : 4.5;

		Color adjusted = fg.AdjustForContrast(bg, level);

		Assert.IsGreaterThanOrEqualTo(required, adjusted.ContrastRatio(bg), $"{foreground} on {background} gave {adjusted.ToHex()}");
		AssertInGamut(adjusted);
	}

	[TestMethod]
	public void AdjustForContrast_KeepsTheHueOfABlueItDarkens()
	{
		Color adjusted = Color.FromHex("#0000FF").AdjustForContrast(Color.FromHex("#777777"), AccessibilityLevel.AA);
		(byte r, byte g, byte b, byte _) = adjusted.ToBytes();

		Assert.IsGreaterThan(r, b, adjusted.ToHex());
		Assert.IsGreaterThan(g, b, adjusted.ToHex());
	}

	[TestMethod]
	public void HslAdjustments_AfterAnOklabRoundTripOfWhite()
	{
		Color roundTripped = Color.FromOklab(White.ToOklab());

		Assert.AreEqual(0.0, roundTripped.ToHsl().S, 1e-9);
		Assert.AreEqual("#7F7F7F", roundTripped.DarkenBy(0.5).ToHex());
		Assert.AreEqual("#7F7F7F", White.MixOklab(White, 0.5).DarkenBy(0.5).ToHex());
	}

	[TestMethod]
	public void LightenBy_OfAnOutOfGamutOklchGreenIsNotBlack()
	{
		Color lightened = Color.FromOklch(new Oklch(0.7, 0.3, 150)).LightenBy(0.1);
		Assert.AreNotEqual("#000000", lightened.ToHex());
	}

	[TestMethod]
	public void MixOklabAndGradient_StayInGamut()
	{
		Color[] primaries =
		[
			NamedColors.Black, NamedColors.White, NamedColors.Red, NamedColors.Lime,
			NamedColors.Blue, NamedColors.Yellow, NamedColors.Cyan, NamedColors.Magenta,
		];

		foreach (Color a in primaries)
		{
			foreach (Color b in primaries)
			{
				Color mixed = a.MixOklab(b, 0.5);
				AssertInGamut(mixed);
				Hsl hsl = mixed.ToHsl();
				Assert.IsTrue(hsl.S is >= 0.0 and <= 1.0 && hsl.L is >= 0.0 and <= 1.0, $"{a.ToHex()} + {b.ToHex()} gave {hsl}");

				foreach (Color stop in a.Gradient(b, 5))
				{
					AssertInGamut(stop);
				}
			}
		}
	}

	[TestMethod]
	public void CyanMixedWithBlue_DesaturatesRatherThanDarkens()
	{
		Color mixed = NamedColors.Cyan.MixOklab(NamedColors.Blue, 0.5);
		Assert.AreNotEqual("#292929", mixed.ToGrayscale().ToHex());
		double saturation = mixed.ToHsl().S;
		Assert.AreEqual(System.Math.Max(0.0, saturation - 0.3), mixed.DesaturateBy(0.3).ToHsl().S, 0.02);
	}

	[TestMethod]
	public void HslAndHsv_OfOutOfGamutInputAreInRange()
	{
		Srgb over = new(1.0000001, 0.99999998, 0.9999997);
		Assert.AreEqual(0.0, Hsl.FromSrgb(over).S, 1e-6);

		Hsv hsv = Hsv.FromSrgb(new Srgb(-0.2, 0.5, 1.3));
		Assert.IsTrue(hsv.S is >= 0.0 and <= 1.0 && hsv.V is >= 0.0 and <= 1.0, hsv.ToString());
	}

	[TestMethod]
	public void MixOklabAndLerp_FromTransparent_KeepTheOtherColorsHue()
	{
		Color transparent = NamedColors.Transparent;
		Color[] opaques = [White, NamedColors.Red, NamedColors.Blue, Color.FromHex("#336699")];
		double[] factors = [0.25, 0.5, 0.75];
		foreach (Color opaque in opaques)
		{
			foreach (double t in factors)
			{
				Color expected = opaque.WithAlpha(t);
				Assert.AreEqual(expected.ToHex(), transparent.MixOklab(opaque, t).ToHex(), $"MixOklab to {opaque.ToHex()} at {t}");
				Assert.AreEqual(expected.ToHex(), transparent.Lerp(opaque, t).ToHex(), $"Lerp to {opaque.ToHex()} at {t}");
				Assert.AreEqual(expected.ToHex(), opaque.MixOklab(transparent, 1.0 - t).ToHex(), $"MixOklab from {opaque.ToHex()} at {t}");
			}
		}
	}

	[TestMethod]
	public void Gradient_FromTransparent_FadesInWithoutDarkening()
	{
		IReadOnlyList<Color> stops = NamedColors.Transparent.Gradient(White, 5);
		string[] expected = ["#00000000", "#FFFFFF40", "#FFFFFF80", "#FFFFFFBF", "#FFFFFF"];
		string[] actual = [stops[0].ToHex(), stops[1].ToHex(), stops[2].ToHex(), stops[3].ToHex(), stops[4].ToHex()];
		CollectionAssert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void MixOklab_InvisibleEnd_ContributesNoColor()
	{
		Color mixed = NamedColors.Red.WithAlpha(0.0).MixOklab(NamedColors.Blue, 0.5);
		Assert.AreEqual(NamedColors.Blue.WithAlpha(0.5).ToHex(), mixed.ToHex());
	}

	[TestMethod]
	public void MixOklabAndLerp_BothTransparent_AreTransparentBlack()
	{
		Color clearRed = NamedColors.Red.WithAlpha(0.0);
		Color clearBlue = NamedColors.Blue.WithAlpha(0.0);
		Assert.AreEqual("#00000000", clearRed.MixOklab(clearBlue, 0.5).ToHex());
		Assert.AreEqual("#00000000", clearRed.Lerp(clearBlue, 0.5).ToHex());
	}

	[TestMethod]
	public void MixOklabAndLerp_OpaqueInputs_MatchStraightInterpolation()
	{
		Color a = Color.FromHex("#336699");
		Color b = Color.FromHex("#FFCC00");
		Oklab la = a.ToOklab();
		Oklab lb = b.ToOklab();
		Color straightMix = Color.FromOklab(new Oklab((la.L + lb.L) / 2, (la.A + lb.A) / 2, (la.B + lb.B) / 2));
		Color straightLerp = new((a.R + b.R) / 2, (a.G + b.G) / 2, (a.B + b.B) / 2, 1.0);
		Assert.AreEqual(straightMix.ToHex(), a.MixOklab(b, 0.5).ToHex());
		Assert.AreEqual(straightLerp.ToHex(), a.Lerp(b, 0.5).ToHex());
	}

	private static void AssertInGamut(Color color)
	{
		Assert.IsTrue(
			color.R is >= 0.0 and <= 1.0 && color.G is >= 0.0 and <= 1.0 && color.B is >= 0.0 and <= 1.0,
			$"{color} is out of gamut");
	}
}
