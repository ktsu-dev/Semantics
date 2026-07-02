// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class SectionRoundTripTests
{
	private static Progression SampleProgression() =>
		Progression.Create([Chord.Parse("Dm7"), Chord.Parse("G7")], Duration.Half);

	[TestMethod]
	public void ToStringWithLabelAndKey()
	{
		Section s = Section.Create(SectionType.Verse, SampleProgression(), "Verse 1", Key.Create(PitchClass.Create(NoteLetter.C, Accidental.Natural), Mode.Major));
		Assert.AreEqual("[Verse] \"Verse 1\" (C major)\n4/4  Dm7 / G7 /", s.ToString());
	}

	[TestMethod]
	public void RoundTripMinimal()
	{
		Section s = Section.Create(SectionType.Intro, SampleProgression());
		Assert.AreEqual(s, Section.Parse(s.ToString()));
	}

	[TestMethod]
	public void RoundTripWithLabelAndKey()
	{
		Section s = Section.Create(SectionType.Chorus, SampleProgression(), "Chorus", Key.Create(PitchClass.Create(NoteLetter.A, Accidental.Natural), Mode.Aeolian));
		Assert.AreEqual(s, Section.Parse(s.ToString()));
	}
}
