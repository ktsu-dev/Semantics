// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Colors;

using System;
using System.Collections.Generic;

using ktsu.Semantics.Color;

[TestClass]
public class ColorOperationTests
{
	[TestMethod]
	public void DistanceTo_Self_IsZero()
	{
		Color c = Color.FromSrgb(0.3, 0.6, 0.9);
		Assert.AreEqual(0.0, c.DistanceTo(c), 1e-12);
	}

	[TestMethod]
	public void DistanceTo_IsSymmetric()
	{
		Color a = Color.FromSrgb(0.1, 0.2, 0.3);
		Color b = Color.FromSrgb(0.9, 0.8, 0.7);
		Assert.AreEqual(a.DistanceTo(b), b.DistanceTo(a), 1e-12);
	}

	[TestMethod]
	public void Lerp_AtEndpoints_ReturnsEndpoints()
	{
		Color a = Color.FromLinear(0.0, 0.0, 0.0);
		Color b = Color.FromLinear(1.0, 1.0, 1.0);
		Assert.AreEqual(0.0, a.Lerp(b, 0.0).R, 1e-12);
		Assert.AreEqual(1.0, a.Lerp(b, 1.0).R, 1e-12);
		Assert.AreEqual(0.5, a.Lerp(b, 0.5).R, 1e-12);
	}

	[TestMethod]
	public void MixOklab_Halfway_IsBetweenEndpoints()
	{
		Color a = Color.FromSrgb(0.0, 0.0, 0.0);
		Color b = Color.FromSrgb(1.0, 1.0, 1.0);
		Color mid = a.MixOklab(b, 0.5);
		Assert.IsTrue(mid.R > a.R && mid.R < b.R, $"mid.R was {mid.R}");
	}

	[TestMethod]
	public void Gradient_ReturnsRequestedStepsWithMatchingEndpoints()
	{
		Color a = Color.FromSrgb(0.0, 0.0, 0.0);
		Color b = Color.FromSrgb(1.0, 1.0, 1.0);
		IReadOnlyList<Color> gradient = a.Gradient(b, 5);
		Assert.AreEqual(5, gradient.Count);
		Assert.AreEqual(a.R, gradient[0].R, 1e-9);
		Assert.AreEqual(b.R, gradient[4].R, 1e-9);
	}

	[TestMethod]
	public void Gradient_TooFewSteps_Throws() =>
		Assert.ThrowsExactly<ArgumentException>(() => Color.FromSrgb(0, 0, 0).Gradient(Color.FromSrgb(1, 1, 1), 1));
}
