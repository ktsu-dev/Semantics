// Copyright (c) 2023-2026 ktsu-dev contributors

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

	[TestMethod]
	[DataRow("[Verse]\n4/4 C | G |\nAm | F |")]
	[DataRow("[Verse]\n4/4 C | G |\nthis is garbage")]
	[DataRow("[Verse]\n4/4 C | G |\n\nAm | F |")]
	[DataRow("[Verse]\r\n4/4 C | G |\r\nAm | F |\r\n")]
	public void TryParseRejectsALineAfterTheProgression(string text)
	{
		Assert.IsFalse(Section.TryParse(text, out Section? result));
		Assert.IsNull(result);
		Assert.ThrowsExactly<FormatException>(() => Section.Parse(text));
	}

	[TestMethod]
	[DataRow("[Verse]\n4/4 C | G |\n")]
	[DataRow("[Verse]\n4/4 C | G |\n\n")]
	[DataRow("[Verse]\r\n4/4 C | G |\r\n  \r\n")]
	public void TryParseAcceptsTrailingBlankLines(string text)
	{
		Assert.IsTrue(Section.TryParse(text, out Section? result));
		Assert.AreEqual(Section.Parse("[Verse]\n4/4 C | G |"), result);
	}

	[TestMethod]
	[DataRow("[Verse, Chorus]\n4/4  Dm7 / G7 /")]
	[DataRow("[42]\n4/4  Dm7 / G7 /")]
	[DataRow("[-1]\n4/4  Dm7 / G7 /")]
	[DataRow("[ Verse ]\n4/4  Dm7 / G7 /")]
	[DataRow("[verse]\n4/4  Dm7 / G7 /")]
	public void TryParseRejectsHeaderThatIsNotOneSectionTypeName(string text)
	{
		Assert.IsFalse(Section.TryParse(text, out Section? result));
		Assert.IsNull(result);
	}

	[TestMethod]
	public void TryParseAcceptsEverySectionTypeName()
	{
		foreach (SectionType type in Enum.GetValues<SectionType>())
		{
			Section s = Section.Create(type, SampleProgression());
			Assert.AreEqual(s, Section.Parse(s.ToString()));
		}
	}

	[TestMethod]
	[DataRow("Verse (reprise)")]
	[DataRow("Outro (fade")]
	[DataRow("Chorus x2)")]
	[DataRow("The \"big\" one")]
	[DataRow("Bridge ]")]
	[DataRow("Back\\slash \\n")]
	[DataRow("Two\nlines\r")]
	[DataRow("")]
	public void RoundTripLabelWithDelimiters(string label)
	{
		Key key = Key.Create(PitchClass.Create(NoteLetter.C, Accidental.Natural), Mode.Major);
		Section withoutKey = Section.Create(SectionType.Verse, SampleProgression(), label);
		Section withKey = Section.Create(SectionType.Verse, SampleProgression(), label, key);
		Assert.AreEqual(withoutKey, Section.Parse(withoutKey.ToString()));
		Assert.AreEqual(withKey, Section.Parse(withKey.ToString()));
	}

	[TestMethod]
	public void ToStringEscapesQuoteBackslashAndLineBreaksInLabel()
	{
		Section s = Section.Create(SectionType.Verse, SampleProgression(), "a\"b\\c\nd\re");
		Assert.AreEqual("[Verse] \"a\\\"b\\\\c\\nd\\re\"\n4/4  Dm7 / G7 /", s.ToString());
	}

	[TestMethod]
	public void ParseKeepsBackslashThatIsNotAnEscape()
	{
		Section s = Section.Parse("[Verse] \"A\\B\"\n4/4  Dm7 / G7 /");
		Assert.AreEqual("A\\B", s.Label);
	}
}
