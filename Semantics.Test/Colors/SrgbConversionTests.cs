// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Colors;

using System.Numerics;

using ktsu.Semantics.Color;

[TestClass]
public class SrgbConversionTests
{
	[TestMethod]
	public void SrgbToLinearToSrgb_RoundTripsToIdentity()
	{
		for (int i = 0; i <= 100; i++)
		{
			double channel = i / 100.0;
			Srgb original = new(channel, channel, channel);
			Srgb roundTripped = original.ToLinear().ToSrgb();
			Assert.AreEqual(original.R, roundTripped.R, 1e-9);
		}
	}

	[TestMethod]
	public void Srgb_MidGray_DecodesToSmallerLinearValue()
	{
		// sRGB 0.5 is perceptual mid-gray; its linear value is ~0.214 (proves gamma decode happens).
		Color c = Color.FromSrgb(0.5, 0.5, 0.5);
		Assert.AreEqual(0.21404, c.R, 1e-4);
	}

	[TestMethod]
	public void Srgb_EndpointsMapToLinearEndpoints()
	{
		Assert.AreEqual(0.0, Color.FromSrgb(0.0, 0.0, 0.0).R, 1e-12);
		Assert.AreEqual(1.0, Color.FromSrgb(1.0, 1.0, 1.0).R, 1e-12);
	}

	[TestMethod]
	public void ToSrgbVector4_ReturnsGammaEncodedChannels()
	{
		Color c = Color.FromSrgb(0.5, 0.5, 0.5, 1.0);
		Vector4 v = c.ToSrgbVector4();
		Assert.AreEqual(0.5f, v.X, 1e-4f);
		Assert.AreEqual(1.0f, v.W, 1e-6f);
	}
}
