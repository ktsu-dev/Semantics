// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class PitchTests
{
	[TestMethod]
	public void MiddleC_IsMidi60_C4()
	{
		Pitch c4 = Pitch.Create(60);
		Assert.AreEqual(0, c4.PitchClass.Value);
		Assert.AreEqual(4, c4.Octave);
		Assert.AreEqual("C4", c4.Name);
	}

	[TestMethod]
	public void Create_RejectsOutOfRange()
	{
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Pitch.Create(128));
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Pitch.Create(-1));
	}

	[TestMethod]
	public void Parse_ParsesSharpsAndFlats()
	{
		Assert.AreEqual(60, Pitch.Parse("C4").Midi);
		Assert.AreEqual(61, Pitch.Parse("C#4").Midi);
		Assert.AreEqual(61, Pitch.Parse("Db4").Midi);
		Assert.AreEqual(58, Pitch.Parse("Bb3").Midi);
	}

	[TestMethod]
	public void Transpose_MovesBySemitones()
	{
		Assert.AreEqual(67, Pitch.Create(60).Transpose(7).Midi);
		Assert.AreEqual(53, Pitch.Create(60).Transpose(-7).Midi);
	}

	[TestMethod]
	public void TypedCreateMatchesParse() =>
		Assert.AreEqual(Pitch.Parse("C#4"), Pitch.Create(NoteLetter.C, Accidental.Sharp, 4));

	[TestMethod]
	public void ToStringRoundTrips()
	{
		Pitch p = Pitch.Parse("F#3");
		Assert.AreEqual("F#3", p.ToString());
		Assert.AreEqual(p, Pitch.Parse(p.ToString()));
	}

	[TestMethod]
	public void TryParseFailsOnBadOctave()
	{
		Assert.IsFalse(Pitch.TryParse("Cx", out Pitch? result));
		Assert.IsNull(result);
	}
}
