// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class IntervalTests
{
	[TestMethod]
	public void Between_ReturnsSignedDistance()
	{
		Interval up = Interval.Between(Pitch.Create(60), Pitch.Create(67));
		Assert.AreEqual(7, up.Semitones);

		Interval down = Interval.Between(Pitch.Create(67), Pitch.Create(60));
		Assert.AreEqual(-7, down.Semitones);
	}

	[TestMethod]
	public void Folded_ReducesMagnitudeButKeepsDirection()
	{
		Assert.AreEqual(7, Interval.Create(19).Folded.Semitones);
		Assert.AreEqual(-1, Interval.Create(-13).Folded.Semitones);
		Assert.AreEqual(0, Interval.Create(24).Folded.Semitones);
	}

	[TestMethod]
	public void Direction_IsSign()
	{
		Assert.AreEqual(1, Interval.Create(3).Direction);
		Assert.AreEqual(-1, Interval.Create(-3).Direction);
		Assert.AreEqual(0, Interval.Create(0).Direction);
	}

	[TestMethod]
	public void ToStringIsSignedSemitones()
	{
		Assert.AreEqual("7", Interval.Create(7).ToString());
		Assert.AreEqual("-5", Interval.Create(-5).ToString());
	}

	[TestMethod]
	public void RoundTrip()
	{
		foreach (int s in new[] { 0, 7, -5, 14 })
		{
			Interval i = Interval.Create(s);
			Assert.AreEqual(i, Interval.Parse(i.ToString()));
		}
	}

	[TestMethod]
	public void TryParseFailsOnGarbage()
	{
		Assert.IsFalse(Interval.TryParse("x", out Interval? result));
		Assert.IsNull(result);
	}
}
