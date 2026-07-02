// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class ModeTests
{
	[TestMethod]
	public void Major_HasIonianIntervals()
	{
		int[] expected = [0, 2, 4, 5, 7, 9, 11];
		CollectionAssert.AreEqual(expected, Mode.Major.Intervals.ToArray());
	}

	[TestMethod]
	public void HarmonicMinor_HasRaisedSeventh()
	{
		int[] expected = [0, 2, 3, 5, 7, 8, 11];
		CollectionAssert.AreEqual(expected, Mode.HarmonicMinor.Intervals.ToArray());
	}

	[TestMethod]
	public void Parse_IsCaseInsensitive_AndEqualsStaticInstance()
	{
		Assert.AreEqual(Mode.Lydian, Mode.Parse("LYDIAN"));
	}

	[TestMethod]
	public void Parse_RejectsUnknown()
	{
		_ = Assert.ThrowsExactly<FormatException>(() => Mode.Parse("bebop"));
	}

	[TestMethod]
	public void ToStringRoundTripsForAllShapes()
	{
		foreach (string name in new[] { "major", "dorian", "harmonic_minor", "octatonic_hw", "blues_major" })
		{
			Mode mode = Mode.Parse(name);
			Assert.AreEqual(name, mode.ToString());
			Assert.AreEqual(mode, Mode.Parse(mode.ToString()));
		}
	}

	[TestMethod]
	public void TryParseFailsOnUnknownMode()
	{
		Assert.IsFalse(Mode.TryParse("bogus", out Mode? result));
		Assert.IsNull(result);
	}

	[TestMethod]
	public void AllSevenDiatonicModes_ArePresent()
	{
		Mode[] diatonic =
		[
			Mode.Ionian, Mode.Dorian, Mode.Phrygian, Mode.Lydian,
			Mode.Mixolydian, Mode.Aeolian, Mode.Locrian,
		];
		foreach (Mode mode in diatonic)
		{
			Assert.AreEqual(7, mode.DegreeCount);
		}

		Assert.AreEqual(Mode.Major, Mode.Ionian);
	}

	[TestMethod]
	public void MelodicMinor_HasNaturalSixthAndSeventh()
	{
		int[] expected = [0, 2, 3, 5, 7, 9, 11];
		CollectionAssert.AreEqual(expected, Mode.MelodicMinor.Intervals.ToArray());
	}

	[TestMethod]
	public void PhrygianDominant_HasFlatTwoAndMajorThird()
	{
		int[] expected = [0, 1, 4, 5, 7, 8, 10];
		CollectionAssert.AreEqual(expected, Mode.PhrygianDominant.Intervals.ToArray());
	}

	[TestMethod]
	public void OctatonicScales_HaveEightDegrees()
	{
		int[] halfWhole = [0, 1, 3, 4, 6, 7, 9, 10];
		int[] wholeHalf = [0, 2, 3, 5, 6, 8, 9, 11];
		CollectionAssert.AreEqual(halfWhole, Mode.OctatonicHalfWhole.Intervals.ToArray());
		CollectionAssert.AreEqual(wholeHalf, Mode.OctatonicWholeHalf.Intervals.ToArray());
		Assert.AreEqual(8, Mode.OctatonicHalfWhole.DegreeCount);
	}

	[TestMethod]
	public void Altered_IsSuperLocrian()
	{
		int[] expected = [0, 1, 3, 4, 6, 8, 10];
		CollectionAssert.AreEqual(expected, Mode.Altered.Intervals.ToArray());
		Assert.AreEqual(Mode.Altered, Mode.Parse("altered"));
	}

	[TestMethod]
	public void LydianDominant_IsFourthModeOfMelodicMinor()
	{
		int[] expected = [0, 2, 4, 6, 7, 9, 10];
		CollectionAssert.AreEqual(expected, Mode.LydianDominant.Intervals.ToArray());
	}

	[TestMethod]
	public void WholeTone_HasSixDegrees()
	{
		int[] expected = [0, 2, 4, 6, 8, 10];
		CollectionAssert.AreEqual(expected, Mode.WholeTone.Intervals.ToArray());
		Assert.AreEqual(6, Mode.WholeTone.DegreeCount);
	}

	[TestMethod]
	public void Chromatic_HasTwelveDegrees()
	{
		Assert.AreEqual(12, Mode.Chromatic.DegreeCount);
	}

	[TestMethod]
	public void Pentatonics_HaveFiveDegrees()
	{
		int[] majorPent = [0, 2, 4, 7, 9];
		int[] minorPent = [0, 3, 5, 7, 10];
		CollectionAssert.AreEqual(majorPent, Mode.MajorPentatonic.Intervals.ToArray());
		CollectionAssert.AreEqual(minorPent, Mode.MinorPentatonic.Intervals.ToArray());
	}

	[TestMethod]
	public void BluesMinor_HasBlueNote()
	{
		int[] expected = [0, 3, 5, 6, 7, 10];
		CollectionAssert.AreEqual(expected, Mode.BluesMinor.Intervals.ToArray());
	}
}
