// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class CadenceTests
{
	private static Key CMajor => Key.Create(PitchClass.Create(0), Mode.Major);

	[TestMethod]
	public void Cadences_DetectsAuthentic()
	{
		System.Collections.Generic.IReadOnlyList<CadenceInstance> cadences =
			Progression.Parse("G | C").Cadences(CMajor);
		Assert.AreEqual(1, cadences.Count);
		Assert.AreEqual(Cadence.Authentic, cadences[0].Type);
		Assert.AreEqual(1, cadences[0].Index);
	}

	[TestMethod]
	public void Cadences_DetectsPlagalHalfAndDeceptive()
	{
		Assert.AreEqual(Cadence.Plagal, Progression.Parse("F | C").Cadences(CMajor)[0].Type);
		Assert.AreEqual(Cadence.Half, Progression.Parse("C | G").Cadences(CMajor)[0].Type);
		Assert.AreEqual(Cadence.Deceptive, Progression.Parse("G | Am").Cadences(CMajor)[0].Type);
	}

	[TestMethod]
	public void Cadences_ReportsNoneForNonCadentialMotion()
	{
		System.Collections.Generic.IReadOnlyList<CadenceInstance> cadences =
			Progression.Parse("C | Am").Cadences(CMajor);
		Assert.AreEqual(0, cadences.Count);
	}

	[TestMethod]
	public void CadenceInstance_Create_RejectsNegativeIndex() =>
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => CadenceInstance.Create(-1, Cadence.Authentic));
}
