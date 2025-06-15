// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ElectricalQuantitiesTests
{
	[TestMethod]
	public void ElectricCurrent_BasicOperations_ShouldWork()
	{
		// Test creation
		ElectricCurrent<double> current1 = ElectricCurrent<double>.FromAmperes(5.0);
		ElectricCurrent<double> current2 = ElectricCurrent<double>.FromAmperes(3.0);

		// Test arithmetic operations
		ElectricCurrent<double> sum = current1 + current2;
		ElectricCurrent<double> difference = current1 - current2;
		ElectricCurrent<double> scaled = current1 * 2.0;
		ElectricCurrent<double> divided = current1 / 2.0;

		Assert.AreEqual(8.0, sum.Value, 1e-10);
		Assert.AreEqual(2.0, difference.Value, 1e-10);
		Assert.AreEqual(10.0, scaled.Value, 1e-10);
		Assert.AreEqual(2.5, divided.Value, 1e-10);
	}

	[TestMethod]
	public void ElectricPotential_BasicOperations_ShouldWork()
	{
		// Test creation from different units
		ElectricPotential<double> voltage1 = ElectricPotential<double>.FromVolts(12.0);
		ElectricPotential<double> voltage2 = ElectricPotential<double>.FromVolts(5.0);

		// Test arithmetic operations
		ElectricPotential<double> sum = voltage1 + voltage2;
		ElectricPotential<double> difference = voltage1 - voltage2;

		Assert.AreEqual(17.0, sum.Value, 1e-10);
		Assert.AreEqual(7.0, difference.Value, 1e-10);
	}

	[TestMethod]
	public void ElectricResistance_OhmsLawCalculations_ShouldWork()
	{
		// Test Ohm's law relationships
		ElectricPotential<double> voltage = ElectricPotential<double>.FromVolts(12.0);
		ElectricCurrent<double> current = ElectricCurrent<double>.FromAmperes(2.0);
		ElectricResistance<double> resistance = ElectricResistance<double>.FromOhms(6.0);

		// V = I * R
		ElectricPotential<double> calculatedVoltage = current * resistance;
		Assert.AreEqual(voltage.Value, calculatedVoltage.Value, 1e-10);

		// R = V / I
		ElectricResistance<double> calculatedResistance = ElectricResistance<double>.FromOhms(voltage.Value / current.Value);
		Assert.AreEqual(resistance.Value, calculatedResistance.Value, 1e-10);
	}

	[TestMethod]
	public void ElectricCapacitance_BasicOperations_ShouldWork()
	{
		ElectricCapacitance<double> capacitance1 = ElectricCapacitance<double>.FromFarads(1e-6);
		ElectricCapacitance<double> capacitance2 = ElectricCapacitance<double>.FromFarads(0.5e-6);

		// Test series and parallel capacitance
		double seriesCapacitance = 1.0 / ((1.0 / capacitance1.Value) + (1.0 / capacitance2.Value));
		double parallelCapacitance = capacitance1.Value + capacitance2.Value;

		Assert.AreEqual(3.33333e-7, seriesCapacitance, 1e-12);
		Assert.AreEqual(1.5e-6, parallelCapacitance, 1e-15);
	}

	[TestMethod]
	public void ElectricCharge_BasicOperations_ShouldWork()
	{
		ElectricCharge<double> charge1 = ElectricCharge<double>.FromCoulombs(5.0);
		ElectricCharge<double> charge2 = ElectricCharge<double>.FromCoulombs(3.0);

		ElectricCharge<double> sum = charge1 + charge2;
		ElectricCharge<double> difference = charge1 - charge2;

		Assert.AreEqual(8.0, sum.Value, 1e-10);
		Assert.AreEqual(2.0, difference.Value, 1e-10);
	}

	[TestMethod]
	public void ElectricField_BasicOperations_ShouldWork()
	{
		ElectricField<double> field1 = ElectricField<double>.FromVoltsPerMeter(100.0);
		ElectricField<double> field2 = ElectricField<double>.FromVoltsPerMeter(50.0);

		ElectricField<double> sum = field1 + field2;
		ElectricField<double> difference = field1 - field2;

		Assert.AreEqual(150.0, sum.Value, 1e-10);
		Assert.AreEqual(50.0, difference.Value, 1e-10);
	}

	[TestMethod]
	public void ElectricConductivity_BasicOperations_ShouldWork()
	{
		ElectricConductivity<double> conductivity = ElectricConductivity<double>.FromSiemensPerMeter(1e6);
		double resistivity = 1.0 / conductivity.Value;

		Assert.AreEqual(1e-6, resistivity, 1e-15);
	}

	[TestMethod]
	public void ElectricFlux_BasicOperations_ShouldWork()
	{
		ElectricFlux<double> flux1 = ElectricFlux<double>.Create(2.0);
		ElectricFlux<double> flux2 = ElectricFlux<double>.Create(1.0);

		ElectricFlux<double> sum = flux1 + flux2;
		ElectricFlux<double> difference = flux1 - flux2;

		Assert.AreEqual(3.0, sum.Value, 1e-10);
		Assert.AreEqual(1.0, difference.Value, 1e-10);
	}

	[TestMethod]
	public void Permittivity_BasicOperations_ShouldWork()
	{
		Permittivity<double> permittivity = Permittivity<double>.FromFaradsPerMeter(8.854e-12);

		// Test relative permittivity calculation (vacuum permittivity)
		Assert.AreEqual(8.854e-12, permittivity.Value, 1e-17);
	}

	[TestMethod]
	public void ElectricPowerDensity_BasicOperations_ShouldWork()
	{
		ElectricPowerDensity<double> powerDensity1 = ElectricPowerDensity<double>.Create(1000.0);
		ElectricPowerDensity<double> powerDensity2 = ElectricPowerDensity<double>.Create(500.0);

		ElectricPowerDensity<double> sum = powerDensity1 + powerDensity2;
		ElectricPowerDensity<double> difference = powerDensity1 - powerDensity2;

		Assert.AreEqual(1500.0, sum.Value, 1e-10);
		Assert.AreEqual(500.0, difference.Value, 1e-10);
	}

	[TestMethod]
	public void ImpedanceAC_BasicOperations_ShouldWork()
	{
		ImpedanceAC<double> impedance1 = ImpedanceAC<double>.FromOhms(50.0);
		ImpedanceAC<double> impedance2 = ImpedanceAC<double>.FromOhms(25.0);

		// Test series and parallel impedance
		ImpedanceAC<double> seriesImpedance = impedance1 + impedance2;
		double parallelImpedance = 1.0 / ((1.0 / impedance1.Value) + (1.0 / impedance2.Value));

		Assert.AreEqual(75.0, seriesImpedance.Value, 1e-10);
		Assert.AreEqual(16.666667, parallelImpedance, 1e-6);
	}

	[TestMethod]
	public void ElectricalQuantities_ZeroValues_ShouldWork()
	{
		ElectricCurrent<double> zeroCurrent = ElectricCurrent<double>.FromAmperes(0.0);
		ElectricPotential<double> zeroVoltage = ElectricPotential<double>.FromVolts(0.0);
		ElectricResistance<double> zeroResistance = ElectricResistance<double>.FromOhms(0.0);

		Assert.AreEqual(0.0, zeroCurrent.Value);
		Assert.AreEqual(0.0, zeroVoltage.Value);
		Assert.AreEqual(0.0, zeroResistance.Value);
	}

	[TestMethod]
	public void ElectricalQuantities_NegativeValues_ShouldWork()
	{
		ElectricCurrent<double> negativeCurrent = ElectricCurrent<double>.FromAmperes(-5.0);
		ElectricPotential<double> negativeVoltage = ElectricPotential<double>.FromVolts(-12.0);

		Assert.AreEqual(-5.0, negativeCurrent.Value);
		Assert.AreEqual(-12.0, negativeVoltage.Value);

		// Test negation operator
		ElectricCurrent<double> positiveFromNegation = -negativeCurrent;
		Assert.AreEqual(5.0, positiveFromNegation.Value);
	}

	[TestMethod]
	public void ElectricalQuantities_ToString_ShouldWork()
	{
		ElectricCurrent<double> current = ElectricCurrent<double>.FromAmperes(2.5);
		ElectricPotential<double> voltage = ElectricPotential<double>.FromVolts(12.0);

		Assert.IsNotNull(current.ToString());
		Assert.IsNotNull(voltage.ToString());
		Assert.IsTrue(current.ToString().Contains("2.5"));
		Assert.IsTrue(voltage.ToString().Contains("12"));
	}
}
