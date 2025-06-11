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
		var length = 100.Meters();
		var time = 10.Seconds();

		// Act
		var velocity = length / time;

		// Assert
		Assert.AreEqual(10.0, velocity.MetersPerSecond<double>(), 1e-10);
	}

	[TestMethod]
	public void AccelerationFromVelocityAndTime_ShouldCalculateCorrectly()
	{
		// Arrange
		var velocity = 20.MetersPerSecond();
		var time = 5.Seconds();

		// Act
		var acceleration = velocity / time;

		// Assert
		Assert.AreEqual(4.0, acceleration.MetersPerSecondSquared<double>(), 1e-10);
	}

	[TestMethod]
	public void ForceFromMassAndAcceleration_ShouldCalculateCorrectly()
	{
		// Arrange
		var mass = 10.Kilograms();
		var acceleration = 9.8.MetersPerSecondSquared();

		// Act
		var force = mass * acceleration;

		// Assert
		Assert.AreEqual(98.0, force.Newtons<double>(), 1e-10);
	}

	[TestMethod]
	public void EnergyFromForceAndLength_ShouldCalculateCorrectly()
	{
		// Arrange
		var force = 50.Newtons();
		var length = 2.Meters();

		// Act
		var energy = force * length;

		// Assert
		Assert.AreEqual(100.0, energy.Joules<double>(), 1e-10);
	}

	[TestMethod]
	public void PowerFromEnergyAndTime_ShouldCalculateCorrectly()
	{
		// Arrange
		var energy = 1000.Joules();
		var time = 5.Seconds();

		// Act
		var power = energy / time;

		// Assert
		Assert.AreEqual(200.0, power.Watts<double>(), 1e-10);
	}

	[TestMethod]
	public void AreaFromLengthSquared_ShouldCalculateCorrectly()
	{
		// Arrange
		var length = 5.Meters();

		// Act
		var area = length * length;

		// Assert
		Assert.AreEqual(25.0, area.SquareMeters<double>(), 1e-10);
	}

	[TestMethod]
	public void VolumeFromAreaAndLength_ShouldCalculateCorrectly()
	{
		// Arrange
		var area = 25.SquareMeters();
		var height = 4.Meters();

		// Act
		var volume = area * height;

		// Assert
		Assert.AreEqual(100.0, volume.CubicMeters<double>(), 1e-10);
	}

	[TestMethod]
	public void DensityFromMassAndVolume_ShouldCalculateCorrectly()
	{
		// Arrange
		var mass = 50.Kilograms();
		var volume = 5.CubicMeters();

		// Act
		var density = mass / volume;

		// Assert
		Assert.AreEqual(10.0, density.KilogramsPerCubicMeter<double>(), 1e-10);
	}

	[TestMethod]
	public void MassFromDensityAndVolume_ShouldCalculateCorrectly()
	{
		// Arrange
		var density = 800.KilogramsPerCubicMeter();
		var volume = 0.5.CubicMeters();

		// Act
		var mass = density * volume;

		// Assert
		Assert.AreEqual(400.0, mass.Kilograms<double>(), 1e-10);
	}

	[TestMethod]
	public void PressureFromForceAndArea_ShouldCalculateCorrectly()
	{
		// Arrange
		var force = 1000.Newtons();
		var area = 10.SquareMeters();

		// Act
		var pressure = force / area;

		// Assert
		Assert.AreEqual(100.0, pressure.Pascals<double>(), 1e-10);
	}

	[TestMethod]
	public void ForceFromPressureAndArea_ShouldCalculateCorrectly()
	{
		// Arrange
		var pressure = 200.Pascals();
		var area = 5.SquareMeters();

		// Act
		var force = pressure * area;

		// Assert
		Assert.AreEqual(1000.0, force.Newtons<double>(), 1e-10);
	}

	[TestMethod]
	public void MomentumFromMassAndVelocity_ShouldCalculateCorrectly()
	{
		// Arrange
		var mass = 5.Kilograms();
		var velocity = 10.MetersPerSecond();

		// Act
		var momentum = mass * velocity;

		// Assert
		Assert.AreEqual(50.0, momentum.KilogramMetersPerSecond<double>(), 1e-10);
	}

	[TestMethod]
	public void ElectricPotentialFromCurrentAndResistance_ShouldCalculateCorrectly()
	{
		// Arrange
		var current = 2.Amperes();
		var resistance = 10.Ohms();

		// Act
		var voltage = current * resistance;

		// Assert
		Assert.AreEqual(20.0, voltage.Volts<double>(), 1e-10);
	}

	[TestMethod]
	public void PowerFromVoltageAndCurrent_ShouldCalculateCorrectly()
	{
		// Arrange
		var voltage = 12.Volts();
		var current = 3.Amperes();

		// Act
		var power = voltage * current;

		// Assert
		Assert.AreEqual(36.0, power.Watts<double>(), 1e-10);
	}

	[TestMethod]
	public void ComplexCalculation_ShouldWorkCorrectly()
	{
		// Arrange - Calculate kinetic energy: KE = 0.5 * m * v^2
		var mass = 10.Kilograms();
		var velocity = 20.MetersPerSecond();

		// Act
		var kineticEnergy = 0.5 * mass * (velocity * velocity);

		// Assert
		Assert.AreEqual(2000.0, kineticEnergy.Joules<double>(), 1e-10);
	}

	[TestMethod]
	public void DistanceWithAcceleration_ShouldCalculateCorrectly()
	{
		// Arrange - Calculate distance: s = v*t + 0.5*a*t^2
		var initialVelocity = 5.MetersPerSecond();
		var acceleration = 2.MetersPerSecondSquared();
		var time = 10.Seconds();

		// Act
		var distance = (initialVelocity * time) + (0.5 * acceleration * (time * time));

		// Assert
		Assert.AreEqual(150.0, distance.Meters<double>(), 1e-10);
	}
}
