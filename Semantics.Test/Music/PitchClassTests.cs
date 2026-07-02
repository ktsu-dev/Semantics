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

	[TestMethod]
	public void TypedCreateComposesLetterAndAccidental()
	{
		PitchClass cSharp = PitchClass.Create(NoteLetter.C, Accidental.Sharp);
		Assert.AreEqual(1, cSharp.Value);
	}

	[TestMethod]
	public void ToStringIsSharpSpelled()
	{
		Assert.AreEqual("C#", PitchClass.Create(NoteLetter.C, Accidental.Sharp).ToString());
	}

	[TestMethod]
	public void ParseAcceptsFlatsAndNormalisesToSharp()
	{
		PitchClass dFlat = PitchClass.Parse("Db");
		Assert.AreEqual(1, dFlat.Value);
		Assert.AreEqual("C#", dFlat.ToString());
	}

	[TestMethod]
	public void RoundTripOfCanonicalOutput()
	{
		PitchClass parsed = PitchClass.Parse("Db");
		Assert.AreEqual(parsed, PitchClass.Parse(parsed.ToString()));
	}

	[TestMethod]
	public void ParseThrowsFormatExceptionOnGarbage() =>
		_ = Assert.ThrowsExactly<FormatException>(() => PitchClass.Parse("H"));

	[TestMethod]
	public void TryParseReturnsFalseOnGarbage()
	{
		Assert.IsFalse(PitchClass.TryParse("H", out PitchClass? result));
		Assert.IsNull(result);
	}
}
