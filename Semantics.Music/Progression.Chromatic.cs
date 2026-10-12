// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Music;

using System.Collections.Generic;
using System.Linq;

public sealed partial record Progression
{
	/// <summary>Classifies each non-diatonic chord in the progression relative to a key.</summary>
	/// <param name="key">The key to analyze against.</param>
	/// <returns>One entry per non-diatonic chord; diatonic chords are omitted.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
	public IReadOnlyList<ChromaticAnalysis> ChromaticChords(Key key)
	{
		Ensure.NotNull(key);
		List<ChromaticAnalysis> result = [];
		for (int i = 0; i < Chords.Count; i++)
		{
			Chord chord = Chords[i].Chord;
			if (IsDiatonic(key, chord))
			{
				continue;
			}

			(ChromaticKind kind, string? detail) = ClassifyChromatic(key, chord);
			result.Add(ChromaticAnalysis.Create(i, kind, detail));
		}

		return result;
	}

	private static bool AllTonesInScale(Chord chord, Scale scale) =>
		chord.ChordTones().All(offset => scale.Contains(PitchClass.Create(chord.Root.Value + offset)));

	private static bool IsDiatonic(Key key, Chord chord)
	{
		Scale scale = key.Scale;
		return AllTonesInScale(chord, scale);
	}

	// Names the degree a secondary dominant resolves to by the key's own triad on that degree, so every
	// seven-degree mode gets its own case and quality (ii in mixolydian, II in lydian) rather than
	// borrowing the major or natural-minor table. Roman-numeral analysis spells only seven degrees, so a
	// scale with any other count has no secondary dominants to name, and neither has the tonic, a
	// chromatic degree, or a degree whose stacked thirds form no recognised triad.
	private static string? SecondaryTargetNumeral(Key key, PitchClass targetRoot)
	{
		if (key.Mode.DegreeCount != 7)
		{
			return null;
		}

		ScaleDegree target = key.Scale.DegreeOf(targetRoot);
		if (target.Alteration != 0 || target.Degree == 1)
		{
			return null;
		}

		ChordQuality? quality = DiatonicTriadQuality(key.Scale, targetRoot);
		return quality is null
			? null
			: key.RomanNumeralOf(new Chord { Root = targetRoot, Quality = quality.Value });
	}

	private static (ChromaticKind Kind, string? Detail) ClassifyChromatic(Key key, Chord chord)
	{
		// Secondary dominant: a major triad or dominant seventh a perfect fifth above a diatonic degree.
		bool dominantQuality = chord.Quality == ChordQuality.Major
			&& chord.Seventh is SeventhType.None or SeventhType.Dominant;
		if (dominantQuality)
		{
			string? targetNumeral = SecondaryTargetNumeral(key, PitchClass.Create(chord.Root.Value - 7));
			if (targetNumeral is not null)
			{
				return (ChromaticKind.SecondaryDominant, "V/" + targetNumeral);
			}
		}

		// Neapolitan: a major triad on the lowered second degree.
		if (chord.Quality == ChordQuality.Major
			&& chord.Root.Value == PitchClass.Create(key.Tonic.Value + 1).Value)
		{
			return (ChromaticKind.Neapolitan, "bII");
		}

		// Borrowed: diatonic to the parallel mode of the same tonic.
		Mode parallelMode = key.Mode == Mode.Major ? Mode.Aeolian : Mode.Major;
		Scale parallel = Scale.Create(key.Tonic, parallelMode);
		if (AllTonesInScale(chord, parallel))
		{
			string source = parallelMode == Mode.Aeolian ? "parallel minor" : "parallel major";
			return (ChromaticKind.BorrowedChord, "from " + source);
		}

		return (ChromaticKind.Chromatic, null);
	}
}
