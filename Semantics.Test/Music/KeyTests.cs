// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class KeyTests
{
	private static Key CMajor => Key.Create(PitchClass.Create(0), Mode.Major);
	private static Key CMinor => Key.Create(PitchClass.Create(0), Mode.Aeolian);

	[TestMethod]
	public void RomanNumeral_TonicMajorSeventh()
	{
		Assert.AreEqual("Imaj7", CMajor.RomanNumeralOf(Chord.Parse("Cmaj7")));
	}

	[TestMethod]
	public void RomanNumeral_SupertonicMinorSeventhIsLowerCase()
	{
		Assert.AreEqual("ii7", CMajor.RomanNumeralOf(Chord.Parse("Dm7")));
	}

	[TestMethod]
	public void RomanNumeral_FlatSixInMinor()
	{
		// Ab major triad is the flat-VI in C minor (Aeolian already contains Ab, so no accidental).
		Assert.AreEqual("VI", CMinor.RomanNumeralOf(Chord.Parse("Ab")));
	}

	[TestMethod]
	public void RomanNumeral_ChromaticGetsAccidental()
	{
		// D-flat major triad is the flat-II in C major (chromatic).
		Assert.AreEqual("bII", CMajor.RomanNumeralOf(Chord.Parse("Db")));
	}

	[TestMethod]
	public void FunctionOf_ReturnsScaleDegree()
	{
		ScaleDegree fifth = CMajor.FunctionOf(PitchClass.Create(7));
		Assert.AreEqual(5, fifth.Degree);
		Assert.AreEqual(0, fifth.Alteration);
	}
}
