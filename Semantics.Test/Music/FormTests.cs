// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class FormTests
{
	private static Key CMajor => Key.Create(PitchClass.Create(0), Mode.Major);

	private static Section Section(SectionType type, string chords) =>
		ktsu.Semantics.Music.Section.Create(type, Progression.Parse("4/4  " + chords));

	// One chord per bar, each held for the whole bar.
	private static string Bars(params string[] chords) => string.Join(" | ", chords.Select(chord => chord + " / / /"));

	[TestMethod]
	public void Of_ExtractsAABAPattern_AndNamesThirtyTwoBarForm()
	{
		Section a = Section(SectionType.Verse, Bars("C", "Am", "F", "G", "C", "Am", "F", "C"));
		Section b = Section(SectionType.Bridge, Bars("F", "G", "Em", "Am", "F", "G", "Dm", "G"));
		Arrangement arrangement = Arrangement.Create(CMajor, [a, a, b, a]);
		Form form = arrangement.Form;
		Assert.AreEqual("AABA", form.Pattern);
		Assert.AreEqual(NamedForm.ThirtyTwoBarAABA, form.Name);
		Assert.AreEqual("AABA", form.ToString());
		Assert.AreEqual(form, Form.Parse(form.ToString()));
	}

	[TestMethod]
	public void Of_AABAOfShortSections_IsNotTheThirtyTwoBarForm()
	{
		Section a = Section(SectionType.Verse, Bars("C", "G"));
		Section b = Section(SectionType.Bridge, Bars("F", "G"));
		Form form = Arrangement.Create(CMajor, [a, a, b, a]).Form;
		Assert.AreEqual("AABA", form.Pattern);
		Assert.AreEqual(NamedForm.Unknown, form.Name);
		Assert.AreEqual("AABA (Unknown)", form.ToString());
		Assert.AreEqual(form, Form.Parse(form.ToString()));
	}

	[TestMethod]
	public void Parse_BareAABA_IsTakenAsTheThirtyTwoBarForm()
	{
		Assert.AreEqual(NamedForm.ThirtyTwoBarAABA, Form.Parse("AABA").Name);
	}

	[TestMethod]
	public void Of_RecognizesVerseChorus_FromTheSpecExample()
	{
		Section verse = Section(SectionType.Verse, Bars("Am", "F", "C", "G"));
		Section chorus = Section(SectionType.Chorus, Bars("F", "G", "C", "C"));
		Section bridge = Section(SectionType.Bridge, Bars("Dm", "Em", "F", "G"));
		Form form = Arrangement.Create(CMajor, [verse, chorus, verse, chorus, bridge, chorus]).Form;
		Assert.AreEqual("ABABCB", form.Pattern);
		Assert.AreEqual(NamedForm.VerseChorus, form.Name);
		Assert.AreEqual("ABABCB (VerseChorus)", form.ToString());
		Assert.AreEqual(form, Form.Parse(form.ToString()));
	}

	[TestMethod]
	public void Of_VerseChorus_AllowsAnIntroPreChorusAndOutro()
	{
		Section intro = Section(SectionType.Intro, Bars("C"));
		Section verse = Section(SectionType.Verse, Bars("Am", "F"));
		Section pre = Section(SectionType.PreChorus, Bars("Dm", "G"));
		Section chorus = Section(SectionType.Chorus, Bars("F", "C"));
		Section outro = Section(SectionType.Outro, Bars("C"));
		Assert.AreEqual(NamedForm.VerseChorus, Arrangement.Create(CMajor, [intro, verse, pre, chorus, verse, pre, chorus, outro]).Form.Name);
	}

	[TestMethod]
	public void Of_VerseNotAnsweredByAChorus_IsNotVerseChorus()
	{
		Section verse = Section(SectionType.Verse, Bars("Am", "F"));
		Section chorus = Section(SectionType.Chorus, Bars("F", "C"));
		Assert.AreNotEqual(NamedForm.VerseChorus, Arrangement.Create(CMajor, [verse, verse, chorus, verse, chorus]).Form.Name);
		Assert.AreNotEqual(NamedForm.VerseChorus, Arrangement.Create(CMajor, [verse, chorus, verse, chorus, verse]).Form.Name);
		Assert.AreNotEqual(NamedForm.VerseChorus, Arrangement.Create(CMajor, [verse, chorus, verse]).Form.Name);
	}

	[TestMethod]
	public void Of_AllVerses_StaysStrophic()
	{
		Section verse = Section(SectionType.Verse, Bars("Am", "F"));
		Assert.AreEqual(NamedForm.Strophic, Arrangement.Create(CMajor, [verse, verse, verse]).Form.Name);
	}

	[TestMethod]
	public void TwelveBarBluesForm_RoundTripsThroughText()
	{
		Section blues = Section(SectionType.Verse, Bars("C7", "C7", "C7", "C7", "F7", "F7", "C7", "C7", "G7", "F7", "C7", "C7"));
		Form form = Arrangement.Create(CMajor, [blues]).Form;
		Assert.AreEqual(NamedForm.TwelveBarBlues, form.Name);
		Assert.AreEqual("A (TwelveBarBlues)", form.ToString());
		Assert.AreEqual(form, Form.Parse(form.ToString()));
	}

	[TestMethod]
	[DataRow("A (Blues)")]
	[DataRow("A (1)")]
	[DataRow("A (Binary, Rondo)")]
	[DataRow("A TwelveBarBlues")]
	[DataRow("A (TwelveBarBlues")]
	[DataRow("A ()")]
	[DataRow(" (Unknown)")]
	[DataRow("a (Unknown)")]
	public void TryParse_RejectsAMalformedName(string text)
	{
		Assert.IsFalse(Form.TryParse(text, out Form? form));
		Assert.IsNull(form);
	}

	[TestMethod]
	public void Of_RecognizesTernaryAndBinary()
	{
		Section a = Section(SectionType.Verse, "C | G");
		Section b = Section(SectionType.Chorus, "F | C");
		Assert.AreEqual(NamedForm.Ternary, Arrangement.Create(CMajor, [a, b, a]).Form.Name);
		Assert.AreEqual(NamedForm.Binary, Arrangement.Create(CMajor, [a, b]).Form.Name);
	}

	[TestMethod]
	public void Of_RecognizesTwelveBarBlues()
	{
		Section blues = Section(SectionType.Verse, "C7 / / / | C7 / / / | C7 / / / | C7 / / / | F7 / / / | F7 / / / | C7 / / / | C7 / / / | G7 / / / | F7 / / / | C7 / / / | C7 / / /");
		Arrangement arrangement = Arrangement.Create(CMajor, [blues]);
		Assert.AreEqual(NamedForm.TwelveBarBlues, arrangement.Form.Name);
	}

	[TestMethod]
	public void Of_TwentySixDistinctSections_RoundTripsThroughParse()
	{
		Arrangement arrangement = Arrangement.Create(CMajor, DistinctSections(Form.MaxDistinctSections));
		Form form = arrangement.Form;
		Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZ", form.Pattern);
		Assert.AreEqual(form, Form.Parse(form.ToString()));
	}

	[TestMethod]
	public void Of_MoreDistinctSectionsThanLetters_Throws()
	{
		Arrangement arrangement = Arrangement.Create(CMajor, DistinctSections(Form.MaxDistinctSections + 1));
		_ = Assert.ThrowsExactly<ArgumentException>(() => Form.Of(arrangement));
	}

	[TestMethod]
	public void Of_RepeatsBeyondTwentySixSections_StillLabelsByStructure()
	{
		List<Section> sections = DistinctSections(Form.MaxDistinctSections);
		sections.Add(sections[0]);
		Form form = Arrangement.Create(CMajor, sections).Form;
		Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZA", form.Pattern);
	}

	// Sections with different chord counts never share a structure, so section i has i + 1 bars of C.
	private static List<Section> DistinctSections(int count) =>
		[.. Enumerable.Range(1, count).Select(bars => Section(SectionType.Verse, string.Join(" | ", Enumerable.Repeat("C", bars))))];

	[TestMethod]
	public void Parse_AppliesLetterRecognition()
	{
		Form form = Form.Parse("ABACA");
		Assert.AreEqual(NamedForm.Rondo, form.Name);
		Assert.HasCount(5, form.Letters);
	}

	[TestMethod]
	public void Parse_RejectsNonLetters() =>
		_ = Assert.ThrowsExactly<FormatException>(() => Form.Parse("A1B"));

	[TestMethod]
	public void ToStringRoundTripsPattern()
	{
		Form form = Form.Parse("AABA");
		Assert.AreEqual("AABA", form.ToString());
		Assert.AreEqual(form, Form.Parse(form.ToString()));
	}

	[TestMethod]
	public void TryParseFailsOnLowercase()
	{
		Assert.IsFalse(Form.TryParse("aaba", out Form? result));
		Assert.IsNull(result);
	}

	[TestMethod]
	public void Of_RecognizesTwelveBarBlues_WithHeldChords()
	{
		Section blues = Section(
			SectionType.Verse,
			"C7 / / / / / / / / / / / / / / / | F7 / / / / / / / | C7 / / / / / / / | G7 / / / | F7 / / / | C7 / / / / / / /");
		Assert.AreEqual(12, blues.Bars);
		Assert.AreEqual(NamedForm.TwelveBarBlues, Arrangement.Create(CMajor, [blues]).Form.Name);
	}

	[TestMethod]
	public void Of_RecognizesTwelveBarBlues_ByDownbeatChord()
	{
		Section blues = Section(SectionType.Verse, "C7 / / / | C7 / / / | C7 / / / | C7 / G7 / | F7 / / / | F7 / / / | C7 / / / | C7 / / / | G7 / / / | F7 / / / | C7 / / / | C7 / G7 /");
		Assert.AreEqual(NamedForm.TwelveBarBlues, Arrangement.Create(CMajor, [blues]).Form.Name);
	}

	[TestMethod]
	public void Of_DoesNotCallTwelveQuarterNoteChordsABlues()
	{
		Section shortSection = Section(SectionType.Verse, "C7 C7 C7 C7 | F7 F7 C7 C7 | G7 F7 C7 C7");
		Assert.AreEqual(3, shortSection.Bars);
		Assert.AreNotEqual(NamedForm.TwelveBarBlues, Arrangement.Create(CMajor, [shortSection]).Form.Name);
	}
}
