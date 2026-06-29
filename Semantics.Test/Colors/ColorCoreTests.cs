// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Colors;

using System.Numerics;

using ktsu.Semantics.Color;

[TestClass]
public class ColorCoreTests
{
	[TestMethod]
	public void FromLinear_StoresChannelsAndDefaultsAlphaToOne()
	{
		Color c = Color.FromLinear(0.1, 0.2, 0.3);
		Assert.AreEqual(0.1, c.R, 1e-12);
		Assert.AreEqual(0.2, c.G, 1e-12);
		Assert.AreEqual(0.3, c.B, 1e-12);
		Assert.AreEqual(1.0, c.A, 1e-12);
	}

	[TestMethod]
	public void WithAlpha_ReplacesAlphaOnly()
	{
		Color c = Color.FromLinear(0.1, 0.2, 0.3, 1.0).WithAlpha(0.5);
		Assert.AreEqual(0.1, c.R, 1e-12);
		Assert.AreEqual(0.5, c.A, 1e-12);
	}

	[TestMethod]
	public void Clamp_BringsChannelsIntoUnitRange()
	{
		Color c = Color.FromLinear(-0.5, 0.5, 1.5, 2.0).Clamp();
		Assert.AreEqual(0.0, c.R, 1e-12);
		Assert.AreEqual(0.5, c.G, 1e-12);
		Assert.AreEqual(1.0, c.B, 1e-12);
		Assert.AreEqual(1.0, c.A, 1e-12);
	}

	[TestMethod]
	public void ToLinearVector4_ReturnsFloatChannels()
	{
		Vector4 v = Color.FromLinear(0.1, 0.2, 0.3, 0.4).ToLinearVector4();
		Assert.AreEqual(0.1f, v.X, 1e-6f);
		Assert.AreEqual(0.4f, v.W, 1e-6f);
	}
}
