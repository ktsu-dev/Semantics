// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class ChromaticAnalysisTests
{
	private static Key CMajor => Key.Create(PitchClass.Create(0), Mode.Major);

	[TestMethod]
	public void ChromaticChords_SkipsDiatonicChords()
	{
		System.Collections.Generic.IReadOnlyList<ChromaticAnalysis> analyses =
			Progression.Parse("4/4  C | Dm | G7 | C").ChromaticChords(CMajor);
		Assert.IsEmpty(analyses);
	}

	[TestMethod]
	public void ChromaticChords_DetectsSecondaryDominantOfDominant()
	{
		// D7 in C is V/V (resolving toward G, the dominant).
		System.Collections.Generic.IReadOnlyList<ChromaticAnalysis> analyses =
			Progression.Parse("4/4  C | D7 | G7 | C").ChromaticChords(CMajor);
		Assert.HasCount(1, analyses);
		Assert.AreEqual(ChromaticKind.SecondaryDominant, analyses[0].Kind);
		Assert.AreEqual("V/V", analyses[0].Detail);
		Assert.AreEqual(1, analyses[0].Index);
	}

	[TestMethod]
	public void ChromaticChords_DetectsNeapolitan()
	{
		System.Collections.Generic.IReadOnlyList<ChromaticAnalysis> analyses =
			Progression.Parse("4/4  C | Db | G7").ChromaticChords(CMajor);
		Assert.AreEqual(ChromaticKind.Neapolitan, analyses[0].Kind);
		Assert.AreEqual("bII", analyses[0].Detail);
	}

	[TestMethod]
	public void ChromaticChords_DetectsBorrowedMinorSubdominant()
	{
		// Fm in C major is iv borrowed from the parallel minor.
		System.Collections.Generic.IReadOnlyList<ChromaticAnalysis> analyses =
			Progression.Parse("4/4  C | Fm | C").ChromaticChords(CMajor);
		Assert.AreEqual(ChromaticKind.BorrowedChord, analyses[0].Kind);
		Assert.AreEqual("from parallel minor", analyses[0].Detail);
	}

	[TestMethod]
	public void ChromaticChords_ParsedIonianKey_IsAnalyzedAsMajor()
	{
		Key ionian = Key.Parse("C ionian");

		System.Collections.Generic.IReadOnlyList<ChromaticAnalysis> secondary =
			Progression.Parse("4/4  C | D7 | G7 | C").ChromaticChords(ionian);
		Assert.AreEqual("V/V", secondary[0].Detail);

		System.Collections.Generic.IReadOnlyList<ChromaticAnalysis> borrowed =
			Progression.Parse("4/4  C | Fm | C").ChromaticChords(ionian);
		Assert.AreEqual(ChromaticKind.BorrowedChord, borrowed[0].Kind);
		Assert.AreEqual("from parallel minor", borrowed[0].Detail);
	}

	[TestMethod]
	public void ChromaticAnalysis_Create_RejectsNegativeIndex() =>
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ChromaticAnalysis.Create(-1, ChromaticKind.Chromatic, null));

	[TestMethod]
	[DataRow("G mixolydian", "4/4  G | E | Am", "V/ii")]
	[DataRow("C lydian", "4/4  C | A | D", "V/II")]
	[DataRow("D dorian", "4/4  Dm | B | Em", "V/ii")]
	[DataRow("A aeolian", "4/4  Am | A7 | Dm", "V/iv")]
	[DataRow("C major", "4/4  C | E7 | Am", "V/vi")]
	[DataRow("A harmonic_minor", "4/4  Am | G | Am", "V/III+")]
	public void ChromaticChords_NamesSecondaryDominantTargetByTheModesOwnTriad(string key, string progression, string expected)
	{
		System.Collections.Generic.IReadOnlyList<ChromaticAnalysis> analyses =
			Progression.Parse(progression).ChromaticChords(Key.Parse(key));
		Assert.HasCount(1, analyses);
		Assert.AreEqual(ChromaticKind.SecondaryDominant, analyses[0].Kind);
		Assert.AreEqual(expected, analyses[0].Detail);
	}

	[TestMethod]
	public void ChromaticChords_DoesNotNameSecondaryDominantsInScalesWithoutSevenDegrees()
	{
		// F is a fifth above B♭, degree 8 of C octatonic_hw. Wrapping that degree onto seven used to
		// label it V/i, the tonic, which is exactly what a secondary dominant excludes.
		System.Collections.Generic.IReadOnlyList<ChromaticAnalysis> analyses =
			Progression.Parse("4/4  C | F | C").ChromaticChords(Key.Parse("C octatonic_hw"));
		Assert.HasCount(1, analyses);
		Assert.AreNotEqual(ChromaticKind.SecondaryDominant, analyses[0].Kind);
	}
}
