// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using ktsu.Semantics.Quantities;
using ktsu.Semantics.Quantities.Units;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class AudioEngineeringTests
{
	private const double Tolerance = 1e-6;
	private const double DbTolerance = 1e-3;

	[TestMethod]
	public void Gain_ToDecibels_UsesAmplitudeConvention()
	{
		// A gain factor of 2 is approximately +6.0206 dB.
		Decibels<double> db = Gain<double>.Create(2.0).ToDecibels();
		Assert.AreEqual(6.0206, db.Value, DbTolerance);
	}

	[TestMethod]
	public void Decibels_Unity_IsZeroDbAndUnityGain()
	{
		Assert.AreEqual(0.0, Decibels<double>.Unity.Value, Tolerance);
		Assert.AreEqual(1.0, Decibels<double>.Unity.ToAmplitude().Value, Tolerance);
	}

	[TestMethod]
	public void Decibels_AmplitudeRoundTrip()
	{
		Gain<double> gain = Gain<double>.Create(0.5);
		Gain<double> roundTrip = gain.ToDecibels().ToAmplitude();
		Assert.AreEqual(gain.Value, roundTrip.Value, Tolerance);
	}

	[TestMethod]
	public void Decibels_PowerConvention_TenDbIsTenfold()
	{
		Ratio<double> power = Decibels<double>.Create(10.0).ToPower();
		Assert.AreEqual(10.0, power.Value, Tolerance);
		Assert.AreEqual(10.0, Decibels<double>.FromPower(10.0).Value, Tolerance);
	}

	[TestMethod]
	public void Decibels_Addition_CombinesLevels()
	{
		Decibels<double> sum = Decibels<double>.Create(-6.0) + Decibels<double>.Create(-6.0);
		Assert.AreEqual(-12.0, sum.Value, Tolerance);
	}

	[TestMethod]
	public void Semitones_Octave_IsFrequencyRatioOfTwo()
	{
		Assert.AreEqual(2.0, Semitones<double>.Octave.ToFrequencyRatio().Value, Tolerance);
		Assert.AreEqual(12.0, Semitones<double>.FromFrequencyRatio(Ratio<double>.Create(2.0)).Value, Tolerance);
	}

	[TestMethod]
	public void Semitones_ToCents_IsHundredfold()
	{
		Assert.AreEqual(1200.0, Semitones<double>.Octave.ToCents().Value, Tolerance);
		Assert.AreEqual(12.0, Cents<double>.Create(1200.0).ToSemitones().Value, Tolerance);
	}

	[TestMethod]
	public void Cents_OctaveIsFrequencyRatioOfTwo()
	{
		Assert.AreEqual(2.0, Cents<double>.Create(1200.0).ToFrequencyRatio().Value, Tolerance);
		Assert.AreEqual(1200.0, Cents<double>.FromFrequencyRatio(Ratio<double>.Create(2.0)).Value, Tolerance);
	}

	[TestMethod]
	public void RatioPercent_RoundTrip()
	{
		// Percent is a unit of the Dimensionless dimension, not a separate type.
		Assert.AreEqual(0.5, Ratio<double>.FromPercent(50.0).Value, Tolerance);
		Assert.AreEqual(50.0, Ratio<double>.Create(0.5).In(Units.Percent), Tolerance);
		Assert.AreEqual(0.25, Ratio<double>.FromPercent(25.0).Value, Tolerance);
	}

	[TestMethod]
	public void Gain_Is_A_Generated_Ratio_Overload()
	{
		// Gain widens implicitly to the generated Ratio and is V0 (non-negative
		// through the guarded From{Unit} factories).
		Gain<double> gain = Gain<double>.Create(2.0);
		Ratio<double> asRatio = gain;
		Assert.AreEqual(2.0, asRatio.Value, Tolerance);
		Assert.ThrowsExactly<ArgumentException>(() => Gain<double>.FromDimensionless(-1.0));
	}

	[TestMethod]
	public void Gain_Cascading_Multiplies()
	{
		Gain<double> total = Gain<double>.Create(2.0) * Gain<double>.Create(0.5);
		Assert.AreEqual(1.0, total.Value, Tolerance);
		Assert.AreEqual(Gain<double>.Unity.Value, total.Value, Tolerance);
	}

	[TestMethod]
	public void Decibels_FromPowerRatio_Takes_The_Generated_Ratio()
	{
		Decibels<double> db = Decibels<double>.FromPowerRatio(Ratio<double>.Create(100.0));
		Assert.AreEqual(20.0, db.Value, Tolerance);
	}

	[TestMethod]
	public void Ratio_Arithmetic_Comes_From_The_Generator()
	{
		// The audio-specific Ratio struct is gone; the generated Dimensionless
		// Ratio supplies the arithmetic.
		Ratio<double> mix = Ratio<double>.Create(0.5) * Ratio<double>.Create(0.5);
		Assert.AreEqual(0.25, mix.Value, Tolerance);
		// Same-type division yields the raw storage ratio, as for every quantity.
		Assert.AreEqual(2.0, Ratio<double>.Create(1.0) / Ratio<double>.Create(0.5), Tolerance);
	}

	[TestMethod]
	public void QFactor_FromBandwidth_AndBack()
	{
		QFactor<double> q = QFactor<double>.FromBandwidth(1000.0, 100.0);
		Assert.AreEqual(10.0, q.Value, Tolerance);
		Assert.AreEqual(100.0, q.BandwidthAt(1000.0), Tolerance);
	}

	[TestMethod]
	public void QFactor_OctaveBandwidthRoundTrip()
	{
		QFactor<double> q = QFactor<double>.FromBandwidthInOctaves(1.0);
		// One octave bandwidth corresponds to Q = sqrt(2)/(2-1) ≈ 1.41421.
		Assert.AreEqual(1.41421356, q.Value, 1e-5);
		Assert.AreEqual(1.0, q.ToBandwidthInOctaves(), 1e-6);
	}

	[TestMethod]
	public void NormalizedParameter_Linear_MapsMidpoint()
	{
		NormalizedParameter<double> p = NormalizedParameter<double>.Linear(0.0, 10.0);
		Assert.AreEqual(0.0, p.Denormalize(0.0), Tolerance);
		Assert.AreEqual(5.0, p.Denormalize(0.5), Tolerance);
		Assert.AreEqual(10.0, p.Denormalize(1.0), Tolerance);
		Assert.AreEqual(0.5, p.Normalize(5.0), Tolerance);
	}

	[TestMethod]
	public void NormalizedParameter_Linear_ClampsOutOfRange()
	{
		NormalizedParameter<double> p = NormalizedParameter<double>.Linear(0.0, 10.0);
		Assert.AreEqual(0.0, p.Denormalize(-1.0), Tolerance);
		Assert.AreEqual(10.0, p.Denormalize(2.0), Tolerance);
		Assert.AreEqual(0.0, p.Normalize(-5.0), Tolerance);
		Assert.AreEqual(1.0, p.Normalize(50.0), Tolerance);
	}

	[TestMethod]
	public void NormalizedParameter_Logarithmic_MidpointIsGeometricMean()
	{
		NormalizedParameter<double> p = NormalizedParameter<double>.Logarithmic(20.0, 20000.0);
		Assert.AreEqual(20.0, p.Denormalize(0.0), Tolerance);
		Assert.AreEqual(20000.0, p.Denormalize(1.0), 1e-3);
		// Geometric mean of 20 and 20000 = sqrt(400000) ≈ 632.4555.
		Assert.AreEqual(632.4555, p.Denormalize(0.5), 1e-3);
		Assert.AreEqual(0.5, p.Normalize(632.4555), 1e-6);
	}

	[TestMethod]
	public void NormalizedParameter_WithCenter_PlacesCenterAtMidpoint()
	{
		NormalizedParameter<double> p = NormalizedParameter<double>.WithCenter(0.0, 10.0, 2.0);
		Assert.AreEqual(2.0, p.Denormalize(0.5), 1e-6);
		Assert.AreEqual(0.0, p.Denormalize(0.0), Tolerance);
		Assert.AreEqual(10.0, p.Denormalize(1.0), Tolerance);
	}

	[TestMethod]
	public void NormalizedParameter_Skewed_RoundTrips()
	{
		NormalizedParameter<double> p = NormalizedParameter<double>.Skewed(0.0, 10.0, 2.0);
		Assert.AreEqual(2.5, p.Denormalize(0.5), 1e-6); // 10 * 0.5^2
		for (double x = 0.0; x <= 1.0; x += 0.1)
		{
			Assert.AreEqual(x, p.Normalize(p.Denormalize(x)), 1e-6);
		}
	}

	[TestMethod]
	public void NormalizedParameter_Clamp_BoundsValue()
	{
		NormalizedParameter<double> p = NormalizedParameter<double>.Linear(1.0, 5.0);
		Assert.AreEqual(1.0, p.Clamp(-2.0), Tolerance);
		Assert.AreEqual(5.0, p.Clamp(9.0), Tolerance);
		Assert.AreEqual(3.0, p.Clamp(3.0), Tolerance);
	}

	[TestMethod]
	public void NormalizedParameter_InvalidArguments_Throw()
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => NormalizedParameter<double>.Logarithmic(0.0, 1000.0));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => NormalizedParameter<double>.Logarithmic(-10.0, 10.0));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => NormalizedParameter<double>.Skewed(0.0, 1.0, 0.0));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => NormalizedParameter<double>.WithCenter(0.0, 10.0, 20.0));
	}
}
