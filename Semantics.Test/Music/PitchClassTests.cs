// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class PitchClassTests
{
	[TestMethod]
	public void Create_WrapsValuesIntoZeroToEleven()
	{
		Assert.AreEqual(0, PitchClass.Create(12).Value);
		Assert.AreEqual(11, PitchClass.Create(-1).Value);
		Assert.AreEqual(2, PitchClass.Create(14).Value);
	}

	[TestMethod]
	public void Name_UsesSharpSpelling()
	{
		Assert.AreEqual("C", PitchClass.Create(0).Name);
		Assert.AreEqual("C#", PitchClass.Create(1).Name);
		Assert.AreEqual("B", PitchClass.Create(11).Name);
	}

	[TestMethod]
	public void Equality_IsByValue()
	{
		Assert.AreEqual(PitchClass.Create(13), PitchClass.Create(1));
	}
}
