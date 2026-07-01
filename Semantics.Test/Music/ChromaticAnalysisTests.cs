// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

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
			Progression.Parse("C | Dm | G7 | C").ChromaticChords(CMajor);
		Assert.AreEqual(0, analyses.Count);
	}

	[TestMethod]
	public void ChromaticChords_DetectsSecondaryDominantOfDominant()
	{
		// D7 in C is V/V (resolving toward G, the dominant).
		System.Collections.Generic.IReadOnlyList<ChromaticAnalysis> analyses =
			Progression.Parse("C | D7 | G7 | C").ChromaticChords(CMajor);
		Assert.AreEqual(1, analyses.Count);
		Assert.AreEqual(ChromaticKind.SecondaryDominant, analyses[0].Kind);
		Assert.AreEqual("V/V", analyses[0].Detail);
		Assert.AreEqual(1, analyses[0].Index);
	}

	[TestMethod]
	public void ChromaticChords_DetectsNeapolitan()
	{
		System.Collections.Generic.IReadOnlyList<ChromaticAnalysis> analyses =
			Progression.Parse("C | Db | G7").ChromaticChords(CMajor);
		Assert.AreEqual(ChromaticKind.Neapolitan, analyses[0].Kind);
		Assert.AreEqual("bII", analyses[0].Detail);
	}

	[TestMethod]
	public void ChromaticChords_DetectsBorrowedMinorSubdominant()
	{
		// Fm in C major is iv borrowed from the parallel minor.
		System.Collections.Generic.IReadOnlyList<ChromaticAnalysis> analyses =
			Progression.Parse("C | Fm | C").ChromaticChords(CMajor);
		Assert.AreEqual(ChromaticKind.BorrowedChord, analyses[0].Kind);
		Assert.AreEqual("from parallel minor", analyses[0].Detail);
	}

	[TestMethod]
	public void ChromaticAnalysis_Create_RejectsNegativeIndex() =>
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ChromaticAnalysis.Create(-1, ChromaticKind.Chromatic, null));
}
