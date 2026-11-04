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

			Chord? next = i + 1 < Chords.Count ? Chords[i + 1].Chord : null;
			(ChromaticKind kind, string? detail) = ClassifyChromatic(key, chord, next);
			result.Add(ChromaticAnalysis.Create(i, kind, detail));
		}

		return result;
	}

	private static bool AllTonesInScale(Chord chord, Scale scale) =>
		chord.ChordTones().All(offset => scale.Contains(PitchClass.Create(chord.Root.Value + offset)));

	private static bool IsDiatonic(Key key, Chord chord) =>
		AllTonesInScale(chord, key.Scale) || IsHarmonicMinorDominant(key, chord) || IsHarmonicMinorLeadingToneChord(key, chord);

	// A minor key's dominant raises its leading tone: E and E7 in A minor are the key's own V and V7,
	// not chords borrowed from A major, and every other analysis already treats them that way. A key
	// counts as minor here when its scale has a minor third and a minor seventh, so the raised
	// seventh is the one tone the dominant adds.
	private static bool IsHarmonicMinorDominant(Key key, Chord chord) =>
		IsMinorWithoutLeadingTone(key)
		&& chord.Root.Value == PitchClass.Create(key.Tonic.Value + 7).Value
		&& chord.Quality == ChordQuality.Major
		&& chord.Seventh is SeventhType.None or SeventhType.Dominant
		&& OnlyAddsLeadingTone(key, chord);

	// The chord built on that raised seventh belongs to the key for the same reason: G#dim and G#dim7
	// in A minor are its vii° and vii°7, which the analysis spec files under Dominant with V. A
	// half-diminished seventh qualifies too, provided the leading tone is still the only tone it adds.
	private static bool IsHarmonicMinorLeadingToneChord(Key key, Chord chord) =>
		IsMinorWithoutLeadingTone(key)
		&& chord.Root.Value == PitchClass.Create(key.Tonic.Value + 11).Value
		&& chord.Quality == ChordQuality.Diminished
		&& chord.Seventh is SeventhType.None or SeventhType.Diminished or SeventhType.Dominant
		&& OnlyAddsLeadingTone(key, chord);

	private static bool IsMinorWithoutLeadingTone(Key key)
	{
		int tonic = key.Tonic.Value;
		Scale scale = key.Scale;
		return scale.Contains(PitchClass.Create(tonic + 3))
			&& scale.Contains(PitchClass.Create(tonic + 10))
			&& !scale.Contains(PitchClass.Create(tonic + 11));
	}

	private static bool OnlyAddsLeadingTone(Key key, Chord chord)
	{
		int leadingTone = PitchClass.Create(key.Tonic.Value + 11).Value;
		return chord.ChordTones().All(offset =>
		{
			PitchClass tone = PitchClass.Create(chord.Root.Value + offset);
			return tone.Value == leadingTone || key.Scale.Contains(tone);
		});
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

		// Only a stable triad can be tonicized: a diminished or augmented degree (vii° in major, ii° or
		// III+ in minor) is no local tonic, so a chord a fifth above it is merely chromatic.
		ChordQuality? quality = DiatonicTriadQuality(key.Scale, targetRoot);
		return quality is ChordQuality.Major or ChordQuality.Minor
			? key.RomanNumeralOf(new Chord { Root = targetRoot, Quality = quality.Value })
			: null;
	}

	// Italian, French and German sixths, all built on the lowered sixth degree. A chord has no
	// spelling, only pitch classes, so the augmented sixth (F# over Ab in C) is the chord's minor
	// seventh (Gb) and each variant has the shape of a dominant seventh on that root:
	//   It+6  Ab C F#     Ab7no5
	//   Fr+6  Ab C D F#   Ab7b5
	//   Ger+6 Ab C Eb F#  Ab7
	// The Italian and French shapes are not otherwise idiomatic there. The German one is exactly a
	// dominant seventh, so it is read as an augmented sixth only where it resolves the way one does:
	// to the dominant, or to a tonic chord over the dominant in the bass (the cadential six-four).
	private static string? AugmentedSixthVariant(Key key, Chord chord, Chord? next)
	{
		int tonic = key.Tonic.Value;
		if (chord.Root.Value != PitchClass.Create(tonic + 8).Value
			|| chord.Seventh != SeventhType.Dominant
			|| chord.Sixth != SixthType.None
			|| chord.Tensions != ChordTensions.None
			|| chord.Omissions.HasFlag(ChordOmissions.Third))
		{
			return null;
		}

		bool fifthOmitted = chord.Omissions.HasFlag(ChordOmissions.Fifth);
		if (chord.Quality == ChordQuality.Major && fifthOmitted)
		{
			return "It+6";
		}

		if (chord.Quality == ChordQuality.MajorFlatFive && !fifthOmitted)
		{
			return "Fr+6";
		}

		int dominant = PitchClass.Create(tonic + 7).Value;
		bool resolvesToDominant = next is not null
			&& (next.Root.Value == dominant
				|| (next.Root.Value == tonic && next.Bass?.Value == dominant));
		return chord.Quality == ChordQuality.Major && !fifthOmitted && resolvesToDominant
			? "Ger+6"
			: null;
	}

	private static (ChromaticKind Kind, string? Detail) ClassifyChromatic(Key key, Chord chord, Chord? next)
	{
		string? augmentedSixth = AugmentedSixthVariant(key, chord, next);
		if (augmentedSixth is not null)
		{
			return (ChromaticKind.AugmentedSixth, augmentedSixth);
		}

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
