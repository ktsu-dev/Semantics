// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class HarmonicFunctionTests
{
	private static Key CMajor => Key.Create(PitchClass.Create(0), Mode.Major);

	[TestMethod]
	public void RomanNumerals_LabelsTwoFiveOne()
	{
		Progression progression = Progression.Parse("4/4  Dm7 | G7 | Cmaj7");
		System.Collections.Generic.IReadOnlyList<string> numerals = progression.RomanNumerals(CMajor);
		string[] expected = ["ii7", "V7", "Imaj7"];
		string[] actual = [.. numerals];
		CollectionAssert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void Functions_ClassifyDiatonicChords()
	{
		Progression progression = Progression.Parse("4/4  C | Dm | G | Am");
		System.Collections.Generic.IReadOnlyList<HarmonicFunction> functions = progression.Functions(CMajor);
		Assert.AreEqual(HarmonicFunction.Tonic, functions[0]);      // I
		Assert.AreEqual(HarmonicFunction.Predominant, functions[1]); // ii
		Assert.AreEqual(HarmonicFunction.Dominant, functions[2]);    // V
		Assert.AreEqual(HarmonicFunction.Tonic, functions[3]);       // vi
	}

	[TestMethod]
	public void Functions_MarksChromaticRoot()
	{
		Progression progression = Progression.Parse("4/4  C | Db");
		System.Collections.Generic.IReadOnlyList<HarmonicFunction> functions = progression.Functions(CMajor);
		Assert.AreEqual(HarmonicFunction.Chromatic, functions[1]);
	}
}
