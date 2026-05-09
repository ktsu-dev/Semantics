// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Quantities;

using System;
using ktsu.Semantics.Quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Verifies the Vector0 invariants locked in <c>docs/strategy-unified-vector-quantities.md</c>:
/// <list type="bullet">
/// <item><description>Issue #50: factories reject negative inputs with <see cref="ArgumentException"/>.</description></item>
/// <item><description>Issue #52: V0 - V0 returns the same V0 of <c>T.Abs(a - b)</c>.</description></item>
/// </list>
/// </summary>
[TestClass]
public sealed class Vector0InvariantTests
{
	private const double Tolerance = 1e-10;

	// =========================================================== #50: Non-negativity

	[TestMethod]
	public void Speed_FromMetersPerSecond_Negative_Throws()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Speed<double>.FromMetersPerSecond(-1.0));

	[TestMethod]
	public void Mass_FromKilogram_Negative_Throws()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Mass<double>.FromKilogram(-0.5));

	[TestMethod]
	public void Length_FromMeter_Negative_Throws()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Length<double>.FromMeter(-3.0));

	[TestMethod]
	public void Energy_FromJoule_Negative_Throws()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Energy<double>.FromJoule(-100.0));

	[TestMethod]
	public void Speed_FromMetersPerSecond_Zero_Allowed()
	{
		Speed<double> s = Speed<double>.FromMetersPerSecond(0.0);
		Assert.AreEqual(0.0, s.Value, Tolerance);
	}

	[TestMethod]
	public void Mass_FromKilogram_Positive_Returns_Same_Value()
	{
		Mass<double> m = Mass<double>.FromKilogram(2.5);
		Assert.AreEqual(2.5, m.Value, Tolerance);
	}

	// V0 overloads inherit non-negativity from their dimension.

	[TestMethod]
	public void Distance_FromMeter_Negative_Throws()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Distance<double>.FromMeter(-1.0));

	[TestMethod]
	public void Weight_FromNewton_Negative_Throws()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Weight<double>.FromNewton(-9.81));

	// V1 quantities are signed and accept any input.

	[TestMethod]
	public void Velocity1D_FromMetersPerSecond_Negative_Allowed()
	{
		Velocity1D<double> v = Velocity1D<double>.FromMetersPerSecond(-3.5);
		Assert.AreEqual(-3.5, v.Value, Tolerance);
	}

	[TestMethod]
	public void TemperatureDelta_FromKelvin_Negative_Allowed()
	{
		TemperatureDelta<double> dt = TemperatureDelta<double>.FromKelvin(-10.0);
		Assert.AreEqual(-10.0, dt.Value, Tolerance);
	}

	// Storage-type genericity for the guard.

	[TestMethod]
	public void Mass_FromKilogram_Negative_Throws_With_Float()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Mass<float>.FromKilogram(-1.0f));

	[TestMethod]
	public void Mass_FromKilogram_Negative_Throws_With_Decimal()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Mass<decimal>.FromKilogram(-1m));

	// =========================================================== #52: Absolute subtraction

	[TestMethod]
	public void Mass_Minus_Larger_Mass_Returns_Mass_Of_Absolute_Difference()
	{
		Mass<double> small = Mass<double>.FromKilogram(3.0);
		Mass<double> large = Mass<double>.FromKilogram(5.0);
		Mass<double> diff = small - large;
		Assert.AreEqual(2.0, diff.Value, Tolerance);
		Assert.IsInstanceOfType<Mass<double>>(diff);
	}

	[TestMethod]
	public void Mass_Minus_Smaller_Mass_Returns_Positive_Mass()
	{
		Mass<double> large = Mass<double>.FromKilogram(5.0);
		Mass<double> small = Mass<double>.FromKilogram(3.0);
		Mass<double> diff = large - small;
		Assert.AreEqual(2.0, diff.Value, Tolerance);
	}

	[TestMethod]
	public void Speed_Minus_Speed_Returns_Speed_Of_Absolute_Difference()
	{
		Speed<double> a = Speed<double>.FromMetersPerSecond(20.0);
		Speed<double> b = Speed<double>.FromMetersPerSecond(50.0);
		Speed<double> diff = a - b;
		Assert.AreEqual(30.0, diff.Value, Tolerance);
	}

	[TestMethod]
	public void Length_Minus_Length_Returns_Length()
	{
		Length<double> a = Length<double>.FromMeter(7.0);
		Length<double> b = Length<double>.FromMeter(2.0);
		Length<double> diff = a - b;
		Assert.AreEqual(5.0, diff.Value, Tolerance);
	}

	// V0 overloads preserve their type under subtraction (no longer fall through to V1).

	[TestMethod]
	public void Weight_Minus_Weight_Stays_Weight_With_Absolute_Difference()
	{
		Weight<double> a = Weight<double>.FromNewton(100.0);
		Weight<double> b = Weight<double>.FromNewton(150.0);
		Weight<double> diff = a - b;
		Assert.AreEqual(50.0, diff.Value, Tolerance);
		Assert.IsInstanceOfType<Weight<double>>(diff);
	}

	[TestMethod]
	public void Distance_Minus_Distance_Stays_Distance()
	{
		Distance<double> a = Distance<double>.FromMeter(2.5);
		Distance<double> b = Distance<double>.FromMeter(7.5);
		Distance<double> diff = a - b;
		Assert.AreEqual(5.0, diff.Value, Tolerance);
		Assert.IsInstanceOfType<Distance<double>>(diff);
	}

	// Storage-type genericity for subtraction.

	[TestMethod]
	public void Mass_Minus_Mass_With_Float_Storage()
	{
		Mass<float> a = Mass<float>.FromKilogram(1.0f);
		Mass<float> b = Mass<float>.FromKilogram(4.0f);
		Mass<float> diff = a - b;
		Assert.AreEqual(3.0f, diff.Value, 1e-6f);
	}

	[TestMethod]
	public void Mass_Minus_Mass_With_Decimal_Storage()
	{
		Mass<decimal> a = Mass<decimal>.FromKilogram(1m);
		Mass<decimal> b = Mass<decimal>.FromKilogram(4m);
		Mass<decimal> diff = a - b;
		Assert.AreEqual(3m, diff.Value);
	}

	// Vector0Guards.EnsureNonNegative directly (sanity check on the helper).

	[TestMethod]
	public void Vector0Guards_Allows_Zero_And_Positive()
	{
		Assert.AreEqual(0.0, Vector0Guards.EnsureNonNegative(0.0, "v"));
		Assert.AreEqual(3.5, Vector0Guards.EnsureNonNegative(3.5, "v"));
	}

	[TestMethod]
	public void Vector0Guards_Throws_On_Negative_With_ParamName()
	{
		ArgumentException ex = Assert.ThrowsExactly<ArgumentException>(
			() => Vector0Guards.EnsureNonNegative(-1.0, "myParam"));
		Assert.AreEqual("myParam", ex.ParamName);
	}
}
