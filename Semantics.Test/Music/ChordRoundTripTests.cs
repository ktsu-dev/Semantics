// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class ChordRoundTripTests
{
	private static readonly string[] Corpus =
	[
		"C", "Cm", "Cdim", "Caug", "Csus2", "Csus4", "C5",
		"Cmaj7", "C7", "Cm7", "Cdim7", "Cm7b5", "CmMaj7",
		"C6", "Cm6", "C9", "Cm9", "C11", "C13",
		"C7b9", "C7#9", "C7#11", "C7b13", "Cadd9",
		"C/G", "Dm7/G", "F#m7b5", "Bbmaj7",
	];

	[TestMethod]
	public void CanonicalOutputRoundTrips()
	{
		foreach (string symbol in Corpus)
		{
			Chord chord = Chord.Parse(symbol);
			Chord reparsed = Chord.Parse(chord.ToString());
			Assert.AreEqual(chord, reparsed, $"round-trip failed for '{symbol}' -> '{chord}'");
		}
	}

	[TestMethod]
	public void TryParseReturnsFalseOnEmpty()
	{
		Assert.IsFalse(Chord.TryParse("", out Chord? result));
		Assert.IsNull(result);
	}
}
