// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Quantities;

using ktsu.Semantics.Quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Verifies that <c>QuantitiesGenerator</c> emits one <c>From{Unit}</c> factory for every
/// unit listed in a dimension's <c>availableUnits</c>, applying the conversion factor.
/// Issue #48.
/// </summary>
[TestClass]
public sealed class MultiUnitFactoryTests
{
	private const double Tolerance = 1e-9;

	// ---- Length ----

	[TestMethod]
	public void Length_FromMeter_Identity()
	{
		Length<double> l = Length<double>.FromMeter(1.0);
		Assert.AreEqual(1.0, l.Value, Tolerance);
	}

	[TestMethod]
	public void Length_FromKilometer_Scales_By_1000()
	{
		Length<double> l = Length<double>.FromKilometer(1.0);
		Assert.AreEqual(1000.0, l.Value, Tolerance);
	}

	[TestMethod]
	public void Length_FromCentimeter_Scales_By_0_01()
	{
		Length<double> l = Length<double>.FromCentimeter(1.0);
		Assert.AreEqual(0.01, l.Value, Tolerance);
	}

	[TestMethod]
	public void Length_FromMillimeter_Scales_By_0_001()
	{
		Length<double> l = Length<double>.FromMillimeter(1.0);
		Assert.AreEqual(0.001, l.Value, Tolerance);
	}

	[TestMethod]
	public void Length_FromFoot_Uses_FeetToMeters_Constant()
	{
		Length<double> l = Length<double>.FromFoot(1.0);
		Assert.AreEqual(0.3048, l.Value, Tolerance);
	}

	[TestMethod]
	public void Length_FromInch_Uses_InchesToMeters_Constant()
	{
		Length<double> l = Length<double>.FromInch(1.0);
		Assert.AreEqual(0.0254, l.Value, Tolerance);
	}

	[TestMethod]
	public void Length_FromMile_Uses_MileToMeters_Constant()
	{
		Length<double> l = Length<double>.FromMile(1.0);
		Assert.AreEqual(1609.344, l.Value, Tolerance);
	}

	// ---- Mass ----

	[TestMethod]
	public void Mass_FromKilogram_Identity()
	{
		Mass<double> m = Mass<double>.FromKilogram(1.0);
		Assert.AreEqual(1.0, m.Value, Tolerance);
	}

	[TestMethod]
	public void Mass_FromGram_Scales_By_0_001()
	{
		Mass<double> m = Mass<double>.FromGram(1.0);
		Assert.AreEqual(0.001, m.Value, Tolerance);
	}

	[TestMethod]
	public void Mass_FromPound_Uses_PoundToKilograms_Constant()
	{
		Mass<double> m = Mass<double>.FromPound(1.0);
		Assert.AreEqual(0.45359237, m.Value, Tolerance);
	}

	// ---- Time / Duration ----

	[TestMethod]
	public void Duration_FromMinute_Equals_60_Seconds()
	{
		Duration<double> d = Duration<double>.FromMinute(1.0);
		Assert.AreEqual(60.0, d.Value, Tolerance);
	}

	[TestMethod]
	public void Duration_FromHour_Equals_3600_Seconds()
	{
		Duration<double> d = Duration<double>.FromHour(1.0);
		Assert.AreEqual(3600.0, d.Value, Tolerance);
	}

	// ---- Semantic overloads inherit their dimension's full unit set ----

	[TestMethod]
	public void Distance_FromKilometer_Scales_By_1000()
	{
		Distance<double> d = Distance<double>.FromKilometer(1.0);
		Assert.AreEqual(1000.0, d.Value, Tolerance);
	}

	[TestMethod]
	public void Diameter_FromMillimeter_Scales_By_0_001()
	{
		Diameter<double> d = Diameter<double>.FromMillimeter(1.0);
		Assert.AreEqual(0.001, d.Value, Tolerance);
	}

	[TestMethod]
	public void Wavelength_FromNanometer_Uses_Nano_Magnitude()
	{
		Wavelength<double> w = Wavelength<double>.FromNanometer(550.0);
		Assert.AreEqual(550.0e-9, w.Value, 1e-15);
	}

	// ---- Storage genericity ----

	[TestMethod]
	public void Length_FromKilometer_Works_With_Float()
	{
		Length<float> l = Length<float>.FromKilometer(1.0f);
		Assert.AreEqual(1000.0f, l.Value, 1e-3f);
	}

	[TestMethod]
	public void Length_FromFoot_Works_With_Decimal()
	{
		Length<decimal> l = Length<decimal>.FromFoot(1m);
		Assert.AreEqual(0.3048m, l.Value);
	}
}
