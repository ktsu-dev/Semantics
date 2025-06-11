// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

[TestClass]
public class PhysicalQuantityExtensionMethodTests
{
	[TestMethod]
	public void NumericExtensions_ShouldCreateLengthQuantities()
	{
		// Act
		Length meters = 10.Meters();
		Length kilometers = 5.Kilometers();
		Length millimeters = 200.Millimeters();
		Length centimeters = 30.Centimeters();
		Length inches = 12.Inches();
		Length feet = 6.Feet();
		Length yards = 3.Yards();
		Length miles = 2.Miles();

		// Assert
		Assert.AreEqual(10.0, meters.Meters<double>(), 1e-10);
		Assert.AreEqual(5.0, kilometers.Kilometers<double>(), 1e-10);
		Assert.AreEqual(200.0, millimeters.Millimeters<double>(), 1e-10);
		Assert.AreEqual(30.0, centimeters.Centimeters<double>(), 1e-10);
		Assert.AreEqual(12.0, inches.Inches<double>(), 1e-10);
		Assert.AreEqual(6.0, feet.Feet<double>(), 1e-10);
		Assert.AreEqual(3.0, yards.Yards<double>(), 1e-10);
		Assert.AreEqual(2.0, miles.Miles<double>(), 1e-10);
	}

	[TestMethod]
	public void NumericExtensions_ShouldCreateTimeQuantities()
	{
		// Act
		Time seconds = 60.Seconds();
		Time minutes = 5.Minutes();
		Time hours = 2.Hours();
		Time days = 1.Days();
		Time milliseconds = 500.Milliseconds();

		// Assert
		Assert.AreEqual(60.0, seconds.Seconds<double>(), 1e-10);
		Assert.AreEqual(5.0, minutes.Minutes<double>(), 1e-10);
		Assert.AreEqual(2.0, hours.Hours<double>(), 1e-10);
		Assert.AreEqual(1.0, days.Days<double>(), 1e-10);
		Assert.AreEqual(500.0, milliseconds.Milliseconds<double>(), 1e-10);
	}

	[TestMethod]
	public void NumericExtensions_ShouldCreateMassQuantities()
	{
		// Act
		Mass kilograms = 75.Kilograms();
		Mass grams = 500.Grams();
		Mass pounds = 180.Pounds();
		Mass ounces = 16.Ounces();
		Mass stones = 12.Stones();

		// Assert
		Assert.AreEqual(75.0, kilograms.Kilograms<double>(), 1e-10);
		Assert.AreEqual(500.0, grams.Grams<double>(), 1e-10);
		Assert.AreEqual(180.0, pounds.Pounds<double>(), 1e-10);
		Assert.AreEqual(16.0, ounces.Ounces<double>(), 1e-10);
		Assert.AreEqual(12.0, stones.Stones<double>(), 1e-10);
	}

	[TestMethod]
	public void NumericExtensions_ShouldCreateTemperatureQuantities()
	{
		// Act
		Temperature kelvin = 300.Kelvin();
		Temperature celsius = 25.Celsius();
		Temperature fahrenheit = 70.Fahrenheit();

		// Assert
		Assert.AreEqual(300.0, kelvin.Kelvin<double>(), 1e-10);
		Assert.AreEqual(25.0, celsius.Celsius<double>(), 1e-10);
		Assert.AreEqual(70.0, fahrenheit.Fahrenheit<double>(), 1e-10);
	}

	[TestMethod]
	public void NumericExtensions_ShouldCreateAreaQuantities()
	{
		// Act
		Area squareMeters = 50.SquareMeters();
		Area squareKilometers = 2.SquareKilometers();
		Area squareFeet = 100.SquareFeet();
		Area acres = 5.Acres();

		// Assert
		Assert.AreEqual(50.0, squareMeters.SquareMeters<double>(), 1e-10);
		Assert.AreEqual(2.0, squareKilometers.SquareKilometers<double>(), 1e-10);
		Assert.AreEqual(100.0, squareFeet.SquareFeet<double>(), 1e-10);
		Assert.AreEqual(5.0, acres.Acres<double>(), 1e-10);
	}

	[TestMethod]
	public void NumericExtensions_ShouldCreateVolumeQuantities()
	{
		// Act
		Volume cubicMeters = 5.CubicMeters();
		Volume liters = 1000.Liters();
		Volume milliliters = 500.Milliliters();
		Volume cubicFeet = 35.CubicFeet();
		Volume gallons = 10.USGallons();

		// Assert
		Assert.AreEqual(5.0, cubicMeters.CubicMeters<double>(), 1e-10);
		Assert.AreEqual(1000.0, liters.Liters<double>(), 1e-10);
		Assert.AreEqual(500.0, milliliters.Milliliters<double>(), 1e-10);
		Assert.AreEqual(35.0, cubicFeet.CubicFeet<double>(), 1e-10);
		Assert.AreEqual(10.0, gallons.USGallons<double>(), 1e-10);
	}

	[TestMethod]
	public void NumericExtensions_ShouldCreateAngleQuantities()
	{
		// Act
		Angle radians = (Math.PI / 4).Radians();
		Angle degrees = 45.Degrees();
		Angle gradians = 50.Gradians();
		Angle revolutions = 0.5.Revolutions();

		// Assert
		Assert.AreEqual(Math.PI / 4, radians.Radians<double>(), 1e-10);
		Assert.AreEqual(45.0, degrees.Degrees<double>(), 1e-10);
		Assert.AreEqual(50.0, gradians.Gradians<double>(), 1e-10);
		Assert.AreEqual(0.5, revolutions.Revolutions<double>(), 1e-10);

		// Confirm conversions between angles work correctly
		Assert.AreEqual(45.0, radians.Degrees<double>(), 1e-10);
		Assert.AreEqual(Math.PI / 4, degrees.Radians<double>(), 1e-10);
	}

	[TestMethod]
	public void NumericExtensions_ShouldCreateVelocityQuantities()
	{
		// Act
		Velocity metersPerSecond = 10.MetersPerSecond();
		Velocity kilometersPerHour = 36.KilometersPerHour();
		Velocity milesPerHour = 60.MilesPerHour();
		Velocity knots = 20.Knots();

		// Assert
		Assert.AreEqual(10.0, metersPerSecond.MetersPerSecond<double>(), 1e-10);
		Assert.AreEqual(36.0, kilometersPerHour.KilometersPerHour<double>(), 1e-10);
		Assert.AreEqual(60.0, milesPerHour.MilesPerHour<double>(), 1e-10);
		Assert.AreEqual(20.0, knots.Knots<double>(), 1e-10);

		// Check that 36 km/h is approximately 10 m/s
		Assert.AreEqual(10.0, kilometersPerHour.MetersPerSecond<double>(), 1e-2);
	}

	[TestMethod]
	public void NumericExtensions_ShouldCreateForceQuantities()
	{
		// Act
		Force newtons = 100.Newtons();
		Force poundForce = 25.PoundForce();
		Force kiloNewtons = 5.KiloNewtons();

		// Assert
		Assert.AreEqual(100.0, newtons.Newtons<double>(), 1e-10);
		Assert.AreEqual(25.0, poundForce.PoundForce<double>(), 1e-10);
		Assert.AreEqual(5.0, kiloNewtons.KiloNewtons<double>(), 1e-10);
	}

	[TestMethod]
	public void NumericExtensions_ShouldCreateEnergyQuantities()
	{
		// Act
		Energy joules = 1000.Joules();
		Energy kilojoules = 2.Kilojoules();
		Energy calories = 500.Calories();
		Energy kilocalories = 2.5.Kilocalories();
		Energy wattHours = 250.WattHours();
		Energy kilowattHours = 1.5.KilowattHours();

		// Assert
		Assert.AreEqual(1000.0, joules.Joules<double>(), 1e-10);
		Assert.AreEqual(2.0, kilojoules.Kilojoules<double>(), 1e-10);
		Assert.AreEqual(500.0, calories.Calories<double>(), 1e-10);
		Assert.AreEqual(2.5, kilocalories.Kilocalories<double>(), 1e-10);
		Assert.AreEqual(250.0, wattHours.WattHours<double>(), 1e-10);
		Assert.AreEqual(1.5, kilowattHours.KilowattHours<double>(), 1e-10);
	}

	[TestMethod]
	public void NumericExtensions_ShouldCreatePowerQuantities()
	{
		// Act
		Power watts = 100.Watts();
		Power kilowatts = 2.5.Kilowatts();
		Power horsepower = 10.Horsepower();

		// Assert
		Assert.AreEqual(100.0, watts.Watts<double>(), 1e-10);
		Assert.AreEqual(2.5, kilowatts.Kilowatts<double>(), 1e-10);
		Assert.AreEqual(10.0, horsepower.Horsepower<double>(), 1e-10);
	}

	[TestMethod]
	public void GenericConversion_ShouldWorkWithDifferentNumericTypes()
	{
		// Act
		Length lengthFromInt = 10.Meters();
		Length lengthFromDouble = 10.0.Meters();
		Length lengthFromFloat = 10f.Meters();
		Length lengthFromDecimal = 10m.Meters();

		// Assert
		Assert.AreEqual(10, lengthFromInt.Meters<int>());
		Assert.AreEqual(10.0, lengthFromDouble.Meters<double>(), 1e-10);
		Assert.AreEqual(10f, lengthFromFloat.Meters<float>(), 1e-6f);
		Assert.AreEqual(10m, lengthFromDecimal.Meters<decimal>());
	}

	[TestMethod]
	public void QuantityConversion_ShouldRoundTrip()
	{
		// Arrange
		Length originalLength = 100.Meters();

		// Act - Convert to feet and back to meters
		double feet = originalLength.Feet<double>();
		Length backToMeters = feet.Feet();

		// Assert
		Assert.AreEqual(100.0, backToMeters.Meters<double>(), 1e-10);
	}
}
