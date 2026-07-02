// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class ProgressionRoundTripTests
{
	[TestMethod]
	public void ToStringRendersBeatSlashesAndBarLines()
	{
		Progression p = Progression.Create(
		[
			ChordEvent.Create(Chord.Parse("Dm7"), Duration.Half),
			ChordEvent.Create(Chord.Parse("G7"), Duration.Half),
			ChordEvent.Create(Chord.Parse("Cmaj7"), Duration.Whole),
		]);
		Assert.AreEqual("4/4  Dm7 / G7 / | Cmaj7 / / /", p.ToString());
	}

	[TestMethod]
	public void RoundTripWholeBeatDurations()
	{
		Progression p = Progression.Create([Chord.Parse("Am7"), Chord.Parse("D7")], Duration.Quarter);
		Assert.AreEqual(p, Progression.Parse(p.ToString()));
	}

	[TestMethod]
	public void RoundTripSubBeatDurationUsesEscape()
	{
		Progression p = Progression.Create([ChordEvent.Create(Chord.Parse("C"), Duration.Eighth)]);
		Assert.AreEqual("4/4  C@1/8", p.ToString());
		Assert.AreEqual(p, Progression.Parse(p.ToString()));
	}
}
