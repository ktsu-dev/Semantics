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
}
