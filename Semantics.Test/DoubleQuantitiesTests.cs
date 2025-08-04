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
public static class DoubleQuantitiesTests
{
	private const double Tolerance = 1e-10;

	[TestClass]
	public class MechanicalQuantitiesDoubleTests
	{
		[TestMethod]
		public void Length_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			Length length = Length.FromMeters(10.123456789);
			Length<double> genericLength = length;
			Length backToDouble = (Length)genericLength;

			// Assert
			Assert.AreEqual(10.123456789, length.Value, Tolerance);
			Assert.AreEqual(10.123456789, genericLength.Value, Tolerance);
			Assert.AreEqual(10.123456789, backToDouble.Value, Tolerance);
		}

		[TestMethod]
		public void Mass_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			Mass mass = Mass.FromKilograms(25.987654321);
			Mass<double> genericMass = mass;
			Mass backToDouble = (Mass)genericMass;

			// Assert
			Assert.AreEqual(25.987654321, mass.Value, Tolerance);
			Assert.AreEqual(25.987654321, genericMass.Value, Tolerance);
			Assert.AreEqual(25.987654321, backToDouble.Value, Tolerance);
		}

		[TestMethod]
		public void Pressure_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			Pressure pressure = Pressure.FromPascals(101325.123456789);
			Pressure<double> genericPressure = pressure;
			Pressure backToDouble = (Pressure)genericPressure;

			// Assert
			Assert.AreEqual(101325.123456789, pressure.Value, Tolerance);
			Assert.AreEqual(101325.123456789, genericPressure.Value, Tolerance);
			Assert.AreEqual(101325.123456789, backToDouble.Value, Tolerance);
		}

		[TestMethod]
		public void Area_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			Area area = Area.FromSquareMeters(42.195780490812);
			Area<double> genericArea = area;
			Area backToDouble = (Area)genericArea;

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
			AmountOfSubstance amount = AmountOfSubstance.FromMoles(2.5e-6);
			AmountOfSubstance<double> genericAmount = amount;
			AmountOfSubstance backToDouble = (AmountOfSubstance)genericAmount;

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
			Concentration<double> genericConcentration = (Concentration<double>)concentration;
			Concentration backToDouble = (Concentration)genericConcentration;

			// Assert
			Assert.AreEqual(0.123456789, concentration.Value, Tolerance);
			Assert.AreEqual(0.123456789, genericConcentration.Value, Tolerance);
			Assert.AreEqual(0.123456789, backToDouble.Value, Tolerance);
		}

		[TestMethod]
		public void PH_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			var ph = PH.FromPHValue(7.35);
			var genericPh = (Generic.PH<double>)ph;
			var backToDouble = (PH)genericPh;

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
			RadioactiveActivity activity = RadioactiveActivity.FromBecquerels(1e12);
			RadioactiveActivity<double> genericActivity = activity;
			RadioactiveActivity backToDouble = (RadioactiveActivity)genericActivity;

			// Assert
			Assert.AreEqual(1e12, activity.Value, Tolerance);
			Assert.AreEqual(1e12, genericActivity.Value, Tolerance);
			Assert.AreEqual(1e12, backToDouble.Value, Tolerance);
		}

		[TestMethod]
		public void AbsorbedDose_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			AbsorbedDose dose = AbsorbedDose.FromGrays(0.001);
			AbsorbedDose<double> genericDose = dose;
			AbsorbedDose backToDouble = (AbsorbedDose)genericDose;

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
			Wavelength<double> genericWavelength = (Wavelength<double>)wavelength;
			Wavelength backToDouble = (Wavelength)genericWavelength;

			// Assert
			Assert.AreEqual(532.1e-9, wavelength.Value, Tolerance);
			Assert.AreEqual(532.1e-9, genericWavelength.Value, Tolerance);
			Assert.AreEqual(532.1e-9, backToDouble.Value, Tolerance);
		}

		[TestMethod]
		public void RefractiveIndex_CreateAndConvert_ShouldWork()
		{
			// Arrange & Act
			RefractiveIndex<double> refractiveIndex = RefractiveIndex.Create(1.333);
			RefractiveIndex<double> genericRefractiveIndex = refractiveIndex;
			RefractiveIndex backToDouble = (RefractiveIndex)genericRefractiveIndex;

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
			Length length = Length.FromMeters(highPrecisionValue);
			Length<double> genericLength = length;
			Length backToDouble = (Length)genericLength;

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
			Length doubleLength = Length.FromMeters(10.123456789);
			Length<double> genericLength = Length<double>.FromMeters(10.123456789);

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
				Length<double> generic = nullLength!;
			});
		}
	}
}
