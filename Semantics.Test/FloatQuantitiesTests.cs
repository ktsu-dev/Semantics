// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using ktsu.Semantics.Float;
using ktsu.Semantics.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests for Float precision concrete quantity implementations.
/// Tests the three-tier architecture: Generic → Float concrete types.
/// </summary>
[TestClass]
public static class FloatQuantitiesTests
{
	private const float Tolerance = 1e-6f;

	[TestClass]
	public class MechanicalQuantitiesFloatTests
	{
		[TestMethod]
		public void Length_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			Length length = Length.FromMeters(10.5f);
			Length<float> genericLength = length;
			Length backToFloat = (Length)genericLength;

			// Assert
			Assert.AreEqual(10.5f, length.Value, Tolerance);
			Assert.AreEqual(10.5f, genericLength.Value, Tolerance);
			Assert.AreEqual(10.5f, backToFloat.Value, Tolerance);
		}

		[TestMethod]
		public void Mass_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			Mass mass = Mass.FromKilograms(25.7f);
			Mass<float> genericMass = mass;
			Mass backToFloat = (Mass)genericMass;

			// Assert
			Assert.AreEqual(25.7f, mass.Value, Tolerance);
			Assert.AreEqual(25.7f, genericMass.Value, Tolerance);
			Assert.AreEqual(25.7f, backToFloat.Value, Tolerance);
		}

		[TestMethod]
		public void Force_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			Force force = Force.FromNewtons(100.3f);
			Force<float> genericForce = force;
			Force backToFloat = (Force)genericForce;

			// Assert
			Assert.AreEqual(100.3f, force.Value, Tolerance);
			Assert.AreEqual(100.3f, genericForce.Value, Tolerance);
			Assert.AreEqual(100.3f, backToFloat.Value, Tolerance);
		}

		[TestMethod]
		public void Energy_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			Energy energy = Energy.FromJoules(500.8f);
			Energy<float> genericEnergy = energy;
			Energy backToFloat = (Energy)genericEnergy;

			// Assert
			Assert.AreEqual(500.8f, energy.Value, Tolerance);
			Assert.AreEqual(500.8f, genericEnergy.Value, Tolerance);
			Assert.AreEqual(500.8f, backToFloat.Value, Tolerance);
		}

		[TestMethod]
		public void Velocity_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			Velocity velocity = Velocity.FromMetersPerSecond(15.2f);
			Velocity<float> genericVelocity = velocity;
			Velocity backToFloat = (Velocity)genericVelocity;

			// Assert
			Assert.AreEqual(15.2f, velocity.Value, Tolerance);
			Assert.AreEqual(15.2f, genericVelocity.Value, Tolerance);
			Assert.AreEqual(15.2f, backToFloat.Value, Tolerance);
		}
	}

	[TestClass]
	public class ElectricalQuantitiesFloatTests
	{
		[TestMethod]
		public void ElectricCurrent_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			ElectricCurrent current = ElectricCurrent.FromAmperes(2.5f);
			ElectricCurrent<float> genericCurrent = current;
			ElectricCurrent backToFloat = (ElectricCurrent)genericCurrent;

			// Assert
			Assert.AreEqual(2.5f, current.Value, Tolerance);
			Assert.AreEqual(2.5f, genericCurrent.Value, Tolerance);
			Assert.AreEqual(2.5f, backToFloat.Value, Tolerance);
		}

		[TestMethod]
		public void ElectricPotential_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			ElectricPotential voltage = ElectricPotential.FromVolts(12.0f);
			ElectricPotential<float> genericVoltage = voltage;
			ElectricPotential backToFloat = (ElectricPotential)genericVoltage;

			// Assert
			Assert.AreEqual(12.0f, voltage.Value, Tolerance);
			Assert.AreEqual(12.0f, genericVoltage.Value, Tolerance);
			Assert.AreEqual(12.0f, backToFloat.Value, Tolerance);
		}

		[TestMethod]
		public void ElectricResistance_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			ElectricResistance resistance = ElectricResistance.FromOhms(47.5f);
			ElectricResistance<float> genericResistance = resistance;
			ElectricResistance backToFloat = (ElectricResistance)genericResistance;

			// Assert
			Assert.AreEqual(47.5f, resistance.Value, Tolerance);
			Assert.AreEqual(47.5f, genericResistance.Value, Tolerance);
			Assert.AreEqual(47.5f, backToFloat.Value, Tolerance);
		}
	}

	[TestClass]
	public class ThermalQuantitiesFloatTests
	{
		[TestMethod]
		public void Temperature_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			Temperature temperature = Temperature.FromKelvin(298.15f);
			Temperature<float> genericTemperature = temperature;
			Temperature backToFloat = (Temperature)genericTemperature;

			// Assert
			Assert.AreEqual(298.15f, temperature.Value, Tolerance);
			Assert.AreEqual(298.15f, genericTemperature.Value, Tolerance);
			Assert.AreEqual(298.15f, backToFloat.Value, Tolerance);
		}

		[TestMethod]
		public void Heat_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			Heat heat = Heat.FromJoules(1000.0f);
			Heat<float> genericHeat = heat;
			Heat backToFloat = (Heat)genericHeat;

			// Assert
			Assert.AreEqual(1000.0f, heat.Value, Tolerance);
			Assert.AreEqual(1000.0f, genericHeat.Value, Tolerance);
			Assert.AreEqual(1000.0f, backToFloat.Value, Tolerance);
		}
	}

	[TestClass]
	public class AcousticQuantitiesFloatTests
	{
		[TestMethod]
		public void Frequency_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			Frequency frequency = Frequency.FromHertz(440.0f);
			Frequency<float> genericFrequency = frequency;
			Frequency backToFloat = (Frequency)genericFrequency;

			// Assert
			Assert.AreEqual(440.0f, frequency.Value, Tolerance);
			Assert.AreEqual(440.0f, genericFrequency.Value, Tolerance);
			Assert.AreEqual(440.0f, backToFloat.Value, Tolerance);
		}

		[TestMethod]
		public void SoundPressure_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			SoundPressure soundPressure = SoundPressure.FromPascals(0.02f);
			SoundPressure<float> genericSoundPressure = (SoundPressure<float>)soundPressure;
			SoundPressure backToFloat = (SoundPressure)genericSoundPressure;

			// Assert
			Assert.AreEqual(0.02f, soundPressure.Value, Tolerance);
			Assert.AreEqual(0.02f, genericSoundPressure.Value, Tolerance);
			Assert.AreEqual(0.02f, backToFloat.Value, Tolerance);
		}
	}

	[TestClass]
	public class ToStringTests
	{
		[TestMethod]
		public void FloatQuantities_ToString_ShouldMatchGenericToString()
		{
			// Arrange
			Length floatLength = Length.FromMeters(10.5f);
			Length<float> genericLength = Length<float>.FromMeters(10.5f);

			// Act & Assert
			Assert.AreEqual(genericLength.ToString(), floatLength.ToString());
		}
	}

	[TestClass]
	public class NullSafetyTests
	{
		[TestMethod]
		public void ImplicitConversion_NullFloatQuantity_ShouldThrowArgumentNullException()
		{
			// Arrange
			Length? nullLength = null;

			// Act & Assert
			Assert.ThrowsException<ArgumentNullException>(() =>
			{
				Length<float> generic = nullLength!;
			});
		}
	}
}
