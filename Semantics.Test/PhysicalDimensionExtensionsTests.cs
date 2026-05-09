// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class PhysicalDimensionExtensionsTests
{
	private const double Tolerance = 1e-10;

	[TestMethod]
	public void Length_Extensions_ConvertToBaseUnits()
	{
		Assert.AreEqual(1.0, 1.0.Meters().Value, Tolerance);
		Assert.AreEqual(1000.0, 1.0.Kilometers().Value, Tolerance);
		Assert.AreEqual(0.01, 1.0.Centimeters().Value, Tolerance);
		Assert.AreEqual(0.001, 1.0.Millimeters().Value, Tolerance);
		Assert.AreEqual(0.3048, 1.0.Feet().Value, Tolerance);
		Assert.AreEqual(0.0254, 1.0.Inches().Value, Tolerance);
		Assert.AreEqual(0.9144, 1.0.Yards().Value, Tolerance);
		Assert.AreEqual(1609.344, 1.0.Miles().Value, Tolerance);
	}

	[TestMethod]
	public void Mass_Extensions_ConvertToBaseUnits()
	{
		Assert.AreEqual(1.0, 1.0.Kilograms().Value, Tolerance);
		Assert.AreEqual(0.001, 1.0.Grams().Value, Tolerance);
		Assert.AreEqual(0.45359237, 1.0.Pounds().Value, 1e-6);
		Assert.AreEqual(0.028349523125, 1.0.Ounces().Value, 1e-6);
		Assert.AreEqual(6.35029318, 1.0.Stones().Value, 1e-5);
	}

	[TestMethod]
	public void Time_Extensions_ConvertToBaseUnits()
	{
		Assert.AreEqual(1.0, 1.0.Seconds().Value, Tolerance);
		Assert.AreEqual(60.0, 1.0.Minutes().Value, Tolerance);
		Assert.AreEqual(3600.0, 1.0.Hours().Value, Tolerance);
		Assert.AreEqual(86400.0, 1.0.Days().Value, Tolerance);
		Assert.AreEqual(0.001, 1.0.Milliseconds().Value, Tolerance);
	}

	[TestMethod]
	public void Area_Extensions_ConvertToBaseUnits()
	{
		Assert.AreEqual(1.0, 1.0.SquareMeters().Value, Tolerance);
		Assert.AreEqual(1_000_000.0, 1.0.SquareKilometers().Value, Tolerance);
		Assert.AreEqual(0.09290304, 1.0.SquareFeet().Value, 1e-10);
		Assert.AreEqual(4046.8564224, 1.0.Acres().Value, 1e-7);
	}

	[TestMethod]
	public void Volume_Extensions_ConvertToBaseUnits()
	{
		Assert.AreEqual(1.0, 1.0.CubicMeters().Value, Tolerance);
		Assert.AreEqual(0.001, 1.0.Liters().Value, Tolerance);
		Assert.AreEqual(0.000001, 1.0.Milliliters().Value, Tolerance);
		Assert.AreEqual(0.028316846592, 1.0.CubicFeet().Value, 1e-7);
		Assert.AreEqual(0.003785411784, 1.0.USGallons().Value, 1e-8);
	}
}

