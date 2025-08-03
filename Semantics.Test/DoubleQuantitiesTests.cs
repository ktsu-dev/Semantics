// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using ktsu.Semantics.Double;
using ktsu.Semantics.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests for Double precision concrete quantity implementations.
/// Tests the three-tier architecture: Generic → Double concrete types.
/// </summary>
[TestClass]
public class DoubleQuantitiesTests
{
	private const double Tolerance = 1e-10;

	[TestClass]
	public class MechanicalQuantitiesDoubleTests
	{
		[TestMethod]
		public void Length_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			var length = Length.FromMeters(10.123456789);
			var genericLength = (Generic.Length<double>)length;
			var backToDouble = (Length)genericLength;

			// Assert
			Assert.AreEqual(10.123456789, length.Value, Tolerance);
			Assert.AreEqual(10.123456789, genericLength.Value, Tolerance);
			Assert.AreEqual(10.123456789, backToDouble.Value, Tolerance);
		}

		[TestMethod]
		public void Mass_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			var mass = Mass.FromKilograms(25.987654321);
			var genericMass = (Generic.Mass<double>)mass;
			var backToDouble = (Mass)genericMass;

			// Assert
			Assert.AreEqual(25.987654321, mass.Value, Tolerance);
			Assert.AreEqual(25.987654321, genericMass.Value, Tolerance);
			Assert.AreEqual(25.987654321, backToDouble.Value, Tolerance);
		}

		[TestMethod]
		public void Pressure_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			var pressure = Pressure.FromPascals(101325.123456789);
			var genericPressure = (Generic.Pressure<double>)pressure;
			var backToDouble = (Pressure)genericPressure;

			// Assert
			Assert.AreEqual(101325.123456789, pressure.Value, Tolerance);
			Assert.AreEqual(101325.123456789, genericPressure.Value, Tolerance);
			Assert.AreEqual(101325.123456789, backToDouble.Value, Tolerance);
		}

		[TestMethod]
		public void Area_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			var area = Area.FromSquareMeters(42.195780490812);
			var genericArea = (Generic.Area<double>)area;
			var backToDouble = (Area)genericArea;

			// Assert
			Assert.AreEqual(42.195780490812, area.Value, Tolerance);
			Assert.AreEqual(42.195780490812, genericArea.Value, Tolerance);
			Assert.AreEqual(42.195780490812, backToDouble.Value, Tolerance);
		}
	}

	[TestClass]
	public class ChemicalQuantitiesDoubleTests
	{
		[TestMethod]
		public void AmountOfSubstance_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			var amount = AmountOfSubstance.FromMoles(2.5e-6);
			var genericAmount = (Generic.AmountOfSubstance<double>)amount;
			var backToDouble = (AmountOfSubstance)genericAmount;

			// Assert
			Assert.AreEqual(2.5e-6, amount.Value, Tolerance);
			Assert.AreEqual(2.5e-6, genericAmount.Value, Tolerance);
			Assert.AreEqual(2.5e-6, backToDouble.Value, Tolerance);
		}

		[TestMethod]
		public void Concentration_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			var concentration = Concentration.FromMolarConcentration(0.123456789);
			var genericConcentration = (Generic.Concentration<double>)concentration;
			var backToDouble = (Concentration)genericConcentration;

			// Assert
			Assert.AreEqual(0.123456789, concentration.Value, Tolerance);
			Assert.AreEqual(0.123456789, genericConcentration.Value, Tolerance);
			Assert.AreEqual(0.123456789, backToDouble.Value, Tolerance);
		}

		[TestMethod]
		public void pH_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			var ph = pH.FromPHValue(7.35);
			var genericPh = (Generic.pH<double>)ph;
			var backToDouble = (pH)genericPh;

			// Assert
			Assert.AreEqual(7.35, ph.Value, Tolerance);
			Assert.AreEqual(7.35, genericPh.Value, Tolerance);
			Assert.AreEqual(7.35, backToDouble.Value, Tolerance);
		}
	}

	[TestClass]
	public class NuclearQuantitiesDoubleTests
	{
		[TestMethod]
		public void RadioactiveActivity_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			var activity = RadioactiveActivity.FromBecquerels(1e12);
			var genericActivity = (Generic.RadioactiveActivity<double>)activity;
			var backToDouble = (RadioactiveActivity)genericActivity;

			// Assert
			Assert.AreEqual(1e12, activity.Value, Tolerance);
			Assert.AreEqual(1e12, genericActivity.Value, Tolerance);
			Assert.AreEqual(1e12, backToDouble.Value, Tolerance);
		}

		[TestMethod]
		public void AbsorbedDose_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			var dose = AbsorbedDose.FromGrays(0.001);
			var genericDose = (Generic.AbsorbedDose<double>)dose;
			var backToDouble = (AbsorbedDose)genericDose;

			// Assert
			Assert.AreEqual(0.001, dose.Value, Tolerance);
			Assert.AreEqual(0.001, genericDose.Value, Tolerance);
			Assert.AreEqual(0.001, backToDouble.Value, Tolerance);
		}
	}

	[TestClass]
	public class OpticalQuantitiesDoubleTests
	{
		[TestMethod]
		public void Wavelength_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			var wavelength = Wavelength.FromNanometers(532.1);
			var genericWavelength = (Generic.Wavelength<double>)wavelength;
			var backToDouble = (Wavelength)genericWavelength;

			// Assert
			Assert.AreEqual(532.1e-9, wavelength.Value, Tolerance);
			Assert.AreEqual(532.1e-9, genericWavelength.Value, Tolerance);
			Assert.AreEqual(532.1e-9, backToDouble.Value, Tolerance);
		}

		[TestMethod]
		public void RefractiveIndex_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			var refractiveIndex = RefractiveIndex.Create(1.333);
			var genericRefractiveIndex = (Generic.RefractiveIndex<double>)refractiveIndex;
			var backToDouble = (RefractiveIndex)genericRefractiveIndex;

			// Assert
			Assert.AreEqual(1.333, refractiveIndex.Value, Tolerance);
			Assert.AreEqual(1.333, genericRefractiveIndex.Value, Tolerance);
			Assert.AreEqual(1.333, backToDouble.Value, Tolerance);
		}
	}

	[TestClass]
	public class PrecisionTests
	{
		[TestMethod]
		public void DoubleQuantities_ShouldMaintainHighPrecision()
		{
			// Arrange - Use a value that would lose precision in float
			double highPrecisionValue = Math.PI * 1e10;
			
			// Act
			var length = Length.FromMeters(highPrecisionValue);
			var genericLength = (Generic.Length<double>)length;
			var backToDouble = (Length)genericLength;

			// Assert - Should maintain full double precision
			Assert.AreEqual(highPrecisionValue, length.Value, Tolerance);
			Assert.AreEqual(highPrecisionValue, genericLength.Value, Tolerance);
			Assert.AreEqual(highPrecisionValue, backToDouble.Value, Tolerance);
		}
	}

	[TestClass]
	public class ToStringTests
	{
		[TestMethod]
		public void DoubleQuantities_ToString_ShouldMatchGenericToString()
		{
			// Arrange
			var doubleLength = Length.FromMeters(10.123456789);
			var genericLength = Generic.Length<double>.FromMeters(10.123456789);

			// Act & Assert
			Assert.AreEqual(genericLength.ToString(), doubleLength.ToString());
		}
	}

	[TestClass]
	public class NullSafetyTests
	{
		[TestMethod]
		public void ImplicitConversion_NullDoubleQuantity_ShouldThrowArgumentNullException()
		{
			// Arrange
			Length? nullLength = null;

			// Act & Assert
			Assert.ThrowsException<ArgumentNullException>(() => 
			{
				Generic.Length<double> generic = nullLength!;
			});
		}
	}
}