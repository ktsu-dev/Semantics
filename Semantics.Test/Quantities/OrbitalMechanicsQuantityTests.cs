// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using ktsu.Semantics.Quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Covers the three dimensions orbital mechanics cannot be written without —
/// <c>GravitationalParameter</c> (L³T⁻²), <c>SpecificEnergy</c> (L²T⁻²) and
/// <c>SpecificAngularMomentum</c> (L²T⁻¹) — and the relationships declared on them. Issue #240.
/// </summary>
/// <remarks>
/// Two of the three are name collisions on exponent vectors that already exist:
/// <c>SpecificEnergy</c> shares L²T⁻² with <c>AbsorbedDose</c> and <c>EquivalentDose</c>, and
/// <c>SpecificAngularMomentum</c> shares L²T⁻¹ with <c>KinematicViscosity</c>. SEM008 checks the
/// exponents, so what it cannot check is what these tests are for: the sign of a cross product, and
/// whether a form can hold a negative value at all.
/// </remarks>
[TestClass]
public sealed class OrbitalMechanicsQuantityTests
{
	/// <summary>Standard gravitational parameter of the Earth, in m³/s² (IERS 2010).</summary>
	private const double EarthMu = 3.986004418e14;

	/// <summary>Mean equatorial radius of the Earth, in metres.</summary>
	private const double EarthRadius = 6.371e6;

	// ------------------------------------------------ GravitationalParameter

	/// <summary>
	/// g = mu / r². At the Earth's surface this lands on the familiar ~9.82 m/s², which is the
	/// check that the L³T⁻² exponents and the m³/s² unit describe the same quantity.
	/// </summary>
	[TestMethod]
	public void GravitationalParameter_Over_Area_Is_An_Acceleration()
	{
		GravitationalParameter<double> mu = GravitationalParameter<double>.FromCubicMeterPerSecondSquared(EarthMu);
		Area<double> rSquared = Area<double>.FromSquareMeter(EarthRadius * EarthRadius);

		AccelerationMagnitude<double> g = mu / rSquared;

		Assert.AreEqual(EarthMu / (EarthRadius * EarthRadius), g.Value, 1e-9);
		Assert.AreEqual(9.82, g.Value, 0.01);
	}

	/// <summary>
	/// mu / r is a specific energy — the magnitude form, because mu and r are both positive.
	/// </summary>
	[TestMethod]
	public void GravitationalParameter_Over_Length_Is_A_SpecificEnergy()
	{
		GravitationalParameter<double> mu = GravitationalParameter<double>.FromCubicMeterPerSecondSquared(EarthMu);
		Length<double> r = Length<double>.FromMeter(EarthRadius);

		SpecificEnergyMagnitude<double> energy = mu / r;

		Assert.AreEqual(EarthMu / EarthRadius, energy.Value, 1e-3);
	}

	/// <summary>
	/// Published values of mu are quoted in km³/s², so the conversion is the one a caller actually
	/// reaches for. The factor is 1e9 exactly, not 1e3.
	/// </summary>
	[TestMethod]
	public void GravitationalParameter_Converts_From_CubicKilometerPerSecondSquared()
	{
		GravitationalParameter<double> mu = GravitationalParameter<double>.FromCubicKilometerPerSecondSquared(398600.4418);

		Assert.AreEqual(EarthMu, mu.Value, 1.0);
	}

	// ------------------------------------------------------- SpecificEnergy

	/// <summary>
	/// The reason <c>SpecificEnergy</c>'s base is a signed vector1 and not a vector0: specific
	/// orbital energy is epsilon = -mu / 2a, which is negative for every bound orbit. A vector0
	/// base would run <c>Vector0Guards.EnsureNonNegative</c> and throw on the ordinary case of a
	/// satellite in orbit — a type that fails its own assertion on every real input.
	/// </summary>
	[TestMethod]
	public void SpecificOrbitalEnergy_Holds_The_Negative_Value_Every_Bound_Orbit_Has()
	{
		// A 400 km circular orbit: a = R + 400 km.
		double semiMajorAxis = EarthRadius + 400e3;
		double expected = -EarthMu / (2.0 * semiMajorAxis);

		SpecificOrbitalEnergy<double> epsilon = SpecificOrbitalEnergy<double>.FromJoulePerKilogram(expected);

		Assert.IsLessThan(0.0, epsilon.Value, "A bound orbit has negative specific orbital energy.");
		Assert.AreEqual(expected, epsilon.Value, 1e-3);
	}

	/// <summary>
	/// The vector0 form alongside it keeps the non-negativity guard, which is what makes it the
	/// right form for specific kinetic energy.
	/// </summary>
	[TestMethod]
	public void SpecificKineticEnergy_Rejects_A_Negative_Value()
	{
		_ = Assert.ThrowsExactly<System.ArgumentException>(
			() => SpecificKineticEnergy<double>.FromJoulePerKilogram(-1.0));
	}

	// ----------------------------------------------- SpecificAngularMomentum

	/// <summary>
	/// h = r × v, not v × r. The exponents cannot tell the two apart — a cross product and its
	/// negation have identical dimensions — so the only thing standing between correct and
	/// silently negated is that the relationship is declared on <c>Length</c>, which is what puts
	/// the operands in that order. This is the same hazard the torque declaration got wrong.
	/// </summary>
	[TestMethod]
	public void Displacement3D_Cross_Velocity3D_Has_The_Sign_Of_r_Cross_v()
	{
		Displacement3D<double> r = new() { X = 0.5, Y = 0.0, Z = 0.0 };
		Velocity3D<double> v = new() { X = 0.0, Y = 10.0, Z = 0.0 };

		SpecificAngularMomentum3D<double> h = r.Cross(v);

		// +0.5 x̂ crossed with +10 ŷ points along +ẑ. The other order gives -5, which is what this
		// would assert if the relationship were declared on Velocity instead of Length.
		Assert.AreEqual(0.0, h.X, 1e-10);
		Assert.AreEqual(0.0, h.Y, 1e-10);
		Assert.AreEqual(5.0, h.Z, 1e-10);
	}

	/// <summary>
	/// The direction of h is what encodes the orbital plane, so the magnitude form has to be
	/// reachable from the vector form rather than replacing it.
	/// </summary>
	[TestMethod]
	public void SpecificAngularMomentum3D_Magnitude_Is_The_Magnitude_Form()
	{
		SpecificAngularMomentum3D<double> h = new() { X = 3.0, Y = 4.0, Z = 0.0 };

		SpecificAngularMomentumMagnitude<double> magnitude = h.Magnitude();

		Assert.AreEqual(5.0, magnitude.Value, 1e-10);
	}
}
