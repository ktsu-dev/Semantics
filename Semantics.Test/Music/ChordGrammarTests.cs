// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class ChordGrammarTests
{
	[TestMethod]
	[DataRow("C7add13", "C7add13", new[] { 0, 4, 7, 10, 21 })]
	[DataRow("C9add13", "C9add13", new[] { 0, 4, 7, 10, 14, 21 })]
	[DataRow("CM9", "Cmaj9", new[] { 0, 4, 7, 11, 14 })]
	[DataRow("CM11", "Cmaj11", new[] { 0, 4, 7, 11, 14, 17 })]
	[DataRow("CM13", "Cmaj13", new[] { 0, 4, 7, 11, 14, 17, 21 })]
	[DataRow("CmM9", "Cmmaj9", new[] { 0, 3, 7, 11, 14 })]
	[DataRow("C(#11)", "C(#11)", new[] { 0, 4, 7, 18 })]
	[DataRow("C13b9", "C13b9", new[] { 0, 4, 7, 10, 13, 17, 21 })]
	[DataRow("C13#11", "C13#11", new[] { 0, 4, 7, 10, 14, 18, 21 })]
	[DataRow("Cm7#5", "Cm7#5", new[] { 0, 3, 8, 10 })]
	[DataRow("Cm+", "Cm#5", new[] { 0, 3, 8 })]
	[DataRow("C7-9", "C7b9", new[] { 0, 4, 7, 10, 13 })]
	[DataRow("C7+9", "C7#9", new[] { 0, 4, 7, 10, 15 })]
	[DataRow("C7-5", "C7b5", new[] { 0, 4, 6, 10 })]
	[DataRow("Cadd9(#11)", "C(#11)add9", new[] { 0, 4, 7, 14, 18 })]
	public void Parse_GrammarSpellings_KeepTheirMeaning(string symbol, string canonical, int[] tones)
	{
		Chord chord = Chord.Parse(symbol);
		Assert.AreEqual(canonical, chord.ToString(), symbol);
		Assert.AreSequenceEqual(tones, [.. chord.ChordTones()], symbol);
	}

	[TestMethod]
	[DataRow("C/Gm")]
	[DataRow("C/E/G")]
	[DataRow("Am/G/F")]
	[DataRow("C7/G#m7b5")]
	[DataRow("C/Bb7")]
	[DataRow("C/Gxyz")]
	[DataRow("C57")]
	[DataRow("C77")]
	[DataRow("C7m")]
	[DataRow("Cmm7")]
	[DataRow("C13b13")]
	[DataRow("Csus4#5")]
	[DataRow("Cmsus4")]
	[DataRow("C7b9b9")]
	[DataRow("C7b5#5")]
	public void Parse_RejectsInvalidGrammar(string symbol)
	{
		Assert.IsFalse(Chord.TryParse(symbol, out Chord? chord), symbol);
		Assert.IsNull(chord);
	}

	[TestMethod]
	public void Parse_LegacySpellingsRemainReadable()
	{
		(string Input, string Canonical)[] cases =
		[
			("C79", "C9"),
			("Cmaj79", "Cmaj9"),
			("C713", "C13"),
			("Csus47", "C7sus4"),
			("C6add9", "C6/9"),
			("Cmmaj7", "Cmmaj7"),
		];

		foreach ((string input, string canonical) in cases)
		{
			Assert.AreEqual(canonical, Chord.Parse(input).ToString(), input);
		}
	}

	[TestMethod]
	public void CanonicalExtensionSymbolsRoundTripWithTones()
	{
		string[] symbols =
		[
			"C9", "Cm9", "Cmaj9", "Caug9", "C11", "C13", "Cmaj13",
			"C7sus4", "C9sus4", "C6/9", "Cm6/9", "C13b9#11",
		];

		foreach (string symbol in symbols)
		{
			Chord chord = Chord.Parse(symbol);
			Assert.AreEqual(symbol, chord.ToString(), symbol);
			Assert.AreEqual(chord, Chord.Parse(chord.ToString()), symbol);
		}
	}
}
