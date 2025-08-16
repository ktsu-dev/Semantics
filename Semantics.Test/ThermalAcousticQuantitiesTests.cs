// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public static class ThermalAcousticQuantitiesTests
{
	[TestClass]
	public class ThermalQuantitiesTests
	{
		[TestMethod]
		public void Temperature_BasicOperations_ShouldWork()
		{
			// Test temperature creation and conversions
			Temperature<double> tempCelsius = Temperature<double>.Create(25.0 + 273.15); // Celsius to Kelvin
			Temperature<double> tempKelvin = Temperature<double>.Create(298.15);
			Temperature<double> tempFahrenheit = Temperature<double>.Create(((77.0 - 32) * 5 / 9) + 273.15); // Fahrenheit to Kelvin

			// Test conversions
			Assert.AreEqual(298.15, tempCelsius.Value, 1e-10);
			Assert.AreEqual(298.15, tempKelvin.Value, 1e-10);
			Assert.AreEqual(298.15, tempFahrenheit.Value, 1e-10);
		}

		[TestMethod]
		public void Temperature_ArithmeticOperations_ShouldWork()
		{
			Temperature<double> temp1 = Temperature<double>.Create(283.15); // 10°C
			Temperature<double> temp2 = Temperature<double>.Create(278.15); // 5°C

			Temperature<double> sum = temp1 + temp2;
			Temperature<double> difference = temp1 - temp2;
			Temperature<double> scaled = temp1 * 2.0;

			Assert.AreEqual(561.3, sum.Value, 1e-10);
			Assert.AreEqual(5.0, difference.Value, 1e-10);
			Assert.AreEqual(566.3, scaled.Value, 1e-10);
		}

		[TestMethod]
		public void ThermalConductivity_BasicOperations_ShouldWork()
		{
			ThermalConductivity<double> conductivity1 = ThermalConductivity<double>.Create(200.0); // Copper
			ThermalConductivity<double> conductivity2 = ThermalConductivity<double>.Create(0.5);   // Insulator

			// Test arithmetic operations
			ThermalConductivity<double> sum = conductivity1 + conductivity2;
			double ratio = conductivity1.Value / conductivity2.Value;

			Assert.AreEqual(200.5, sum.Value, 1e-10);
			Assert.AreEqual(400.0, ratio, 1e-10);
		}

		[TestMethod]
		public void SpecificHeat_BasicOperations_ShouldWork()
		{
			SpecificHeat<double> specificHeat = SpecificHeat<double>.Create(4184.0); // Water

			// Test scaling
			SpecificHeat<double> doubledHeat = specificHeat * 2.0;
			Assert.AreEqual(8368.0, doubledHeat.Value, 1e-10);
		}

		[TestMethod]
		public void ThermalResistance_BasicOperations_ShouldWork()
		{
			ThermalResistance<double> resistance1 = ThermalResistance<double>.Create(0.1);
			ThermalResistance<double> resistance2 = ThermalResistance<double>.Create(0.2);

			// Series thermal resistance
			ThermalResistance<double> seriesResistance = resistance1 + resistance2;
			Assert.AreEqual(0.3, seriesResistance.Value, 1e-10);

			// Parallel thermal resistance
			double parallelResistance = 1.0 / ((1.0 / resistance1.Value) + (1.0 / resistance2.Value));
			Assert.AreEqual(0.0666667, parallelResistance, 1e-6);
		}

		[TestMethod]
		public void HeatFlux_BasicOperations_ShouldWork()
		{
			Power<double> heatFlux1 = Power<double>.Create(1000.0);
			Power<double> heatFlux2 = Power<double>.Create(500.0);

			Power<double> sum = heatFlux1 + heatFlux2;
			Power<double> difference = heatFlux1 - heatFlux2;

			Assert.AreEqual(1500.0, sum.Value, 1e-10);
			Assert.AreEqual(500.0, difference.Value, 1e-10);
		}

		[TestMethod]
		public void ThermalExpansion_BasicOperations_ShouldWork()
		{
			ThermalExpansion<double> expansion = ThermalExpansion<double>.Create(12e-6); // Steel

			// Test temperature-dependent expansion
			Temperature<double> tempChange = Temperature<double>.Create(100.0);
			double strain = expansion.Value * tempChange.Value;

			Assert.AreEqual(0.0012, strain, 1e-10);
		}
	}

	[TestClass]
	public class AcousticQuantitiesTests
	{
		[TestMethod]
		public void SoundPressure_BasicOperations_ShouldWork()
		{
			SoundPressure<double> pressure1 = SoundPressure<double>.Create(20.0); // About 120 dB SPL
			SoundPressure<double> pressure2 = SoundPressure<double>.Create(2.0);  // About 100 dB SPL

			SoundPressure<double> sum = pressure1 + pressure2;
			double ratio = pressure1.Value / pressure2.Value;

			Assert.AreEqual(22.0, sum.Value, 1e-10);
			Assert.AreEqual(10.0, ratio, 1e-10);
		}

		[TestMethod]
		public void SoundIntensity_BasicOperations_ShouldWork()
		{
			SoundIntensity<double> intensity = SoundIntensity<double>.Create(1e-6); // Moderate sound

			// Test scaling
			SoundIntensity<double> doubled = intensity * 2.0;
			Assert.AreEqual(2e-6, doubled.Value, 1e-16);
		}

		[TestMethod]
		public void Frequency_BasicOperations_ShouldWork()
		{
			Frequency<double> frequency1 = Frequency<double>.Create(440.0); // A4 note
			Frequency<double> frequency2 = Frequency<double>.Create(1000.0);

			// Test unit conversions
			Assert.AreEqual(440.0, frequency1.Value, 1e-10);
			Assert.AreEqual(1000.0, frequency2.Value, 1e-10);
		}

		[TestMethod]
		public void WaveLength_FrequencyRelationship_ShouldWork()
		{
			Frequency<double> frequency = Frequency<double>.Create(1000.0);
			double soundSpeed = 343.0; // m/s in air at room temperature
			double expectedWaveLength = soundSpeed / frequency.Value;

			Length<double> waveLength = Length<double>.Create(expectedWaveLength);
			Assert.AreEqual(0.343, waveLength.Value, 1e-10);
		}

		[TestMethod]
		public void AcousticImpedance_BasicOperations_ShouldWork()
		{
			AcousticImpedance<double> impedance = AcousticImpedance<double>.Create(415.0); // Air at room temperature

			// Test scaling
			AcousticImpedance<double> scaled = impedance * 2.0;
			Assert.AreEqual(830.0, scaled.Value, 1e-10);
		}

		[TestMethod]
		public void SoundLevel_DecibelCalculations_ShouldWork()
		{
			// Test sound pressure level calculations
			double referencePressure = 20e-6; // 20 μPa reference
			SoundPressure<double> pressure = SoundPressure<double>.Create(2.0);

			double spl = 20 * Math.Log10(pressure.Value / referencePressure);
			Assert.AreEqual(100.0, spl, 1e-10); // 100 dB SPL
		}

		[TestMethod]
		public void AcousticPower_BasicOperations_ShouldWork()
		{
			Power<double> power1 = Power<double>.Create(0.1);
			Power<double> power2 = Power<double>.Create(0.05);

			Power<double> sum = power1 + power2;
			double ratio = power1.Value / power2.Value;

			Assert.AreEqual(0.15, sum.Value, 1e-10);
			Assert.AreEqual(2.0, ratio, 1e-10);
		}

		[TestMethod]
		public void ResonantFrequency_BasicCalculations_ShouldWork()
		{
			// Simple harmonic oscillator: f = 1/(2π) * sqrt(k/m)
			double springConstant = 1000.0; // N/m
			double mass = 1.0; // kg
			double expectedFreq = 1.0 / (2.0 * Math.PI) * Math.Sqrt(springConstant / mass);

			Frequency<double> frequency = Frequency<double>.Create(expectedFreq);
			Assert.AreEqual(5.032, frequency.Value, 1e-3);
		}
	}

	[TestClass]
	public class ThermalAcousticInteractionTests
	{
		[TestMethod]
		public void TemperatureEffect_OnSoundSpeed_ShouldWork()
		{
			// Sound speed in air: v = sqrt(γRT/M)
			Temperature<double> temp1 = Temperature<double>.Create(293.15); // 20°C
			Temperature<double> temp2 = Temperature<double>.Create(273.15); // 0°C

			// Approximate sound speed at 20°C and 0°C
			double soundSpeed20 = 343.0; // m/s
			double soundSpeed0 = 331.0;  // m/s

			double speedRatio = soundSpeed20 / soundSpeed0;
			double tempRatio = Math.Sqrt(temp1.Value / temp2.Value);

			Assert.AreEqual(tempRatio, speedRatio, 1e-2);
		}

		[TestMethod]
		public void ThermalNoise_BasicCalculations_ShouldWork()
		{
			// Johnson-Nyquist thermal noise: V²=4kTRΔf
			Temperature<double> temperature = Temperature<double>.Create(300.0);
			double resistance = 1000.0; // Ω
			double bandwidth = 1000.0;  // Hz
			double boltzmannConstant = 1.38e-23; // J/K

			double thermalNoise = 4 * boltzmannConstant * temperature.Value * resistance * bandwidth;
			Assert.AreEqual(1.656e-14, thermalNoise, 1e-17);
		}

		[TestMethod]
		public void ThermoacousticEffect_QualitativeTest_ShouldWork()
		{
			// Test that we can represent thermoacoustic quantities
			Temperature<double> tempGradient = Temperature<double>.Create(100.0);
			Power<double> acousticPower = Power<double>.Create(0.01);
			Frequency<double> frequency = Frequency<double>.Create(1000.0);

			// These should all be valid quantities
			Assert.IsTrue(tempGradient.Value > 0);
			Assert.IsTrue(acousticPower.Value > 0);
			Assert.IsTrue(frequency.Value > 0);
		}
	}

	[TestClass]
	public class EdgeCasesAndErrorConditionsTests
	{
		[TestMethod]
		public void Temperature_AbsoluteZero_ShouldWork()
		{
			Temperature<double> absoluteZero = Temperature<double>.Create(0.0);
			Assert.AreEqual(0.0, absoluteZero.Value);
		}

		[TestMethod]
		public void Temperature_BelowAbsoluteZero_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentException>(() => Temperature<double>.Create(-1.0));
		}

		[TestMethod]
		public void Frequency_Zero_ShouldWork()
		{
			Frequency<double> zeroFreq = Frequency<double>.Create(0.0);
			Assert.AreEqual(0.0, zeroFreq.Value);
		}

		[TestMethod]
		public void Frequency_Negative_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentException>(() => Frequency<double>.Create(-1.0));
		}

		[TestMethod]
		public void ThermalQuantities_VeryLargeValues_ShouldWork()
		{
			Temperature<double> veryHot = Temperature<double>.Create(1e9); // 1 billion K
			ThermalConductivity<double> highConductivity = ThermalConductivity<double>.Create(1e6);

			Assert.AreEqual(1e9, veryHot.Value);
			Assert.AreEqual(1e6, highConductivity.Value);
		}

		[TestMethod]
		public void AcousticQuantities_VerySmallValues_ShouldWork()
		{
			SoundPressure<double> veryQuiet = SoundPressure<double>.Create(1e-12); // Extremely quiet
			Assert.AreEqual(1e-12, veryQuiet.Value);
		}
	}
}
