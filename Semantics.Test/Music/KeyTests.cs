// Copyright (c) 2023-2026 ktsu-dev contributors

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
	public void RomanNumeral_DistinctDegreesNeverShareALabel()
	{
		// The roman numeral table only spells seven degrees, so a mode with more of them has no
		// label left for the eighth onwards. Wrapping would hand two distinct degrees the same
		// string; the guard refuses instead, matching ChordFromRomanNumeral's own bounds check.
		foreach (Mode mode in new[] { Mode.Chromatic, Mode.OctatonicHalfWhole, Mode.OctatonicWholeHalf })
		{
			Key key = Key.Create(PitchClass.Create(0), mode);
			HashSet<string> seen = [];

			for (int degree = 1; degree <= mode.DegreeCount; degree++)
			{
				Chord chord = Chord.Parse(key.Scale.PitchClasses[degree - 1].Name);

				if (degree > 7)
				{
					_ = Assert.ThrowsExactly<ArgumentException>(
						() => key.RomanNumeralOf(chord),
						$"{mode.Name} degree {degree} has no roman numeral, so it must not be labelled.");
					continue;
				}

				string numeral = key.RomanNumeralOf(chord);
				Assert.IsTrue(seen.Add(numeral), $"{mode.Name} degree {degree} reuses the label '{numeral}'.");
			}
		}
	}

	[TestMethod]
	public void RomanNumeral_ChromaticKeyStillLabelsTheFirstSevenDegrees()
	{
		Key chromatic = Key.Create(PitchClass.Create(0), Mode.Chromatic);
		Assert.AreEqual("I", chromatic.RomanNumeralOf(Chord.Parse("C")));
		Assert.AreEqual("II", chromatic.RomanNumeralOf(Chord.Parse("C#")));
		Assert.AreEqual("VII", chromatic.RomanNumeralOf(Chord.Parse("F#")));
	}

	[TestMethod]
	public void FunctionOf_ReturnsScaleDegree()
	{
		ScaleDegree fifth = CMajor.FunctionOf(PitchClass.Create(7));
		Assert.AreEqual(5, fifth.Degree);
		Assert.AreEqual(0, fifth.Alteration);
	}

	[TestMethod]
	public void ToStringIsTonicSpaceMode()
	{
		Key k = Key.Create(PitchClass.Create(NoteLetter.A, Accidental.Natural), Mode.Aeolian);
		Assert.AreEqual("A aeolian", k.ToString());
	}

	[TestMethod]
	public void RoundTrip()
	{
		Key k = Key.Create(PitchClass.Create(NoteLetter.E, Accidental.Flat), Mode.Major);
		Assert.AreEqual(k, Key.Parse(k.ToString()));
	}

	[TestMethod]
	public void TryParseFailsOnUnknownMode()
	{
		Assert.IsFalse(Key.TryParse("C bogus", out Key? result));
		Assert.IsNull(result);
	}
}
