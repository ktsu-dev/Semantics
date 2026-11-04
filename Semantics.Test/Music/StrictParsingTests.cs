// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

/// <summary>
/// The music types parse what they print and nothing looser: no whitespace or '+' around a number, no event of zero or
/// negative length, and no slash after a chord whose length is already stated.
/// </summary>
[TestClass]
public class StrictParsingTests
{
	[TestMethod]
	[DataRow(" 1 / 4 ")]
	[DataRow("+1/+4")]
	[DataRow("1/ 4")]
	[DataRow("1 /4")]
	[DataRow("+1/4")]
	public void Duration_RejectsWhitespaceAndPlus(string text) =>
		Assert.IsFalse(Duration.TryParse(text, out _));

	[TestMethod]
	[DataRow(" 64 ")]
	[DataRow("+64")]
	[DataRow("-0")]
	public void Velocity_RejectsWhitespaceAndSigns(string text) =>
		Assert.IsFalse(Velocity.TryParse(text, out _));

	[TestMethod]
	[DataRow(" 3 / 4 ")]
	[DataRow("+3/4")]
	[DataRow("3/+4")]
	public void TimeSignature_RejectsWhitespaceAndPlus(string text) =>
		Assert.IsFalse(TimeSignature.TryParse(text, out _));

	[TestMethod]
	[DataRow(" 7 ")]
	[DataRow("+7")]
	[DataRow("- 7")]
	public void Interval_RejectsWhitespaceAndPlus(string text) =>
		Assert.IsFalse(Interval.TryParse(text, out _));

	[TestMethod]
	public void Interval_StillAcceptsANegativeCount()
	{
		Assert.IsTrue(Interval.TryParse("-7", out Interval? interval));
		Assert.AreEqual(-7, interval.Semitones);
	}

	[TestMethod]
	[DataRow("C4: 1/4 :v 64 ")]
	[DataRow("C4:+1/4:v+64")]
	public void Note_RejectsWhitespaceAndPlus(string text) =>
		Assert.IsFalse(Note.TryParse(text, out _));

	[TestMethod]
	[DataRow(" 120bpm@1/4")]
	[DataRow("120 bpm@1/4")]
	[DataRow("+120bpm@1/4")]
	[DataRow("120bpm@ 1/4 ")]
	public void Tempo_RejectsWhitespaceAndPlus(string text) =>
		Assert.IsFalse(Tempo.TryParse(text, out _));

	[TestMethod]
	[DataRow(120.0)]
	[DataRow(97.5)]
	[DataRow(1e20)]
	[DataRow(1e-5)]
	public void Tempo_StillRoundTrips(double bpm)
	{
		Tempo tempo = Tempo.Create(bpm, Duration.Quarter);
		Assert.AreEqual(tempo, Tempo.Parse(tempo.ToString()));
	}

	[TestMethod]
	[DataRow("C4:-1/4:v64")]
	[DataRow("C4:0/4:v64")]
	public void Note_RejectsANonPositiveDuration(string text) =>
		Assert.IsFalse(Note.TryParse(text, out _));

	[TestMethod]
	[DataRow("R:0/4")]
	[DataRow("R:-1/4")]
	public void Rest_RejectsANonPositiveDuration(string text) =>
		Assert.IsFalse(Rest.TryParse(text, out _));

	[TestMethod]
	[DataRow("Am:-3/4")]
	[DataRow("Am:0/1")]
	public void ChordEvent_RejectsANonPositiveDuration(string text) =>
		Assert.IsFalse(ChordEvent.TryParse(text, out _));

	[TestMethod]
	[DataRow("4/4  C@-1/4 G / / /")]
	[DataRow("4/4  C@0/1 G / / /")]
	public void Progression_RejectsANonPositiveCell(string text) =>
		Assert.IsFalse(Progression.TryParse(text, out _));

	[TestMethod]
	public void Create_RejectsANonPositiveDuration()
	{
		Duration zero = Duration.Create(0, 1);
		Duration negative = Duration.Create(-1, 4);
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Note.Create(Pitch.Create(60), zero, Velocity.MezzoForte));
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Rest.Create(negative));
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ChordEvent.Create(Chord.Parse("C"), zero));
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Progression.Create([Chord.Parse("C")], negative));
	}

	[TestMethod]
	public void Duration_StillCarriesAZeroAndANegativeValue()
	{
		Assert.AreEqual(Duration.Create(0, 1), Duration.Parse("0/1"));
		Assert.AreEqual(Duration.Create(-1, 4), Duration.Parse("-1/4"));
	}

	[TestMethod]
	[DataRow("4/4  C@1/2 / / | G")]
	[DataRow("4/4  C@1/2 / | G")]
	[DataRow("4/4  C@1/2 | / G")]
	public void Progression_RejectsASlashAfterAnExplicitDuration(string text)
	{
		Assert.IsFalse(Progression.TryParse(text, out Progression? progression));
		Assert.IsNull(progression);
		_ = Assert.ThrowsExactly<FormatException>(() => Progression.Parse(text));
	}

	[TestMethod]
	public void Progression_BarLinesFollowTheBarAfterSubBeatChords()
	{
		Progression progression = Progression.Create(
		[
			ChordEvent.Create(Chord.Parse("C"), Duration.Eighth),
			ChordEvent.Create(Chord.Parse("G"), Duration.Create(3, 8)),
			ChordEvent.Create(Chord.Parse("F"), Duration.Half),
			ChordEvent.Create(Chord.Parse("E"), Duration.Whole),
		]);
		Assert.AreEqual("4/4  C@1/8 G@3/8 F / | E / / /", progression.ToString());
		Assert.AreEqual(progression, Progression.Parse(progression.ToString()));
	}

	[TestMethod]
	public void Progression_ChordHeldAcrossTheBarLineCarriesIt()
	{
		Progression progression = Progression.Create(
		[
			ChordEvent.Create(Chord.Parse("C"), Duration.Create(3, 4)),
			ChordEvent.Create(Chord.Parse("G"), Duration.Half),
			ChordEvent.Create(Chord.Parse("F"), Duration.Create(3, 4)),
		]);
		Assert.AreEqual("4/4  C / / G | / F / /", progression.ToString());
		Assert.AreEqual(progression, Progression.Parse(progression.ToString()));
	}

	[TestMethod]
	public void Progression_ChordHeldOverSeveralBarsGetsEachBarLine()
	{
		Progression progression = Progression.Create([ChordEvent.Create(Chord.Parse("C"), Duration.Create(9, 4))]);
		Assert.AreEqual("4/4  C / / / | / / / / | /", progression.ToString());
		Assert.AreEqual(progression, Progression.Parse(progression.ToString()));
	}

	[TestMethod]
	public void Progression_BarLineInsideAnExplicitCellIsLeftOut()
	{
		Progression progression = Progression.Create(
		[
			ChordEvent.Create(Chord.Parse("C"), Duration.Create(7, 8)),
			ChordEvent.Create(Chord.Parse("G"), Duration.Create(3, 8)),
			ChordEvent.Create(Chord.Parse("F"), Duration.Half),
		]);
		Assert.AreEqual("4/4  C@7/8 G@3/8 F /", progression.ToString());
		Assert.AreEqual(progression, Progression.Parse(progression.ToString()));
	}

	[TestMethod]
	public void Arrangement_AcceptsExtraBlankLinesBetweenSections()
	{
		Arrangement arrangement = Sample();
		Assert.AreEqual(arrangement, Arrangement.Parse(arrangement.ToString().Replace("\n\n", "\n\n\n")));
	}

	[TestMethod]
	[DataRow("\n \n")]
	[DataRow("\n\t\n")]
	[DataRow("\n  \n\n\t\n")]
	public void Arrangement_AcceptsWhitespaceOnlySeparatorLines(string separator)
	{
		Arrangement arrangement = Sample();
		Assert.AreEqual(arrangement, Arrangement.Parse(arrangement.ToString().Replace("\n\n", separator)));
	}

	[TestMethod]
	public void Arrangement_AcceptsBlankLinesAroundTheChart()
	{
		Arrangement arrangement = Sample();
		Assert.AreEqual(arrangement, Arrangement.Parse($"\n\n{arrangement}\n\n"));
	}

	private static Arrangement Sample()
	{
		Key key = Key.Create(PitchClass.Create(0), Mode.Major);
		Progression verse = Progression.Parse("4/4  C / / / | G / / /");
		Progression chorus = Progression.Parse("4/4  F / / / | C / / /");
		return Arrangement.Create(key,
		[
			Section.Create(SectionType.Verse, verse, "Verse 1"),
			Section.Create(SectionType.Chorus, chorus),
		]);
	}

	[TestMethod]
	[DataRow("[Verse] (C major) \"Verse 1\"\n4/4  C / / /")]
	[DataRow("[Verse]garbage\n4/4  C / / /")]
	[DataRow("[Verse] \"V\" trailing junk\n4/4  C / / /")]
	[DataRow("[Verse] (C major) junk\n4/4  C / / /")]
	[DataRow("[Verse] \n4/4  C / / /")]
	[DataRow("[Verse]  \"V\"\n4/4  C / / /")]
	[DataRow("[Verse] \"V\" \n4/4  C / / /")]
	[DataRow("[Verse] \"V\"(C major)\n4/4  C / / /")]
	[DataRow("[Verse](C major)\n4/4  C / / /")]
	public void Section_RejectsAHeaderOutsideTheGrammar(string text) =>
		Assert.IsFalse(Section.TryParse(text, out _));

	[TestMethod]
	public void Section_StillParsesALabelAndKey()
	{
		Assert.IsTrue(Section.TryParse("[Verse] \"Verse 1\" (C major)\n4/4  C / / /", out Section? section));
		Assert.AreEqual(SectionType.Verse, section.Type);
		Assert.AreEqual("Verse 1", section.Label);
		Assert.AreEqual(Key.Parse("C major"), section.Key);
	}
}
