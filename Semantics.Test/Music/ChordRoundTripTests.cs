// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class ChordRoundTripTests
{
	private static readonly string[] Corpus =
	[
		"C", "Cm", "Cdim", "Caug", "Csus2", "Csus4", "C5",
		"Cmaj7", "C7", "Cm7", "Cdim7", "Cm7b5", "Cmmaj7", "Cmmaj9",
		"C6", "Cm6", "C9", "Cm9", "C11", "C13",
		"Cmaj9", "Cmaj13", "Caug9", "C7b9", "C7#9", "C7#11", "C7b13", "Cadd9",
		"C7add13", "C9add13", "Cmaj7add13",
		"C/G", "Dm7/G", "F#m7b5", "A#maj7",
		"C6/9", "Cm6/9", "C6/9/G",
		"C7sus4", "C9sus4", "Cadd11", "Cadd13", "Cmadd11", "Cm#5",
		"C(#11)", "C(b9)", "C(b6)", "C7b5", "Cmaj7b5", "C9b5", "C7b5b9", "C(b5)", "A#(b5)", "C13b5",
	];

	[TestMethod]
	public void CanonicalOutputRoundTrips()
	{
		foreach (string symbol in Corpus)
		{
			Chord chord = Chord.Parse(symbol);
			Assert.AreEqual(symbol, chord.ToString(), $"non-canonical corpus entry '{symbol}'");
			Chord reparsed = Chord.Parse(chord.ToString());
			Assert.AreEqual(chord, reparsed, $"round-trip failed for '{symbol}' -> '{chord}'");
		}
	}

	[TestMethod]
	public void EveryExpressibleChordRoundTrips()
	{
		PitchClass[] roots = [PitchClass.Create(0), PitchClass.Create(6)];
		ChordQuality[] qualities = Enum.GetValues<ChordQuality>();
		SeventhType[] sevenths = Enum.GetValues<SeventhType>();
		SixthType[] sixths = Enum.GetValues<SixthType>();
		ChordOmissions[] omissions = [ChordOmissions.None, ChordOmissions.Third, ChordOmissions.Fifth, ChordOmissions.Third | ChordOmissions.Fifth];
		PitchClass?[] basses = [null, PitchClass.Create(7)];

		foreach (PitchClass root in roots)
		{
			foreach (ChordQuality quality in qualities)
			{
				foreach (SeventhType seventh in sevenths)
				{
					foreach (SixthType sixth in sixths)
					{
						for (int tensionBits = 0; tensionBits < 128; tensionBits++)
						{
							foreach (ChordOmissions chordOmissions in omissions)
							{
								foreach (PitchClass? bass in basses)
								{
									Chord chord = new()
									{
										Root = root,
										Quality = quality,
										Seventh = seventh,
										Sixth = sixth,
										Tensions = (ChordTensions)tensionBits,
										Omissions = chordOmissions,
										Bass = bass,
									};
									if (!ChordSymbolWriter.IsExpressible(chord))
									{
										continue;
									}

									string symbol = chord.ToString();
									Assert.AreEqual(chord, Chord.Parse(symbol), $"round-trip failed for {chord}");
								}
							}
						}
					}
				}
			}
		}
	}

	[TestMethod]
	public void DiminishedMajorSeventhRoundTrips()
	{
		Chord chord = new() { Quality = ChordQuality.Diminished, Seventh = SeventhType.Major };

		Assert.AreEqual("Cdimmaj7", chord.ToString());

		Chord reparsed = Chord.Parse(chord.ToString());

		Assert.AreEqual(SeventhType.Major, reparsed.Seventh);
		Assert.AreEqual(ChordQuality.Diminished, reparsed.Quality);
		Assert.AreEqual(chord, reparsed);
	}

	[TestMethod]
	public void MinorSharpFiveUsesMinorRomanNumeralAndSharpFiveSuffix()
	{
		Key cMajor = Key.Create(PitchClass.Create(0), Mode.Major);
		Chord chord = Chord.Parse("Cm#5");

		Assert.AreEqual("i#5", cMajor.RomanNumeralOf(chord));
		Assert.AreEqual(chord, cMajor.ChordFromRomanNumeral("i#5"));
	}

	[TestMethod]
	public void TryParseReturnsFalseOnEmpty()
	{
		Assert.IsFalse(Chord.TryParse("", out Chord? result));
		Assert.IsNull(result);
	}
}
