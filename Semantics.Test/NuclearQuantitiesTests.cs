// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using ktsu.Semantics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class NuclearQuantitiesTests
{
	[TestMethod]
	public void AbsorbedDose_Conversions_And_Basics()
	{
		AbsorbedDose<double> fromRads = AbsorbedDose<double>.FromRads(100.0);
		Assert.AreEqual(1.0, fromRads.Value, 1e-12);

		AbsorbedDose<double> fromMicro = AbsorbedDose<double>.FromMicrograys(1_000_000.0);
		Assert.AreEqual(1.0, fromMicro.Value, 1e-12);

		Energy<double> energy = Energy<double>.FromJoules(10.0);
		Mass<double> mass = Mass<double>.FromKilograms(2.0);
		AbsorbedDose<double> fromEoverM = AbsorbedDose<double>.FromEnergyAndMass(energy, mass);
		Assert.AreEqual(5.0, fromEoverM.Value, 1e-12);

		Assert.ThrowsExactly<ArgumentException>(() => AbsorbedDose<double>.FromEnergyAndMass(energy, Mass<double>.FromKilograms(0.0)));
	}

	[TestMethod]
	public void AbsorbedDose_Calculations_And_Exceptions()
	{
		AbsorbedDose<double> dose = AbsorbedDose<double>.FromGrays(2.0);
		Mass<double> mass = Mass<double>.FromKilograms(3.0);
		Energy<double> energy = dose.CalculateEnergyAbsorbed(mass);
		Assert.AreEqual(6.0, energy.Value, 1e-12);

		EquivalentDose<double> ed = dose.CalculateEquivalentDose(5.0);
		Assert.AreEqual(10.0, ed.Value, 1e-12);

		Time<double> t2 = Time<double>.FromSeconds(2.0);
		double rate = AbsorbedDose<double>.FromGrays(4.0).CalculateDoseRate(t2);
		Assert.AreEqual(2.0, rate, 1e-12);
		Assert.ThrowsExactly<ArgumentException>(() => AbsorbedDose<double>.FromGrays(1.0).CalculateDoseRate(Time<double>.FromSeconds(0.0)));

		Mass<double> massFromEnergy = AbsorbedDose<double>.FromGrays(5.0).CalculateMass(Energy<double>.FromJoules(10.0));
		Assert.AreEqual(2.0, massFromEnergy.Value, 1e-12);
		Assert.ThrowsExactly<InvalidOperationException>(() => AbsorbedDose<double>.FromGrays(0.0).CalculateMass(Energy<double>.FromJoules(1.0)));

		Assert.IsTrue(AbsorbedDose<double>.FromGrays(5.0).IsPotentiallyLethal());
		Assert.IsFalse(AbsorbedDose<double>.FromGrays(3.0).IsPotentiallyLethal());
		Assert.IsTrue(AbsorbedDose<double>.FromGrays(0.3).CausesRadiationSickness());
		Assert.IsFalse(AbsorbedDose<double>.FromGrays(0.2).CausesRadiationSickness());
	}

	[TestMethod]
	public void EquivalentDose_Conversions_And_Calculations()
	{
		EquivalentDose<double> fromRems = EquivalentDose<double>.FromRems(100.0);
		Assert.AreEqual(1.0, fromRems.Value, 1e-12);

		EquivalentDose<double> fromMicro = EquivalentDose<double>.FromMicrosieverts(1_000_000.0);
		Assert.AreEqual(1.0, fromMicro.Value, 1e-12);

		EquivalentDose<double> fromDwr = EquivalentDose<double>.FromAbsorbedDoseAndWeightingFactor(AbsorbedDose<double>.FromGrays(2.0), 10.0);
		Assert.AreEqual(20.0, fromDwr.Value, 1e-12);

		AbsorbedDose<double> backToD = EquivalentDose<double>.FromSieverts(10.0).CalculateAbsorbedDose(5.0);
		Assert.AreEqual(2.0, backToD.Value, 1e-12);
		Assert.ThrowsExactly<ArgumentException>(() => EquivalentDose<double>.FromSieverts(1.0).CalculateAbsorbedDose(0.0));

		double rate = EquivalentDose<double>.FromSieverts(4.0).CalculateDoseRate(Time<double>.FromSeconds(2.0));
		Assert.AreEqual(2.0, rate, 1e-12);
		Assert.ThrowsExactly<ArgumentException>(() => EquivalentDose<double>.FromSieverts(1.0).CalculateDoseRate(Time<double>.FromSeconds(0.0)));

		Assert.IsTrue(EquivalentDose<double>.FromSieverts(0.03).ExceedsAnnualWorkerLimit());
		Assert.IsFalse(EquivalentDose<double>.FromSieverts(0.01).ExceedsAnnualWorkerLimit());
		Assert.IsTrue(EquivalentDose<double>.FromSieverts(0.002).ExceedsPublicLimit());
		Assert.IsFalse(EquivalentDose<double>.FromSieverts(0.0005).ExceedsPublicLimit());

		EquivalentDose<double> eff = EquivalentDose<double>.FromSieverts(10.0).CalculateEffectiveDoseContribution(0.12);
		Assert.AreEqual(1.2, eff.Value, 1e-12);
	}

	[TestMethod]
	public void Exposure_Conversions_And_Calculations()
	{
		Exposure<double> fromR = Exposure<double>.FromRoentgens(1.0);
		Assert.AreEqual(2.58e-4, fromR.Value, 1e-12);

		Exposure<double> frommR = Exposure<double>.FromMilliroentgens(1000.0);
		Assert.AreEqual(2.58e-4, frommR.Value, 1e-12);

		ElectricCharge<double> q = ElectricCharge<double>.Create(1.0);
		Mass<double> m = Mass<double>.FromKilograms(2.0);
		Exposure<double> x = Exposure<double>.FromChargeAndMass(q, m);
		Assert.AreEqual(0.5, x.Value, 1e-12);
		Assert.ThrowsExactly<ArgumentException>(() => Exposure<double>.FromChargeAndMass(q, Mass<double>.FromKilograms(0.0)));

		ElectricCharge<double> calcQ = Exposure<double>.FromCoulombsPerKilogram(0.2).CalculateCharge(Mass<double>.FromKilograms(2.0));
		Assert.AreEqual(0.4, calcQ.Value, 1e-12);

		Assert.ThrowsExactly<InvalidOperationException>(() => Exposure<double>.FromCoulombsPerKilogram(0.0).CalculateMass(ElectricCharge<double>.Create(1.0)));
		Mass<double> calcM = Exposure<double>.FromCoulombsPerKilogram(0.5).CalculateMass(ElectricCharge<double>.Create(1.0));
		Assert.AreEqual(2.0, calcM.Value, 1e-12);

		double rate = Exposure<double>.FromCoulombsPerKilogram(2.0).CalculateExposureRate(Time<double>.FromSeconds(4.0));
		Assert.AreEqual(0.5, rate, 1e-12);
		Assert.ThrowsExactly<ArgumentException>(() => Exposure<double>.FromCoulombsPerKilogram(1.0).CalculateExposureRate(Time<double>.FromSeconds(0.0)));

		AbsorbedDose<double> doseInAir = Exposure<double>.FromRoentgens(1.0).ToAbsorbedDoseInAir();
		Assert.AreEqual(0.00876, doseInAir.Value, 1e-12);
	}
}
