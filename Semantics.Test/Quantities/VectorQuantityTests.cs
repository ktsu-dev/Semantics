// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Quantities;

using ktsu.Semantics.Quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Covers <see cref="IVector0{TSelf, T}"/>..<see cref="IVector4{TSelf, T}"/> contracts:
/// magnitude extraction, typed dot/cross products, vector arithmetic, and V0 invariants.
/// Issue #54.
/// </summary>
[TestClass]
public sealed class VectorQuantityTests
{
	private const double Tolerance = 1e-10;

	// -------------------------------------------------------------- Magnitude

	[TestMethod]
	public void Velocity3D_Magnitude_Of_3_4_0_Is_Speed_5()
	{
		Velocity3D<double> v = new() { X = 3.0, Y = 4.0, Z = 0.0 };
		Speed<double> s = v.Magnitude();
		Assert.AreEqual(5.0, s.Value, Tolerance);
	}

	[TestMethod]
	public void Force3D_Magnitude_Is_Always_NonNegative_Even_With_Negative_Components()
	{
		Force3D<double> f = new() { X = -3.0, Y = -4.0, Z = 0.0 };
		ForceMagnitude<double> m = f.Magnitude();
		Assert.AreEqual(5.0, m.Value, Tolerance);
	}

	[TestMethod]
	public void Velocity3D_Magnitude_Returns_Speed_Type_Statically()
	{
		Velocity3D<double> v = new() { X = 1.0, Y = 0.0, Z = 0.0 };
		Speed<double> s = v.Magnitude();
		Assert.IsInstanceOfType<Speed<double>>(s);
	}

	[TestMethod]
	public void Velocity3D_Magnitude_Of_Zero_Vector_Is_Zero()
	{
		Velocity3D<double> zero = Velocity3D<double>.Zero;
		Speed<double> s = zero.Magnitude();
		Assert.AreEqual(0.0, s.Value, Tolerance);
	}

	// ------------------------------------------------------ Typed dot product

	[TestMethod]
	public void Force3D_Dot_Displacement3D_Returns_Energy_Aligned()
	{
		Force3D<double> f = new() { X = 10.0, Y = 0.0, Z = 0.0 };
		Displacement3D<double> r = new() { X = 2.0, Y = 0.0, Z = 0.0 };
		Energy<double> work = f.Dot(r);
		Assert.AreEqual(20.0, work.Value, Tolerance);
	}

	[TestMethod]
	public void Force3D_Dot_Displacement3D_Is_Zero_For_Perpendicular()
	{
		Force3D<double> f = new() { X = 10.0, Y = 0.0, Z = 0.0 };
		Displacement3D<double> r = new() { X = 0.0, Y = 5.0, Z = 0.0 };
		Energy<double> work = f.Dot(r);
		Assert.AreEqual(0.0, work.Value, Tolerance);
	}

	// ---------------------------------------------------- Typed cross product

	[TestMethod]
	public void Force3D_Cross_Displacement3D_Returns_Torque3D()
	{
		Force3D<double> f = new() { X = 0.0, Y = 10.0, Z = 0.0 };
		Displacement3D<double> r = new() { X = 0.5, Y = 0.0, Z = 0.0 };
		Torque3D<double> t = f.Cross(r);
		// (Y*rZ - Z*rY, Z*rX - X*rZ, X*rY - Y*rX) = (0, 0, -5)
		Assert.AreEqual(0.0, t.X, Tolerance);
		Assert.AreEqual(0.0, t.Y, Tolerance);
		Assert.AreEqual(-5.0, t.Z, Tolerance);
	}

	[TestMethod]
	public void Force3D_Cross_Self_Is_Zero_Vector()
	{
		Force3D<double> f = new() { X = 1.0, Y = 2.0, Z = 3.0 };
		// Same-dimension structural cross returns Force3D (the dimension itself, not a typed dimensional product).
		Force3D<double> c = f.Cross(f);
		Assert.AreEqual(0.0, c.X, Tolerance);
		Assert.AreEqual(0.0, c.Y, Tolerance);
		Assert.AreEqual(0.0, c.Z, Tolerance);
	}

	// --------------------------------------------- Same-dimension dot product

	[TestMethod]
	public void Velocity3D_Dot_Velocity3D_Returns_Raw_Storage_Scalar()
	{
		// Same-dimension Dot is structural and returns the raw storage type; it isn't
		// a typed dimensional product (no "Speed²" exists in the type system).
		Velocity3D<double> a = new() { X = 1.0, Y = 2.0, Z = 3.0 };
		Velocity3D<double> b = new() { X = 4.0, Y = 5.0, Z = 6.0 };
		double dot = a.Dot(b);
		Assert.AreEqual(32.0, dot, Tolerance);
	}

	// ------------------------------------------------ Vector form arithmetic

	[TestMethod]
	public void Force3D_Plus_Force3D_Stays_Force3D_Componentwise()
	{
		Force3D<double> a = new() { X = 1.0, Y = 2.0, Z = 3.0 };
		Force3D<double> b = new() { X = 4.0, Y = 5.0, Z = 6.0 };
		Force3D<double> sum = a + b;
		Assert.AreEqual(5.0, sum.X, Tolerance);
		Assert.AreEqual(7.0, sum.Y, Tolerance);
		Assert.AreEqual(9.0, sum.Z, Tolerance);
	}

	[TestMethod]
	public void Force3D_Minus_Force3D_Componentwise()
	{
		Force3D<double> a = new() { X = 5.0, Y = 7.0, Z = 9.0 };
		Force3D<double> b = new() { X = 1.0, Y = 2.0, Z = 3.0 };
		Force3D<double> diff = a - b;
		Assert.AreEqual(4.0, diff.X, Tolerance);
		Assert.AreEqual(5.0, diff.Y, Tolerance);
		Assert.AreEqual(6.0, diff.Z, Tolerance);
	}

	[TestMethod]
	public void Force3D_Negation_Inverts_Each_Component()
	{
		Force3D<double> f = new() { X = 1.0, Y = -2.0, Z = 3.0 };
		Force3D<double> n = -f;
		Assert.AreEqual(-1.0, n.X, Tolerance);
		Assert.AreEqual(2.0, n.Y, Tolerance);
		Assert.AreEqual(-3.0, n.Z, Tolerance);
	}

	// ------------------------------------------------------------- V0 + V0

	[TestMethod]
	public void Mass_Plus_Mass_Returns_Mass()
	{
		Mass<double> a = Mass<double>.FromKilogram(3.0);
		Mass<double> b = Mass<double>.FromKilogram(5.0);
		Mass<double> sum = a + b;
		Assert.AreEqual(8.0, sum.Value, Tolerance);
		Assert.IsInstanceOfType<Mass<double>>(sum);
	}

	[TestMethod]
	public void Speed_Plus_Speed_Returns_Speed()
	{
		Speed<double> a = Speed<double>.FromMetersPerSecond(3.0);
		Speed<double> b = Speed<double>.FromMetersPerSecond(5.0);
		Speed<double> sum = a + b;
		Assert.AreEqual(8.0, sum.Value, Tolerance);
	}

	// ------------------------------------------------------------- V0 - V0
	// Locked design decision in #52: V0 - V0 should return the same V0 of T.Abs(a - b).
	// Generator currently emits unsigned subtraction via the SemanticQuantity base, which
	// can produce a negative magnitude. Tracked as a follow-up.

	[TestMethod]
	[Ignore("Locked in #52: V0 - V0 should return the same V0 of T.Abs(a - b). Generator currently emits unsigned subtraction.")]
	public void Mass_Minus_Mass_Returns_Absolute_Difference_Pending52()
	{
		Mass<double> a = Mass<double>.FromKilogram(3.0);
		Mass<double> b = Mass<double>.FromKilogram(5.0);
		Mass<double> diff = a - b;
		Assert.AreEqual(2.0, diff.Value, Tolerance);
	}

	// ---------------------------------------------------- V0 non-negativity
	// Tracked in #50: factories on Vector0 quantities should reject negative inputs
	// with ArgumentException. The current generator does not emit guards.

	[TestMethod]
	[Ignore("Tracked in #50: V0 factories should reject negative inputs.")]
	public void Speed_From_Negative_Throws_Pending50()
	{
		_ = Assert.ThrowsExactly<System.ArgumentException>(
			() => Speed<double>.FromMetersPerSecond(-1.0));
	}

	[TestMethod]
	[Ignore("Tracked in #50: V0 factories should reject negative inputs.")]
	public void Mass_From_Negative_Throws_Pending50()
	{
		_ = Assert.ThrowsExactly<System.ArgumentException>(
			() => Mass<double>.FromKilogram(-1.0));
	}

	// -------------------------------------------------- Magnitude on V1
	// Velocity1D.Magnitude() should return Speed of T.Abs(value).

	[TestMethod]
	public void Velocity1D_Magnitude_Of_Negative_Is_Positive_Speed()
	{
		Velocity1D<double> v = Velocity1D<double>.FromMetersPerSecond(-3.5);
		Speed<double> s = v.Magnitude();
		Assert.AreEqual(3.5, s.Value, Tolerance);
	}
}
