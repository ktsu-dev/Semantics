// Copyright (c) 2023-2026 ktsu-dev contributors

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

	// Scores one chord's fit to a key in two parts, multiplied together.
	//
	// The root and its triad: 0.0 when the root is not in the scale; 1.0 when the chord's triad quality
	// matches the diatonic triad on that degree, or has no third to compare (sus, power and altered
	// fifths) or the degree forms no recognised triad; 0.5 when the qualities differ. Quality-weighting
	// is what separates a key from its relative and parallel neighbours, which share the same roots.
	//
	// Every other tone: the fraction of the chord's pitch classes, slash bass included, that are in the
	// scale. Without it C7 and Cmaj7 score alike everywhere, and the Bb that makes C7 the dominant of F
	// costs C major nothing. A minor key's dominant (E or E7 in A minor) scores as diatonic, raised
	// leading tone and all, since that is the key's own V.
	private static double ScoreChord(Key key, Chord chord)
	{
		Scale scale = key.Scale;
		if (!scale.Contains(chord.Root))
		{
			return 0.0;
		}

		if (IsHarmonicMinorDominant(key, chord))
		{
			return ToneFit(scale, chord, PitchClass.Create(key.Tonic.Value + 11));
		}

		ChordQuality? diatonic = DiatonicTriadQuality(scale, chord.Root);
		bool comparable = chord.Quality is ChordQuality.Major or ChordQuality.Minor
			or ChordQuality.Diminished or ChordQuality.Augmented;
		double triad = !comparable || diatonic is null || chord.Quality == diatonic.Value ? 1.0 : 0.5;
		return triad * ToneFit(scale, chord, null);
	}

	private static double ToneFit(Scale scale, Chord chord, PitchClass? alsoDiatonic)
	{
		List<int> tones = [.. chord.ChordTones().Select(offset => PitchClass.Create(chord.Root.Value + offset).Value)];
		if (chord.Bass is not null && !tones.Contains(chord.Bass.Value))
		{
			tones.Add(chord.Bass.Value);
		}

		int inScale = tones.Count(tone => tone == alsoDiatonic?.Value || scale.Contains(PitchClass.Create(tone)));
		return inScale / (double)tones.Count;
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
