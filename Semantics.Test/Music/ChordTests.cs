// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using System.Collections.Generic;
using ktsu.Semantics.Music;

[TestClass]
public class ChordTests
{
	[TestMethod]
	public void Parse_BareLetterIsMajorTriad()
	{
		Chord c = Chord.Parse("C");
		Assert.AreEqual(0, c.Root.Value);
		Assert.AreEqual(ChordQuality.Major, c.Quality);
		Assert.AreEqual(SeventhType.None, c.Seventh);
	}

	[TestMethod]
	public void Parse_MinorSeventh()
	{
		Chord c = Chord.Parse("Dm7");
		Assert.AreEqual(2, c.Root.Value);
		Assert.AreEqual(ChordQuality.Minor, c.Quality);
		Assert.AreEqual(SeventhType.Dominant, c.Seventh);
	}

	[TestMethod]
	public void Parse_MajorSeventh()
	{
		Chord c = Chord.Parse("Cmaj7");
		Assert.AreEqual(ChordQuality.Major, c.Quality);
		Assert.AreEqual(SeventhType.Major, c.Seventh);
	}

	[TestMethod]
	public void Parse_MinorMajorSeventh_IsBothMinorAndMaj7()
	{
		Chord c = Chord.Parse("Cmmaj7");
		Assert.AreEqual(ChordQuality.Minor, c.Quality);
		Assert.AreEqual(SeventhType.Major, c.Seventh);
	}

	[TestMethod]
	public void Parse_HalfDiminished_IsDiminishedTriadWithDominantSeventh()
	{
		Chord c = Chord.Parse("Cm7b5");
		Assert.AreEqual(ChordQuality.Diminished, c.Quality);
		Assert.AreEqual(SeventhType.Dominant, c.Seventh);
	}

	[TestMethod]
	public void Parse_PowerChord_HasNoThird()
	{
		Chord c = Chord.Parse("C5");
		Assert.AreEqual(ChordQuality.Power, c.Quality);
	}

	[TestMethod]
	public void Parse_SixthChords()
	{
		Assert.AreEqual(SixthType.Natural, Chord.Parse("C6").Sixth);
		Assert.AreEqual(SixthType.Natural, Chord.Parse("Cm6").Sixth);
		Assert.AreEqual(SixthType.Flat, Chord.Parse("Cmb6").Sixth);
	}

	[TestMethod]
	public void Parse_OmitVoicings()
	{
		Assert.IsTrue(Chord.Parse("C7no5").Omissions.HasFlag(ChordOmission.Fifth));
		Assert.IsTrue(Chord.Parse("Cm7no3").Omissions.HasFlag(ChordOmission.Third));
	}

	[TestMethod]
	public void Parse_SlashBass()
	{
		Chord c = Chord.Parse("C/G");
		Assert.AreEqual(0, c.Root.Value);
		Assert.IsNotNull(c.Bass);
		Assert.AreEqual(7, c.Bass!.Value);
	}

	[TestMethod]
	public void Parse_RejectsEmpty()
	{
		_ = Assert.ThrowsExactly<FormatException>(() => Chord.Parse(""));
	}

	[TestMethod]
	public void Voice_PlacesRootAtOctaveAndStacksTones()
	{
		int[] expected = [60, 64, 67];
		int[] midi = [.. Chord.Parse("C").Voice(4).Select(p => p.Midi)];
		CollectionAssert.AreEqual(expected, midi);
	}

	[TestMethod]
	public void Voice_SlashBassSoundsBelowRoot()
	{
		IReadOnlyList<Pitch> voicing = Chord.Parse("C/G").Voice(4);
		Assert.AreEqual(55, voicing[0].Midi); // G3 below C4
		Assert.AreEqual(60, voicing[1].Midi); // C4 root
	}

	[TestMethod]
	public void ChordTones_MatchHeatDeathRomanceQualityTable()
	{
		// Oracle: HeatDeathRomance/scripts/midi/_theory.py QUALITIES, rooted on C.
		// Each entry: chord symbol -> the exact semitone offsets above the root.
		(string Symbol, int[] Tones)[] cases =
		[
			("C", [0, 4, 7]),
			("Cm", [0, 3, 7]),
			("Cdim", [0, 3, 6]),
			("Caug", [0, 4, 8]),
			("C5", [0, 7]),
			("Csus2", [0, 2, 7]),
			("Csus4", [0, 5, 7]),
			("C6", [0, 4, 7, 9]),
			("Cm6", [0, 3, 7, 9]),
			("C7", [0, 4, 7, 10]),
			("Cmaj7", [0, 4, 7, 11]),
			("Cm7", [0, 3, 7, 10]),
			("Cm7b5", [0, 3, 6, 10]),
			("C7no5", [0, 4, 10]),
			("Cm7no3", [0, 7, 10]),
			("C9", [0, 4, 7, 10, 14]),
			("Cmaj9", [0, 4, 7, 11, 14]),
			("Cm9", [0, 3, 7, 10, 14]),
			("Cm11", [0, 3, 7, 10, 14, 17]),
			("Cadd9", [0, 4, 7, 14]),
			("Cmadd9", [0, 3, 7, 14]),
			("Cmb6", [0, 3, 7, 8]),
			("Cadd9no5", [0, 4, 14]),
			("C7b9", [0, 4, 7, 10, 13]),
			("C7b13", [0, 4, 7, 10, 20]),
			("C7b9b13", [0, 4, 7, 10, 13, 20]),
			("C7sus4", [0, 5, 7, 10]),
			("Cmaj7#11", [0, 4, 7, 11, 18]),
			("Cdim7", [0, 3, 6, 9]),
			("C7#9", [0, 4, 7, 10, 15]),
			("C7#5", [0, 4, 8, 10]),
			("Cmmaj7", [0, 3, 7, 11]),
		];

		foreach ((string symbol, int[] expected) in cases)
		{
			int[] actual = [.. Chord.Parse(symbol).ChordTones()];
			CollectionAssert.AreEqual(expected, actual, $"ChordTones mismatch for '{symbol}'.");
		}
	}
}
