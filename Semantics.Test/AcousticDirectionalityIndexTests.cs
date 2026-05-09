// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class AcousticDirectionalityIndexTests
{
	[TestMethod]
	public void FromDirectivityFactor_And_ToDirectivityFactor_ShouldBeConsistent()
	{
		DirectionalityIndex<double> diFromQ1 = DirectionalityIndex<double>.FromDirectivityFactor(1.0);
		Assert.AreEqual(0.0, diFromQ1.Value, 1e-12);

		DirectionalityIndex<double> diFromQ10 = DirectionalityIndex<double>.FromDirectivityFactor(10.0);
		Assert.AreEqual(10.0, diFromQ10.Value, 1e-12);

		double qBack = diFromQ10.ToDirectivityFactor();
		Assert.AreEqual(10.0, qBack, 1e-12);
	}

	[TestMethod]
	public void FromDecibels_And_OnAxisGain_ShouldWork()
	{
		DirectionalityIndex<double> di = DirectionalityIndex<double>.FromDecibels(6.0);
		Assert.AreEqual(6.0, di.Value, 1e-12);
		Assert.AreEqual(6.0, di.OnAxisGain(), 1e-12);
	}

	[TestMethod]
	public void GetDirectivityPattern_Thresholds_ShouldMatch()
	{
		Assert.AreEqual(
			"Omnidirectional (no directivity)",
			DirectionalityIndex<double>.FromDecibels(0.5).GetDirectivityPattern());

		Assert.AreEqual(
			"Slightly directional",
			DirectionalityIndex<double>.FromDecibels(1.0).GetDirectivityPattern());

		Assert.AreEqual(
			"Moderately directional",
			DirectionalityIndex<double>.FromDecibels(3.0).GetDirectivityPattern());

		Assert.AreEqual(
			"Highly directional",
			DirectionalityIndex<double>.FromDecibels(7.0).GetDirectivityPattern());

		Assert.AreEqual(
			"Very directional",
			DirectionalityIndex<double>.FromDecibels(10.0).GetDirectivityPattern());

		Assert.AreEqual(
			"Extremely directional (beam-like)",
			DirectionalityIndex<double>.FromDecibels(15.0).GetDirectivityPattern());
	}

	[TestMethod]
	public void EstimateBeamwidth_ShouldApproximateExpected()
	{
		// DI = 0 dB => Q = 1 => beamwidth ≈ 58.3°
		DirectionalityIndex<double> di0 = DirectionalityIndex<double>.FromDecibels(0.0);
		Assert.AreEqual(58.3, di0.EstimateBeamwidth(), 1e-6);

		// DI = 6 dB => Q = 10^0.6 ≈ 3.981 => beamwidth ≈ 58.3 / sqrt(3.981) ≈ 29.22°
		DirectionalityIndex<double> di6 = DirectionalityIndex<double>.FromDecibels(6.0);
		Assert.AreEqual(29.22, di6.EstimateBeamwidth(), 1e-2);
	}

	[TestMethod]
	public void EstimateFrontToBackRatio_ShouldRespectCap()
	{
		DirectionalityIndex<double> di10 = DirectionalityIndex<double>.FromDecibels(10.0);
		Assert.AreEqual(15.0, di10.EstimateFrontToBackRatio(), 1e-12);

		DirectionalityIndex<double> di25 = DirectionalityIndex<double>.FromDecibels(25.0);
		Assert.AreEqual(30.0, di25.EstimateFrontToBackRatio(), 1e-12);
	}

	[TestMethod]
	public void GetTypicalApplication_Thresholds_ShouldMatch()
	{
		Assert.AreEqual(
			"Ambient sound sources, subwoofers",
			DirectionalityIndex<double>.FromDecibels(1.0).GetTypicalApplication());

		Assert.AreEqual(
			"Monitor speakers, near-field applications",
			DirectionalityIndex<double>.FromDecibels(2.0).GetTypicalApplication());

		Assert.AreEqual(
			"Home audio, bookshelf speakers",
			DirectionalityIndex<double>.FromDecibels(4.0).GetTypicalApplication());

		Assert.AreEqual(
			"Studio monitors, PA speakers",
			DirectionalityIndex<double>.FromDecibels(6.0).GetTypicalApplication());

		Assert.AreEqual(
			"Horn-loaded speakers, line arrays",
			DirectionalityIndex<double>.FromDecibels(8.0).GetTypicalApplication());

		Assert.AreEqual(
			"Highly directional arrays, sound reinforcement",
			DirectionalityIndex<double>.FromDecibels(10.0).GetTypicalApplication());
	}

	[TestMethod]
	public void CoverageAngle_ShouldComputeReasonableAngles()
	{
		// DI = 6 dB, level = -3 dB => adjusted DI = 3 dB => Q ≈ 1.995 => angle ≈ 90°
		DirectionalityIndex<double> di6 = DirectionalityIndex<double>.FromDecibels(6.0);
		double angleMinus3 = di6.CoverageAngle(-3.0);
		Assert.AreEqual(90.0, angleMinus3, 1.0);

		// DI = 6 dB, level = -6 dB => adjusted DI = 0 dB => Q = 1 => angle = 0°
		double angleMinus6 = di6.CoverageAngle(-6.0);
		Assert.AreEqual(0.0, angleMinus6, 1e-9);
	}
}
