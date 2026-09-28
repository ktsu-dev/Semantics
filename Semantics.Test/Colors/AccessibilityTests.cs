// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Colors;

using ktsu.Semantics.Color;

[TestClass]
public class AccessibilityTests
{
	[TestMethod]
	public void BlackOnWhite_HasMaximumContrast()
	{
		Color black = Color.FromSrgb(0.0, 0.0, 0.0);
		Color white = Color.FromSrgb(1.0, 1.0, 1.0);
		Assert.AreEqual(21.0, black.ContrastRatio(white), 1e-2);
	}

	[TestMethod]
	public void SameColor_HasUnitContrast()
	{
		Color gray = Color.FromSrgb(0.5, 0.5, 0.5);
		Assert.AreEqual(1.0, gray.ContrastRatio(gray), 1e-9);
	}

	[TestMethod]
	public void BlackOnWhite_RatesAaa()
	{
		Color black = Color.FromSrgb(0.0, 0.0, 0.0);
		Color white = Color.FromSrgb(1.0, 1.0, 1.0);
		Assert.AreEqual(AccessibilityLevel.AAA, black.AccessibilityLevelAgainst(white));
	}

	[TestMethod]
	public void AdjustForContrast_ReachesRequestedLevel()
	{
		Color background = Color.FromSrgb(1.0, 1.0, 1.0);
		Color faint = Color.FromSrgb(0.85, 0.85, 0.2);
		Color adjusted = faint.AdjustForContrast(background, AccessibilityLevel.AA);
		Assert.IsTrue(
			adjusted.AccessibilityLevelAgainst(background) >= AccessibilityLevel.AA,
			$"contrast was {adjusted.ContrastRatio(background)}");
	}

	[TestMethod]
	public void AdjustForContrast_DarkensOnMidToneBackground()
	{
		// Luminance ~0.296: lightening cannot reach AA here, darkening can.
		Color background = Color.FromSrgb(0.58, 0.58, 0.58);
		Color faint = Color.FromSrgb(0.55, 0.55, 0.55);
		Color adjusted = faint.AdjustForContrast(background, AccessibilityLevel.AA);
		Assert.IsTrue(
			adjusted.AccessibilityLevelAgainst(background) >= AccessibilityLevel.AA,
			$"contrast was {adjusted.ContrastRatio(background)}");
		Assert.IsTrue(adjusted.RelativeLuminance < background.RelativeLuminance);
	}

	[TestMethod]
	public void AdjustForContrast_ReachesLargeTextAAOnUpperMidToneBackground()
	{
		// Luminance ~0.45: white tops out below 3:1, black clears it comfortably.
		Color background = Color.FromSrgb(0.7, 0.7, 0.7);
		Color faint = Color.FromSrgb(0.75, 0.75, 0.75);
		Color adjusted = faint.AdjustForContrast(background, AccessibilityLevel.AA, largeText: true);
		Assert.IsTrue(
			adjusted.AccessibilityLevelAgainst(background, largeText: true) >= AccessibilityLevel.AA,
			$"contrast was {adjusted.ContrastRatio(background)}");
	}

	[TestMethod]
	public void AdjustForContrast_LightensOnDarkBackground()
	{
		Color background = Color.FromSrgb(0.1, 0.1, 0.1);
		Color faint = Color.FromSrgb(0.2, 0.2, 0.2);
		Color adjusted = faint.AdjustForContrast(background, AccessibilityLevel.AA);
		Assert.IsTrue(
			adjusted.AccessibilityLevelAgainst(background) >= AccessibilityLevel.AA,
			$"contrast was {adjusted.ContrastRatio(background)}");
		Assert.IsTrue(adjusted.RelativeLuminance > background.RelativeLuminance);
	}
}
