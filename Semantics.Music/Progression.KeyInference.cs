// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System.Collections.Generic;
using System.Linq;

public sealed partial record Progression
{
	/// <summary>Ranks candidate keys (12 tonics in major and natural minor) by quality-weighted diatonic fit.</summary>
	/// <returns>All 24 candidates, best first; ties break toward a tonic-rooted last then first chord, then major.</returns>
	public IReadOnlyList<KeyMatch> InferKeys()
	{
		PitchClass firstRoot = Chords[0].Chord.Root;
		PitchClass lastRoot = Chords[^1].Chord.Root;
		List<KeyMatch> matches = [];
		foreach (Mode mode in new[] { Mode.Major, Mode.Aeolian })
		{
			for (int tonic = 0; tonic < 12; tonic++)
			{
				Key key = Key.Create(PitchClass.Create(tonic), mode);
				double total = 0.0;
				foreach (ChordEvent chordEvent in Chords)
				{
					total += ScoreChord(key, chordEvent.Chord);
				}

				matches.Add(KeyMatch.Create(key, total / Chords.Count));
			}
		}

		return
		[
			.. matches
				.OrderByDescending(match => match.Score)
				.ThenByDescending(match => match.Key.Tonic.Value == lastRoot.Value)
				.ThenByDescending(match => match.Key.Tonic.Value == firstRoot.Value)
				.ThenByDescending(match => match.Key.Mode == Mode.Major)
				.ThenBy(match => match.Key.Tonic.Value),
		];
	}

	// Scores one chord's fit to a key: 1.0 when its root is diatonic AND its triad quality matches the
	// diatonic triad at that degree, 0.5 when the root is diatonic but the quality differs (or has no
	// comparable third), 0.0 when the root is not in the scale. Quality-weighting is what distinguishes
	// a key from its relative/parallel neighbours, which share the same chord roots.
	private static double ScoreChord(Key key, Chord chord)
	{
		Scale scale = key.Scale;
		if (!scale.Contains(chord.Root))
		{
			return 0.0;
		}

		ChordQuality? diatonic = DiatonicTriadQuality(scale, chord.Root);
		bool comparable = chord.Quality is ChordQuality.Major or ChordQuality.Minor
			or ChordQuality.Diminished or ChordQuality.Augmented;
		if (!comparable || diatonic is null)
		{
			return 1.0;
		}

		return chord.Quality == diatonic.Value ? 1.0 : 0.5;
	}

	// Returns the diatonic triad quality built on a scale degree by stacking scale thirds, or null when
	// the third/fifth do not form a recognised triad.
	private static ChordQuality? DiatonicTriadQuality(Scale scale, PitchClass root)
	{
		IReadOnlyList<PitchClass> pitchClasses = scale.PitchClasses;
		int index = -1;
		for (int i = 0; i < pitchClasses.Count; i++)
		{
			if (pitchClasses[i].Value == root.Value)
			{
				index = i;
				break;
			}
		}

		if (index < 0)
		{
			return null;
		}

		int count = pitchClasses.Count;
		int third = (((pitchClasses[(index + 2) % count].Value - root.Value) % 12) + 12) % 12;
		int fifth = (((pitchClasses[(index + 4) % count].Value - root.Value) % 12) + 12) % 12;
		return (third, fifth) switch
		{
			(4, 7) => ChordQuality.Major,
			(3, 7) => ChordQuality.Minor,
			(3, 6) => ChordQuality.Diminished,
			(4, 8) => ChordQuality.Augmented,
			_ => null,
		};
	}

	/// <summary>Returns the single best-fitting key, or null when no chord root is diatonic to any candidate.</summary>
	/// <returns>The best key, or null for a degenerate all-chromatic input.</returns>
	public Key? InferKey()
	{
		KeyMatch best = InferKeys()[0];
		return best.Score > 0.0 ? best.Key : null;
	}
}
