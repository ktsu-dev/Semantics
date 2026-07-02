// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class KeyInferenceTests
{
	[TestMethod]
	public void InferKey_TwoFiveOne_IsCMajor()
	{
		Key? key = Progression.Parse("4/4  Dm7 | G7 | Cmaj7").InferKey();
		Assert.IsNotNull(key);
		Assert.AreEqual(0, key.Tonic.Value);
		Assert.AreEqual(Mode.Major, key.Mode);
	}

	[TestMethod]
	public void InferKey_DiatonicMinorProgression_IsAMinor()
	{
		Key? key = Progression.Parse("4/4  Am | Dm | Em | Am").InferKey();
		Assert.IsNotNull(key);
		Assert.AreEqual(9, key.Tonic.Value);
		Assert.AreEqual(Mode.Aeolian, key.Mode);
	}

	[TestMethod]
	public void InferKeys_RanksBestFirst_WithTwentyFourCandidates()
	{
		System.Collections.Generic.IReadOnlyList<KeyMatch> matches =
			Progression.Parse("4/4  Dm7 | G7 | Cmaj7").InferKeys();
		Assert.AreEqual(24, matches.Count);
		Assert.IsTrue(matches[0].Score >= matches[1].Score);
	}
}
