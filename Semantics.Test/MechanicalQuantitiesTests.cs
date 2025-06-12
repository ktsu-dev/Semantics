// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests for mechanical quantities with physical constants integration.
/// </summary>
[TestClass]
public class MechanicalQuantitiesTests
{
	private const double Tolerance = 1e-10;

	/// <summary>
	/// Tests Newton's second law: F = ma
	/// </summary>
	[TestMethod]
	public void Force_NewtonsSecondLaw_CalculatesCorrectly()
	{
		// Arrange
		Mass<double> mass = Mass<double>.Create(10.0); // 10 kg
		Acceleration<double> acceleration = Acceleration<double>.Create(5.0); // 5 m/s²

		// Act
		Force<double> force = Force<double>.FromMassAndAcceleration(mass, acceleration);
		double forceValue = force.In(Units.Newton);

		// Assert
		Assert.AreEqual(50.0, forceValue, Tolerance, "F = ma should give 10 kg × 5 m/s² = 50 N");
	}

	/// <summary>
	/// Tests weight calculation using standard gravity.
	/// </summary>
	[TestMethod]
	public void Force_WeightCalculation_UsesStandardGravity()
	{
		// Arrange
		Mass<double> mass = Mass<double>.Create(1.0); // 1 kg

		// Act
		Force<double> weight = Force<double>.FromWeight(mass);
		double weightValue = weight.In(Units.Newton);

		// Expected: 1 kg × 9.80665 m/s² = 9.80665 N
		double expectedWeight = PhysicalConstants.Mechanical.StandardGravity;

		// Assert
		Assert.AreEqual(expectedWeight, weightValue, Tolerance,
			"Weight should use standard gravity constant");
	}

	/// <summary>
	/// Tests acceleration calculation from force and mass.
	/// </summary>
	[TestMethod]
	public void Force_AccelerationCalculation_InversesCorrectly()
	{
		// Arrange
		Force<double> force = Force<double>.Create(100.0); // 100 N
		Mass<double> mass = Mass<double>.Create(20.0); // 20 kg

		// Act
		Acceleration<double> acceleration = force.GetAcceleration(mass);
		double accelerationValue = acceleration.In(Units.MetersPerSecondSquared);

		// Assert
		Assert.AreEqual(5.0, accelerationValue, Tolerance, "a = F/m should give 100 N / 20 kg = 5 m/s²");
	}

	/// <summary>
	/// Tests kinetic energy calculation: KE = ½mv²
	/// </summary>
	[TestMethod]
	public void Energy_KineticEnergy_CalculatesCorrectly()
	{
		// Arrange
		Mass<double> mass = Mass<double>.Create(2.0); // 2 kg
		Velocity<double> velocity = Velocity<double>.Create(10.0); // 10 m/s

		// Act
		Energy<double> kineticEnergy = Energy<double>.FromKineticEnergy(mass, velocity);
		double energyValue = kineticEnergy.In(Units.Joule);

		// Expected: ½ × 2 kg × (10 m/s)² = 100 J
		Assert.AreEqual(100.0, energyValue, Tolerance, "KE = ½mv² should give ½ × 2 × 100 = 100 J");
	}

	/// <summary>
	/// Tests potential energy calculation: PE = mgh
	/// </summary>
	[TestMethod]
	public void Energy_PotentialEnergy_UsesStandardGravity()
	{
		// Arrange
		Mass<double> mass = Mass<double>.Create(5.0); // 5 kg
		Length<double> height = Length<double>.Create(10.0); // 10 m

		// Act
		Energy<double> potentialEnergy = Energy<double>.FromPotentialEnergy(mass, height);
		double energyValue = potentialEnergy.In(Units.Joule);

		// Expected: 5 kg × 9.80665 m/s² × 10 m = 490.3325 J
		double expectedEnergy = 5.0 * PhysicalConstants.Mechanical.StandardGravity * 10.0;

		// Assert
		Assert.AreEqual(expectedEnergy, energyValue, Tolerance,
			"PE = mgh should use standard gravity constant");
	}

	/// <summary>
	/// Tests work-energy theorem: W = F·d
	/// </summary>
	[TestMethod]
	public void Energy_WorkDone_CalculatesCorrectly()
	{
		// Arrange
		Force<double> force = Force<double>.Create(50.0); // 50 N
		Length<double> distance = Length<double>.Create(4.0); // 4 m

		// Act
		Energy<double> work = Energy<double>.FromWork(force, distance);
		double workValue = work.In(Units.Joule);

		// Assert
		Assert.AreEqual(200.0, workValue, Tolerance, "W = F·d should give 50 N × 4 m = 200 J");
	}

	/// <summary>
	/// Tests velocity calculation from kinetic energy.
	/// </summary>
	[TestMethod]
	public void Energy_VelocityFromKineticEnergy_InversesCorrectly()
	{
		// Arrange
		Energy<double> kineticEnergy = Energy<double>.Create(72.0); // 72 J
		Mass<double> mass = Mass<double>.Create(2.0); // 2 kg

		// Act
		Velocity<double> velocity = kineticEnergy.GetVelocityFromKineticEnergy(mass);
		double velocityValue = velocity.In(Units.MetersPerSecond);

		// Expected: v = √(2E/m) = √(2×72/2) = √72 = 6√2 ≈ 8.485 m/s
		double expectedVelocity = Math.Sqrt(72.0);

		// Assert
		Assert.AreEqual(expectedVelocity, velocityValue, Tolerance,
			"v = √(2E/m) should give √72 ≈ 8.485 m/s");
	}

	/// <summary>
	/// Tests pressure calculation from force and area: P = F/A
	/// </summary>
	[TestMethod]
	public void Pressure_ForceAndArea_CalculatesCorrectly()
	{
		// Arrange
		Force<double> force = Force<double>.Create(1000.0); // 1000 N
		Area<double> area = Area<double>.Create(2.0); // 2 m²

		// Act
		Pressure<double> pressure = Pressure<double>.FromForceAndArea(force, area);
		double pressureValue = pressure.In(Units.Pascal);

		// Assert
		Assert.AreEqual(500.0, pressureValue, Tolerance, "P = F/A should give 1000 N / 2 m² = 500 Pa");
	}

	/// <summary>
	/// Tests standard atmospheric pressure constant.
	/// </summary>
	[TestMethod]
	public void Pressure_StandardAtmospheric_UsesCorrectConstant()
	{
		// Act
		Pressure<double> atmosphericPressure = Pressure<double>.StandardAtmospheric();
		double pressureValue = atmosphericPressure.In(Units.Pascal);

		// Assert
		Assert.AreEqual(PhysicalConstants.Mechanical.StandardAtmosphericPressure, pressureValue, Tolerance,
			"Standard atmospheric pressure should use the correct constant");
	}

	/// <summary>
	/// Tests force calculation from pressure and area.
	/// </summary>
	[TestMethod]
	public void Pressure_ForceCalculation_InversesCorrectly()
	{
		// Arrange
		Pressure<double> pressure = Pressure<double>.Create(2000.0); // 2000 Pa
		Area<double> area = Area<double>.Create(0.5); // 0.5 m²

		// Act
		Force<double> force = pressure.GetForce(area);
		double forceValue = force.In(Units.Newton);

		// Assert
		Assert.AreEqual(1000.0, forceValue, Tolerance, "F = P×A should give 2000 Pa × 0.5 m² = 1000 N");
	}

	/// <summary>
	/// Tests hydrostatic pressure calculation: P = ρgh
	/// </summary>
	[TestMethod]
	public void Pressure_HydrostaticPressure_CalculatesCorrectly()
	{
		// Arrange - Water density and 10m depth
		Density<double> waterDensity = Density<double>.Create(1000.0); // 1000 kg/m³
		Length<double> depth = Length<double>.Create(10.0); // 10 m

		// Act
		Pressure<double> hydrostaticPressure = Pressure<double>.FromHydrostaticPressure(waterDensity, depth);
		double pressureValue = hydrostaticPressure.In(Units.Pascal);

		// Expected: 1000 kg/m³ × 9.80665 m/s² × 10 m = 98066.5 Pa
		double expectedPressure = 1000.0 * PhysicalConstants.Mechanical.StandardGravity * 10.0;

		// Assert
		Assert.AreEqual(expectedPressure, pressureValue, Tolerance,
			"Hydrostatic pressure should use standard gravity constant");
	}

	/// <summary>
	/// Tests that physical constants are consistent across calculations.
	/// </summary>
	[TestMethod]
	public void PhysicalConstants_MechanicalConstants_AreConsistent()
	{
		// Test that weight calculation and Units.StandardGravity use the same value
		Mass<double> oneMass = Mass<double>.Create(1.0);
		Force<double> weight = Force<double>.FromWeight(oneMass);
		double weightInNewtons = weight.In(Units.Newton);

		// Should equal the standard gravity constant
		Assert.AreEqual(PhysicalConstants.Mechanical.StandardGravity, weightInNewtons, Tolerance,
			"Weight calculation should be consistent with standard gravity constant");

		// Test atmospheric pressure consistency
		Pressure<double> standardPressure = Pressure<double>.StandardAtmospheric();
		double pressureInPascals = standardPressure.In(Units.Pascal);

		Assert.AreEqual(PhysicalConstants.Mechanical.StandardAtmosphericPressure, pressureInPascals, Tolerance,
			"Standard atmospheric pressure should be consistent");
	}
}
