// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class PhysicalConstantsTests
{
	private const double Tolerance = 1e-10;
	private const double RelaxedTolerance = 1e-8; // For constants with experimental uncertainty

	[TestMethod]
	public void MolarVolumeSTP_MatchesCalculatedValue()
	{
		double gasConstant = PhysicalConstants.Fundamental.GasConstant;
		double temperature = PhysicalConstants.Temperature.StandardTemperature;
		double pressure = 101325.0;

		double calculatedMolarVolume = gasConstant * temperature / pressure;
		double calculatedMolarVolumeInLiters = calculatedMolarVolume * 1000;
		double storedMolarVolume = PhysicalConstants.Chemical.MolarVolumeSTP;

		Assert.AreEqual(calculatedMolarVolumeInLiters, storedMolarVolume, Tolerance);
	}

	[TestMethod]
	public void GasConstant_MatchesCalculatedFromAvogadroAndBoltzmann()
	{
		double avogadroNumber = PhysicalConstants.Fundamental.AvogadroNumber;
		double boltzmannConstant = PhysicalConstants.Fundamental.BoltzmannConstant;

		double calculatedGasConstant = avogadroNumber * boltzmannConstant;
		double storedGasConstant = PhysicalConstants.Fundamental.GasConstant;

		Assert.AreEqual(calculatedGasConstant, storedGasConstant, Tolerance);
	}

	[TestMethod]
	public void Ln2_MatchesCalculatedValue()
	{
		double calculatedLn2 = Math.Log(2.0);
		double storedLn2 = PhysicalConstants.Chemical.Ln2;

		Assert.AreEqual(calculatedLn2, storedLn2, Tolerance);
	}

	[TestMethod]
	public void Ln10_MatchesCalculatedValue()
	{
		double calculatedLn10 = Math.Log(10.0);
		double storedLn10 = PhysicalConstants.Mathematical.Ln10;

		Assert.AreEqual(calculatedLn10, storedLn10, Tolerance,
			"Natural logarithm of 10 should match calculated value");
	}

	[TestMethod]
	public void Log10E_MatchesCalculatedValue()
	{
		double calculatedLog10E = Math.Log10(Math.E);
		double storedLog10E = PhysicalConstants.Mathematical.Log10E;

		Assert.AreEqual(calculatedLog10E, storedLog10E, Tolerance,
			"Base-10 logarithm of e should match calculated value");
	}

	[TestMethod]
	public void Ln10_And_Log10E_AreReciprocals()
	{
		double ln10 = PhysicalConstants.Mathematical.Ln10;
		double log10E = PhysicalConstants.Mathematical.Log10E;

		double product = ln10 * log10E;
		Assert.AreEqual(1.0, product, Tolerance,
			"Ln(10) * Log10(e) should equal 1");
	}

	[TestMethod]
	public void FractionConstants_MatchCalculatedValues()
	{
		// Test mathematical fraction constants
		Assert.AreEqual(0.5, PhysicalConstants.Mathematical.OneHalf, Tolerance,
			"One half should equal 0.5");

		Assert.AreEqual(2.0 / 3.0, PhysicalConstants.Mathematical.TwoThirds, Tolerance,
			"Two thirds should equal 2/3");

		Assert.AreEqual(1.5, PhysicalConstants.Mathematical.ThreeHalves, Tolerance,
			"Three halves should equal 1.5");

		Assert.AreEqual(4.0 / 3.0, PhysicalConstants.Mathematical.FourThirds, Tolerance,
			"Four thirds should equal 4/3");
	}

	[TestMethod]
	public void TemperatureConversionFactors_AreConsistent()
	{
		// Test Celsius to Fahrenheit conversion factors
		double celsiusToFahrenheitSlope = PhysicalConstants.Conversion.CelsiusToFahrenheitSlope;
		double fahrenheitToCelsiusSlope = PhysicalConstants.Conversion.FahrenheitToCelsiusSlope;

		Assert.AreEqual(9.0 / 5.0, celsiusToFahrenheitSlope, Tolerance,
			"Celsius to Fahrenheit slope should be 9/5");

		Assert.AreEqual(5.0 / 9.0, fahrenheitToCelsiusSlope, Tolerance,
			"Fahrenheit to Celsius slope should be 5/9");

		// Test that they are reciprocals
		double product = celsiusToFahrenheitSlope * fahrenheitToCelsiusSlope;
		Assert.AreEqual(1.0, product, Tolerance,
			"Temperature conversion slopes should be reciprocals");
	}

	[TestMethod]
	public void EnergyConversions_AreConsistent()
	{
		// Test kilowatt-hour to joule conversion
		double kwHToJ = PhysicalConstants.Conversion.KilowattHourToJoule;
		double expectedKwHToJ = 1000.0 * 3600.0; // 1 kW = 1000 W, 1 hour = 3600 s
		Assert.AreEqual(expectedKwHToJ, kwHToJ, Tolerance,
			"Kilowatt-hour to joule conversion should be 1000 * 3600");

		// Test that BTU conversion is reasonable (around 1055 J/BTU)
		double btuToJ = PhysicalConstants.Conversion.BtuToJoule;
		Assert.IsTrue(btuToJ is > 1000 and < 1100,
			"BTU to joule conversion should be approximately 1055 J/BTU");
	}

	[TestMethod]
	public void PoundForceConversion_MatchesCalculatedValue()
	{
		// Pound-force = pound-mass * standard gravity
		double poundMassToKg = PhysicalConstants.Mechanical.PoundMassToKilogram;
		double standardGravity = PhysicalConstants.Mechanical.StandardGravity;

		double calculatedPoundForce = poundMassToKg * standardGravity;

		// This should match the conversion factor used in Units.cs for pound-force
		// We can't directly access the unit conversion, but we can verify the calculation
		Assert.IsTrue(calculatedPoundForce is > 4.4 and < 4.5,
			"Pound-force should be approximately 4.448 N");
	}

	[TestMethod]
	public void NuclearMagneton_RelationshipToFundamentalConstants()
	{
		// Nuclear magneton = (e * h) / (4 * π * proton mass)
		// We can't calculate exactly without proton mass, but can verify order of magnitude
		double nuclearMagneton = PhysicalConstants.Nuclear.NuclearMagneton;

		// Nuclear magneton should be much smaller than Bohr magneton
		// Order of magnitude check: should be around 5e-27 J/T
		Assert.IsTrue(nuclearMagneton is > 1e-28 and < 1e-26,
			"Nuclear magneton should be on the order of 5e-27 J/T");
	}

	[TestMethod]
	public void AtomicMassUnit_OrderOfMagnitudeCheck()
	{
		// Atomic mass unit should be approximately 1/12 of carbon-12 atom mass
		double atomicMassUnit = PhysicalConstants.Nuclear.AtomicMassUnit;
		double avogadroNumber = PhysicalConstants.Fundamental.AvogadroNumber;

		// 1 amu * Avogadro's number should be approximately 1 g/mol
		double calculatedMolarMass = atomicMassUnit * avogadroNumber * 1000; // Convert to g/mol

		Assert.IsTrue(calculatedMolarMass is > 0.9 and < 1.1,
			"1 amu * Avogadro's number should be approximately 1 g/mol");
	}

	[TestMethod]
	public void SoundReferenceValues_AreConsistent()
	{
		// Test that sound reference values are related
		double refPressure = PhysicalConstants.Acoustic.ReferenceSoundPressure;
		double refIntensity = PhysicalConstants.Acoustic.ReferenceSoundIntensity;
		double refPower = PhysicalConstants.Acoustic.ReferenceSoundPower;

		// All should be threshold of hearing values
		Assert.AreEqual(20e-6, refPressure, Tolerance,
			"Reference sound pressure should be 20 μPa");

		Assert.AreEqual(1e-12, refIntensity, Tolerance,
			"Reference sound intensity should be 1 pW/m²");

		Assert.AreEqual(1e-12, refPower, Tolerance,
			"Reference sound power should be 1 pW");
	}

	[TestMethod]
	public void WaterConstants_ArePhysicallyReasonable()
	{
		// Water boiling point should be 100°C above absolute zero
		double waterBoiling = PhysicalConstants.Temperature.WaterBoilingPoint;
		double absoluteZero = PhysicalConstants.Temperature.AbsoluteZeroInCelsius;
		double boilingCelsius = waterBoiling - absoluteZero;

		Assert.AreEqual(100.0, boilingCelsius, Tolerance,
			"Water should boil at 100°C above absolute zero");

		// Water triple point should be slightly above absolute zero
		double triplePoint = PhysicalConstants.Temperature.WaterTriplePoint;
		double triplePointCelsius = triplePoint - absoluteZero;

		Assert.AreEqual(0.01, triplePointCelsius, Tolerance,
			"Water triple point should be 0.01°C");
	}

	[TestMethod]
	public void GenericHelpers_ReturnCorrectValues()
	{
		Assert.AreEqual(PhysicalConstants.Fundamental.AvogadroNumber,
			PhysicalConstants.Generic.AvogadroNumber<double>(), Tolerance);

		Assert.AreEqual(PhysicalConstants.Fundamental.GasConstant,
			PhysicalConstants.Generic.GasConstant<double>(), Tolerance);

		Assert.AreEqual(PhysicalConstants.Fundamental.SpeedOfLight,
			PhysicalConstants.Generic.SpeedOfLight<double>(), Tolerance);

		// Test additional Generic helper methods
		Assert.AreEqual(PhysicalConstants.Nuclear.AtomicMassUnit,
			PhysicalConstants.Generic.AtomicMassUnit<double>(), Tolerance);

		Assert.AreEqual(PhysicalConstants.Nuclear.NuclearMagneton,
			PhysicalConstants.Generic.NuclearMagneton<double>(), Tolerance);
	}

	[TestMethod]
	public void GenericHelpers_FloatType_ReturnCorrectValues()
	{
		// Test with float type
		Assert.AreEqual((float)PhysicalConstants.Fundamental.AvogadroNumber,
			PhysicalConstants.Generic.AvogadroNumber<float>(), (float)RelaxedTolerance);

		Assert.AreEqual((float)PhysicalConstants.Fundamental.GasConstant,
			PhysicalConstants.Generic.GasConstant<float>(), (float)RelaxedTolerance);
	}

	[TestMethod]
	public void GenericHelpers_DecimalType_ReturnCorrectValues()
	{
		// Test with decimal type (where precision allows)
		Assert.AreEqual((decimal)PhysicalConstants.Temperature.AbsoluteZeroInCelsius,
			PhysicalConstants.Generic.AbsoluteZeroInCelsius<decimal>());

		Assert.AreEqual((decimal)PhysicalConstants.Mechanical.StandardGravity,
			PhysicalConstants.Generic.StandardGravity<decimal>());
	}
}
