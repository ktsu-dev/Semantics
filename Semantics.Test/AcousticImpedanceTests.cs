// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class AcousticImpedanceTests
{
	private const double Tolerance = 1e-10;

	[TestMethod]
	public void FromRayls_And_FromPascalSecondsPerMeter_ShouldCreateSameValue()
	{
		AcousticImpedance<double> a = AcousticImpedance<double>.FromRayls(415.0);
		AcousticImpedance<double> b = AcousticImpedance<double>.FromPascalSecondsPerMeter(415.0);

		Assert.AreEqual(a.Value, b.Value, Tolerance);
	}

	[TestMethod]
	public void Multiply_Density_And_SoundSpeed_ShouldMatch_Z_Equals_RhoC()
	{
		Density<double> rho = Density<double>.FromKilogramsPerCubicMeter(1.225);
		SoundSpeed<double> c = SoundSpeed<double>.FromMetersPerSecond(343.0);

		AcousticImpedance<double> z = AcousticImpedance<double>.Multiply(rho, c);
		AcousticImpedance<double> z2 = AcousticImpedance<double>.FromDensityAndSoundSpeed(rho, c);

		Assert.AreEqual(rho.Value * c.Value, z.Value, Tolerance);
		Assert.AreEqual(z2.Value, z.Value, Tolerance);
	}

	[TestMethod]
	public void Divide_SoundPressure_By_Velocity_ShouldCreateImpedance()
	{
		SoundPressure<double> p = SoundPressure<double>.Create(2.0);
		Velocity<double> u = Velocity<double>.Create(0.004819);

		AcousticImpedance<double> z = AcousticImpedance<double>.Divide(p, u);

		Assert.AreEqual(p.Value / u.Value, z.Value, 1e-12);
	}

	[TestMethod]
	public void CalculateSoundSpeed_And_Density_ShouldInvertRelations()
	{
		Density<double> rho = Density<double>.FromKilogramsPerCubicMeter(1.225);
		SoundSpeed<double> c = SoundSpeed<double>.FromMetersPerSecond(343.0);
		AcousticImpedance<double> z = AcousticImpedance<double>.FromDensityAndSoundSpeed(rho, c);

		SoundSpeed<double> computedC = AcousticImpedance<double>.CalculateSoundSpeed(z, rho);
		Density<double> computedRho = AcousticImpedance<double>.CalculateDensity(z, c);

		Assert.AreEqual(c.Value, computedC.Value, Tolerance);
		Assert.AreEqual(rho.Value, computedRho.Value, Tolerance);
	}

	[TestMethod]
	public void ForStandardAir_ShouldReturnReasonableValue()
	{
		AcousticImpedance<double> zAir = AcousticImpedance<double>.ForStandardAir();
		Assert.IsTrue(zAir.Value is > 400 and < 450, "Standard air acoustic impedance should be between 400 and 450 Rayls");
	}

	[TestMethod]
	public void Dimension_ShouldBe_AcousticImpedance()
	{
		AcousticImpedance<double> z = AcousticImpedance<double>.Create(1.0);
		Assert.AreEqual(PhysicalDimensions.AcousticImpedance, z.Dimension);
	}
}

