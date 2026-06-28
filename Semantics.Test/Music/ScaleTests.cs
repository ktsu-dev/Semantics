// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class ScaleTests
{
	private static Scale CMajor => Scale.Create(PitchClass.Create(0), Mode.Major);

	[TestMethod]
	public void PitchClasses_AreTheModeOffsetsFromRoot()
	{
		int[] expected = [0, 2, 4, 5, 7, 9, 11];
		int[] values = [.. CMajor.PitchClasses.Select(pc => pc.Value)];
		CollectionAssert.AreEqual(expected, values);
	}

	[TestMethod]
	public void Contains_IsTrueForMembers()
	{
		Assert.IsTrue(CMajor.Contains(PitchClass.Create(4)));   // E
		Assert.IsFalse(CMajor.Contains(PitchClass.Create(1)));  // C#
	}

	[TestMethod]
	public void DegreeOf_DiatonicMemberHasNoAlteration()
	{
		ScaleDegree g = CMajor.DegreeOf(PitchClass.Create(7)); // G is degree 5
		Assert.AreEqual(5, g.Degree);
		Assert.AreEqual(0, g.Alteration);
	}

	[TestMethod]
	public void DegreeOf_ChromaticPrefersFlatOfUpperDegree()
	{
		// Db/C# (pitch class 1) sits between degrees 1 and 2; convention spells it flat-2.
		ScaleDegree flatTwo = CMajor.DegreeOf(PitchClass.Create(1));
		Assert.AreEqual(2, flatTwo.Degree);
		Assert.AreEqual(-1, flatTwo.Alteration);
	}

	[TestMethod]
	public void DegreeOf_SharpFourIsCloserToFourthRaised()
	{
		// F# (pitch class 6) is equidistant from 4 and 5; the tie prefers the flat (♭V).
		ScaleDegree tritone = CMajor.DegreeOf(PitchClass.Create(6));
		Assert.AreEqual(5, tritone.Degree);
		Assert.AreEqual(-1, tritone.Alteration);
	}
}
