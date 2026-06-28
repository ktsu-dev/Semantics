// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

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
	public void Parse_FlatSixInMinor()
	{
		Chord six = CMinor.ChordFromRomanNumeral("VI");
		Assert.AreEqual(8, six.Root.Value); // Ab
		Assert.AreEqual(ChordQuality.Major, six.Quality);
	}

	[TestMethod]
	public void Parse_IsInverseOfRomanNumeralOf()
	{
		string[] numerals = ["Imaj7", "ii7", "iii", "IV", "V7", "vi", "vii°"];
		foreach (string numeral in numerals)
		{
			Chord chord = CMajor.ChordFromRomanNumeral(numeral);
			Assert.AreEqual(numeral, CMajor.RomanNumeralOf(chord), $"Round-trip failed for '{numeral}'.");
		}
	}

	[TestMethod]
	public void Parse_RejectsGarbage()
	{
		_ = Assert.ThrowsExactly<FormatException>(() => CMajor.ChordFromRomanNumeral("xyz"));
	}
}
