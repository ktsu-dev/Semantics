// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class TimeSignatureTests
{
	[TestMethod]
	public void FourFour_BarIsOneWholeNote()
	{
		Assert.AreEqual(Duration.Whole, TimeSignature.Create(4, 4).BarDuration);
	}

	[TestMethod]
	public void SevenEight_BarIsSevenEighths()
	{
		Assert.AreEqual(Duration.Create(7, 8), TimeSignature.Create(7, 8).BarDuration);
	}

	[TestMethod]
	public void BeatDuration_IsOneOverBeatUnit()
	{
		Assert.AreEqual(Duration.Eighth, TimeSignature.Create(7, 8).BeatDuration);
	}

	[TestMethod]
	public void Create_RejectsNonPositive()
	{
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => TimeSignature.Create(0, 4));
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => TimeSignature.Create(4, 0));
	}

	[TestMethod]
	public void ToStringIsBeatsOverUnit()
	{
		Assert.AreEqual("6/8", TimeSignature.Create(6, 8).ToString());
	}

	[TestMethod]
	public void RoundTrip()
	{
		TimeSignature ts = TimeSignature.Create(7, 8);
		Assert.AreEqual(ts, TimeSignature.Parse(ts.ToString()));
	}

	[TestMethod]
	public void TryParseFailsOnNonPositive()
	{
		Assert.IsFalse(TimeSignature.TryParse("0/4", out TimeSignature? result));
		Assert.IsNull(result);
	}
}
