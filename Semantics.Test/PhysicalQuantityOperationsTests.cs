// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

[TestClass]
public class PhysicalQuantityOperationsTests
{
	[TestMethod]
	public void VelocityFromLengthAndTime_ShouldCalculateCorrectly()
	{
		// Arrange
		Length length = 100.Meters();
		Time time = 10.Seconds();

		// Act
		Velocity velocity = length / time;

		// Assert
		Assert.AreEqual(10.0, velocity.MetersPerSecond<double>(), 1e-10);
	}

	[TestMethod]
	public void AccelerationFromVelocityAndTime_ShouldCalculateCorrectly()
	{
		// Arrange
		Velocity velocity = 20.MetersPerSecond();
		Time time = 5.Seconds();

		// Act
		Acceleration acceleration = velocity / time;

		// Assert
		Assert.AreEqual(4.0, acceleration.MetersPerSecondSquared<double>(), 1e-10);
	}

	[TestMethod]
	public void ForceFromMassAndAcceleration_ShouldCalculateCorrectly()
	{
		// Arrange
		Mass mass = 10.Kilograms();
		Acceleration acceleration = 9.8.MetersPerSecondSquared();

		// Act
		Force force = mass * acceleration;

		// Assert
		Assert.AreEqual(98.0, force.Newtons<double>(), 1e-10);
	}

	[TestMethod]
	public void EnergyFromForceAndLength_ShouldCalculateCorrectly()
	{
		// Arrange
		Force force = 50.Newtons();
		Length length = 2.Meters();

		// Act
		Energy energy = force * length;

		// Assert
		Assert.AreEqual(100.0, energy.Joules<double>(), 1e-10);
	}

	[TestMethod]
	public void PowerFromEnergyAndTime_ShouldCalculateCorrectly()
	{
		// Arrange
		Energy energy = 1000.Joules();
		Time time = 5.Seconds();

		// Act
		Power power = energy / time;

		// Assert
		Assert.AreEqual(200.0, power.Watts<double>(), 1e-10);
	}

	[TestMethod]
	public void AreaFromLengthSquared_ShouldCalculateCorrectly()
	{
		// Arrange
		Length length = 5.Meters();

		// Act
		Area area = length * length;

		// Assert
		Assert.AreEqual(25.0, area.SquareMeters<double>(), 1e-10);
	}

	[TestMethod]
	public void VolumeFromAreaAndLength_ShouldCalculateCorrectly()
	{
		// Arrange
		Area area = 25.SquareMeters();
		Length height = 4.Meters();

		// Act
		Volume volume = area * height;

		// Assert
		Assert.AreEqual(100.0, volume.CubicMeters<double>(), 1e-10);
	}

	[TestMethod]
	public void DensityFromMassAndVolume_ShouldCalculateCorrectly()
	{
		// Arrange
		Mass mass = 50.Kilograms();
		Volume volume = 5.CubicMeters();

		// Act
		Density density = mass / volume;

		// Assert
		Assert.AreEqual(10.0, density.KilogramsPerCubicMeter<double>(), 1e-10);
	}

	[TestMethod]
	public void MassFromDensityAndVolume_ShouldCalculateCorrectly()
	{
		// Arrange
		Density density = 800.KilogramsPerCubicMeter();
		Volume volume = 0.5.CubicMeters();

		// Act
		Mass mass = density * volume;

		// Assert
		Assert.AreEqual(400.0, mass.Kilograms<double>(), 1e-10);
	}

	[TestMethod]
	public void PressureFromForceAndArea_ShouldCalculateCorrectly()
	{
		// Arrange
		Force force = 1000.Newtons();
		Area area = 10.SquareMeters();

		// Act
		Pressure pressure = force / area;

		// Assert
		Assert.AreEqual(100.0, pressure.Pascals<double>(), 1e-10);
	}

	[TestMethod]
	public void ForceFromPressureAndArea_ShouldCalculateCorrectly()
	{
		// Arrange
		Pressure pressure = 200.Pascals();
		Area area = 5.SquareMeters();

		// Act
		Force force = pressure * area;

		// Assert
		Assert.AreEqual(1000.0, force.Newtons<double>(), 1e-10);
	}

	[TestMethod]
	public void MomentumFromMassAndVelocity_ShouldCalculateCorrectly()
	{
		// Arrange
		Mass mass = 5.Kilograms();
		Velocity velocity = 10.MetersPerSecond();

		// Act
		Momentum momentum = mass * velocity;

		// Assert
		Assert.AreEqual(50.0, momentum.KilogramMetersPerSecond<double>(), 1e-10);
	}

	[TestMethod]
	public void ElectricPotentialFromCurrentAndResistance_ShouldCalculateCorrectly()
	{
		// Arrange
		ElectricCurrent current = 2.Amperes();
		Resistance resistance = 10.Ohms();

		// Act
		ElectricPotential voltage = current * resistance;

		// Assert
		Assert.AreEqual(20.0, voltage.Volts<double>(), 1e-10);
	}

	[TestMethod]
	public void PowerFromVoltageAndCurrent_ShouldCalculateCorrectly()
	{
		// Arrange
		ElectricPotential voltage = 12.Volts();
		ElectricCurrent current = 3.Amperes();

		// Act
		Power power = voltage * current;

		// Assert
		Assert.AreEqual(36.0, power.Watts<double>(), 1e-10);
	}

	[TestMethod]
	public void ComplexCalculation_ShouldWorkCorrectly()
	{
		// Arrange - Calculate kinetic energy: KE = 0.5 * m * v^2
		Mass mass = 10.Kilograms();
		Velocity velocity = 20.MetersPerSecond();

		// Act
		Energy kineticEnergy = 0.5 * mass * velocity * velocity;

		// Assert
		Assert.AreEqual(2000.0, kineticEnergy.Joules<double>(), 1e-10);
	}

	[TestMethod]
	public void DistanceWithAcceleration_ShouldCalculateCorrectly()
	{
		// Arrange - Calculate distance: s = v*t + 0.5*a*t^2
		Velocity initialVelocity = 5.MetersPerSecond();
		Acceleration acceleration = 2.MetersPerSecondSquared();
		Time time = 10.Seconds();

		// Act
		Length distance = (initialVelocity * time) + (0.5 * acceleration * time * time);

		// Assert
		Assert.AreEqual(150.0, distance.Meters<double>(), 1e-10);
	}
}
