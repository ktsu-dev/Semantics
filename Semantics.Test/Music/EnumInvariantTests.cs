// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class EnumInvariantTests
{
	[TestMethod]
	public void NoteLetterValuesAreNaturalPitchClasses()
	{
		Assert.AreEqual(0, (int)NoteLetter.C);
		Assert.AreEqual(4, (int)NoteLetter.E);
		Assert.AreEqual(11, (int)NoteLetter.B);
	}

	[TestMethod]
	public void AccidentalValuesAreSemitoneOffsets()
	{
		Assert.AreEqual(-1, (int)Accidental.Flat);
		Assert.AreEqual(0, (int)Accidental.Natural);
		Assert.AreEqual(2, (int)Accidental.DoubleSharp);
	}
}
