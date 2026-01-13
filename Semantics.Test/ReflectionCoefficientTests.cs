// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class ReflectionCoefficientTests
{
	private const double Tolerance = 1e-10;

	[TestMethod]
	public void FromImpedances_NormalIncidence_ShouldMatchFormula()
	{
		AcousticImpedance<double> z1 = AcousticImpedance<double>.FromRayls(415.0);
		AcousticImpedance<double> z2 = AcousticImpedance<double>.FromRayls(1000.0);

		ReflectionCoefficient<double> r = ReflectionCoefficient<double>.FromImpedances(z1, z2);
		double expected = (z2.Value - z1.Value) / (z2.Value + z1.Value);
		Assert.AreEqual(expected, r.Value, Tolerance);
	}

	[TestMethod]
	public void TransmissionCoefficient_ShouldBeOneMinusReflection()
	{
		ReflectionCoefficient<double> r = ReflectionCoefficient<double>.FromCoefficient(0.3);
		Assert.AreEqual(0.7, r.TransmissionCoefficient(), Tolerance);
	}

	[TestMethod]
	public void FromAbsorptionCoefficient_And_ToAbsorptionCoefficient_RoundTrip()
	{
		SoundAbsorption<double> alpha = SoundAbsorption<double>.Create(0.25);
		ReflectionCoefficient<double> r = ReflectionCoefficient<double>.FromAbsorptionCoefficient(alpha);
		SoundAbsorption<double> back = r.ToAbsorptionCoefficient();

		Assert.AreEqual(0.75, r.Value, Tolerance);
		Assert.AreEqual(alpha.Value, back.Value, Tolerance);
	}

	[TestMethod]
	public void AtObliqueIncidence_ShouldReduceReflectionComparedToNormal_ForSmallAngles()
	{
		double angleRad = Math.PI / 6.0; // 30 degrees
		double ratio = 1000.0 / 415.0; // Z2/Z1

		ReflectionCoefficient<double> rOblique = ReflectionCoefficient<double>.AtObliqueIncidence(angleRad, ratio);
		double rNormal = (ratio - 1.0) / (ratio + 1.0);

		Assert.IsLessThan(rNormal, rOblique.Value);
	}
}

