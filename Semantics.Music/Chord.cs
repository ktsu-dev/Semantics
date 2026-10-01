// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Music;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

/// <summary>
/// A chord parsed from a symbol such as "Cmaj7", "Dm7", "E7b9", "Cm7b5", "Cmmaj7", "C6", "C6/9", or "C/G".
/// </summary>
public sealed record Chord
{
	/// <summary>Gets the chord root.</summary>
	public PitchClass Root { get; init; } = PitchClass.Create(0);

	/// <summary>Gets the triad quality.</summary>
	public ChordQuality Quality { get; init; } = ChordQuality.Major;

	/// <summary>Gets the seventh, if any.</summary>
	public SeventhType Seventh { get; init; } = SeventhType.None;

	/// <summary>Gets the added sixth, if any.</summary>
	public SixthType Sixth { get; init; } = SixthType.None;

	/// <summary>Gets the upper-structure tensions and alterations.</summary>
	public ChordTensions Tensions { get; init; } = ChordTensions.None;

	/// <summary>Gets the chord tones intentionally omitted from the voicing.</summary>
	public ChordOmissions Omissions { get; init; } = ChordOmissions.None;

	/// <summary>Gets the slash-chord bass, if any (otherwise the root sounds in the bass).</summary>
	public PitchClass? Bass { get; init; }

	/// <summary>Parses a chord symbol using the chord-symbol grammar documented in the music guide.</summary>
	/// <param name="symbol">The chord symbol.</param>
	/// <returns>The parsed chord.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="symbol"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the symbol cannot be parsed.</exception>
	public static Chord Parse(string symbol)
	{
		Ensure.NotNull(symbol);
		return TryParse(symbol, out Chord? result)
			? result
			: throw new FormatException($"Invalid chord symbol '{symbol}'.");
	}

	/// <summary>Tries to parse a chord symbol.</summary>
	/// <param name="symbol">The text to parse.</param>
	/// <param name="result">The parsed chord, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? symbol, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Chord? result)
	{
		result = null;
		return symbol is not null && ChordSymbolReader.TryRead(symbol, out result);
	}

	/// <summary>Returns the chord's semitone offsets above the root, ascending and de-duplicated.</summary>
	/// <returns>Sorted offsets: 0 (root), the third, the fifth, any seventh, any sixth, then tensions.</returns>
	public IReadOnlyList<int> ChordTones()
	{
		SortedSet<int> offsets = [0];

		if (Quality != ChordQuality.Power && !Omissions.HasFlag(ChordOmissions.Third))
		{
			_ = offsets.Add(Quality switch
			{
				ChordQuality.Sus2 => 2,
				ChordQuality.Sus4 => 5,
				ChordQuality.Minor or ChordQuality.MinorSharpFive or ChordQuality.Diminished => 3,
				_ => 4,
			});
		}

		if (!Omissions.HasFlag(ChordOmissions.Fifth))
		{
			_ = offsets.Add(Quality switch
			{
				ChordQuality.Diminished or ChordQuality.MajorFlatFive => 6,
				ChordQuality.Augmented or ChordQuality.MinorSharpFive => 8,
				_ => 7,
			});
		}

		if (Seventh != SeventhType.None)
		{
			_ = offsets.Add(Seventh switch
			{
				SeventhType.Diminished => 9,
				SeventhType.Dominant => 10,
				_ => 11,
			});
		}

		if (Sixth == SixthType.Natural)
		{
			_ = offsets.Add(9);
		}
		else if (Sixth == SixthType.Flat)
		{
			_ = offsets.Add(8);
		}

		AddTension(offsets, ChordTensions.FlatNine, 13);
		AddTension(offsets, ChordTensions.Nine, 14);
		AddTension(offsets, ChordTensions.SharpNine, 15);
		AddTension(offsets, ChordTensions.Eleven, 17);
		AddTension(offsets, ChordTensions.SharpEleven, 18);
		AddTension(offsets, ChordTensions.FlatThirteen, 20);
		AddTension(offsets, ChordTensions.Thirteen, 21);

		return [.. offsets];
	}

	/// <summary>Returns the chord transposed by a number of semitones (root and any slash bass move together).</summary>
	/// <param name="semitones">The signed semitone offset.</param>
	/// <returns>The transposed chord, preserving quality, seventh, sixth, tensions, and omissions.</returns>
	public Chord Transpose(int semitones) => this with
	{
		Root = PitchClass.Create(Root.Value + semitones),
		Bass = Bass is null ? null : PitchClass.Create(Bass.Value + semitones),
	};

	/// <summary>Voices the chord in root position with the root at the given octave.</summary>
	/// <param name="octave">The octave for the root (e.g. 4 places the root at C4 for a C chord).</param>
	/// <returns>The pitches, lowest first; a slash bass (if any) sounds one octave below the root.</returns>
	public IReadOnlyList<Pitch> Voice(int octave) => Voice(octave, 0);

	/// <summary>Voices the chord at the given octave and inversion.</summary>
	/// <param name="octave">The octave for the root (e.g. 4 places the root at C4 for a C chord).</param>
	/// <param name="inversion">
	/// The inversion: 0 root position, 1 first inversion, and so on. Each step raises the next-lowest
	/// chord tone by an octave; the value wraps modulo the number of chord tones.
	/// </param>
	/// <returns>The pitches, lowest first; a slash bass (if any) sounds one octave below the root.</returns>
	public IReadOnlyList<Pitch> Voice(int octave, int inversion)
	{
		List<int> tones = [.. ChordTones()];
		int count = tones.Count;
		if (count > 0)
		{
			int rotation = ((inversion % count) + count) % count;
			for (int i = 0; i < rotation; i++)
			{
				tones[i] += 12;
			}

			tones.Sort();
		}

		Pitch rootPitch = Pitch.Parse(Root.Name + octave.ToString(CultureInfo.InvariantCulture));
		List<Pitch> pitches = [.. tones.Select(offset => rootPitch.Transpose(offset))];

		if (Bass is not null)
		{
			Pitch bassPitch = Pitch.Parse(Bass.Name + (octave - 1).ToString(CultureInfo.InvariantCulture));
			pitches.Insert(0, bassPitch);
		}

		return pitches;
	}

	/// <summary>Returns the canonical symbol, inverse to <see cref="Parse"/> for expressible chords.</summary>
	/// <returns>The canonical chord symbol.</returns>
	public override string ToString() => ChordSymbolWriter.Format(this);

	private void AddTension(SortedSet<int> offsets, ChordTensions flag, int semitones)
	{
		if (Tensions.HasFlag(flag))
		{
			_ = offsets.Add(semitones);
		}
	}
}
