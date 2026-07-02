// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class ChordEventTests
{
	[TestMethod]
	public void ToStringIsChordColonDuration()
	{
		ChordEvent e = ChordEvent.Create(Chord.Parse("Cmaj7"), Duration.Quarter);
		Assert.AreEqual("Cmaj7:1/4", e.ToString());
	}

	[TestMethod]
	public void RoundTripWithSlashChord()
	{
		ChordEvent e = ChordEvent.Create(Chord.Parse("C/G"), Duration.Half);
		Assert.AreEqual(e, ChordEvent.Parse(e.ToString()));
	}

	[TestMethod]
	public void TryParseFailsWithoutColon()
	{
		Assert.IsFalse(ChordEvent.TryParse("Cmaj7", out ChordEvent? result));
		Assert.IsNull(result);
	}
}
