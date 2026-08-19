// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test;

using ktsu.Semantics.Quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Covers the generated <see cref="PhysicalConstants"/> surface. Each constant is materialised by
/// parsing its metadata literal directly into the requested numeric type, so these tests pin both
/// the values and the two literal shapes the metadata uses: exponent notation and long CODATA
/// decimal expansions.
/// </summary>
[TestClass]
public class PhysicalConstantsTests
{
	[TestMethod]
	public void ExactConstant_MaterialisesExactly()
	{
		Assert.AreEqual(299792458d, PhysicalConstants.Fundamental.SpeedOfLight<double>());
	}

	[TestMethod]
	public void ExponentLiteral_ParsesAsExponent()
	{
		// Metadata stores these in exponent form ("6.62607015e-34", "20e-6", "6.02214076e23").
		// The default Parse(string, IFormatProvider) overload rejects exponents for some numeric
		// types, so the generator must pass NumberStyles.Float explicitly.
		Assert.AreEqual(6.62607015e-34d, PhysicalConstants.Fundamental.PlanckConstant<double>());
		Assert.AreEqual(20e-6d, PhysicalConstants.Acoustics.ReferenceSoundPressure<double>());
		Assert.AreEqual(6.02214076e23d, PhysicalConstants.Fundamental.AvogadroNumber<double>());
	}

	[TestMethod]
	public void ExponentLiteral_ParsesForDecimal()
	{
		Assert.AreEqual(0.00002m, PhysicalConstants.Acoustics.ReferenceSoundPressure<decimal>());
		Assert.AreEqual(602214076000000000000000m, PhysicalConstants.Fundamental.AvogadroNumber<decimal>());
	}

	[TestMethod]
	public void LongPrecisionLiteral_MaterialisesForDouble()
	{
		// DegreesPerRadian is stored as a 152-digit expansion of 180/pi.
		Assert.AreEqual(57.29577951308232d, PhysicalConstants.AngularMechanics.DegreesPerRadian<double>(), 1e-13d);
	}

	[TestMethod]
	public void LongPrecisionLiteral_MaterialisesForFloat()
	{
		// A float must get a finite value here; comparing against NaN always fails, so this also
		// guards the narrowing path for the long literals.
		Assert.AreEqual(57.29578f, PhysicalConstants.AngularMechanics.DegreesPerRadian<float>(), 1e-3f);
	}

	[TestMethod]
	public void LongPrecisionLiteral_MaterialisesForDecimal()
	{
		Assert.AreEqual(57.2957795130823m, PhysicalConstants.AngularMechanics.DegreesPerRadian<decimal>(), 1e-12m);
	}

	[TestMethod]
	public void GenericAccessor_AgreesWithDomainAccessor()
	{
		Assert.AreEqual(
			PhysicalConstants.Fundamental.SpeedOfLight<double>(),
			PhysicalConstants.Generic.SpeedOfLight<double>());
		Assert.AreEqual(
			PhysicalConstants.AngularMechanics.DegreesPerRadian<decimal>(),
			PhysicalConstants.Generic.DegreesPerRadian<decimal>());
		Assert.AreEqual(
			PhysicalConstants.Acoustics.ReferenceSoundPressure<float>(),
			PhysicalConstants.Generic.ReferenceSoundPressure<float>());
	}

	[TestMethod]
	public void RepeatedAccess_ReturnsCachedValue()
	{
		// The generator caches each constant in a static field on a generic holder, so repeated
		// reads must be stable (and must not re-parse to a different value).
		double first = PhysicalConstants.Fundamental.GravitationalConstant<double>();
		double second = PhysicalConstants.Fundamental.GravitationalConstant<double>();

		Assert.AreEqual(first, second);
		Assert.AreEqual(6.67430e-11d, first);
	}
}
