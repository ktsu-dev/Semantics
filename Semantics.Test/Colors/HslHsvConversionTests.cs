// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Colors;

using ktsu.Semantics.Color;

[TestClass]
public class HslHsvConversionTests
{
	[TestMethod]
	public void Red_HasZeroHueFullSaturation()
	{
		Hsl hsl = Color.FromSrgb(1.0, 0.0, 0.0).ToHsl();
		Assert.AreEqual(0.0, hsl.H, 1e-6);
		Assert.AreEqual(1.0, hsl.S, 1e-6);
		Assert.AreEqual(0.5, hsl.L, 1e-6);
	}

	[TestMethod]
	public void Green_HasHue120()
	{
		Hsv hsv = Color.FromSrgb(0.0, 1.0, 0.0).ToHsv();
		Assert.AreEqual(120.0, hsv.H, 1e-4);
		Assert.AreEqual(1.0, hsv.S, 1e-6);
		Assert.AreEqual(1.0, hsv.V, 1e-6);
	}

	[TestMethod]
	public void HslRoundTrip_IsIdentity()
	{
		Color original = Color.FromSrgb(0.2, 0.6, 0.9);
		Color roundTripped = Color.FromHsl(original.ToHsl());
		Assert.AreEqual(original.R, roundTripped.R, 1e-9);
		Assert.AreEqual(original.G, roundTripped.G, 1e-9);
		Assert.AreEqual(original.B, roundTripped.B, 1e-9);
	}

	[TestMethod]
	public void HsvRoundTrip_IsIdentity()
	{
		Color original = Color.FromSrgb(0.7, 0.3, 0.55);
		Color roundTripped = Color.FromHsv(original.ToHsv());
		Assert.AreEqual(original.R, roundTripped.R, 1e-9);
		Assert.AreEqual(original.G, roundTripped.G, 1e-9);
		Assert.AreEqual(original.B, roundTripped.B, 1e-9);
	}
}
