// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using ktsu.Semantics.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Advanced integration tests for multi-domain physics relationships.
/// Tests cross-domain consistency using available quantities and constants.
/// </summary>
[TestClass]
public class AdvancedIntegrationTests
{
	private const double Tolerance = 1e-10;

	#region Multi-Domain Physics Tests

	/// <summary>
	/// Tests mechanical relationships using Newton's laws.
	/// </summary>
	[TestMethod]
	public void MechanicalRelations_NewtonianMechanics()
	{
		// Arrange
		Mass<double> mass = Mass<double>.FromKilograms(10.0);
		Acceleration<double> acceleration = Acceleration<double>.FromMetersPerSecondSquared(9.8);
		Velocity<double> velocity = Velocity<double>.FromMetersPerSecond(5.0);

		// Act - Calculate force using F = ma
		Force<double> force = mass * acceleration;
		Momentum<double> momentum = mass * velocity;

		// Assert
		Assert.AreEqual(98.0, force.Value, Tolerance, "F = ma = 10 * 9.8 = 98N");
		Assert.AreEqual(50.0, momentum.Value, Tolerance, "p = mv = 10 * 5 = 50 kg⋅m/s");
	}

	/// <summary>
	/// Tests electrical relationships using Ohm's law.
	/// </summary>
	[TestMethod]
	public void ElectricalRelations_OhmsLaw()
	{
		// Arrange
		ElectricPotential<double> voltage = ElectricPotential<double>.FromVolts(12.0);
		ElectricCurrent<double> current = ElectricCurrent<double>.FromAmperes(2.0);
		ElectricResistance<double> resistance = ElectricResistance<double>.FromOhms(6.0);

		// Act & Assert - Test Ohm's law: V = IR
		ElectricPotential<double> calculatedVoltage = current * resistance;
		Assert.AreEqual(voltage.Value, calculatedVoltage.Value, Tolerance, "V = IR should hold");

		// Test power calculation: P = VI
		Power<double> power = voltage * current;
		Assert.AreEqual(24.0, power.Value, Tolerance, "P = VI = 12 * 2 = 24W");
	}

	/// <summary>
	/// Tests thermal relationships and unit conversions.
	/// </summary>
	[TestMethod]
	public void ThermalRelations_TemperatureConversions()
	{
		// Arrange
		Temperature<double> temperature1 = Temperature<double>.FromKelvin(300.0);
		Temperature<double> temperature2 = Temperature<double>.FromKelvin(350.0);

		// Act
		Temperature<double> deltaT = temperature2 - temperature1;
		double tempCelsius = temperature1.ToCelsius();
		double tempFahrenheit = temperature1.ToFahrenheit();

		// Assert
		Assert.AreEqual(50.0, deltaT.Value, Tolerance, "Temperature difference should be 50K");
		Assert.AreEqual(26.85, tempCelsius, 0.1, "300K should be approximately 26.85°C");
		Assert.AreEqual(80.33, tempFahrenheit, 0.1, "300K should be approximately 80.33°F");
	}

	/// <summary>
	/// Tests chemical concentration calculations.
	/// </summary>
	[TestMethod]
	public void ChemicalRelations_ConcentrationCalculations()
	{
		// Arrange
		AmountOfSubstance<double> amount = AmountOfSubstance<double>.FromMoles(2.0);
		Volume<double> volume = Volume<double>.FromCubicMeters(0.001); // 1 liter

		// Act
		Concentration<double> concentration = amount / volume;

		// Assert
		Assert.AreEqual(2000.0, concentration.Value, Tolerance, "Concentration = amount/volume = 2000 mol/m³");
	}

	/// <summary>
	/// Tests acoustic frequency relationships.
	/// </summary>
	[TestMethod]
	public void AcousticRelations_FrequencyWavelength()
	{
		// Arrange
		Frequency<double> frequency = Frequency<double>.FromHertz(1000.0);
		double speedOfSound = 343.0; // m/s at room temperature

		// Act - Calculate wavelength: λ = c/f
		double wavelength = speedOfSound / frequency.Value;

		// Assert
		Assert.AreEqual(0.343, wavelength, Tolerance, "λ = c/f = 343/1000 = 0.343m");
	}

	/// <summary>
	/// Tests fluid dynamics Reynolds number calculation.
	/// </summary>
	[TestMethod]
	public void FluidDynamicsRelations_ReynoldsNumber()
	{
		// Arrange
		Density<double> density = Density<double>.FromKilogramsPerCubicMeter(1000.0); // Water
		Velocity<double> velocity = Velocity<double>.FromMetersPerSecond(2.0);
		Length<double> length = Length<double>.FromMeters(1.0);
		DynamicViscosity<double> viscosity = DynamicViscosity<double>.Create(0.001); // Water at 20°C

		// Act
		ReynoldsNumber<double> reynolds = ReynoldsNumber<double>.FromFluidProperties(density, velocity, length, viscosity);

		// Assert
		Assert.AreEqual(2000000.0, reynolds.Value, 1.0, "Re = ρvL/μ = 1000*2*1/0.001 = 2,000,000");
	}

	/// <summary>
	/// Tests optical luminous intensity.
	/// </summary>
	[TestMethod]
	public void OpticalRelations_LuminousIntensity()
	{
		// Arrange
		LuminousIntensity<double> intensity = LuminousIntensity<double>.FromCandelas(100.0);

		// Assert
		Assert.AreEqual(100.0, intensity.Value, Tolerance, "Luminous intensity should be 100 cd");
	}

	/// <summary>
	/// Tests nuclear radioactive activity.
	/// </summary>
	[TestMethod]
	public void NuclearRelations_RadioactiveActivity()
	{
		// Arrange
		RadioactiveActivity<double> activity = RadioactiveActivity<double>.FromBecquerels(1000.0);

		// Assert
		Assert.AreEqual(1000.0, activity.Value, Tolerance, "Activity should be 1000 Bq");
		Assert.IsTrue(activity.Value > 0, "Activity should be positive");
	}

	#endregion

	#region Cross-Domain Integration Tests

	/// <summary>
	/// Tests thermal-mechanical integration using pressure-temperature relationships.
	/// </summary>
	[TestMethod]
	public void ThermalMechanicalIntegration_PressureTemperature()
	{
		// Arrange
		Temperature<double> temperature = Temperature<double>.FromKelvin(300.0);
		Pressure<double> pressure = Pressure<double>.FromPascals(PhysicalConstants.Generic.StandardAtmosphericPressure<double>());

		// Act - Test basic relationships
		double temperatureRatio = temperature.Value / PhysicalConstants.Generic.StandardTemperature<double>();
		double pressureRatio = pressure.Value / PhysicalConstants.Generic.StandardAtmosphericPressure<double>();

		// Assert
		Assert.AreEqual(1.098, temperatureRatio, 0.001, "Temperature ratio at 300K vs STP");
		Assert.AreEqual(1.0, pressureRatio, Tolerance, "Pressure ratio at standard conditions");
	}

	/// <summary>
	/// Tests electrical-thermal integration using Joule heating.
	/// </summary>
	[TestMethod]
	public void ElectricalThermalIntegration_JouleHeating()
	{
		// Arrange
		ElectricCurrent<double> current = ElectricCurrent<double>.FromAmperes(10.0);
		ElectricResistance<double> resistance = ElectricResistance<double>.FromOhms(5.0);
		Time<double> time = Time<double>.FromSeconds(60.0);

		// Act - Calculate power and energy
		ElectricPotential<double> voltage = current * resistance;
		Power<double> power = voltage * current;
		Energy<double> energy = Energy<double>.Create(power.Value * time.Value);

		// Assert
		Assert.AreEqual(50.0, voltage.Value, Tolerance, "V = IR = 10 * 5 = 50V");
		Assert.AreEqual(500.0, power.Value, Tolerance, "P = VI = 50 * 10 = 500W");
		Assert.AreEqual(30000.0, energy.Value, Tolerance, "E = Pt = 500 * 60 = 30000J");
	}

	/// <summary>
	/// Tests chemical-thermal integration using gas constants.
	/// </summary>
	[TestMethod]
	public void ChemicalThermalIntegration_GasLaws()
	{
		// Arrange
		AmountOfSubstance<double> amount = AmountOfSubstance<double>.FromMoles(1.0);
		Temperature<double> temperature = Temperature<double>.FromKelvin(PhysicalConstants.Generic.StandardTemperature<double>());
		double gasConstant = PhysicalConstants.Generic.GasConstant<double>();

		// Act - Calculate RT for ideal gas
		double RT = gasConstant * temperature.Value;

		// Assert - Use more appropriate tolerance for floating point multiplication
		Assert.AreEqual(2271.098, RT, 0.01, "RT = 8.314 * 273.15 = 2271.098 J/mol");
	}

	/// <summary>
	/// Tests acoustic-mechanical integration using sound pressure and velocity.
	/// </summary>
	[TestMethod]
	public void AcousticMechanicalIntegration_SoundWaves()
	{
		// Arrange
		Frequency<double> frequency = Frequency<double>.FromHertz(1000.0);
		Velocity<double> velocity = Velocity<double>.FromMetersPerSecond(343.0); // Speed of sound

		// Act - Calculate wavelength
		double wavelength = velocity.Value / frequency.Value;

		// Assert
		Assert.AreEqual(0.343, wavelength, Tolerance, "λ = v/f = 343/1000 = 0.343m");
	}

	#endregion
}
