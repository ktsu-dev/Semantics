// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System.Collections.Generic;
using System.Linq;

public sealed partial record Progression
{
	private static readonly string[] MajorDegreeRomans = ["I", "ii", "iii", "IV", "V", "vi", "vii°"];
	private static readonly string[] MinorDegreeRomans = ["i", "ii°", "III", "iv", "v", "VI", "VII"];

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

	private static bool IsDiatonic(Key key, Chord chord)
	{
		Scale scale = key.Scale;
		return chord.ChordTones().All(offset => scale.Contains(PitchClass.Create(chord.Root.Value + offset)));
	}

	private static (ChromaticKind Kind, string? Detail) ClassifyChromatic(Key key, Chord chord)
	{
		// Secondary dominant: a major triad or dominant seventh a perfect fifth above a diatonic degree.
		bool dominantQuality = chord.Quality == ChordQuality.Major
			&& chord.Seventh is SeventhType.None or SeventhType.Dominant;
		if (dominantQuality)
		{
			ScaleDegree target = key.Scale.DegreeOf(PitchClass.Create(chord.Root.Value - 7));
			if (target.Alteration == 0 && target.Degree != 1)
			{
				string[] table = key.Mode == Mode.Major ? MajorDegreeRomans : MinorDegreeRomans;
				return (ChromaticKind.SecondaryDominant, "V/" + table[(target.Degree - 1) % 7]);
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
		if (chord.ChordTones().All(offset => parallel.Contains(PitchClass.Create(chord.Root.Value + offset))))
		{
			string source = parallelMode == Mode.Aeolian ? "parallel minor" : "parallel major";
			return (ChromaticKind.BorrowedChord, "from " + source);
		}

		return (ChromaticKind.Chromatic, null);
	}
}
