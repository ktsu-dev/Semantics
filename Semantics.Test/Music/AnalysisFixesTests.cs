// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Music;

using System.Collections.Generic;
using System.Linq;
using ktsu.Semantics.Music;

[TestClass]
public class AnalysisFixesTests
{
	private static Key CMajor => Key.Create(PitchClass.Create(0), Mode.Major);
	private static Key AMinor => Key.Create(PitchClass.Create(9), Mode.Aeolian);
	private static Key CMajorPentatonic => Key.Create(PitchClass.Create(0), Mode.MajorPentatonic);
	private static Key AMinorPentatonic => Key.Create(PitchClass.Create(9), Mode.MinorPentatonic);

	private static readonly string[] PentatonicNumerals = ["I", "vi", "V", "I"];

	private static readonly HarmonicFunction[] PentatonicFunctions =
		[HarmonicFunction.Tonic, HarmonicFunction.Tonic, HarmonicFunction.Dominant, HarmonicFunction.Tonic];

	[TestMethod]
	public void Cadences_AHeldDominantIsOneHalfCadence()
	{
		IReadOnlyList<CadenceInstance> cadences = Progression.Parse("4/4  C | G | G | G").Cadences(CMajor);
		Assert.HasCount(1, cadences);
		Assert.AreEqual(Cadence.Half, cadences[0].Type);
		Assert.AreEqual(1, cadences[0].Index);
	}

	[TestMethod]
	public void Cadences_VToV7IsNotAHalfCadence()
	{
		IReadOnlyList<CadenceInstance> cadences = Progression.Parse("4/4  C | G | G7 | C").Cadences(CMajor);
		Assert.DoesNotContain(cadence => cadence.Index == 2, cadences, string.Join(", ", cadences));
		Assert.AreEqual(Cadence.Authentic, cadences[^1].Type);
	}

	[TestMethod]
	public void Pentatonic_SpellsAgainstTheMajorScale()
	{
		Progression progression = Progression.Parse("4/4  C | Am | G | C");
		Assert.AreSequenceEqual(PentatonicNumerals, [.. progression.RomanNumerals(CMajorPentatonic)]);
		Assert.AreSequenceEqual(PentatonicFunctions, [.. progression.Functions(CMajorPentatonic)]);

		IReadOnlyList<CadenceInstance> cadences = progression.Cadences(CMajorPentatonic);
		Assert.DoesNotContain(cadence => cadence.Type == Cadence.Plagal, cadences);
		Assert.AreEqual(Cadence.Authentic, cadences.Single(cadence => cadence.Index == 3).Type);
		Assert.AreEqual(Cadence.Half, cadences.Single(cadence => cadence.Index == 2).Type);
	}

	[TestMethod]
	public void Pentatonic_MinorSpellsAgainstNaturalMinor()
	{
		Assert.AreEqual("III", AMinorPentatonic.RomanNumeralOf(Chord.Parse("C")));
		Assert.AreEqual("VII", AMinorPentatonic.RomanNumeralOf(Chord.Parse("G")));
		Assert.AreEqual(Chord.Parse("G"), CMajorPentatonic.ChordFromRomanNumeral("V"));
		Assert.AreEqual(
			HarmonicFunction.Tonic,
			Progression.Parse("4/4  C").Functions(AMinorPentatonic)[0]);
	}

	[TestMethod]
	[DataRow("4/4  C | F#7 | Bdim | C", "C major")]
	[DataRow("4/4  Am | F# | Bdim | Am", "A aeolian")]
	[DataRow("4/4  Am | G | Am", "A harmonic_minor")]
	public void ChromaticChords_ADominantOfADiminishedTriadIsNotSecondary(string chart, string key)
	{
		IReadOnlyList<ChromaticAnalysis> analyses = Progression.Parse(chart).ChromaticChords(Key.Parse(key));
		Assert.DoesNotContain(a => a.Kind == ChromaticKind.SecondaryDominant, analyses, string.Join(", ", analyses));
	}

	[TestMethod]
	[DataRow("4/4  C | D7 | G", "C major", "V/V")]
	[DataRow("4/4  C | A7 | Dm", "C major", "V/ii")]
	[DataRow("4/4  C | E7 | Am", "C major", "V/vi")]
	[DataRow("4/4  Am | A7 | Dm", "A aeolian", "V/iv")]
	public void ChromaticChords_SecondaryDominantsOfStableTriadsAreUnchanged(string chart, string key, string detail)
	{
		ChromaticAnalysis analysis = Progression.Parse(chart).ChromaticChords(Key.Parse(key)).Single();
		Assert.AreEqual(ChromaticKind.SecondaryDominant, analysis.Kind);
		Assert.AreEqual(detail, analysis.Detail);
	}

	[TestMethod]
	[DataRow("4/4  Am | Dm | E7 | Am")]
	[DataRow("4/4  Am | E | Am")]
	[DataRow("4/4  Am | E7b9 | Am")]
	public void ChromaticChords_AMinorKeysOwnDominantIsDiatonic(string chart) =>
		Assert.IsEmpty(Progression.Parse(chart).ChromaticChords(AMinor));

	[TestMethod]
	public void ChromaticChords_BorrowedChordsAreUnchanged()
	{
		ChromaticAnalysis fm = Progression.Parse("4/4  C | Fm | C").ChromaticChords(CMajor).Single();
		Assert.AreEqual(ChromaticKind.BorrowedChord, fm.Kind);
		Assert.AreEqual("from parallel minor", fm.Detail);
	}

	[TestMethod]
	[DataRow("4/4  C | Ab7no5 | G | C", "C major", "It+6")]
	[DataRow("4/4  C | Ab7b5 | G | C", "C major", "Fr+6")]
	[DataRow("4/4  C | Ab7 | G | C", "C major", "Ger+6")]
	[DataRow("4/4  C | Ab7 | C/G | G | C", "C major", "Ger+6")]
	[DataRow("4/4  Am | F7no5 | E | Am", "A aeolian", "It+6")]
	[DataRow("4/4  Am | F7b5 | E | Am", "A aeolian", "Fr+6")]
	[DataRow("4/4  Am | F7 | E | Am", "A aeolian", "Ger+6")]
	public void ChromaticChords_DetectsAugmentedSixths(string chart, string key, string detail)
	{
		ChromaticAnalysis analysis = Progression.Parse(chart).ChromaticChords(Key.Parse(key)).Single(a => a.Index == 1);
		Assert.AreEqual(ChromaticKind.AugmentedSixth, analysis.Kind);
		Assert.AreEqual(detail, analysis.Detail);
	}

	[TestMethod]
	public void ChromaticChords_ADominantSeventhOnFlatSixIsGermanOnlyWhenItResolvesLikeOne()
	{
		ChromaticAnalysis analysis = Progression.Parse("4/4  C | Ab7 | Db | C").ChromaticChords(CMajor).Single(a => a.Index == 1);
		Assert.AreNotEqual(ChromaticKind.AugmentedSixth, analysis.Kind);
	}

	[TestMethod]
	[DataRow("4/4  C7 | Fmaj7 | C7", 5, "major")]
	[DataRow("4/4  Fmaj7 | C7", 5, "major")]
	[DataRow("4/4  Bbmaj7 | F7", 10, "major")]
	[DataRow("4/4  Am | E7 | Am", 9, "aeolian")]
	[DataRow("4/4  Dm7 | G7 | Cmaj7", 0, "major")]
	[DataRow("4/4  Am | Dm | Em | Am", 9, "aeolian")]
	public void InferKey_WeighsEveryChordTone(string chart, int tonic, string mode)
	{
		Key? key = Progression.Parse(chart).InferKey();
		Assert.IsNotNull(key);
		Assert.AreEqual(tonic, key.Tonic.Value, key.ToString());
		Assert.AreEqual(mode, key.Mode.Name, key.ToString());
	}
}
