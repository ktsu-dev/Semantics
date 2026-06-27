// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Quantities;

using ktsu.Semantics.Quantities;
using ktsu.Semantics.Quantities.Units;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Covers the unit catalog backfilled from main (imperial/US/CGS/traditional units and
/// SI-prefixed conveniences) plus the corrected Fahrenheit/Rankine affine conversions.
/// </summary>
[TestClass]
public sealed class UnitBackfillTests
{
	private const double Tolerance = 1e-9;

	// ---- Temperature: the affine conversion was inverted before the backfill ----

	[TestMethod]
	public void Temperature_FromFahrenheit_FreezingPoint_Is_273_15_Kelvin()
	{
		Temperature<double> t = Temperature<double>.FromFahrenheit(32.0);
		Assert.AreEqual(273.15, t.Value, Tolerance);
	}

	[TestMethod]
	public void Temperature_FromFahrenheit_BoilingPoint_Is_373_15_Kelvin()
	{
		Temperature<double> t = Temperature<double>.FromFahrenheit(212.0);
		Assert.AreEqual(373.15, t.Value, Tolerance);
	}

	[TestMethod]
	public void Temperature_In_Fahrenheit_RoundTrips()
	{
		Temperature<double> t = Temperature<double>.FromFahrenheit(98.6);
		Assert.AreEqual(98.6, t.In(Units.Fahrenheit), Tolerance);
	}

	[TestMethod]
	public void Temperature_FromRankine_Is_Absolute_With_Fahrenheit_Degrees()
	{
		Temperature<double> t = Temperature<double>.FromRankine(491.67);
		Assert.AreEqual(273.15, t.Value, Tolerance);
	}

	// ---- Length / Area / Volume ----

	[TestMethod]
	public void Length_FromNauticalMiles_Is_1852_Meters()
	{
		Length<double> l = Length<double>.FromNauticalMile(1.0);
		Assert.AreEqual(1852.0, l.Value, Tolerance);
		Assert.AreEqual(1.0, l.In(Units.NauticalMile), Tolerance);
	}

	[TestMethod]
	public void Area_FromAcres_And_Hectares()
	{
		Assert.AreEqual(4046.8564224, Area<double>.FromAcre(1.0).Value, Tolerance);
		Assert.AreEqual(10000.0, Area<double>.FromHectare(1.0).Value, Tolerance);
		Assert.AreEqual(2589988.110336, Area<double>.FromSquareMile(1.0).Value, Tolerance);
		Assert.AreEqual(1e6, Area<double>.FromSquareKilometer(1.0).Value, Tolerance);
	}

	[TestMethod]
	public void Volume_From_Imperial_And_US_Units()
	{
		Assert.AreEqual(0.028316846592, Volume<double>.FromCubicFoot(1.0).Value, Tolerance);
		Assert.AreEqual(0.00454609, Volume<double>.FromImperialGallon(1.0).Value, Tolerance);
		Assert.AreEqual(0.000946352946, Volume<double>.FromUSQuart(1.0).Value, Tolerance);
		Assert.AreEqual(1e-6, Volume<double>.FromCubicCentimeter(1.0).Value, Tolerance);
	}

	// ---- Mass ----

	[TestMethod]
	public void Mass_FromStone_And_ShortTons()
	{
		Assert.AreEqual(6.35029318, Mass<double>.FromStone(1.0).Value, Tolerance);
		Assert.AreEqual(907.18474, Mass<double>.FromShortTon(1.0).Value, Tolerance);
	}

	[TestMethod]
	public void Mass_FromAtomicMassUnits_Is_CODATA_Value()
	{
		Mass<double> m = Mass<double>.FromAtomicMassUnit(1.0);
		Assert.AreEqual(1.66053906660e-27, m.Value, 1e-36);
	}

	// ---- Mechanics ----

	[TestMethod]
	public void Speed_FromKnots_Is_NauticalMilePerHour()
	{
		Speed<double> s = Speed<double>.FromKnot(1.0);
		Assert.AreEqual(1852.0 / 3600.0, s.Value, Tolerance);
	}

	[TestMethod]
	public void AccelerationMagnitude_FromStandardGravity_Is_9_80665()
	{
		AccelerationMagnitude<double> a = AccelerationMagnitude<double>.FromStandardGravity(1.0);
		Assert.AreEqual(9.80665, a.Value, Tolerance);
	}

	[TestMethod]
	public void ForceMagnitude_FromPoundsForce_And_Dynes()
	{
		Assert.AreEqual(4.4482216152605, ForceMagnitude<double>.FromPoundForce(1.0).Value, Tolerance);
		Assert.AreEqual(1e-5, ForceMagnitude<double>.FromDyne(1.0).Value, Tolerance);
	}

	[TestMethod]
	public void Pressure_FromKilopascals_And_Torr()
	{
		Assert.AreEqual(1000.0, Pressure<double>.FromKilopascal(1.0).Value, Tolerance);
		Assert.AreEqual(101325.0, Pressure<double>.FromTorr(760.0).Value, 1e-6);
	}

	[TestMethod]
	public void Energy_From_Btus_WattHours_Ergs_Kilocalories()
	{
		Assert.AreEqual(1055.05585262, Energy<double>.FromBtu(1.0).Value, Tolerance);
		Assert.AreEqual(3600.0, Energy<double>.FromWattHour(1.0).Value, Tolerance);
		Assert.AreEqual(1e-7, Energy<double>.FromErg(1.0).Value, Tolerance);
		Assert.AreEqual(4184.0, Energy<double>.FromKilocalorie(1.0).Value, Tolerance);
	}

	[TestMethod]
	public void Power_FromKilowatts_And_Megawatts()
	{
		Assert.AreEqual(1500.0, Power<double>.FromKilowatt(1.5).Value, Tolerance);
		Assert.AreEqual(1e6, Power<double>.FromMegawatt(1.0).Value, Tolerance);
	}

	// ---- Electromagnetism ----

	[TestMethod]
	public void ChargeMagnitude_FromAmpereHours_Is_3600_Coulombs()
	{
		Assert.AreEqual(3600.0, ChargeMagnitude<double>.FromAmpereHour(1.0).Value, Tolerance);
	}

	[TestMethod]
	public void Capacitance_From_SI_Prefixed_Farads()
	{
		Assert.AreEqual(1e-6, Capacitance<double>.FromMicrofarad(1.0).Value, 1e-15);
		Assert.AreEqual(1e-12, Capacitance<double>.FromPicofarad(1.0).Value, 1e-21);
	}

	// ---- Frequency / angle / ratio ----

	[TestMethod]
	public void Frequency_FromKilohertz_And_Megahertz()
	{
		Assert.AreEqual(1000.0, Frequency<double>.FromKilohertz(1.0).Value, Tolerance);
		Assert.AreEqual(1e6, Frequency<double>.FromMegahertz(1.0).Value, Tolerance);
	}

	[TestMethod]
	public void Angle_FromGradians_And_Revolutions()
	{
		Assert.AreEqual(Math.PI, Angle<double>.FromGradian(200.0).Value, Tolerance);
		Assert.AreEqual(2.0 * Math.PI, Angle<double>.FromRevolution(1.0).Value, Tolerance);
	}

	[TestMethod]
	public void Ratio_FromPartPerMillion_And_Billion()
	{
		Assert.AreEqual(1e-6, Ratio<double>.FromPartPerMillion(1.0).Value, 1e-15);
		Assert.AreEqual(1e-9, Ratio<double>.FromPartPerBillion(1.0).Value, 1e-18);
	}

	// ---- Chemistry ----

	[TestMethod]
	public void Concentration_FromMillimolars_Is_One_MolePerCubicMeter()
	{
		Assert.AreEqual(1.0, Concentration<double>.FromMillimolar(1.0).Value, Tolerance);
	}

	[TestMethod]
	public void MolarMass_FromDaltons_Equals_GramPerMole()
	{
		Assert.AreEqual(0.001, MolarMass<double>.FromDalton(1.0).Value, Tolerance);
		Assert.AreEqual(MolarMass<double>.FromGramPerMole(1.0).Value, MolarMass<double>.FromDalton(1.0).Value, Tolerance);
	}

	// ---- Radiology ----

	[TestMethod]
	public void Traditional_Radiological_Units_Convert_To_SI()
	{
		Assert.AreEqual(3.7e10, RadioactiveActivity<double>.FromCurie(1.0).Value, 1.0);
		Assert.AreEqual(1.0, AbsorbedDose<double>.FromRad(100.0).Value, Tolerance);
		Assert.AreEqual(1.0, EquivalentDose<double>.FromRem(100.0).Value, Tolerance);
		Assert.AreEqual(2.58e-4, Exposure<double>.FromRoentgen(1.0).Value, 1e-12);
	}

	// ---- Density ----

	[TestMethod]
	public void Density_FromGramPerCubicCentimeter_Is_1000_KilogramPerCubicMeter()
	{
		Assert.AreEqual(1000.0, Density<double>.FromGramPerCubicCentimeter(1.0).Value, Tolerance);
		Assert.AreEqual(1.0, Density<double>.FromGramPerLiter(1.0).Value, Tolerance);
	}

	// ---- Physical constants backfilled into domains.json ----

	[TestMethod]
	public void Acoustic_Reference_Constants_Are_Available()
	{
		Assert.AreEqual(20e-6, PhysicalConstants.Generic.ReferenceSoundPressure<double>(), 1e-12);
		Assert.AreEqual(1e-12, PhysicalConstants.Generic.ReferenceSoundIntensity<double>(), 1e-21);
		Assert.AreEqual(0.161, PhysicalConstants.Generic.SabineConstant<double>(), Tolerance);
	}

	[TestMethod]
	public void Optical_Nuclear_And_Fluid_Constants_Are_Available()
	{
		Assert.AreEqual(683.0, PhysicalConstants.Generic.LuminousEfficacy<double>(), Tolerance);
		Assert.AreEqual(5.0507837461e-27, PhysicalConstants.Generic.NuclearMagneton<double>(), 1e-36);
		Assert.AreEqual(1.225, PhysicalConstants.Generic.StandardAirDensity<double>(), Tolerance);
		Assert.AreEqual(0.0728, PhysicalConstants.Generic.WaterSurfaceTension<double>(), Tolerance);
	}
}
