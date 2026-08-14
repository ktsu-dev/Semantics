// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Colors;

using ktsu.Semantics.Color;

[TestClass]
public class ColorAdjustmentTests
{
	private static readonly Color[] Samples =
	[
		Color.FromSrgb(1.0, 0.0, 0.0),
		Color.FromSrgb(0.2, 0.6, 0.9),
		Color.FromSrgb(0.7, 0.3, 0.55, 0.4),
		Color.FromSrgb(0.5, 0.5, 0.5),
	];

	// --- Hsl native ops (source of truth) ---

	[TestMethod]
	public void Hsl_SaturationOps_SetAddSubtractMultiply_Clamped()
	{
		Hsl h = new(30.0, 0.6, 0.5);
		Assert.AreEqual(0.2, h.WithSaturation(0.2).S, 1e-12);
		Assert.AreEqual(1.0, h.SaturateBy(1.0).S, 1e-12);
		Assert.AreEqual(0.5, h.DesaturateBy(0.1).S, 1e-12);
		Assert.AreEqual(0.0, h.DesaturateBy(5.0).S, 1e-12);
		Assert.AreEqual(0.3, h.MultiplySaturation(0.5).S, 1e-12);
		Assert.AreEqual(0.0, h.ToGrayscale().S, 1e-12);
	}

	[TestMethod]
	public void Hsl_LightnessOps_Clamped()
	{
		Hsl h = new(30.0, 0.6, 0.5);
		Assert.AreEqual(0.3, h.WithLightness(0.3).L, 1e-12);
		Assert.AreEqual(0.7, h.LightenBy(0.2).L, 1e-12);
		Assert.AreEqual(0.0, h.DarkenBy(1.0).L, 1e-12);
		Assert.AreEqual(0.25, h.MultiplyLightness(0.5).L, 1e-12);
	}

	[TestMethod]
	public void Hsl_OffsetHue_WrapsInto0To360()
	{
		Assert.AreEqual(15.0, new Hsl(30.0, 0.6, 0.5).OffsetHue(345.0).H, 1e-9);
		Assert.AreEqual(350.0, new Hsl(10.0, 0.6, 0.5).OffsetHue(-20.0).H, 1e-9);
	}

	// --- Oklch perceptual ops ---

	[TestMethod]
	public void Oklch_LightnessOps_ClampedTo01_PreserveHue()
	{
		Oklch o = Color.FromSrgb(0.2, 0.6, 0.9).ToOklch();
		Assert.IsGreaterThan(o.L, o.LightenBy(0.1).L);
		Assert.AreEqual(o.H, o.LightenBy(0.1).H, 1e-12);
		Assert.AreEqual(0.0, o.DarkenBy(5.0).L, 1e-12);
		Assert.AreEqual(1.0, o.WithLightness(2.0).L, 1e-12);
	}

	[TestMethod]
	public void Oklch_ChromaOps_FlooredAtZero()
	{
		Oklch o = Color.FromSrgb(0.8, 0.2, 0.2).ToOklch();
		Assert.AreEqual(o.C * 0.5, o.MultiplyChroma(0.5).C, 1e-12);
		Assert.AreEqual(0.0, o.DesaturateBy(999.0).C, 1e-12);
		Assert.AreEqual(0.0, o.ToGrayscale().C, 1e-12);
		Assert.AreEqual(0.0, o.WithChroma(-1.0).C, 1e-12);
		Assert.IsGreaterThan(o.C, o.SaturateBy(0.05).C);
	}

	// --- Srgb: native invert + HSL round-trip cylindrical ops ---

	[TestMethod]
	public void Srgb_Invert_IsPerChannelComplementAndSelfInverse()
	{
		Srgb s = new(0.8, 0.2, 0.1);
		Srgb inv = s.Invert();
		Assert.AreEqual(0.2, inv.R, 1e-12);
		Assert.AreEqual(0.8, inv.G, 1e-12);
		Assert.AreEqual(0.9, inv.B, 1e-12);
		Assert.AreEqual(s.R, s.Invert().Invert().R, 1e-12);
	}

	[TestMethod]
	public void Srgb_CylindricalOps_MatchExplicitHslRoundTrip()
	{
		Srgb s = new(0.8, 0.2, 0.1);
		Srgb viaOp = s.LightenBy(0.1);
		Srgb viaHsl = s.ToHsl().LightenBy(0.1).ToSrgb();
		Assert.AreEqual(viaHsl.R, viaOp.R, 1e-12);
		Assert.AreEqual(viaHsl.G, viaOp.G, 1e-12);
		Assert.AreEqual(viaHsl.B, viaOp.B, 1e-12);
	}

	[TestMethod]
	public void Srgb_ToGrayscale_HasEqualChannels()
	{
		Srgb g = new Srgb(0.8, 0.2, 0.1).ToGrayscale();
		Assert.AreEqual(g.R, g.G, 1e-9);
		Assert.AreEqual(g.G, g.B, 1e-9);
	}

	// --- Color convenience: forwards to Hsl, preserves alpha ---

	[TestMethod]
	public void Color_CylindricalOps_ForwardToHsl()
	{
		Color c = Color.FromSrgb(0.7, 0.3, 0.55, 0.4);
		Assert.AreEqual(c.ToHsl().LightenBy(0.1).L, c.LightenBy(0.1).ToHsl().L, 1e-9);
		Assert.AreEqual(c.ToHsl().WithSaturation(0.2).S, c.WithSaturation(0.2).ToHsl().S, 1e-9);
	}

	[TestMethod]
	public void Color_Ops_PreserveAlpha()
	{
		foreach (Color c in Samples)
		{
			Assert.AreEqual(c.A, c.LightenBy(0.1).A, 1e-12);
			Assert.AreEqual(c.A, c.WithSaturation(0.3).A, 1e-12);
			Assert.AreEqual(c.A, c.Invert().A, 1e-12);
			Assert.AreEqual(c.A, c.ToGrayscale().A, 1e-12);
		}
	}

	[TestMethod]
	public void Color_Invert_MatchesSrgbInvertRoundTrip()
	{
		Color c = Color.FromSrgb(0.7, 0.3, 0.55, 0.4);
		Color expected = Color.FromSrgb(c.ToSrgb().Invert(), c.A);
		Assert.AreEqual(expected.R, c.Invert().R, 1e-12);
		Assert.AreEqual(expected.G, c.Invert().G, 1e-12);
		Assert.AreEqual(expected.B, c.Invert().B, 1e-12);
	}

	[TestMethod]
	public void Color_ToGrayscale_RemovesSaturationKeepsLightness()
	{
		foreach (Color c in Samples)
		{
			Color gray = c.ToGrayscale();
			Assert.AreEqual(0.0, gray.ToHsl().S, 1e-9);
			Assert.AreEqual(c.ToHsl().L, gray.ToHsl().L, 1e-9);
		}
	}

	[TestMethod]
	public void Color_OffsetHue_FullTurnIsIdentity()
	{
		Color c = Color.FromSrgb(0.2, 0.6, 0.9);
		Assert.AreEqual(c.ToHsl().H, c.OffsetHue(360.0).ToHsl().H, 1e-6);
	}
}
