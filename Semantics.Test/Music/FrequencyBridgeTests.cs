// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class FrequencyBridgeTests
{
	[TestMethod]
	public void A4_Is440Hz()
	{
		Assert.AreEqual(440.0, Pitch.FromName("A4").FrequencyHz, 1e-9);
	}

	[TestMethod]
	public void MiddleC_IsAbout261Point63Hz()
	{
		Assert.AreEqual(261.6255653, Pitch.Create(60).FrequencyHz, 1e-6);
	}

	[TestMethod]
	public void FromFrequency_RoundTripsToNearestPitch()
	{
		Assert.AreEqual(69, Pitch.FromFrequency(440.0).Midi);
		Assert.AreEqual(60, Pitch.FromFrequency(261.63).Midi);
		Assert.AreEqual(69, Pitch.FromFrequency(445.0).Midi); // snaps to the nearest semitone
	}

	[TestMethod]
	public void FromFrequency_RejectsNonPositive()
	{
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Pitch.FromFrequency(0.0));
	}

	[TestMethod]
	public void Interval_OctaveIs1200Cents()
	{
		Assert.AreEqual(1200.0, Interval.Create(12).Cents, 1e-9);
		Assert.AreEqual(-700.0, Interval.Create(-7).Cents, 1e-9);
	}
}
