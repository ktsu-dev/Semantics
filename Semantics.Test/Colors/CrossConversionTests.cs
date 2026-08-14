// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Colors;

using ktsu.Semantics.Color;

[TestClass]
public class CrossConversionTests
{
	// A spread of colors covering primaries, greys, and off-axis tints.
	private static readonly Color[] Samples =
	[
		Color.FromSrgb(1.0, 0.0, 0.0),
		Color.FromSrgb(0.0, 1.0, 0.0),
		Color.FromSrgb(0.0, 0.0, 1.0),
		Color.FromSrgb(0.2, 0.6, 0.9),
		Color.FromSrgb(0.7, 0.3, 0.55),
		Color.FromSrgb(0.5, 0.5, 0.5),
		Color.FromSrgb(0.05, 0.05, 0.05),
	];

	[TestMethod]
	public void SatelliteCrossConversions_MatchTheColorHubPath()
	{
		foreach (Color sample in Samples)
		{
			Srgb srgb = sample.ToSrgb();
			Hsl hsl = sample.ToHsl();
			Hsv hsv = sample.ToHsv();
			Oklab oklab = sample.ToOklab();
			Oklch oklch = sample.ToOklch();

			// Cross-family hops must agree with routing manually through Color. Comparing in the
			// target space (rather than round-tripping back) isolates the routing: both paths feed
			// identical values into the same math, so agreement is near-exact.
			AssertOklabEqual(srgb.ToOklab(), Oklab.FromColor(Color.FromSrgb(srgb)));
			AssertOklabEqual(hsl.ToOklab(), Oklab.FromColor(Color.FromHsl(hsl)));
			AssertOklabEqual(hsv.ToOklab(), Oklab.FromColor(Color.FromHsv(hsv)));

			// Oklch reaches the sRGB family through Oklab, which must match Color's own ToOklch.
			// (Comparing oklch.ToOklab() against a Color round-trip would instead measure Oklab
			// round-trip error, not routing, so we check a cross-family method here.)
			AssertOklchEqual(Oklch.FromSrgb(srgb), Color.FromSrgb(srgb).ToOklch());

			// The satellite hub pair agrees with the equivalent methods on Color.
			AssertColorsEqual(oklab.ToColor(), Color.FromOklab(oklab), 1e-12);
			AssertColorsEqual(oklch.ToColor(), Color.FromOklch(oklch), 1e-12);
		}
	}

	[TestMethod]
	public void SameFamilyConversions_DoNotRoundTripThroughLinear()
	{
		foreach (Color sample in Samples)
		{
			Srgb srgb = sample.ToSrgb();

			// sRGB -> HSL/HSV must be the direct definition, not a gamma decode/encode detour.
			AssertHslEqual(srgb.ToHsl(), Hsl.FromSrgb(srgb));
			AssertHsvEqual(srgb.ToHsv(), Hsv.FromSrgb(srgb));
			AssertSrgbEqual(Srgb.FromHsl(srgb.ToHsl()), srgb, 1e-12);
			AssertSrgbEqual(Srgb.FromHsv(srgb.ToHsv()), srgb, 1e-12);
		}
	}

	[TestMethod]
	public void EverySatelliteRoundTripsBackToColor()
	{
		foreach (Color sample in Samples)
		{
			AssertColorsEqual(Srgb.FromColor(sample).ToColor(), sample, 1e-9);
			AssertColorsEqual(Hsl.FromColor(sample).ToColor(), sample, 1e-9);
			AssertColorsEqual(Hsv.FromColor(sample).ToColor(), sample, 1e-9);
			AssertColorsEqual(Oklab.FromColor(sample).ToColor(), sample, 1e-6);
			AssertColorsEqual(Oklch.FromColor(sample).ToColor(), sample, 1e-6);
		}
	}

	[TestMethod]
	public void CrossFamilyChain_RoundTrips()
	{
		foreach (Color sample in Samples)
		{
			// Hsl -> Oklch -> Hsv -> Oklab -> Srgb, then straight back through Color.
			Oklch viaHsl = sample.ToHsl().ToOklch();
			Hsv viaOklch = viaHsl.ToHsv();
			Oklab viaHsv = viaOklch.ToOklab();
			Srgb viaOklab = viaHsv.ToSrgb();
			AssertColorsEqual(viaOklab.ToColor(), sample, 1e-6);
		}
	}

	private static void AssertColorsEqual(Color actual, Color expected, double tolerance)
	{
		Assert.AreEqual(expected.R, actual.R, tolerance);
		Assert.AreEqual(expected.G, actual.G, tolerance);
		Assert.AreEqual(expected.B, actual.B, tolerance);
	}

	private static void AssertSrgbEqual(Srgb actual, Srgb expected, double tolerance)
	{
		Assert.AreEqual(expected.R, actual.R, tolerance);
		Assert.AreEqual(expected.G, actual.G, tolerance);
		Assert.AreEqual(expected.B, actual.B, tolerance);
	}

	private static void AssertHslEqual(Hsl actual, Hsl expected)
	{
		Assert.AreEqual(expected.H, actual.H, 1e-12);
		Assert.AreEqual(expected.S, actual.S, 1e-12);
		Assert.AreEqual(expected.L, actual.L, 1e-12);
	}

	private static void AssertHsvEqual(Hsv actual, Hsv expected)
	{
		Assert.AreEqual(expected.H, actual.H, 1e-12);
		Assert.AreEqual(expected.S, actual.S, 1e-12);
		Assert.AreEqual(expected.V, actual.V, 1e-12);
	}

	private static void AssertOklabEqual(Oklab actual, Oklab expected)
	{
		Assert.AreEqual(expected.L, actual.L, 1e-12);
		Assert.AreEqual(expected.A, actual.A, 1e-12);
		Assert.AreEqual(expected.B, actual.B, 1e-12);
	}

	private static void AssertOklchEqual(Oklch actual, Oklch expected)
	{
		Assert.AreEqual(expected.L, actual.L, 1e-12);
		Assert.AreEqual(expected.C, actual.C, 1e-12);
		Assert.AreEqual(expected.H, actual.H, 1e-12);
	}
}
