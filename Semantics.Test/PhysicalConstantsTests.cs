// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

[TestClass]
public class PhysicalConstantsTests
{
	[TestMethod]
	public void MetricPrefixes_ShouldHaveCorrectValues()
	{
		Assert.AreEqual(1e1, PhysicalConstants.Deca, 1e-10);
		Assert.AreEqual(1e2, PhysicalConstants.Hecto, 1e-10);
		Assert.AreEqual(1e3, PhysicalConstants.Kilo, 1e-10);
		Assert.AreEqual(1e6, PhysicalConstants.Mega, 1e-10);
		Assert.AreEqual(1e9, PhysicalConstants.Giga, 1e-10);
		Assert.AreEqual(1e12, PhysicalConstants.Tera, 1e-10);
		Assert.AreEqual(1e15, PhysicalConstants.Peta, 1e-10);
		Assert.AreEqual(1e18, PhysicalConstants.Exa, 1e-10);
		Assert.AreEqual(1e21, PhysicalConstants.Zetta, 1e-10);
		Assert.AreEqual(1e24, PhysicalConstants.Yotta, 1e-10);
		Assert.AreEqual(1e-1, PhysicalConstants.Deci, 1e-10);
		Assert.AreEqual(1e-2, PhysicalConstants.Centi, 1e-10);
		Assert.AreEqual(1e-3, PhysicalConstants.Milli, 1e-10);
		Assert.AreEqual(1e-6, PhysicalConstants.Micro, 1e-10);
		Assert.AreEqual(1e-9, PhysicalConstants.Nano, 1e-10);
		Assert.AreEqual(1e-12, PhysicalConstants.Pico, 1e-10);
		Assert.AreEqual(1e-15, PhysicalConstants.Femto, 1e-10);
		Assert.AreEqual(1e-18, PhysicalConstants.Atto, 1e-10);
		Assert.AreEqual(1e-21, PhysicalConstants.Zepto, 1e-10);
		Assert.AreEqual(1e-24, PhysicalConstants.Yocto, 1e-10);
	}

	[TestMethod]
	public void LengthConversionFactors_ShouldHaveCorrectValues()
	{
		Assert.AreEqual(0.3048, PhysicalConstants.FeetToMetersFactor, 1e-10);
		Assert.AreEqual(0.0254, PhysicalConstants.InchesToMetersFactor, 1e-10);
		Assert.AreEqual(0.9144, PhysicalConstants.YardsToMetersFactor, 1e-10);
		Assert.AreEqual(1609.344, PhysicalConstants.MilesToMetersFactor, 1e-10);
		Assert.AreEqual(1852, PhysicalConstants.NauticalMilesToMetersFactor, 1e-10);
		Assert.AreEqual(1.8288, PhysicalConstants.FathomsToMetersFactor, 1e-10);
		Assert.AreEqual(1.495978707e11, PhysicalConstants.AstronomicalUnitsToMetersFactor, 1e-10);
		Assert.AreEqual(9.4607304725808e15, PhysicalConstants.LightYearsToMetersFactor, 1e-10);
		Assert.AreEqual(3.08567758149137e16, PhysicalConstants.ParsecsToMetersFactor, 1e-10);
	}

	[TestMethod]
	public void AngularConversionFactors_ShouldHaveCorrectValues()
	{
		Assert.AreEqual(0.01745329251994329576923690768489, PhysicalConstants.DegreesToRadiansFactor, 1e-10);
		Assert.AreEqual(0.0157079632679489661923132169164, PhysicalConstants.GradiansToRadiansFactor, 1e-10);
		Assert.AreEqual(0.00029088820866572159615394846141459, PhysicalConstants.MinutesToRadiansFactor, 1e-10);
		Assert.AreEqual(4.8481368110953599358991410235795e-6, PhysicalConstants.SecondsToRadiansFactor, 1e-10);
		Assert.AreEqual(6.283185307179586476925286766559, PhysicalConstants.RevolutionsToRadiansFactor, 1e-10);
	}

	[TestMethod]
	public void MassConversionFactors_ShouldHaveCorrectValues()
	{
		Assert.AreEqual(0.45359237, PhysicalConstants.PoundsToKilogramsFactor, 1e-10);
		Assert.AreEqual(0.028349523125, PhysicalConstants.OuncesToKilogramsFactor, 1e-10);
		Assert.AreEqual(6.35029318, PhysicalConstants.StonesToKilogramsFactor, 1e-10);
		Assert.AreEqual(1016.0469088, PhysicalConstants.ImperialTonsToKilogramsFactor, 1e-10);
		Assert.AreEqual(907.18474, PhysicalConstants.USTonsToKilogramsFactor, 1e-10);
		Assert.AreEqual(1000, PhysicalConstants.MetricTonsToKilogramsFactor, 1e-10);
	}

	[TestMethod]
	public void TemperatureConversionFactors_ShouldHaveCorrectValues()
	{
		Assert.AreEqual(1, PhysicalConstants.CelsiusToKelvinFactor, 1e-10);
		Assert.AreEqual(273.15, PhysicalConstants.CelsiusToKelvinOffset, 1e-10);
		Assert.AreEqual(9.0 / 5.0, PhysicalConstants.FahrenheitToCelsiusFactor, 1e-10);
		Assert.AreEqual(32, PhysicalConstants.FahrenheitToCelsiusOffset, 1e-10);
	}
}
