// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Quantities;

using ktsu.Semantics.Quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Covers semantic overload conversions and metadata-driven relationships.
/// Issue #55.
/// </summary>
[TestClass]
public sealed class SemanticOverloadTests
{
	private const double Tolerance = 1e-10;

	// ----------------------------------------- Implicit widening to base

	[TestMethod]
	public void Weight_Widens_Implicitly_To_ForceMagnitude()
	{
		Weight<double> w = Weight<double>.FromNewtons(686.0);
		ForceMagnitude<double> baseValue = w; // implicit conversion
		Assert.AreEqual(686.0, baseValue.Value, Tolerance);
	}

	[TestMethod]
	public void Distance_Widens_Implicitly_To_Length()
	{
		Distance<double> d = Distance<double>.FromMeters(42.0);
		Length<double> len = d;
		Assert.AreEqual(42.0, len.Value, Tolerance);
	}

	[TestMethod]
	public void Diameter_Widens_Implicitly_To_Length()
	{
		Diameter<double> diam = Diameter<double>.FromMeters(10.0);
		Length<double> len = diam;
		Assert.AreEqual(10.0, len.Value, Tolerance);
	}

	// ---------------------------------------- Explicit narrowing from base

	[TestMethod]
	public void ForceMagnitude_Narrows_Explicitly_To_Weight()
	{
		ForceMagnitude<double> fm = ForceMagnitude<double>.FromNewtons(686.0);
		Weight<double> w = (Weight<double>)fm;
		Assert.AreEqual(686.0, w.Value, Tolerance);
	}

	[TestMethod]
	public void Length_Narrows_Explicitly_To_Distance()
	{
		Length<double> len = Length<double>.FromMeters(42.0);
		Distance<double> d = (Distance<double>)len;
		Assert.AreEqual(42.0, d.Value, Tolerance);
	}

	// --------------------------------------------- From(base) factory

	[TestMethod]
	public void Weight_From_ForceMagnitude_Constructs()
	{
		ForceMagnitude<double> fm = ForceMagnitude<double>.FromNewtons(100.0);
		Weight<double> w = Weight<double>.From(fm);
		Assert.AreEqual(100.0, w.Value, Tolerance);
	}

	[TestMethod]
	public void Distance_From_Length_Constructs()
	{
		Length<double> len = Length<double>.FromMeters(7.0);
		Distance<double> d = Distance<double>.From(len);
		Assert.AreEqual(7.0, d.Value, Tolerance);
	}

	// -------------------- Round-trip widen/narrow preserves value

	[TestMethod]
	public void Weight_RoundTrip_Through_ForceMagnitude_Preserves_Value()
	{
		Weight<double> original = Weight<double>.FromNewtons(123.456);
		ForceMagnitude<double> widened = original;
		Weight<double> narrowed = (Weight<double>)widened;
		Assert.AreEqual(original.Value, narrowed.Value, Tolerance);
	}

	// ------------------ Metadata-defined relationship: Diameter <-> Radius

	[TestMethod]
	public void Diameter_ToRadius_Halves_Value()
	{
		Diameter<double> d = Diameter<double>.FromMeters(10.0);
		Radius<double> r = d.ToRadius();
		Assert.AreEqual(5.0, r.Value, Tolerance);
	}

	[TestMethod]
	public void Diameter_FromRadius_Doubles_Value()
	{
		Radius<double> r = Radius<double>.FromMeters(5.0);
		Diameter<double> d = Diameter<double>.FromRadius(r);
		Assert.AreEqual(10.0, d.Value, Tolerance);
	}

	[TestMethod]
	public void Diameter_RoundTrip_Through_Radius_Preserves_Value()
	{
		Diameter<double> d = Diameter<double>.FromMeters(20.0);
		Radius<double> r = d.ToRadius();
		Diameter<double> back = Diameter<double>.FromRadius(r);
		Assert.AreEqual(d.Value, back.Value, Tolerance);
	}

	// ----------------- V0 overload subtraction
	// Locked in #52: V0 - V0 returns the same V0 of T.Abs(a - b).
	// Generator currently emits a Force1D-returning subtraction for Weight - Weight,
	// which violates that rule. The current behaviour is documented here so the fix
	// in #52 can replace this test with the correct shape.

	[TestMethod]
	public void Weight_Minus_Weight_Currently_Returns_Force1D_PendingFix52()
	{
		Weight<double> a = Weight<double>.FromNewtons(100.0);
		Weight<double> b = Weight<double>.FromNewtons(150.0);
		Force1D<double> diff = a - b; // current generator behaviour; #52 plans Weight of |a - b|.
		Assert.AreEqual(-50.0, diff.Value, Tolerance);
	}

	// ----------------- Storage-type genericity sanity

	[TestMethod]
	public void Diameter_ToRadius_Works_With_Float_Storage()
	{
		Diameter<float> d = Diameter<float>.FromMeters(10.0f);
		Radius<float> r = d.ToRadius();
		Assert.AreEqual(5.0f, r.Value, 1e-6f);
	}

	[TestMethod]
	public void Diameter_ToRadius_Works_With_Decimal_Storage()
	{
		Diameter<decimal> d = Diameter<decimal>.FromMeters(10m);
		Radius<decimal> r = d.ToRadius();
		Assert.AreEqual(5m, r.Value);
	}
}
