// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Quantities;

using ktsu.Semantics.Quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Covers the quantities backfilled from main: new electrical/chemical/thermal/photometric
/// dimensions, acoustic overloads and coefficients, and the hand-written logarithmic-scale
/// companions (sound levels and pH).
/// </summary>
[TestClass]
public sealed class QuantityBackfillTests
{
	private const double Tolerance = 1e-9;

	// ---- New generated dimensions ----

	[TestMethod]
	public void Luminance_Factories_And_Intensity_Relationship()
	{
		Luminance<double> nit = Luminance<double>.FromNits(100.0);
		Assert.AreEqual(100.0, nit.Value, Tolerance);
		Assert.AreEqual(3.4262590996353905, Luminance<double>.FromFootLamberts(1.0).Value, 1e-9);

		LuminousIntensity<double> intensity = nit * Area<double>.FromSquareMeters(2.0);
		Assert.AreEqual(200.0, intensity.Value, Tolerance);
	}

	[TestMethod]
	public void Permittivity_And_Conductivity_Factories()
	{
		Assert.AreEqual(8.854e-12, Permittivity<double>.FromFaradPerMeter(8.854e-12).Value, 1e-21);
		Assert.AreEqual(5.96e7, ElectricConductivity<double>.FromSiemensPerMeter(5.96e7).Value, 1.0);
	}

	[TestMethod]
	public void ElectricFlux_From_Field_Times_Area()
	{
		ElectricFieldMagnitude<double> field = ElectricFieldMagnitude<double>.FromVoltPerMeter(100.0);
		ElectricFlux<double> flux = field * Area<double>.FromSquareMeters(0.5);
		Assert.AreEqual(50.0, flux.Value, Tolerance);
	}

	[TestMethod]
	public void ElectricPowerDensity_Times_Volume_Is_Power()
	{
		ElectricPowerDensity<double> density = ElectricPowerDensity<double>.FromWattPerCubicMeter(250.0);
		Power<double> power = density * Volume<double>.FromCubicMeters(4.0);
		Assert.AreEqual(1000.0, power.Value, Tolerance);
	}

	[TestMethod]
	public void Sensitivity_Times_Pressure_Is_Voltage()
	{
		// A 50 mV/Pa microphone at 1 Pa (94 dB SPL) produces 50 mV.
		Sensitivity<double> mic = Sensitivity<double>.FromVoltPerPascal(0.05);
		VoltageMagnitude<double> output = mic * Pressure<double>.FromPascals(1.0);
		Assert.AreEqual(0.05, output.Value, Tolerance);
	}

	[TestMethod]
	public void ThermalResistance_And_RateConstant_Factories()
	{
		Assert.AreEqual(2.5, ThermalResistance<double>.FromKelvinPerWatt(2.5).Value, Tolerance);
		Assert.AreEqual(0.693, RateConstant<double>.FromPerSecond(0.693).Value, Tolerance);
	}

	[TestMethod]
	public void Loudness_And_Sharpness_Factories()
	{
		Assert.AreEqual(2.0, Loudness<double>.FromSones(2.0).Value, Tolerance);
		Assert.AreEqual(1.5, Sharpness<double>.FromAcum(1.5).Value, Tolerance);
	}

	// ---- New overloads ----

	[TestMethod]
	public void Impedance_Widens_To_Resistance()
	{
		Impedance<double> z = Impedance<double>.FromOhms(8.0);
		Resistance<double> r = z;
		Assert.AreEqual(8.0, r.Value, Tolerance);

		// Ohm's law applies through the overload: V = I·Z.
		VoltageMagnitude<double> v = CurrentMagnitude<double>.FromAmperes(2.0) * z;
		Assert.AreEqual(16.0, v.Value, Tolerance);
	}

	[TestMethod]
	public void SoundPressure_And_SoundPower_Widen_To_Bases()
	{
		Pressure<double> p = SoundPressure<double>.FromPascals(0.2);
		Assert.AreEqual(0.2, p.Value, Tolerance);

		Power<double> w = SoundPower<double>.FromWatts(0.01);
		Assert.AreEqual(0.01, w.Value, Tolerance);
	}

	[TestMethod]
	public void Acoustic_Coefficients_Are_Ratio_Overloads()
	{
		SoundAbsorption<double> alpha = SoundAbsorption<double>.Create(0.85);
		Ratio<double> asRatio = alpha;
		Assert.AreEqual(0.85, asRatio.Value, Tolerance);

		Assert.AreEqual(0.65, NoiseReductionCoefficient<double>.Create(0.65).Value, Tolerance);
		Assert.AreEqual(52.0, SoundTransmissionClass<double>.Create(52.0).Value, Tolerance);
	}

	[TestMethod]
	public void ReflectionCoefficient_Is_Signed()
	{
		// A pressure-release boundary reflects with inverted phase.
		ReflectionCoefficient<double> r = ReflectionCoefficient<double>.Create(-1.0);
		Assert.AreEqual(-1.0, r.Value, Tolerance);
	}

	// ---- Logarithmic-scale companions ----

	[TestMethod]
	public void SoundPressureLevel_From_Pressure_And_Back()
	{
		// 0.02 Pa is 1000× the 20 µPa reference: 20·log10(1000) = 60 dB.
		SoundPressureLevel<double> spl = SoundPressureLevel<double>.FromSoundPressure(SoundPressure<double>.FromPascals(0.02));
		Assert.AreEqual(60.0, spl.Value, 1e-9);

		SoundPressure<double> back = spl.ToSoundPressure();
		Assert.AreEqual(0.02, back.Value, 1e-12);
	}

	[TestMethod]
	public void SoundIntensityLevel_From_Intensity_And_Back()
	{
		SoundIntensityLevel<double> sil = SoundIntensityLevel<double>.FromSoundIntensity(SoundIntensity<double>.FromWattPerSquareMeter(1e-6));
		Assert.AreEqual(60.0, sil.Value, 1e-9);
		Assert.AreEqual(1e-6, sil.ToSoundIntensity().Value, 1e-15);
	}

	[TestMethod]
	public void SoundPowerLevel_From_Power_And_Back()
	{
		SoundPowerLevel<double> swl = SoundPowerLevel<double>.FromSoundPower(SoundPower<double>.FromWatts(1e-3));
		Assert.AreEqual(90.0, swl.Value, 1e-9);
		Assert.AreEqual(1e-3, swl.ToSoundPower().Value, 1e-12);
	}

	[TestMethod]
	public void DirectionalityIndex_RoundTrips_Intensity_Ratio()
	{
		DirectionalityIndex<double> di = DirectionalityIndex<double>.FromIntensityRatio(Ratio<double>.Create(10.0));
		Assert.AreEqual(10.0, di.Value, 1e-9);
		Assert.AreEqual(10.0, di.ToIntensityRatio().Value, 1e-9);
		Assert.AreEqual(0.0, DirectionalityIndex<double>.Omnidirectional.Value, Tolerance);
	}

	[TestMethod]
	public void PH_From_Concentration_And_Back()
	{
		// Pure water: [H+] = 1e-7 mol/L.
		PH<double> ph = PH<double>.FromHydrogenConcentration(Concentration<double>.FromMolars(1e-7));
		Assert.AreEqual(7.0, ph.Value, 1e-9);

		Concentration<double> back = ph.ToHydrogenConcentration();
		Assert.AreEqual(Concentration<double>.FromMolars(1e-7).Value, back.Value, 1e-12);
	}

	[TestMethod]
	public void PH_Acidity_Classification_And_POH()
	{
		PH<double> lemon = PH<double>.Create(2.0);
		Assert.IsTrue(lemon.IsAcidic);
		Assert.IsFalse(lemon.IsBasic);
		Assert.AreEqual(12.0, lemon.ToPOH().Value, Tolerance);
		Assert.AreEqual(7.0, PH<double>.Neutral.Value, Tolerance);
		Assert.IsFalse(PH<double>.Neutral.IsAcidic);
	}

	[TestMethod]
	public void Levels_Compare_And_Add_In_Decibel_Space()
	{
		SoundPressureLevel<double> quiet = SoundPressureLevel<double>.FromDecibels(40.0);
		SoundPressureLevel<double> loud = SoundPressureLevel<double>.FromDecibels(90.0);
		Assert.IsTrue(quiet < loud);
		Assert.AreEqual(50.0, (loud - quiet).Value, Tolerance);
	}
}
