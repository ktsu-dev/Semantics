// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

[TestClass]
public class PhysicalQuantityTests
{
	[TestMethod]
	public void Length_BasicConversions_WorkCorrectly()
	{
		// Arrange & Act
		Length meters = 1.0.Meters();
		Length centimeters = 100.0.Centimeters();
		Length feet = 3.28084.Feet();

		// Assert
		Assert.AreEqual(1.0, meters.Meters<double>(), 1e-10);
		Assert.AreEqual(100.0, centimeters.Centimeters<double>(), 1e-10);
		Assert.AreEqual(3.28084, feet.Feet<double>(), 1e-5);

		// Test conversions
		Assert.AreEqual(100.0, meters.Centimeters<double>(), 1e-10);
		Assert.AreEqual(1.0, centimeters.Meters<double>(), 1e-10);
		Assert.AreEqual(1.0, feet.Meters<double>(), 1e-5);
	}

	[TestMethod]
	public void Length_ArithmeticOperations_WorkCorrectly()
	{
		// Arrange
		Length length1 = 5.0.Meters();
		Length length2 = 3.0.Meters();

		// Act & Assert
		Length sum = length1 + length2;
		Assert.AreEqual(8.0, sum.Meters<double>());

		Length difference = length1 - length2;
		Assert.AreEqual(2.0, difference.Meters<double>());

		Length scaled = length1 * 2.0;
		Assert.AreEqual(10.0, scaled.Meters<double>());

		Length divided = length1 / 2.0;
		Assert.AreEqual(2.5, divided.Meters<double>());
	}

	[TestMethod]
	public void Length_MultiplicationCreatesArea_WorksCorrectly()
	{
		// Arrange
		Length length1 = 5.0.Meters();
		Length length2 = 4.0.Meters();

		// Act
		Area area = length1 * length2;

		// Assert
		Assert.IsInstanceOfType<Area>(area);
		Assert.AreEqual(20.0, area.SquareMeters<double>());
	}

	[TestMethod]
	public void Area_BasicConversions_WorkCorrectly()
	{
		// Arrange & Act
		Area squareMeters = 1.0.SquareMeters();
		Area squareFeet = 10.7639.SquareFeet();

		// Assert
		Assert.AreEqual(1.0, squareMeters.SquareMeters<double>(), 1e-10);
		Assert.AreEqual(10.7639, squareFeet.SquareFeet<double>(), 1e-4);

		// Test conversion
		Assert.AreEqual(1.0, squareFeet.SquareMeters<double>(), 1e-4);
	}

	[TestMethod]
	public void Area_MultiplicationWithLengthCreatesVolume_WorksCorrectly()
	{
		// Arrange
		Area area = 10.0.SquareMeters();
		Length height = 3.0.Meters();

		// Act
		Volume volume = area * height;

		// Assert
		Assert.IsInstanceOfType<Volume>(volume);
		Assert.AreEqual(30.0, volume.CubicMeters<double>());
	}

	[TestMethod]
	public void Volume_BasicConversions_WorkCorrectly()
	{
		// Arrange & Act
		Volume cubicMeters = 1.0.CubicMeters();
		Volume liters = 1000.0.Liters();

		// Assert
		Assert.AreEqual(1.0, cubicMeters.CubicMeters<double>(), 1e-10);
		Assert.AreEqual(1000.0, liters.Liters<double>(), 1e-10);

		// Test conversion
		Assert.AreEqual(1.0, liters.CubicMeters<double>(), 1e-10);
	}

	[TestMethod]
	public void Time_BasicConversions_WorkCorrectly()
	{
		// Arrange & Act
		Time seconds = 1.0.Seconds();
		Time minutes = 1.0.Minutes();
		Time hours = 1.0.Hours();

		// Assert
		Assert.AreEqual(1.0, seconds.Seconds<double>(), 1e-10);
		Assert.AreEqual(1.0, minutes.Minutes<double>(), 1e-10);
		Assert.AreEqual(1.0, hours.Hours<double>(), 1e-10);

		// Test conversions
		Assert.AreEqual(60.0, minutes.Seconds<double>(), 1e-10);
		Assert.AreEqual(3600.0, hours.Seconds<double>(), 1e-10);
		Assert.AreEqual(60.0, hours.Minutes<double>(), 1e-10);
	}

	[TestMethod]
	public void PhysicalQuantity_Comparison_WorksCorrectly()
	{
		// Arrange
		Length length1 = 5.0.Meters();
		Length length2 = 3.0.Meters();
		Length length3 = 5.0.Meters();

		// Act & Assert
		Assert.IsTrue(length1 > length2);
		Assert.IsTrue(length2 < length1);
		Assert.IsTrue(length1 >= length3);
		Assert.IsTrue(length1 <= length3);
		Assert.IsTrue(length1 == length3);
		Assert.IsFalse(length1 == length2);
	}

	[TestMethod]
	public void PhysicalQuantity_StringRepresentation_WorksCorrectly()
	{
		// Arrange
		Length length = 5.0.Meters();
		Area area = 10.0.SquareMeters();
		Time time = 30.0.Seconds();

		// Act
		string lengthString = length.ToString();
		string areaString = area.ToString();
		string timeString = time.ToString();

		// Assert
		Assert.IsTrue(lengthString.Contains('5'));
		Assert.IsTrue(lengthString.Contains('m'));
		Assert.IsTrue(areaString.Contains("10"));
		Assert.IsTrue(areaString.Contains("m²"));
		Assert.IsTrue(timeString.Contains("30"));
		Assert.IsTrue(timeString.Contains('s'));
	}

	[TestMethod]
	public void PhysicalQuantity_AbsAndClamp_WorkCorrectly()
	{
		// Arrange
		Length negativeLength = (-5.0).Meters();
		Length positiveLength = 10.0.Meters();

		// Act
		Length absoluteLength = negativeLength.Abs();
		Length clampedLength = positiveLength.Clamp(2.0, 8.0);

		// Assert
		Assert.AreEqual(5.0, absoluteLength.Meters<double>());
		Assert.AreEqual(8.0, clampedLength.Meters<double>());
	}

	[TestMethod]
	public void PhysicalQuantity_Power_WorksCorrectly()
	{
		// Arrange
		Length length = 3.0.Meters();

		// Act
		Length squared = length.Pow(2);
		Length cubed = length.Pow(3);

		// Assert
		Assert.AreEqual(9.0, squared.Meters<double>());
		Assert.AreEqual(27.0, cubed.Meters<double>());
	}

	[TestMethod]
	public void PhysicalQuantity_CompareTo_WorksCorrectly()
	{
		// Arrange
		Length length1 = 5.0.Meters();
		Length length2 = 3.0.Meters();
		Length length3 = 5.0.Meters();

		// Act & Assert
		Assert.IsTrue(length1.CompareTo(length2) > 0);
		Assert.IsTrue(length2.CompareTo(length1) < 0);
		Assert.AreEqual(0, length1.CompareTo(length3));
	}

	[TestMethod]
	public void PhysicalQuantity_ImperialConversions_WorkCorrectly()
	{
		// Arrange & Act
		Length inches = 12.0.Inches();
		Length feet = 1.0.Feet();
		Length yards = (1.0 / 3.0).Yards();

		// Assert - all should be approximately equal
		Assert.AreEqual(feet.Meters<double>(), inches.Meters<double>(), 1e-10);
		Assert.AreEqual(feet.Meters<double>(), yards.Meters<double>(), 1e-10);
	}

	[TestMethod]
	public void PhysicalQuantity_MetricPrefixes_WorkCorrectly()
	{
		// Arrange & Act
		Length meters = 1.0.Meters();
		Length kilometers = 0.001.Kilometers();
		Length centimeters = 100.0.Centimeters();
		Length millimeters = 1000.0.Millimeters();

		// Assert - all should be equal to 1 meter
		Assert.AreEqual(1.0, meters.Meters<double>(), 1e-10);
		Assert.AreEqual(1.0, kilometers.Meters<double>(), 1e-10);
		Assert.AreEqual(1.0, centimeters.Meters<double>(), 1e-10);
		Assert.AreEqual(1.0, millimeters.Meters<double>(), 1e-10);
	}
}
