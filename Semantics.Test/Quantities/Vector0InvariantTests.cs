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
	public void Mass_FromKilograms_Negative_Throws()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Mass<double>.FromKilograms(-0.5));

	[TestMethod]
	public void Length_FromMeters_Negative_Throws()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Length<double>.FromMeters(-3.0));

	[TestMethod]
	public void Energy_FromJoules_Negative_Throws()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Energy<double>.FromJoules(-100.0));

	[TestMethod]
	public void Speed_FromMetersPerSecond_Zero_Allowed()
	{
		Speed<double> s = Speed<double>.FromMetersPerSecond(0.0);
		Assert.AreEqual(0.0, s.Value, Tolerance);
	}

	[TestMethod]
	public void Mass_FromKilograms_Positive_Returns_Same_Value()
	{
		Mass<double> m = Mass<double>.FromKilograms(2.5);
		Assert.AreEqual(2.5, m.Value, Tolerance);
	}

	// V0 overloads inherit non-negativity from their dimension.

	[TestMethod]
	public void Distance_FromMeters_Negative_Throws()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Distance<double>.FromMeters(-1.0));

	[TestMethod]
	public void Weight_FromNewtons_Negative_Throws()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Weight<double>.FromNewtons(-9.81));

	// V1 quantities are signed and accept any input.

	[TestMethod]
	public void Velocity1D_FromMetersPerSecond_Negative_Allowed()
	{
		Velocity1D<double> v = Velocity1D<double>.FromMetersPerSecond(-3.5);
		Assert.AreEqual(-3.5, v.Value, Tolerance);
	}

	[TestMethod]
	public void TemperatureDelta_FromKelvins_Negative_Allowed()
	{
		TemperatureDelta<double> dt = TemperatureDelta<double>.FromKelvins(-10.0);
		Assert.AreEqual(-10.0, dt.Value, Tolerance);
	}

	// Storage-type genericity for the guard.

	[TestMethod]
	public void Mass_FromKilograms_Negative_Throws_With_Float()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Mass<float>.FromKilograms(-1.0f));

	[TestMethod]
	public void Mass_FromKilograms_Negative_Throws_With_Decimal()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Mass<decimal>.FromKilograms(-1m));

	// =========================================================== #52: Absolute subtraction

	[TestMethod]
	public void Mass_Minus_Larger_Mass_Returns_Mass_Of_Absolute_Difference()
	{
		Mass<double> small = Mass<double>.FromKilograms(3.0);
		Mass<double> large = Mass<double>.FromKilograms(5.0);
		Mass<double> diff = small - large;
		Assert.AreEqual(2.0, diff.Value, Tolerance);
		Assert.IsInstanceOfType<Mass<double>>(diff);
	}

	[TestMethod]
	public void Mass_Minus_Smaller_Mass_Returns_Positive_Mass()
	{
		Mass<double> large = Mass<double>.FromKilograms(5.0);
		Mass<double> small = Mass<double>.FromKilograms(3.0);
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
		Length<double> a = Length<double>.FromMeters(7.0);
		Length<double> b = Length<double>.FromMeters(2.0);
		Length<double> diff = a - b;
		Assert.AreEqual(5.0, diff.Value, Tolerance);
	}

	// V0 overloads preserve their type under subtraction (no longer fall through to V1).

	[TestMethod]
	public void Weight_Minus_Weight_Stays_Weight_With_Absolute_Difference()
	{
		Weight<double> a = Weight<double>.FromNewtons(100.0);
		Weight<double> b = Weight<double>.FromNewtons(150.0);
		Weight<double> diff = a - b;
		Assert.AreEqual(50.0, diff.Value, Tolerance);
		Assert.IsInstanceOfType<Weight<double>>(diff);
	}

	[TestMethod]
	public void Distance_Minus_Distance_Stays_Distance()
	{
		Distance<double> a = Distance<double>.FromMeters(2.5);
		Distance<double> b = Distance<double>.FromMeters(7.5);
		Distance<double> diff = a - b;
		Assert.AreEqual(5.0, diff.Value, Tolerance);
		Assert.IsInstanceOfType<Distance<double>>(diff);
	}

	// Storage-type genericity for subtraction.

	[TestMethod]
	public void Mass_Minus_Mass_With_Float_Storage()
	{
		Mass<float> a = Mass<float>.FromKilograms(1.0f);
		Mass<float> b = Mass<float>.FromKilograms(4.0f);
		Mass<float> diff = a - b;
		Assert.AreEqual(3.0f, diff.Value, 1e-6f);
	}

	[TestMethod]
	public void Mass_Minus_Mass_With_Decimal_Storage()
	{
		Mass<decimal> a = Mass<decimal>.FromKilograms(1m);
		Mass<decimal> b = Mass<decimal>.FromKilograms(4m);
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

	// =========================================================== #51: Strict-positive overloads

	// Wavelength, Period, HalfLife declare physicalConstraints.minExclusive: "0" in
	// dimensions.json; their generated From{Unit} factories use Vector0Guards.EnsurePositive
	// instead of EnsureNonNegative, rejecting zero as well as negative values.

	[TestMethod]
	public void Wavelength_FromMeters_Zero_Throws()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Wavelength<double>.FromMeters(0.0));

	[TestMethod]
	public void Wavelength_FromMeters_Negative_Throws()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Wavelength<double>.FromMeters(-1e-9));

	[TestMethod]
	public void Wavelength_FromMeters_Positive_Succeeds()
	{
		Wavelength<double> w = Wavelength<double>.FromMeters(550e-9);
		Assert.AreEqual(550e-9, w.Value, 1e-15);
	}

	[TestMethod]
	public void Wavelength_FromNanometers_Zero_Throws()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Wavelength<double>.FromNanometers(0.0));

	[TestMethod]
	public void Period_FromSeconds_Zero_Throws()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => Period<double>.FromSeconds(0.0));

	[TestMethod]
	public void Period_FromSeconds_Positive_Succeeds()
	{
		Period<double> p = Period<double>.FromSeconds(0.001);
		Assert.AreEqual(0.001, p.Value, Tolerance);
	}

	[TestMethod]
	public void HalfLife_FromSeconds_Zero_Throws()
		=> _ = Assert.ThrowsExactly<ArgumentException>(() => HalfLife<double>.FromSeconds(0.0));

	// The base type (Length, Duration) has no minExclusive, so zero is still allowed —
	// only the constrained overloads reject it.

	[TestMethod]
	public void Length_FromMeters_Zero_Allowed()
	{
		Length<double> l = Length<double>.FromMeters(0.0);
		Assert.AreEqual(0.0, l.Value, Tolerance);
	}

	[TestMethod]
	public void Duration_FromSeconds_Zero_Allowed()
	{
		Duration<double> d = Duration<double>.FromSeconds(0.0);
		Assert.AreEqual(0.0, d.Value, Tolerance);
	}

	// Other Length / Duration overloads without the constraint also still allow zero.

	[TestMethod]
	public void Distance_FromMeters_Zero_Allowed()
	{
		Distance<double> d = Distance<double>.FromMeters(0.0);
		Assert.AreEqual(0.0, d.Value, Tolerance);
	}

	[TestMethod]
	public void Latency_FromSeconds_Zero_Allowed()
	{
		// Latency has no minExclusive — a zero-latency response is meaningful.
		Latency<double> l = Latency<double>.FromSeconds(0.0);
		Assert.AreEqual(0.0, l.Value, Tolerance);
	}

	// Vector0Guards.EnsurePositive directly.

	[TestMethod]
	public void Vector0Guards_EnsurePositive_Allows_Positive()
	{
		Assert.AreEqual(3.5, Vector0Guards.EnsurePositive(3.5, "v"));
	}

	[TestMethod]
	public void Vector0Guards_EnsurePositive_Throws_On_Zero_With_ParamName()
	{
		ArgumentException ex = Assert.ThrowsExactly<ArgumentException>(
			() => Vector0Guards.EnsurePositive(0.0, "myParam"));
		Assert.AreEqual("myParam", ex.ParamName);
	}

	[TestMethod]
	public void Vector0Guards_EnsurePositive_Throws_On_Negative_With_ParamName()
	{
		ArgumentException ex = Assert.ThrowsExactly<ArgumentException>(
			() => Vector0Guards.EnsurePositive(-1.0, "myParam"));
		Assert.AreEqual("myParam", ex.ParamName);
	}
}
