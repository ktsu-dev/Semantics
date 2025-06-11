// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.Test.Quantities.Conversions;
using ktsu.Semantics.Conversions;

[TestClass]
public class ConversionTests
{
	[TestMethod]
	public void LengthConversions_MetersToKilometers_ShouldConvertCorrectly()
	{
		// Arrange
		Length length = 1000.0.Meters();

		// Act
		double kilometers = length.ToUnit<double, Length>("kilometers");

		// Assert
		Assert.AreEqual(1.0, kilometers, 1e-10);
	}

	[TestMethod]
	public void LengthConversions_KilometersToMeters_ShouldConvertCorrectly()
	{
		// Arrange
		double kilometers = 2.5;

		// Act
		Length length = kilometers.ToQuantity<double, Length>("kilometers");
		double meters = length.ToUnit<double, Length>("meters");

		// Assert
		Assert.AreEqual(2500.0, meters, 1e-10);
	}

	[TestMethod]
	public void LengthConversions_FeetToMeters_ShouldConvertCorrectly()
	{
		// Arrange
		double feet = 3.28084;

		// Act
		Length length = feet.ToQuantity<double, Length>("feet");
		double meters = length.ToUnit<double, Length>("meters");

		// Assert
		Assert.AreEqual(1.0, meters, 1e-5);
	}

	[TestMethod]
	public void MassConversions_GramsToKilograms_ShouldConvertCorrectly()
	{
		// Arrange
		double grams = 1500.0;

		// Act
		Mass mass = grams.ToQuantity<double, Mass>("grams");
		double kilograms = mass.ToUnit<double, Mass>("kilograms");

		// Assert
		Assert.AreEqual(1.5, kilograms, 1e-10);
	}

	[TestMethod]
	public void MassConversions_PoundsToKilograms_ShouldConvertCorrectly()
	{
		// Arrange
		double pounds = 2.20462;

		// Act
		Mass mass = pounds.ToQuantity<double, Mass>("pounds");
		double kilograms = mass.ToUnit<double, Mass>("kilograms");

		// Assert
		Assert.AreEqual(1.0, kilograms, 1e-5);
	}

	[TestMethod]
	public void ConversionRegistry_GetAvailableUnits_ShouldReturnExpectedUnits()
	{
		// Act
		IEnumerable<string> lengthUnits = ConversionExtensions.GetAvailableUnits<Length>();
		IEnumerable<string> massUnits = ConversionExtensions.GetAvailableUnits<Mass>();

		// Convert to lists to avoid multiple enumeration warnings
		List<string> lengthUnitsList = [.. lengthUnits];
		List<string> massUnitsList = [.. massUnits];

		// Assert
		Assert.IsTrue(lengthUnitsList.Contains("meters"));
		Assert.IsTrue(lengthUnitsList.Contains("kilometers"));
		Assert.IsTrue(lengthUnitsList.Contains("feet"));
		Assert.IsTrue(lengthUnitsList.Contains("inches"));

		Assert.IsTrue(massUnitsList.Contains("kilograms"));
		Assert.IsTrue(massUnitsList.Contains("grams"));
		Assert.IsTrue(massUnitsList.Contains("pounds"));
		Assert.IsTrue(massUnitsList.Contains("ounces"));
	}

	[TestMethod]
	public void ConversionRegistry_GetBaseUnit_ShouldReturnCorrectBaseUnits()
	{
		// Act
		string lengthBaseUnit = ConversionExtensions.GetBaseUnit<Length>();
		string massBaseUnit = ConversionExtensions.GetBaseUnit<Mass>();

		// Assert
		Assert.AreEqual("meters", lengthBaseUnit);
		Assert.AreEqual("kilograms", massBaseUnit);
	}

	[TestMethod]
	public void ConversionRegistry_Calculator_ShouldReturnSameInstance()
	{
		// Act
		IConversionCalculator<Length> calculator1 = ConversionRegistry.GetCalculator<Length>();
		IConversionCalculator<Length> calculator2 = ConversionRegistry.GetCalculator<Length>();

		// Assert
		Assert.AreSame(calculator1, calculator2);
	}

	[TestMethod]
	public void LengthConversions_TraditionalApi_ShouldStillWork()
	{
		// Arrange & Act
		Length meters = 1000.0.Meters();
		double kilometers = meters.Kilometers<double>();

		// Assert
		Assert.AreEqual(1.0, kilometers, 1e-10);
	}

	[TestMethod]
	public void MassConversions_TraditionalApi_ShouldStillWork()
	{
		// Arrange & Act
		Mass kilograms = 1.5.Kilograms();
		double grams = kilograms.Grams<double>();

		// Assert
		Assert.AreEqual(1500.0, grams, 1e-10);
	}
}
