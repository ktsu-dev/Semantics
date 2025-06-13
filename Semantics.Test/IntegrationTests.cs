// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Integration tests demonstrating quantities from multiple physics domains working together.
/// </summary>
[TestClass]
public class IntegrationTests
{
	private const double Tolerance = 1e-10;

	/// <summary>
	/// Tests ideal gas law using thermal, chemical, and mechanical quantities.
	/// PV = nRT demonstration.
	/// </summary>
	[TestMethod]
	public void IdealGasLaw_CombinesThreeDomainsCorrectly()
	{
		// Arrange - Standard conditions
		var pressure = Pressure<double>.FromPascals(101325.0);        // 1 atm (Mechanical)
		var volume = Volume<double>.FromCubicMeters(0.0224);          // 22.4 L (Mechanical)
		var temperature = Temperature<double>.FromKelvin(273.15);      // 0°C (Thermal)
		var gasConstant = PhysicalConstants.Generic.GasConstant<double>();

		// Act - Calculate amount of substance using ideal gas law: n = PV/RT
		var pressureValue = pressure.Value;
		var volumeValue = volume.Value;
		var temperatureValue = temperature.Value;
		var calculatedMoles = (pressureValue * volumeValue) / (gasConstant * temperatureValue);
		var expectedMoles = AmountOfSubstance<double>.FromMoles(1.0); // Should be ~1 mole at STP

		// Assert
		Assert.AreEqual(expectedMoles.Value, calculatedMoles, 0.01,
			"Ideal gas law should predict approximately 1 mole at STP");
	}

	/// <summary>
	/// Tests electromagnetic energy calculations combining electrical and optical domains.
	/// E = hf for photons and electrical power relationships.
	/// </summary>
	[TestMethod]
	public void ElectromagneticEnergy_CombinesElectricalAndOpticalDomains()
	{
		// Arrange - Green light LED
		var frequency = Frequency<double>.FromHertz(5.45e14);         // Green light ~550nm (Acoustic)
		var voltage = ElectricPotential<double>.FromVolts(3.3);       // LED forward voltage (Electrical)
		var current = ElectricCurrent<double>.FromAmperes(0.02);      // 20mA LED current (Electrical)

		// Act - Calculate photon energy and electrical power
		var photonEnergy = Frequency<double>.GetPhotonEnergy(frequency);  // E = hf
		var electricalPower = voltage * current;                          // P = VI

		// Assert - Verify reasonable values
		Assert.IsTrue(photonEnergy.Value > 0, "Photon energy should be positive");
		Assert.AreEqual(0.066, electricalPower.Value, 0.001, "Electrical power should be 66mW");

		// Green light photon should be around 2.25 eV = 3.6e-19 J
		Assert.IsTrue(photonEnergy.Value > 3e-19 && photonEnergy.Value < 4e-19,
			"Green light photon energy should be around 3.6e-19 J");
	}

	/// <summary>
	/// Tests acoustic-mechanical relationships using sound in different media.
	/// Combines acoustic, mechanical, and fluid dynamics domains.
	/// </summary>
	[TestMethod]
	public void SoundPropagation_CombinesAcousticMechanicalAndFluidDomains()
	{
		// Arrange - Sound in air
		var frequency = Frequency<double>.FromHertz(1000.0);          // 1 kHz tone (Acoustic)
		var airDensity = Density<double>.FromKilogramsPerCubicMeter(1.225); // Air density (Mechanical)
		var soundSpeed = SoundSpeed<double>.FromMetersPerSecond(343.0);     // Sound speed in air (Acoustic)

		// Act - Calculate acoustic properties
		var wavelength = soundSpeed / frequency;                      // λ = v/f
		var acousticImpedance = AcousticImpedance<double>.FromDensityAndSoundSpeed(airDensity, soundSpeed); // Z = ρc

		// Assert
		Assert.AreEqual(0.343, wavelength.Value, 0.001, "1 kHz wavelength in air should be ~34.3 cm");
		Assert.IsTrue(acousticImpedance.Value > 400 && acousticImpedance.Value < 450,
			"Air acoustic impedance should be around 415 kg/(m²·s)");
	}

	/// <summary>
	/// Tests nuclear decay and energy release combining nuclear and thermal domains.
	/// </summary>
	[TestMethod]
	public void NuclearDecay_CombinesNuclearAndThermalDomains()
	{
		// Arrange - Radioactive source
		var initialActivity = RadioactiveActivity<double>.FromBecquerels(1000.0); // 1000 Bq (Nuclear)
		var halfLife = Time<double>.FromSeconds(3600.0);                         // 1 hour half-life (Mechanical)
		var time = Time<double>.FromSeconds(7200.0);                             // 2 hours elapsed (Mechanical)

		// Act - Calculate decay (simplified calculation for 2 half-lives)
		var halfLives = time.Value / halfLife.Value; // 2 half-lives
		var remainingFraction = Math.Pow(0.5, halfLives);
		var remainingActivity = RadioactiveActivity<double>.FromBecquerels(initialActivity.Value * remainingFraction);
		var decayedAtoms = initialActivity.Value - remainingActivity.Value; // Simplified decay count

		// Convert to thermal energy (simplified - each decay releases ~1 MeV = 1.6e-13 J)
		var energyPerDecay = Energy<double>.FromJoules(1.6e-13);
		var totalThermalEnergy = Energy<double>.Create(decayedAtoms * energyPerDecay.Value);

		// Assert
		Assert.AreEqual(250.0, remainingActivity.Value, 1.0,
			"Activity should be 1/4 original after 2 half-lives");
		Assert.IsTrue(totalThermalEnergy.Value > 0, "Thermal energy should be released from decay");
	}

	/// <summary>
	/// Tests fluid dynamics and thermodynamics in heat transfer.
	/// Combines fluid dynamics, thermal, and mechanical domains.
	/// </summary>
	[TestMethod]
	public void HeatTransferInFlowingFluid_CombinesMultipleDomains()
	{
		// Arrange - Hot water flowing through pipe
		var fluidTemp = Temperature<double>.FromCelsius(80.0);         // Hot water (Thermal)
		var ambientTemp = Temperature<double>.FromCelsius(20.0);       // Ambient temperature (Thermal)
		var flowRate = VolumetricFlowRate<double>.FromLitersPerSecond(2.0); // 2 L/s flow (Fluid Dynamics)
		var waterDensity = Density<double>.FromKilogramsPerCubicMeter(1000.0); // Water density (Mechanical)
		var specificHeat = SpecificHeat<double>.FromJoulesPerKilogramKelvin(4186.0); // Water specific heat (Thermal)

		// Act - Calculate mass flow rate and thermal power manually
		var massFlowRateValue = waterDensity.Value * flowRate.Value;  // ρ * V̇ = ṁ
		var tempDifference = fluidTemp - ambientTemp;
		var thermalPower = Power<double>.Create(massFlowRateValue * specificHeat.Value * tempDifference.Value);

		// Assert
		Assert.AreEqual(2.0, massFlowRateValue, 0.01, "Mass flow rate should be 2 kg/s for water");
		Assert.IsTrue(thermalPower.Value > 500000 && thermalPower.Value < 600000,
			"Thermal power should be around 500 kW");
	}

	/// <summary>
	/// Tests chemical reaction kinetics with thermal effects.
	/// Combines chemical and thermal domains.
	/// </summary>
	[TestMethod]
	public void ChemicalReactionKinetics_CombinesChemicalAndThermalDomains()
	{
		// Arrange - Arrhenius equation parameters
		var preExponentialFactor = 1e12; // A factor in s⁻¹
		var activationEnergy = ActivationEnergy<double>.Create(50000.0); // 50 kJ/mol (Chemical)
		var lowTemp = Temperature<double>.FromCelsius(25.0);           // Room temperature (Thermal)
		var highTemp = Temperature<double>.FromCelsius(100.0);         // Elevated temperature (Thermal)

		// Act - Calculate rate constants at different temperatures
		var lowTempRate = RateConstant<double>.FromArrheniusEquation(preExponentialFactor, activationEnergy, lowTemp);
		var highTempRate = RateConstant<double>.FromArrheniusEquation(preExponentialFactor, activationEnergy, highTemp);

		// Assert - Higher temperature should give higher rate constant
		Assert.IsTrue(highTempRate.Value > lowTempRate.Value,
			"Rate constant should increase with temperature");

		// Rate should increase significantly (exponentially) with temperature
		double rateRatio = highTempRate.Value / lowTempRate.Value;
		Assert.IsTrue(rateRatio > 5.0,
			"Rate constant should increase substantially with 75°C temperature increase");
	}

	/// <summary>
	/// Tests mechanical energy conversion with electrical generation.
	/// Combines mechanical and electrical domains.
	/// </summary>
	[TestMethod]
	public void MechanicalToElectricalConversion_CombinesMechanicalAndElectricalDomains()
	{
		// Arrange - Generator parameters
		var torque = Torque<double>.FromNewtonMeters(100.0);          // Generator torque (Mechanical)
		var angularVelocity = AngularVelocity<double>.FromRadiansPerSecond(150.0); // ~1430 RPM (Mechanical)
		var efficiency = 0.85; // 85% efficiency

		// Act - Calculate mechanical and electrical power manually
		var mechanicalPowerValue = torque.Value * angularVelocity.Value;  // P = τω
		var mechanicalPower = Power<double>.Create(mechanicalPowerValue);
		var electricalPower = Power<double>.Create(mechanicalPower.Value * efficiency);

		// For a 3-phase generator at 400V line-to-line
		var voltage = ElectricPotential<double>.FromVolts(400.0);     // Line voltage (Electrical)
		var currentValue = electricalPower.Value / (voltage.Value * Math.Sqrt(3));    // 3-phase current (simplified)

		// Assert
		Assert.AreEqual(15000.0, mechanicalPower.Value, 0.1, "Mechanical power should be 15 kW");
		Assert.AreEqual(12750.0, electricalPower.Value, 1.0, "Electrical power should be 12.75 kW");
		Assert.IsTrue(currentValue > 18 && currentValue < 19,
			"3-phase current should be around 18.4 A");
	}

	/// <summary>
	/// Tests optical fiber transmission with electrical and optical domains.
	/// </summary>
	[TestMethod]
	public void OpticalFiberTransmission_CombinesOpticalAndElectricalDomains()
	{
		// Arrange - Fiber optic system
		var laserPower = Power<double>.FromWatts(0.001);              // 1 mW laser (Electrical)
		var fiberLengthKm = 10.0;                                     // 10 km fiber (simplified)
		var attenuationPerKm = 0.2; // 0.2 dB/km attenuation
		var coreRefractiveIndex = RefractiveIndex<double>.Create(1.46); // Silica core (Optical)
		var claddingRefractiveIndex = RefractiveIndex<double>.Create(1.45); // Silica cladding (Optical)

				// Act - Calculate numerical aperture and received power
		var numericalAperture = Math.Sqrt(Math.Pow(coreRefractiveIndex.Value, 2) -
			Math.Pow(claddingRefractiveIndex.Value, 2));

		// Calculate power loss due to attenuation
		var totalAttenuation = attenuationPerKm * fiberLengthKm;
		var powerLossFactor = Math.Pow(10, -totalAttenuation / 10); // Convert dB to linear
		var receivedPower = Power<double>.Create(laserPower.Value * powerLossFactor);

		// Assert
		Assert.IsTrue(numericalAperture > 0.1 && numericalAperture < 0.2,
			"Numerical aperture should be around 0.17");
		Assert.IsTrue(receivedPower.Value < laserPower.Value,
			"Received power should be less than transmitted power");
		Assert.IsTrue(receivedPower.Value > 0.0006,
			"Should receive more than 0.6 mW after 10 km with 0.2 dB/km loss");
	}
}
