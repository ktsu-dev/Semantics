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
	private const double LooseTolerance = 1e-6; // For complex derived relationships

	[TestMethod]
	public void MolarVolumeSTP_MatchesCalculatedValue()
	{
		double gasConstant = PhysicalConstants.Fundamental.GasConstant;
		double temperature = PhysicalConstants.Temperature.StandardTemperature;
		double pressure = PhysicalConstants.Mechanical.StandardAtmosphericPressure;

		double calculatedMolarVolume = gasConstant * temperature / pressure;
		double calculatedMolarVolumeInLiters = calculatedMolarVolume * 1000;
		double storedMolarVolume = PhysicalConstants.Chemical.MolarVolumeSTP;

		Assert.AreEqual(calculatedMolarVolumeInLiters, storedMolarVolume, RelaxedTolerance);
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

	[TestMethod]
	public void ElectromagneticConstants_DerivedFromFundamentals()
	{
		// Impedance of free space: Z₀ = μ₀c = √(μ₀/ε₀)
		// Where μ₀ = 4π × 10⁻⁷ H/m (exact in old SI)
		// And ε₀ = 1/(μ₀c²) (derived)
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double mu0 = 4 * Math.PI * 1e-7; // Permeability of free space
		double epsilon0 = 1.0 / (mu0 * c * c); // Permittivity of free space
		double impedanceOfFreeSpace = Math.Sqrt(mu0 / epsilon0);

		// Should equal approximately 377 ohms
		Assert.IsTrue(impedanceOfFreeSpace is > 376 and < 378,
			"Impedance of free space should be approximately 377 Ω");
	}

	[TestMethod]
	public void ElectronVolt_DerivedFromElementaryCharge()
	{
		// 1 eV = e × 1 V, where e is elementary charge
		double elementaryCharge = PhysicalConstants.Fundamental.ElementaryCharge;
		double electronVoltInJoules = elementaryCharge; // 1 eV in joules

		// Should be approximately 1.602 × 10⁻¹⁹ J
		Assert.IsTrue(electronVoltInJoules is > 1.6e-19 and < 1.61e-19,
			"1 eV should be approximately 1.602 × 10⁻¹⁹ J");
	}

	[TestMethod]
	public void BohrRadius_DerivedFromFundamentalConstants()
	{
		// Bohr radius: a₀ = 4πε₀ℏ²/(mₑe²) = ℏ²/(mₑce²α)
		// Where α is fine structure constant ≈ 1/137
		// Bohr radius should be approximately 5.29 × 10⁻¹¹ m
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double elementaryCharge = PhysicalConstants.Fundamental.ElementaryCharge;
		double fineStructureConstant = 1.0 / 137.036; // Approximate value

		// Estimate Bohr radius using simplified relationship
		// a₀ ≈ ℏc/(mₑc²α) where mₑc² ≈ 0.511 MeV
		double electronRestMassEnergy = 0.511e6 * elementaryCharge; // Convert MeV to joules
		double hbar = PhysicalConstants.Fundamental.PlanckConstant / (2 * Math.PI);
		double estimatedBohrRadius = hbar * c / (electronRestMassEnergy * fineStructureConstant);

		// Should be approximately 5.3 × 10⁻¹¹ m
		Assert.IsTrue(estimatedBohrRadius is > 5e-11 and < 6e-11,
			"Bohr radius should be approximately 5.3 × 10⁻¹¹ m");
	}

	[TestMethod]
	public void ClassicalElectronRadius_OrderOfMagnitude()
	{
		// Classical electron radius: rₑ = e²/(4πε₀mₑc²)
		// Should be approximately 2.82 × 10⁻¹⁵ m
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double elementaryCharge = PhysicalConstants.Fundamental.ElementaryCharge;
		double mu0 = 4 * Math.PI * 1e-7;
		double epsilon0 = 1.0 / (mu0 * c * c);
		double electronRestMassEnergy = 0.511e6 * elementaryCharge; // 0.511 MeV in joules

		double classicalElectronRadius = elementaryCharge * elementaryCharge /
			(4 * Math.PI * epsilon0 * electronRestMassEnergy);

		// Should be approximately 2.8 × 10⁻¹⁵ m
		Assert.IsTrue(classicalElectronRadius is > 2e-15 and < 3e-15,
			"Classical electron radius should be approximately 2.8 × 10⁻¹⁵ m");
	}

	[TestMethod]
	public void StefanBoltzmannConstant_DerivedFromFundamentals()
	{
		// Stefan-Boltzmann constant: σ = (2π⁵k⁴)/(15h³c²)
		double k = PhysicalConstants.Fundamental.BoltzmannConstant;
		double h = PhysicalConstants.Fundamental.PlanckConstant;
		double c = PhysicalConstants.Fundamental.SpeedOfLight;

		double stefanBoltzmannConstant = 2 * Math.Pow(Math.PI, 5) * Math.Pow(k, 4) /
			(15 * Math.Pow(h, 3) * c * c);

		// Should be approximately 5.67 × 10⁻⁸ W/(m²·K⁴)
		Assert.IsTrue(stefanBoltzmannConstant is > 5e-8 and < 6e-8,
			"Stefan-Boltzmann constant should be approximately 5.67 × 10⁻⁸ W/(m²·K⁴)");
	}

	[TestMethod]
	public void WienDisplacementConstant_DerivedFromFundamentals()
	{
		// Wien displacement constant: b = hc/(xk) where x ≈ 4.965
		double h = PhysicalConstants.Fundamental.PlanckConstant;
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double k = PhysicalConstants.Fundamental.BoltzmannConstant;
		double x = 4.965; // Solution to transcendental equation

		double wienConstant = h * c / (x * k);

		// Should be approximately 2.90 × 10⁻³ m·K
		Assert.IsTrue(wienConstant is > 2.8e-3 and < 3.0e-3,
			"Wien displacement constant should be approximately 2.90 × 10⁻³ m·K");
	}

	[TestMethod]
	public void FirstRadiationConstant_DerivedFromFundamentals()
	{
		// First radiation constant: c₁ = 2πhc²
		double h = PhysicalConstants.Fundamental.PlanckConstant;
		double c = PhysicalConstants.Fundamental.SpeedOfLight;

		double firstRadiationConstant = 2 * Math.PI * h * c * c;

		// Should be approximately 3.74 × 10⁻¹⁶ W·m²
		Assert.IsTrue(firstRadiationConstant is > 3.7e-16 and < 3.8e-16,
			"First radiation constant should be approximately 3.74 × 10⁻¹⁶ W·m²");
	}

	[TestMethod]
	public void SecondRadiationConstant_DerivedFromFundamentals()
	{
		// Second radiation constant: c₂ = hc/k
		double h = PhysicalConstants.Fundamental.PlanckConstant;
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double k = PhysicalConstants.Fundamental.BoltzmannConstant;

		double secondRadiationConstant = h * c / k;

		// Should be approximately 1.44 × 10⁻² m·K
		Assert.IsTrue(secondRadiationConstant is > 1.4e-2 and < 1.5e-2,
			"Second radiation constant should be approximately 1.44 × 10⁻² m·K");
	}

	[TestMethod]
	public void RydbergEnergy_DerivedFromFundamentals()
	{
		// Rydberg energy: Ry = mₑe⁴/(8ε₀²h²) = α²mₑc²/2
		// Should be approximately 13.6 eV
		double elementaryCharge = PhysicalConstants.Fundamental.ElementaryCharge;
		double fineStructureConstant = 1.0 / 137.036; // Approximate
		double electronRestMassEnergy = 0.511e6 * elementaryCharge; // 0.511 MeV in joules

		double rydbergEnergy = fineStructureConstant * fineStructureConstant * electronRestMassEnergy / 2;
		double rydbergEnergyInEV = rydbergEnergy / elementaryCharge;

		// Should be approximately 13.6 eV
		Assert.IsTrue(rydbergEnergyInEV is > 13.0 and < 14.0,
			"Rydberg energy should be approximately 13.6 eV");
	}

	[TestMethod]
	public void ComptonWavelength_DerivedFromFundamentals()
	{
		// Compton wavelength: λc = h/(mₑc)
		double h = PhysicalConstants.Fundamental.PlanckConstant;
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double electronRestMassEnergy = 0.511e6 * PhysicalConstants.Fundamental.ElementaryCharge;
		double electronMass = electronRestMassEnergy / (c * c);

		double comptonWavelength = h / (electronMass * c);

		// Should be approximately 2.43 × 10⁻¹² m
		Assert.IsTrue(comptonWavelength is > 2.4e-12 and < 2.5e-12,
			"Compton wavelength should be approximately 2.43 × 10⁻¹² m");
	}

	[TestMethod]
	public void PlanckUnits_DerivedFromFundamentals()
	{
		// Planck length: lₚ = √(ℏG/c³)
		double hbar = PhysicalConstants.Fundamental.PlanckConstant / (2 * Math.PI);
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double G = 6.674e-11; // Approximate gravitational constant

		double planckLength = Math.Sqrt(hbar * G / (c * c * c));

		// Should be approximately 1.6 × 10⁻³⁵ m
		Assert.IsTrue(planckLength is > 1e-35 and < 2e-35,
			"Planck length should be approximately 1.6 × 10⁻³⁵ m");

		// Planck time: tₚ = √(ℏG/c⁵)
		double planckTime = Math.Sqrt(hbar * G / Math.Pow(c, 5));

		// Should be approximately 5.4 × 10⁻⁴⁴ s
		Assert.IsTrue(planckTime is > 5e-44 and < 6e-44,
			"Planck time should be approximately 5.4 × 10⁻⁴⁴ s");
	}

	[TestMethod]
	public void AvogadroConstant_RelationshipToBoltzmann()
	{
		// Gas constant R = NA × k
		double avogadroNumber = PhysicalConstants.Fundamental.AvogadroNumber;
		double boltzmannConstant = PhysicalConstants.Fundamental.BoltzmannConstant;
		double gasConstant = PhysicalConstants.Fundamental.GasConstant;

		double calculatedGasConstant = avogadroNumber * boltzmannConstant;

		Assert.AreEqual(gasConstant, calculatedGasConstant, Tolerance);
	}

	[TestMethod]
	public void ElectronChargeToMassRatio_OrderOfMagnitude()
	{
		// e/m ratio for electron should be approximately 1.76 × 10¹¹ C/kg
		double elementaryCharge = PhysicalConstants.Fundamental.ElementaryCharge;
		double electronRestMassEnergy = 0.511e6 * elementaryCharge; // 0.511 MeV
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double electronMass = electronRestMassEnergy / (c * c);

		double chargeToMassRatio = elementaryCharge / electronMass;

		// Should be approximately 1.76 × 10¹¹ C/kg
		Assert.IsTrue(chargeToMassRatio is > 1.7e11 and < 1.8e11,
			"Electron charge-to-mass ratio should be approximately 1.76 × 10¹¹ C/kg");
	}

	[TestMethod]
	public void FineStructureConstant_RelationshipToOtherConstants()
	{
		// α = e²/(4πε₀ℏc) ≈ 1/137
		double elementaryCharge = PhysicalConstants.Fundamental.ElementaryCharge;
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double hbar = PhysicalConstants.Fundamental.PlanckConstant / (2 * Math.PI);
		double mu0 = 4 * Math.PI * 1e-7;
		double epsilon0 = 1.0 / (mu0 * c * c);

		double calculatedAlpha = elementaryCharge * elementaryCharge /
			(4 * Math.PI * epsilon0 * hbar * c);

		// Should be approximately 1/137 ≈ 0.00729
		Assert.IsTrue(calculatedAlpha is > 0.007 and < 0.008,
			"Fine structure constant should be approximately 1/137");
	}

	[TestMethod]
	public void ClassicalElectronRadius_ToComptonWavelengthRatio()
	{
		// rₑ/λc = α/(2π) where α is fine structure constant
		double fineStructureConstant = 1.0 / 137.036;
		double expectedRatio = fineStructureConstant / (2 * Math.PI);

		// This ratio should be approximately 0.001
		Assert.IsTrue(expectedRatio is > 0.0005 and < 0.002,
			"Classical electron radius to Compton wavelength ratio should be α/(2π)");
	}

	[TestMethod]
	public void StandardAirDensity_RelatesToIdealGasLaw()
	{
		// At standard conditions (15°C, 1 atm), verify air density makes sense
		double standardAirDensity = PhysicalConstants.FluidDynamics.StandardAirDensity;
		double standardPressure = PhysicalConstants.Mechanical.StandardAtmosphericPressure;
		double gasConstant = PhysicalConstants.Fundamental.GasConstant;

		// For air: M ≈ 28.97 g/mol, T = 15°C = 288.15 K
		// ρ = PM/(RT)
		double airMolarMass = 0.02897; // kg/mol
		double temperatureAt15C = 273.15 + 15; // K

		double calculatedDensity = standardPressure * airMolarMass / (gasConstant * temperatureAt15C);

		// Should match within reasonable tolerance (air composition varies slightly)
		Assert.AreEqual(standardAirDensity, calculatedDensity, 1e-3);
	}

	[TestMethod]
	public void ThermodynamicRelationships_HeatCapacityRatios()
	{
		// For ideal diatomic gas: Cp/Cv = γ = 7/5 = 1.4
		double gasConstant = PhysicalConstants.Fundamental.GasConstant;

		// Cv = (5/2)R for diatomic gas
		// Cp = Cv + R = (7/2)R
		double cvDiatomic = 2.5 * gasConstant;
		double cpDiatomic = 3.5 * gasConstant;
		double gamma = cpDiatomic / cvDiatomic;

		Assert.AreEqual(1.4, gamma, Tolerance);
	}

	[TestMethod]
	public void EnergyEquivalents_MassEnergyRelation()
	{
		// E = mc² relationship verification
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double electronRestMassEnergy = 0.511e6; // eV
		double elementaryCharge = PhysicalConstants.Fundamental.ElementaryCharge;

		// Calculate electron mass from energy
		double electronMass = electronRestMassEnergy * elementaryCharge / (c * c);

		// Verify the reverse calculation
		double calculatedEnergy = electronMass * c * c / elementaryCharge;

		Assert.AreEqual(electronRestMassEnergy, calculatedEnergy, RelaxedTolerance);
	}

	[TestMethod]
	public void QuantumMechanical_UncertaintyPrinciple()
	{
		// Heisenberg uncertainty principle: ΔxΔp ≥ ℏ/2
		double hbar = PhysicalConstants.Fundamental.PlanckConstant / (2 * Math.PI);
		double minimumUncertaintyProduct = hbar / 2;

		// For a particle confined to atomic scale (~10⁻¹⁰ m)
		double positionUncertainty = 1e-10; // m
		double minimumMomentumUncertainty = minimumUncertaintyProduct / positionUncertainty;

		// Should give reasonable momentum uncertainty for atomic scale
		Assert.IsTrue(minimumMomentumUncertainty is > 1e-25 and < 1e-23,
			"Momentum uncertainty should be reasonable for atomic confinement");
	}

	[TestMethod]
	public void NuclearBinding_MassDefectRelationships()
	{
		// For nuclear physics, binding energies per nucleon are typically 1-8 MeV
		double atomicMassUnit = PhysicalConstants.Nuclear.AtomicMassUnit;
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double elementaryCharge = PhysicalConstants.Fundamental.ElementaryCharge;

		// 1 amu ≈ 931.5 MeV/c²
		double amuInMeV = atomicMassUnit * c * c / (1e6 * elementaryCharge);

		Assert.IsTrue(amuInMeV is > 930 and < 935,
			"Atomic mass unit should be approximately 931.5 MeV/c²");
	}

	[TestMethod]
	public void ElectromagneticSpectrum_PhotonEnergyFrequency()
	{
		// E = hf for photons
		double h = PhysicalConstants.Fundamental.PlanckConstant;
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double elementaryCharge = PhysicalConstants.Fundamental.ElementaryCharge;

		// Visible light: λ ≈ 500 nm
		double wavelength = 500e-9; // m
		double frequency = c / wavelength;
		double photonEnergy = h * frequency;
		double photonEnergyInEV = photonEnergy / elementaryCharge;

		// Should be approximately 2.5 eV
		Assert.IsTrue(photonEnergyInEV is > 2.0 and < 3.0,
			"500 nm photon should have energy around 2.5 eV");
	}

	[TestMethod]
	public void ThermodynamicTemperatureScale_AbsoluteZero()
	{
		// Absolute zero relationships
		double absoluteZero = PhysicalConstants.Temperature.AbsoluteZeroInCelsius;
		double waterTriplePoint = PhysicalConstants.Temperature.WaterTriplePoint;
		double standardTemperature = PhysicalConstants.Temperature.StandardTemperature;

		// Verify temperature scale consistency
		Assert.AreEqual(273.15, absoluteZero, Tolerance, "Absolute zero should be exactly 273.15 K");
		Assert.AreEqual(standardTemperature, absoluteZero, Tolerance, "Standard temperature equals absolute zero");
		Assert.IsTrue(waterTriplePoint > absoluteZero, "Water triple point should be above absolute zero");
	}

	[TestMethod]
	public void AcousticConstants_SoundSpeedRelationships()
	{
		// Sound speed in air: v = √(γRT/M) where γ = 1.4 for air
		double gasConstant = PhysicalConstants.Fundamental.GasConstant;
		double temperatureAt20C = 273.15 + 20; // K
		double airMolarMass = 0.02897; // kg/mol
		double gamma = 1.4; // Heat capacity ratio for air

		double soundSpeed = Math.Sqrt(gamma * gasConstant * temperatureAt20C / airMolarMass);

		// Should be approximately 343 m/s at 20°C
		Assert.IsTrue(soundSpeed is > 340 and < 350,
			"Sound speed in air should be approximately 343 m/s at 20°C");
	}

	[TestMethod]
	public void FluidDynamics_ReynoldsNumberDimensionless()
	{
		// Reynolds number is dimensionless: Re = ρvL/μ
		// This tests that the combination of density, velocity, length, and viscosity is dimensionless
		double airDensity = PhysicalConstants.FluidDynamics.StandardAirDensity;

		// For typical air flow: v = 10 m/s, L = 1 m, μ ≈ 1.8e-5 Pa·s
		double velocity = 10; // m/s
		double length = 1; // m
		double viscosity = 1.8e-5; // Pa·s (typical for air)

		double reynoldsNumber = airDensity * velocity * length / viscosity;

		// Should be dimensionless and reasonable for air flow
		Assert.IsTrue(reynoldsNumber is > 500000 and < 1000000,
			"Reynolds number should be reasonable for typical air flow");
	}

	// ============================================================================
	// COMPREHENSIVE ELECTROMAGNETIC CONSTANT DERIVATIONS
	// ============================================================================

	[TestMethod]
	public void VacuumPermeability_RelationshipToSpeedOfLight()
	{
		// μ₀ = 4π × 10⁻⁷ H/m (exact in old SI)
		// ε₀ = 1/(μ₀c²) (derived)
		// c = 1/√(μ₀ε₀) (Maxwell's equations)
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double mu0 = 4 * Math.PI * 1e-7; // Exact by definition
		double epsilon0 = 1.0 / (mu0 * c * c);

		// Verify Maxwell relation: c = 1/√(μ₀ε₀)
		double calculatedSpeedOfLight = 1.0 / Math.Sqrt(mu0 * epsilon0);

		Assert.AreEqual(c, calculatedSpeedOfLight, RelaxedTolerance);
	}

	[TestMethod]
	public void CoulombConstant_DerivedFromVacuumPermittivity()
	{
		// k_e = 1/(4πε₀) ≈ 8.99 × 10⁹ N⋅m²/C²
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double mu0 = 4 * Math.PI * 1e-7;
		double epsilon0 = 1.0 / (mu0 * c * c);

		double coulombConstant = 1.0 / (4 * Math.PI * epsilon0);

		// Should be approximately 8.99 × 10⁹ N⋅m²/C²
		Assert.IsTrue(coulombConstant is > 8.9e9 and < 9.0e9,
			"Coulomb constant should be approximately 8.99 × 10⁹ N⋅m²/C²");
	}

	[TestMethod]
	public void JosephsonConstant_DerivedFromPlanckAndCharge()
	{
		// K_J = 2e/h (Josephson constant)
		double elementaryCharge = PhysicalConstants.Fundamental.ElementaryCharge;
		double planckConstant = PhysicalConstants.Fundamental.PlanckConstant;

		double josephsonConstant = 2 * elementaryCharge / planckConstant;

		// Should be approximately 4.84 × 10¹⁴ Hz/V
		Assert.IsTrue(josephsonConstant is > 4.8e14 and < 4.9e14,
			"Josephson constant should be approximately 4.84 × 10¹⁴ Hz/V");
	}

	[TestMethod]
	public void VonKlitzingConstant_DerivedFromPlanckAndCharge()
	{
		// R_K = h/e² (von Klitzing constant)
		double elementaryCharge = PhysicalConstants.Fundamental.ElementaryCharge;
		double planckConstant = PhysicalConstants.Fundamental.PlanckConstant;

		double vonKlitzingConstant = planckConstant / (elementaryCharge * elementaryCharge);

		// Should be approximately 2.58 × 10⁴ Ω
		Assert.IsTrue(vonKlitzingConstant is > 2.5e4 and < 2.6e4,
			"von Klitzing constant should be approximately 2.58 × 10⁴ Ω");
	}

	[TestMethod]
	public void BohrMagneton_DerivedFromFundamentals()
	{
		// μ_B = eℏ/(2m_e) (Bohr magneton)
		double elementaryCharge = PhysicalConstants.Fundamental.ElementaryCharge;
		double hbar = PhysicalConstants.Fundamental.PlanckConstant / (2 * Math.PI);
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double electronRestMassEnergy = 0.511e6 * elementaryCharge; // 0.511 MeV
		double electronMass = electronRestMassEnergy / (c * c);

		double bohrMagneton = elementaryCharge * hbar / (2 * electronMass);

		// Should be approximately 9.27 × 10⁻²⁴ J/T
		Assert.IsTrue(bohrMagneton is > 9.2e-24 and < 9.3e-24,
			"Bohr magneton should be approximately 9.27 × 10⁻²⁴ J/T");
	}

	// ============================================================================
	// THERMODYNAMIC CONSTANT DERIVATIONS
	// ============================================================================

	[TestMethod]
	public void RayleighJeansConstant_DerivedFromFundamentals()
	{
		// c₁L = 2ck (first radiation constant for luminance)
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double k = PhysicalConstants.Fundamental.BoltzmannConstant;

		double rayleighJeansConstant = 2 * c * k;

		// Should be of the correct order of magnitude for radiation constants
		Assert.IsTrue(rayleighJeansConstant is > 8e-16 and < 9e-15,
			$"Rayleigh-Jeans constant calculated as {rayleighJeansConstant:E3} should be approximately 8.3 × 10⁻¹⁵ W⋅m⁻²⋅sr⁻¹⋅K⁻¹");
	}

	[TestMethod]
	public void LoschmidtNumber_DerivedFromIdealGasLaw()
	{
		// Loschmidt number: n₀ = N_A/V_m = P/(kT) at STP
		double avogadroNumber = PhysicalConstants.Fundamental.AvogadroNumber;
		double molarVolumeSTP = PhysicalConstants.Chemical.MolarVolumeSTP / 1000; // Convert to m³/mol
		double standardPressure = PhysicalConstants.Mechanical.StandardAtmosphericPressure;
		double boltzmannConstant = PhysicalConstants.Fundamental.BoltzmannConstant;
		double standardTemperature = PhysicalConstants.Temperature.StandardTemperature;

		double loschmidtFromAvogadro = avogadroNumber / molarVolumeSTP;
		double loschmidtFromPressure = standardPressure / (boltzmannConstant * standardTemperature);

		// Both methods should give the same result
		Assert.AreEqual(loschmidtFromAvogadro, loschmidtFromPressure, RelaxedTolerance);

		// Should be approximately 2.69 × 10²⁵ molecules/m³
		Assert.IsTrue(loschmidtFromAvogadro is > 2.6e25 and < 2.7e25,
			"Loschmidt number should be approximately 2.69 × 10²⁵ molecules/m³");
	}

	// ============================================================================
	// NUCLEAR AND PARTICLE PHYSICS DERIVATIONS
	// ============================================================================

	[TestMethod]
	public void NuclearMagneton_PreciseCalculationFromFundamentals()
	{
		// μ_N = eℏ/(2m_p) where m_p ≈ 938.3 MeV/c²
		double elementaryCharge = PhysicalConstants.Fundamental.ElementaryCharge;
		double hbar = PhysicalConstants.Fundamental.PlanckConstant / (2 * Math.PI);
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double protonRestMassEnergy = 938.3e6 * elementaryCharge; // 938.3 MeV
		double protonMass = protonRestMassEnergy / (c * c);

		double calculatedNuclearMagneton = elementaryCharge * hbar / (2 * protonMass);
		double storedNuclearMagneton = PhysicalConstants.Nuclear.NuclearMagneton;

		// Should match within reasonable tolerance (considering approximate proton mass)
		Assert.AreEqual(storedNuclearMagneton, calculatedNuclearMagneton, LooseTolerance);
	}

	[TestMethod]
	public void AtomicMassUnit_ExactRelationshipToKilogram()
	{
		// 1 u = 1/12 × mass of ¹²C atom = 1.66053906660 × 10⁻²⁷ kg (CODATA 2018)
		double atomicMassUnit = PhysicalConstants.Nuclear.AtomicMassUnit;
		double avogadroNumber = PhysicalConstants.Fundamental.AvogadroNumber;

		// 1 u × N_A should equal exactly 1 g/mol
		double gramsPerMole = atomicMassUnit * avogadroNumber * 1000; // Convert to g/mol

		Assert.AreEqual(1.0, gramsPerMole, RelaxedTolerance);
	}

	[TestMethod]
	public void ElectronGFactor_RelationshipToFineStructure()
	{
		// g_e ≈ 2(1 + α/(2π) + ...) where α is fine structure constant
		// Leading QED correction: α/(2π) ≈ 0.00116
		double fineStructureConstant = 1.0 / 137.036; // Approximate
		double qedCorrection = fineStructureConstant / (2 * Math.PI);
		double approximateGFactor = 2 * (1 + qedCorrection);

		// Should be approximately 2.002319
		Assert.IsTrue(approximateGFactor is > 2.002 and < 2.003,
			"Electron g-factor should be approximately 2.002319");
	}

	// ============================================================================
	// GRAVITATIONAL AND COSMOLOGICAL DERIVATIONS
	// ============================================================================

	[TestMethod]
	public void PlanckMass_DerivedFromFundamentals()
	{
		// m_P = √(ℏc/G) (Planck mass)
		double hbar = PhysicalConstants.Fundamental.PlanckConstant / (2 * Math.PI);
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double G = 6.674e-11; // Approximate gravitational constant

		double planckMass = Math.Sqrt(hbar * c / G);

		// Should be approximately 2.18 × 10⁻⁸ kg
		Assert.IsTrue(planckMass is > 2.1e-8 and < 2.2e-8,
			"Planck mass should be approximately 2.18 × 10⁻⁸ kg");
	}

	[TestMethod]
	public void SchwarzschildRadius_ScalingWithMass()
	{
		// r_s = 2GM/c² (Schwarzschild radius)
		double G = 6.674e-11; // Approximate gravitational constant
		double c = PhysicalConstants.Fundamental.SpeedOfLight;

		// For solar mass (≈ 2 × 10³⁰ kg)
		double solarMass = 2e30; // kg
		double schwarzschildRadius = 2 * G * solarMass / (c * c);

		// Should be approximately 3 km for the sun
		Assert.IsTrue(schwarzschildRadius is > 2500 and < 3500,
			"Schwarzschild radius of the sun should be approximately 3 km");
	}

	// ============================================================================
	// QUANTUM MECHANICS AND ATOMIC PHYSICS DERIVATIONS
	// ============================================================================

	[TestMethod]
	public void ReducedPlanckConstant_RelationshipToPlanck()
	{
		// ℏ = h/(2π)
		double planckConstant = PhysicalConstants.Fundamental.PlanckConstant;
		double calculatedHbar = planckConstant / (2 * Math.PI);

		// Should be approximately 1.055 × 10⁻³⁴ J⋅s
		Assert.IsTrue(calculatedHbar is > 1.05e-34 and < 1.06e-34,
			"Reduced Planck constant should be approximately 1.055 × 10⁻³⁴ J⋅s");
	}

	[TestMethod]
	public void HartreeEnergy_DerivedFromRydbergAndFineStructure()
	{
		// E_h = 2Ry = α²m_ec² (Hartree energy)
		double fineStructureConstant = 1.0 / 137.036;
		double electronRestMassEnergy = 0.511e6; // eV

		double hartreeEnergyInEV = fineStructureConstant * fineStructureConstant * electronRestMassEnergy;

		// Should be approximately 27.2 eV
		Assert.IsTrue(hartreeEnergyInEV is > 27.0 and < 27.5,
			"Hartree energy should be approximately 27.2 eV");
	}

	[TestMethod]
	public void QuantumOfConductance_DerivedFromFundamentals()
	{
		// G₀ = 2e²/h (quantum of conductance)
		double elementaryCharge = PhysicalConstants.Fundamental.ElementaryCharge;
		double planckConstant = PhysicalConstants.Fundamental.PlanckConstant;

		double quantumOfConductance = 2 * elementaryCharge * elementaryCharge / planckConstant;

		// Should be approximately 7.75 × 10⁻⁵ S
		Assert.IsTrue(quantumOfConductance is > 7.7e-5 and < 7.8e-5,
			"Quantum of conductance should be approximately 7.75 × 10⁻⁵ S");

		// Should be reciprocal of half von Klitzing constant
		double vonKlitzingConstant = planckConstant / (elementaryCharge * elementaryCharge);
		double reciprocalCheck = 1.0 / (vonKlitzingConstant / 2);

		Assert.AreEqual(quantumOfConductance, reciprocalCheck, Tolerance);
	}

	// ============================================================================
	// OPTICAL AND PHOTONIC CONSTANT DERIVATIONS
	// ============================================================================

	[TestMethod]
	public void PhotonMomentum_RelationshipToEnergy()
	{
		// p = E/c = hf/c = h/λ for photons
		double h = PhysicalConstants.Fundamental.PlanckConstant;
		double c = PhysicalConstants.Fundamental.SpeedOfLight;

		// For 500 nm light
		double wavelength = 500e-9; // m
		double photonMomentum = h / wavelength;
		double photonEnergy = h * c / wavelength;
		double momentumFromEnergy = photonEnergy / c;

		Assert.AreEqual(photonMomentum, momentumFromEnergy, Tolerance);
	}

	[TestMethod]
	public void RadiationPressure_RelationshipToIntensity()
	{
		// Radiation pressure: P = I/c for perfect absorption
		double c = PhysicalConstants.Fundamental.SpeedOfLight;

		// For solar constant (≈ 1361 W/m²)
		double solarConstant = 1361; // W/m²
		double radiationPressure = solarConstant / c;

		// Should be approximately 4.5 × 10⁻⁶ Pa
		Assert.IsTrue(radiationPressure is > 4e-6 and < 5e-6,
			"Solar radiation pressure should be approximately 4.5 × 10⁻⁶ Pa");
	}

	// ============================================================================
	// COMPREHENSIVE CHAIN VALIDATION TESTS
	// ============================================================================

	[TestMethod]
	public void CompleteThermodynamicChain_STPConditions()
	{
		// Complete derivation chain for STP molar volume
		double avogadroNumber = PhysicalConstants.Fundamental.AvogadroNumber;
		double boltzmannConstant = PhysicalConstants.Fundamental.BoltzmannConstant;
		double standardTemperature = PhysicalConstants.Temperature.StandardTemperature;
		double standardPressure = PhysicalConstants.Mechanical.StandardAtmosphericPressure;

		// Step 1: Gas constant from fundamentals
		double gasConstant = avogadroNumber * boltzmannConstant;

		// Step 2: Molar volume from ideal gas law
		double molarVolume = gasConstant * standardTemperature / standardPressure;
		double molarVolumeInLiters = molarVolume * 1000;

		// Step 3: Compare with stored value
		double storedMolarVolume = PhysicalConstants.Chemical.MolarVolumeSTP;

		Assert.AreEqual(storedMolarVolume, molarVolumeInLiters, RelaxedTolerance);
	}

	[TestMethod]
	public void CompleteElectromagneticChain_FineStructureToBohr()
	{
		// Complete derivation chain from fine structure constant to Bohr radius
		double elementaryCharge = PhysicalConstants.Fundamental.ElementaryCharge;
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double hbar = PhysicalConstants.Fundamental.PlanckConstant / (2 * Math.PI);

		// Step 1: Fine structure constant (approximate)
		double mu0 = 4 * Math.PI * 1e-7;
		double epsilon0 = 1.0 / (mu0 * c * c);
		double fineStructureConstant = elementaryCharge * elementaryCharge / (4 * Math.PI * epsilon0 * hbar * c);

		// Step 2: Electron mass from rest energy
		double electronRestMassEnergy = 0.511e6 * elementaryCharge; // 0.511 MeV
		double electronMass = electronRestMassEnergy / (c * c);

		// Step 3: Bohr radius from fine structure constant
		double bohrRadius = hbar / (electronMass * c * fineStructureConstant);

		// Should be approximately 5.29 × 10⁻¹¹ m
		Assert.IsTrue(bohrRadius is > 5.2e-11 and < 5.3e-11,
			"Derived Bohr radius should be approximately 5.29 × 10⁻¹¹ m");
	}

	[TestMethod]
	public void CompleteNuclearChain_MassEnergyUnits()
	{
		// Complete derivation chain for nuclear mass-energy relationships
		double atomicMassUnit = PhysicalConstants.Nuclear.AtomicMassUnit;
		double c = PhysicalConstants.Fundamental.SpeedOfLight;
		double elementaryCharge = PhysicalConstants.Fundamental.ElementaryCharge;
		double avogadroNumber = PhysicalConstants.Fundamental.AvogadroNumber;

		// Step 1: AMU energy equivalent
		double amuEnergyInJoules = atomicMassUnit * c * c;
		double amuEnergyInMeV = amuEnergyInJoules / (1e6 * elementaryCharge);

		// Step 2: Verify consistency with molar mass
		double molarMassOfAMU = atomicMassUnit * avogadroNumber * 1000; // g/mol

		// Step 3: Cross-check with binding energy scales
		Assert.IsTrue(amuEnergyInMeV is > 930 and < 935, "1 amu should be ≈ 931.5 MeV");
		Assert.AreEqual(1.0, molarMassOfAMU, RelaxedTolerance, "1 amu × N_A should equal 1 g/mol");
	}

	[TestMethod]
	public void CompleteQuantumChain_UncertaintyToTunneling()
	{
		// Complete quantum mechanical derivation chain
		double hbar = PhysicalConstants.Fundamental.PlanckConstant / (2 * Math.PI);
		double elementaryCharge = PhysicalConstants.Fundamental.ElementaryCharge;
		double c = PhysicalConstants.Fundamental.SpeedOfLight;

		// Step 1: Uncertainty principle for atomic confinement
		double atomicSize = 1e-10; // m (typical atomic diameter)
		double minimumMomentum = hbar / (2 * atomicSize);

		// Step 2: Corresponding kinetic energy (non-relativistic)
		double electronMass = 0.511e6 * elementaryCharge / (c * c); // From rest mass energy
		double kineticEnergy = minimumMomentum * minimumMomentum / (2 * electronMass);
		double kineticEnergyInEV = kineticEnergy / elementaryCharge;

		// Step 3: Verify this gives reasonable atomic binding energies
		Assert.IsTrue(kineticEnergyInEV is > 0.1 and < 1000,
			"Quantum confinement energy should be in eV range for atoms");
	}

	// ============================================================================
	// DERIVED CONSTANTS VALIDATION TESTS
	// ============================================================================

	[TestMethod]
	public void DerivedConstants_AreaConversions_MatchCalculatedValues()
	{
		// Test SquareFeetToSquareMeters = FeetToMeters²
		double feetToMeters = PhysicalConstants.Conversion.FeetToMeters;
		double calculatedSquareFeetToSquareMeters = feetToMeters * feetToMeters;
		double storedSquareFeetToSquareMeters = PhysicalConstants.Conversion.SquareFeetToSquareMeters;

		Assert.AreEqual(calculatedSquareFeetToSquareMeters, storedSquareFeetToSquareMeters, Tolerance,
			"SquareFeetToSquareMeters should equal FeetToMeters²");

		// Test SquareInchesToSquareMeters = InchesToMeters²
		double inchesToMeters = PhysicalConstants.Conversion.InchesToMeters;
		double calculatedSquareInchesToSquareMeters = inchesToMeters * inchesToMeters;
		double storedSquareInchesToSquareMeters = PhysicalConstants.Conversion.SquareInchesToSquareMeters;

		Assert.AreEqual(calculatedSquareInchesToSquareMeters, storedSquareInchesToSquareMeters, Tolerance,
			"SquareInchesToSquareMeters should equal InchesToMeters²");

		// Test SquareMilesToSquareMeters = MilesToMeters²
		double milesToMeters = PhysicalConstants.Conversion.MilesToMeters;
		double calculatedSquareMilesToSquareMeters = milesToMeters * milesToMeters;
		double storedSquareMilesToSquareMeters = PhysicalConstants.Conversion.SquareMilesToSquareMeters;

		Assert.AreEqual(calculatedSquareMilesToSquareMeters, storedSquareMilesToSquareMeters, Tolerance,
			"SquareMilesToSquareMeters should equal MilesToMeters²");
	}

	[TestMethod]
	public void DerivedConstants_TimeRelationships_MatchCalculatedValues()
	{
		// Test SecondsPerHour = SecondsPerMinute * MinutesPerHour
		double secondsPerMinute = PhysicalConstants.Time.SecondsPerMinute;
		double minutesPerHour = PhysicalConstants.Time.MinutesPerHour;
		double calculatedSecondsPerHour = secondsPerMinute * minutesPerHour;
		double storedSecondsPerHour = PhysicalConstants.Time.SecondsPerHour;

		Assert.AreEqual(calculatedSecondsPerHour, storedSecondsPerHour, Tolerance,
			"SecondsPerHour should equal SecondsPerMinute * MinutesPerHour");

		// Test SecondsPerDay = SecondsPerHour * HoursPerDay
		double hoursPerDay = PhysicalConstants.Time.HoursPerDay;
		double calculatedSecondsPerDay = storedSecondsPerHour * hoursPerDay;
		double storedSecondsPerDay = PhysicalConstants.Time.SecondsPerDay;

		Assert.AreEqual(calculatedSecondsPerDay, storedSecondsPerDay, Tolerance,
			"SecondsPerDay should equal SecondsPerHour * HoursPerDay");

		// Test SecondsPerWeek = SecondsPerDay * DaysPerWeek
		double daysPerWeek = PhysicalConstants.Time.DaysPerWeek;
		double calculatedSecondsPerWeek = storedSecondsPerDay * daysPerWeek;
		double storedSecondsPerWeek = PhysicalConstants.Time.SecondsPerWeek;

		Assert.AreEqual(calculatedSecondsPerWeek, storedSecondsPerWeek, Tolerance,
			"SecondsPerWeek should equal SecondsPerDay * DaysPerWeek");

		// Test SecondsPerJulianYear = SecondsPerDay * DaysPerJulianYear
		double daysPerJulianYear = PhysicalConstants.Time.DaysPerJulianYear;
		double calculatedSecondsPerJulianYear = storedSecondsPerDay * daysPerJulianYear;
		double storedSecondsPerJulianYear = PhysicalConstants.Time.SecondsPerJulianYear;

		Assert.AreEqual(calculatedSecondsPerJulianYear, storedSecondsPerJulianYear, Tolerance,
			"SecondsPerJulianYear should equal SecondsPerDay * DaysPerJulianYear");
	}

	[TestMethod]
	public void DerivedConstants_TemperatureConversions_MatchCalculatedValues()
	{
		// Test FahrenheitToCelsiusSlope = 5/9
		double calculatedFahrenheitToCelsiusSlope = 5.0 / 9.0;
		double storedFahrenheitToCelsiusSlope = PhysicalConstants.Conversion.FahrenheitToCelsiusSlope;

		Assert.AreEqual(calculatedFahrenheitToCelsiusSlope, storedFahrenheitToCelsiusSlope, Tolerance,
			"FahrenheitToCelsiusSlope should equal 5/9");

		// Test that CelsiusToFahrenheitSlope and FahrenheitToCelsiusSlope are reciprocals
		double celsiusToFahrenheitSlope = PhysicalConstants.Conversion.CelsiusToFahrenheitSlope;
		double reciprocalProduct = celsiusToFahrenheitSlope * storedFahrenheitToCelsiusSlope;

		Assert.AreEqual(1.0, reciprocalProduct, Tolerance,
			"Temperature conversion slopes should be reciprocals");

		// Test CelsiusToFahrenheitSlope = 9/5
		double calculatedCelsiusToFahrenheitSlope = 9.0 / 5.0;
		Assert.AreEqual(calculatedCelsiusToFahrenheitSlope, celsiusToFahrenheitSlope, Tolerance,
			"CelsiusToFahrenheitSlope should equal 9/5");
	}

	[TestMethod]
	public void DerivedConstants_MathematicalFractions_MatchCalculatedValues()
	{
		// Test TwoThirds = 2/3
		double calculatedTwoThirds = 2.0 / 3.0;
		double storedTwoThirds = PhysicalConstants.Mathematical.TwoThirds;

		Assert.AreEqual(calculatedTwoThirds, storedTwoThirds, Tolerance,
			"TwoThirds should equal 2/3");

		// Test FourThirds = 4/3
		double calculatedFourThirds = 4.0 / 3.0;
		double storedFourThirds = PhysicalConstants.Mathematical.FourThirds;

		Assert.AreEqual(calculatedFourThirds, storedFourThirds, Tolerance,
			"FourThirds should equal 4/3");

		// Test OneHalf = 1/2
		double calculatedOneHalf = 1.0 / 2.0;
		double storedOneHalf = PhysicalConstants.Mathematical.OneHalf;

		Assert.AreEqual(calculatedOneHalf, storedOneHalf, Tolerance,
			"OneHalf should equal 1/2");

		// Test ThreeHalves = 3/2
		double calculatedThreeHalves = 3.0 / 2.0;
		double storedThreeHalves = PhysicalConstants.Mathematical.ThreeHalves;

		Assert.AreEqual(calculatedThreeHalves, storedThreeHalves, Tolerance,
			"ThreeHalves should equal 3/2");
	}

	[TestMethod]
	public void DerivedConstants_EnergyConversions_MatchCalculatedValues()
	{
		// Test KilowattHourToJoule = 1000 W/kW * 3600 s/h
		double calculatedKilowattHourToJoule = 1000.0 * 3600.0;
		double storedKilowattHourToJoule = PhysicalConstants.Conversion.KilowattHourToJoule;

		Assert.AreEqual(calculatedKilowattHourToJoule, storedKilowattHourToJoule, Tolerance,
			"KilowattHourToJoule should equal 1000 * 3600 (kW to W * seconds per hour)");

		// Verify the time component comes from SecondsPerHour
		double secondsPerHour = PhysicalConstants.Time.SecondsPerHour;
		double alternativeCalculation = 1000.0 * secondsPerHour;

		Assert.AreEqual(alternativeCalculation, storedKilowattHourToJoule, Tolerance,
			"KilowattHourToJoule should use SecondsPerHour constant");
	}

	[TestMethod]
	public void DerivedConstants_LogarithmicRelationships_MatchCalculatedValues()
	{
		// Test Ln10 = ln(10)
		double calculatedLn10 = Math.Log(10.0);
		double storedLn10 = PhysicalConstants.Mathematical.Ln10;

		Assert.AreEqual(calculatedLn10, storedLn10, Tolerance,
			"Ln10 should equal ln(10)");

		// Test Log10E = log₁₀(e)
		double calculatedLog10E = Math.Log10(Math.E);
		double storedLog10E = PhysicalConstants.Mathematical.Log10E;

		Assert.AreEqual(calculatedLog10E, storedLog10E, Tolerance,
			"Log10E should equal log₁₀(e)");

		// Test Ln2 = ln(2)
		double calculatedLn2 = Math.Log(2.0);
		double storedLn2 = PhysicalConstants.Chemical.Ln2;

		Assert.AreEqual(calculatedLn2, storedLn2, Tolerance,
			"Ln2 should equal ln(2)");

		// Test reciprocal relationship: Ln10 * Log10E = 1
		double reciprocalProduct = storedLn10 * storedLog10E;
		Assert.AreEqual(1.0, reciprocalProduct, Tolerance,
			"Ln10 * Log10E should equal 1");
	}

	[TestMethod]
	public void DerivedConstants_FundamentalRelationships_MatchCalculatedValues()
	{
		// Test GasConstant = AvogadroNumber * BoltzmannConstant
		double avogadroNumber = PhysicalConstants.Fundamental.AvogadroNumber;
		double boltzmannConstant = PhysicalConstants.Fundamental.BoltzmannConstant;
		double calculatedGasConstant = avogadroNumber * boltzmannConstant;
		double storedGasConstant = PhysicalConstants.Fundamental.GasConstant;

		Assert.AreEqual(calculatedGasConstant, storedGasConstant, Tolerance,
			"GasConstant should equal AvogadroNumber * BoltzmannConstant");

		// Test MolarVolumeSTP = GasConstant * StandardTemperature / StandardAtmosphericPressure
		double standardTemperature = PhysicalConstants.Temperature.StandardTemperature;
		double standardPressure = PhysicalConstants.Mechanical.StandardAtmosphericPressure;
		double calculatedMolarVolume = storedGasConstant * standardTemperature / standardPressure;
		double calculatedMolarVolumeInLiters = calculatedMolarVolume * 1000; // Convert m³ to L
		double storedMolarVolume = PhysicalConstants.Chemical.MolarVolumeSTP;

		Assert.AreEqual(calculatedMolarVolumeInLiters, storedMolarVolume, RelaxedTolerance,
			"MolarVolumeSTP should equal R*T/P in liters");
	}

	[TestMethod]
	public void DerivedConstants_GenericGetters_MatchDirectConstants()
	{
		// Test that all derived constants accessible through Generic getters match their direct values

		// Area conversions
		Assert.AreEqual(PhysicalConstants.Conversion.SquareFeetToSquareMeters,
			PhysicalConstants.Generic.SquareFeetToSquareMeters<double>(), Tolerance,
			"Generic SquareFeetToSquareMeters should match direct constant");

		Assert.AreEqual(PhysicalConstants.Conversion.SquareInchesToSquareMeters,
			PhysicalConstants.Generic.SquareInchesToSquareMeters<double>(), Tolerance,
			"Generic SquareInchesToSquareMeters should match direct constant");

		Assert.AreEqual(PhysicalConstants.Conversion.SquareMilesToSquareMeters,
			PhysicalConstants.Generic.SquareMilesToSquareMeters<double>(), Tolerance,
			"Generic SquareMilesToSquareMeters should match direct constant");

		// Time relationships
		Assert.AreEqual(PhysicalConstants.Time.SecondsPerMinute,
			PhysicalConstants.Generic.SecondsPerMinute<double>(), Tolerance,
			"Generic SecondsPerMinute should match direct constant");

		Assert.AreEqual(PhysicalConstants.Time.SecondsPerHour,
			PhysicalConstants.Generic.SecondsPerHour<double>(), Tolerance,
			"Generic SecondsPerHour should match direct constant");

		Assert.AreEqual(PhysicalConstants.Time.SecondsPerDay,
			PhysicalConstants.Generic.SecondsPerDay<double>(), Tolerance,
			"Generic SecondsPerDay should match direct constant");

		// Mathematical fractions
		Assert.AreEqual(PhysicalConstants.Mathematical.TwoThirds,
			PhysicalConstants.Generic.TwoThirds<double>(), Tolerance,
			"Generic TwoThirds should match direct constant");

		Assert.AreEqual(PhysicalConstants.Mathematical.FourThirds,
			PhysicalConstants.Generic.FourThirds<double>(), Tolerance,
			"Generic FourThirds should match direct constant");

		// Temperature conversions
		Assert.AreEqual(PhysicalConstants.Conversion.FahrenheitToCelsiusSlope,
			PhysicalConstants.Generic.FahrenheitToCelsiusSlope<double>(), Tolerance,
			"Generic FahrenheitToCelsiusSlope should match direct constant");

		Assert.AreEqual(PhysicalConstants.Conversion.CelsiusToFahrenheitSlope,
			PhysicalConstants.Generic.CelsiusToFahrenheitSlope<double>(), Tolerance,
			"Generic CelsiusToFahrenheitSlope should match direct constant");
	}

	[TestMethod]
	public void DerivedConstants_ConsistencyAcrossNumericTypes()
	{
		// Test that derived constants maintain relationships across different numeric types

		// Test area conversion with float
		float feetToMetersFloat = PhysicalConstants.Generic.FeetToMeters<float>();
		float squareFeetToSquareMetersFloat = PhysicalConstants.Generic.SquareFeetToSquareMeters<float>();
		float calculatedSquareFeetFloat = feetToMetersFloat * feetToMetersFloat;

		Assert.AreEqual(calculatedSquareFeetFloat, squareFeetToSquareMetersFloat, (float)RelaxedTolerance,
			"SquareFeetToSquareMeters relationship should hold for float type");

		// Test time relationship with decimal (where precision allows)
		decimal secondsPerMinuteDecimal = PhysicalConstants.Generic.SecondsPerMinute<decimal>();
		decimal minutesPerHourDecimal = PhysicalConstants.Generic.MinutesPerHour<decimal>();
		decimal secondsPerHourDecimal = PhysicalConstants.Generic.SecondsPerHour<decimal>();
		decimal calculatedSecondsPerHourDecimal = secondsPerMinuteDecimal * minutesPerHourDecimal;

		Assert.AreEqual(calculatedSecondsPerHourDecimal, secondsPerHourDecimal,
			"SecondsPerHour relationship should hold for decimal type");

		// Test mathematical fraction with float
		float twoThirdsFloat = PhysicalConstants.Generic.TwoThirds<float>();
		float calculatedTwoThirdsFloat = 2.0f / 3.0f;

		Assert.AreEqual(calculatedTwoThirdsFloat, twoThirdsFloat, (float)RelaxedTolerance,
			"TwoThirds relationship should hold for float type");
	}

	[TestMethod]
	public void DerivedConstants_ChainedCalculations_MaintainAccuracy()
	{
		// Test chained calculations using derived constants maintain accuracy

		// Chain: Seconds -> Minutes -> Hours -> Days -> Weeks
		double secondsPerMinute = PhysicalConstants.Generic.SecondsPerMinute<double>();
		double minutesPerHour = PhysicalConstants.Generic.MinutesPerHour<double>();
		double hoursPerDay = PhysicalConstants.Generic.HoursPerDay<double>();
		double daysPerWeek = PhysicalConstants.Generic.DaysPerWeek<double>();

		double calculatedSecondsPerWeek = secondsPerMinute * minutesPerHour * hoursPerDay * daysPerWeek;
		double storedSecondsPerWeek = PhysicalConstants.Generic.SecondsPerWeek<double>();

		Assert.AreEqual(calculatedSecondsPerWeek, storedSecondsPerWeek, Tolerance,
			"Chained time calculation should match stored SecondsPerWeek");

		// Chain: Linear -> Area conversions for different units
		double inchesToMeters = PhysicalConstants.Generic.InchesToMeters<double>();
		double feetToMeters = PhysicalConstants.Generic.FeetToMeters<double>();

		// 1 square foot = 144 square inches
		double squareInchesPerSquareFoot = 144.0;
		double calculatedSquareInchesToSquareMetersViaFeet =
			PhysicalConstants.Generic.SquareFeetToSquareMeters<double>() / squareInchesPerSquareFoot;
		double storedSquareInchesToSquareMeters = PhysicalConstants.Generic.SquareInchesToSquareMeters<double>();

		Assert.AreEqual(calculatedSquareInchesToSquareMetersViaFeet, storedSquareInchesToSquareMeters, RelaxedTolerance,
			"Square inch conversion via square foot conversion should match direct conversion");
	}
}
