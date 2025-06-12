// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class PhysicalConstantsTests
{
	private const double Tolerance = 1e-10;

	[TestMethod]
	public void MolarVolumeSTP_MatchesCalculatedValue()
	{
		double gasConstant = PhysicalConstants.Fundamental.GasConstant;
		double temperature = PhysicalConstants.Temperature.StandardTemperature;
		double pressure = 101325.0;

		double calculatedMolarVolume = gasConstant * temperature / pressure;
		double calculatedMolarVolumeInLiters = calculatedMolarVolume * 1000;
		double storedMolarVolume = PhysicalConstants.Chemical.MolarVolumeSTP;

		Assert.AreEqual(calculatedMolarVolumeInLiters, storedMolarVolume, Tolerance);
	}

	[TestMethod]
	public void GasConstant_MatchesCalculatedFromAvogadroAndBoltzmann()
	{
		double avogadroNumber = PhysicalConstants.Fundamental.AvogadroNumber;
		double boltzmannConstant = PhysicalConstants.Fundamental.BoltzmannConstant;

		double calculatedGasConstant = avogadroNumber * boltzmannConstant;
		double storedGasConstant = PhysicalConstants.Fundamental.GasConstant;

		Assert.AreEqual(calculatedGasConstant, storedGasConstant, Tolerance);
	}

	[TestMethod]
	public void Ln2_MatchesCalculatedValue()
	{
		double calculatedLn2 = Math.Log(2.0);
		double storedLn2 = PhysicalConstants.Chemical.Ln2;

		Assert.AreEqual(calculatedLn2, storedLn2, Tolerance);
	}

	[TestMethod]
	public void GenericHelpers_ReturnCorrectValues()
	{
		Assert.AreEqual(PhysicalConstants.Fundamental.AvogadroNumber,
			PhysicalConstants.Generic.AvogadroNumber<double>(), Tolerance);

		Assert.AreEqual(PhysicalConstants.Fundamental.GasConstant,
			PhysicalConstants.Generic.GasConstant<double>(), Tolerance);
	}
}
