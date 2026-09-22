// Copyright (c) 2023-2026 ktsu-dev contributors

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
		"C6/9", "Cm6/9", "C6/9/G",
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
	public void DiminishedMajorSeventhRoundTrips()
	{
		// The corpus above cannot cover this one. It starts from a symbol, and "Cdimmaj7" parsed
		// wrongly as a diminished seventh still formats and re-parses consistently as "Cdim7" —
		// a stable round trip of the wrong chord. The chord object is the only honest starting
		// point, since it is what the formatter is being asked to be the inverse of.
		Chord chord = new() { Quality = ChordQuality.Diminished, Seventh = SeventhType.Major };

		Assert.AreEqual("Cdimmaj7", chord.ToString());

		Chord reparsed = Chord.Parse(chord.ToString());

		Assert.AreEqual(SeventhType.Major, reparsed.Seventh);
		Assert.AreEqual(ChordQuality.Diminished, reparsed.Quality);
		Assert.AreEqual(chord, reparsed);
	}

	[TestMethod]
	public void TryParseReturnsFalseOnEmpty()
	{
		Assert.IsFalse(Chord.TryParse("", out Chord? result));
		Assert.IsNull(result);
	}
}
