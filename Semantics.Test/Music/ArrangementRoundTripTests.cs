// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class ArrangementRoundTripTests
{
	private static Arrangement Sample()
	{
		Key key = Key.Create(PitchClass.Create(NoteLetter.A, Accidental.Natural), Mode.Aeolian);
		Progression intro = Progression.Create([ChordEvent.Create(Chord.Parse("Am"), Duration.Whole)]);
		Progression verse = Progression.Create([Chord.Parse("Am7"), Chord.Parse("Dm7")], Duration.Half);
		return Arrangement.Create(key,
		[
			Section.Create(SectionType.Intro, intro),
			Section.Create(SectionType.Verse, verse, "Verse 1"),
		]);
	}

	[TestMethod]
	public void ToStringHasKeyLineThenSections()
	{
		Assert.AreEqual(
			"A aeolian\n\n[Intro]\n4/4  Am / / /\n\n[Verse] \"Verse 1\"\n4/4  Am7 / Dm7 /",
			Sample().ToString());
	}

	[TestMethod]
	public void RoundTrip()
	{
		Arrangement a = Sample();
		Assert.AreEqual(a, Arrangement.Parse(a.ToString()));
	}
}
