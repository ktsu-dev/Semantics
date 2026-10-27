// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class RomanNumeralParseTests
{
	private static Key CMajor => Key.Create(PitchClass.Create(0), Mode.Major);
	private static Key CMinor => Key.Create(PitchClass.Create(0), Mode.Aeolian);

	[TestMethod]
	public void Parse_SupertonicMinorSeventh()
	{
		Chord ii = CMajor.ChordFromRomanNumeral("ii7");
		Assert.AreEqual(2, ii.Root.Value); // D
		Assert.AreEqual(ChordQuality.Minor, ii.Quality);
		Assert.AreEqual(SeventhType.Dominant, ii.Seventh);
	}

	[TestMethod]
	public void Parse_TonicMajorSeventh()
	{
		Chord i = CMajor.ChordFromRomanNumeral("Imaj7");
		Assert.AreEqual(0, i.Root.Value);
		Assert.AreEqual(ChordQuality.Major, i.Quality);
		Assert.AreEqual(SeventhType.Major, i.Seventh);
	}

	[TestMethod]
	public void Parse_NeapolitanFlatTwo()
	{
		Chord neapolitan = CMajor.ChordFromRomanNumeral("bII");
		Assert.AreEqual(1, neapolitan.Root.Value); // Db
		Assert.AreEqual(ChordQuality.Major, neapolitan.Quality);
	}

	[TestMethod]
	public void Parse_LeadingToneDiminishedSeventh()
	{
		Chord vii = CMajor.ChordFromRomanNumeral("vii°7");
		Assert.AreEqual(11, vii.Root.Value); // B
		Assert.AreEqual(ChordQuality.Diminished, vii.Quality);
		Assert.AreEqual(SeventhType.Diminished, vii.Seventh);
	}

	[TestMethod]
	public void Parse_LeadingToneHalfDiminishedSeventh()
	{
		Chord vii = CMajor.ChordFromRomanNumeral("viiø7");
		Assert.AreEqual(11, vii.Root.Value); // B
		Assert.AreEqual(Chord.Parse("Bm7b5"), vii);
	}

	[TestMethod]
	public void RomanNumeralOf_KeepsTheSeventhOfAFullyDiminishedSeventh()
	{
		Assert.AreEqual("vii°", CMajor.RomanNumeralOf(Chord.Parse("Bdim")));
		Assert.AreEqual("vii°7", CMajor.RomanNumeralOf(Chord.Parse("Bdim7")));
	}

	[TestMethod]
	public void RomanNumeralOf_SpellsAHalfDiminishedSeventhWithØ()
	{
		Assert.AreEqual("viiø7", CMajor.RomanNumeralOf(Chord.Parse("Bm7b5")));
		Assert.AreEqual(Chord.Parse("Bm7b5"), CMajor.ChordFromRomanNumeral(CMajor.RomanNumeralOf(Chord.Parse("Bm7b5"))));
	}

	[TestMethod]
	public void Parse_FlatSixInMinor()
	{
		Chord six = CMinor.ChordFromRomanNumeral("VI");
		Assert.AreEqual(8, six.Root.Value); // Ab
		Assert.AreEqual(ChordQuality.Major, six.Quality);
	}

	[TestMethod]
	public void Parse_IsInverseOfRomanNumeralOf()
	{
		string[] numerals = ["Imaj7", "ii7", "iii", "IV", "V7", "vi", "vii°", "vii°7", "viiø7"];
		foreach (string numeral in numerals)
		{
			Chord chord = CMajor.ChordFromRomanNumeral(numeral);
			Assert.AreEqual(numeral, CMajor.RomanNumeralOf(chord), $"Round-trip failed for '{numeral}'.");
		}
	}

	[TestMethod]
	[DataRow("Vb9", 7, ChordTensions.FlatNine)]
	[DataRow("V#9", 7, ChordTensions.SharpNine)]
	[DataRow("Vb13", 7, ChordTensions.FlatThirteen)]
	[DataRow("Ib5", 0, ChordTensions.None)]
	public void Parse_AnAlterationAfterAnUpperCaseNumeralKeepsTheRoot(string numeral, int root, ChordTensions tensions)
	{
		Chord chord = CMajor.ChordFromRomanNumeral(numeral);
		Assert.AreEqual(root, chord.Root.Value, numeral);
		Assert.AreEqual(tensions, chord.Tensions, numeral);
	}

	[TestMethod]
	public void Parse_IbFiveIsAMajorFlatFiveTriad() =>
		Assert.AreEqual(ChordQuality.MajorFlatFive, CMajor.ChordFromRomanNumeral("Ib5").Quality);

	[TestMethod]
	public void RomanNumeralOf_KeepsSixthsTensionsAndSuspensions()
	{
		(string Symbol, string Numeral)[] cases =
		[
			("C6", "I6"), ("Am6", "vi6"), ("C6/9", "I6/9"), ("Cadd9", "Iadd9"), ("C9", "I9"),
			("C7b9", "I7b9"), ("G13", "V13"), ("Cmaj7#11", "Imaj7#11"), ("Cmaj7sus4", "Imaj7sus4"),
			("G7sus4", "V7sus4"), ("C(b5)", "I(b5)"), ("Bm7b5", "viiø7"), ("Bm9b5", "viiø9"),
			("Bdim7", "vii°7"), ("Caug7", "I+7"), ("Cm#5", "i#5"),
		];

		foreach ((string symbol, string numeral) in cases)
		{
			Assert.AreEqual(numeral, CMajor.RomanNumeralOf(Chord.Parse(symbol)), symbol);
		}
	}

	[TestMethod]
	public void FromRomanNumeral_InvertsRomanNumeralOf_ForEveryChordWithoutABass()
	{
		Key[] keys = [CMajor, CMinor];
		string[] bodies =
		[
			"", "m", "dim", "aug", "sus2", "sus4", "5", "(b5)", "m#5", "6", "m6", "6/9", "add9", "madd11",
			"7", "maj7", "m7", "mmaj7", "dim7", "m7b5", "m9b5", "aug7", "7b5", "maj7b5", "m7#5",
			"9", "m9", "maj9", "11", "13", "7b9", "7#9", "7#11", "7b13", "13b9#11", "maj7#11",
			"7sus4", "9sus4", "maj7sus4", "maj9sus4", "maj7sus2", "7add13", "(b9)", "(#11)", "7no3", "dimmaj7",
		];

		foreach (Key key in keys)
		{
			foreach (PitchClass degree in key.Scale.PitchClasses)
			{
				foreach (Chord chord in bodies.Select(body => Chord.Parse(degree.Name + body)))
				{
					string numeral = key.RomanNumeralOf(chord);
					Assert.AreEqual(chord, key.ChordFromRomanNumeral(numeral), $"{chord} in {key} -> {numeral}");
				}
			}
		}
	}

	[TestMethod]
	public void Parse_RejectsGarbage()
	{
		_ = Assert.ThrowsExactly<FormatException>(() => CMajor.ChordFromRomanNumeral("xyz"));
	}
}
