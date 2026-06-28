// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;
using System.Globalization;

/// <summary>
/// Represents a concrete pitch as a MIDI note number (0..127). MIDI 60 is middle C (C4).
/// </summary>
public sealed record Pitch
{
	/// <summary>Gets the MIDI note number, 0..127.</summary>
	public int Midi { get; private init; }

	/// <summary>Gets the octave-folded pitch class.</summary>
	public PitchClass PitchClass => PitchClass.Create(Midi);

	/// <summary>Gets the octave number using the convention MIDI 60 = C4.</summary>
	public int Octave => (Midi / 12) - 1;

	/// <summary>Gets the note name, e.g. "C4" or "F#3".</summary>
	public string Name => PitchClass.Name + Octave.ToString(CultureInfo.InvariantCulture);

	/// <summary>Creates a pitch from a MIDI note number.</summary>
	/// <param name="midi">The MIDI note number, 0..127.</param>
	/// <returns>A new pitch.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="midi"/> is outside 0..127.</exception>
	public static Pitch Create(int midi)
	{
		if (midi is < 0 or > 127)
		{
			throw new ArgumentOutOfRangeException(nameof(midi), midi, "MIDI note number must be in 0..127.");
		}

		return new() { Midi = midi };
	}

	/// <summary>Parses a note name such as "C4", "F#3", or "Bb5".</summary>
	/// <param name="name">The note name: a letter A-G, optional # or b, then an octave integer.</param>
	/// <returns>The parsed pitch.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the name cannot be parsed.</exception>
	public static Pitch FromName(string name)
	{
		Ensure.NotNull(name);

		int index = 0;
		int letterPc = char.ToUpperInvariant(name[index]) switch
		{
			'C' => 0,
			'D' => 2,
			'E' => 4,
			'F' => 5,
			'G' => 7,
			'A' => 9,
			'B' => 11,
			_ => throw new FormatException($"Invalid note letter in '{name}'."),
		};
		index++;

		int accidental = 0;
		while (index < name.Length && (name[index] is '#' or 'b' or '♯' or '♭'))
		{
			accidental += name[index] is '#' or '♯' ? 1 : -1;
			index++;
		}

		if (!int.TryParse(name[index..], NumberStyles.Integer, CultureInfo.InvariantCulture, out int octave))
		{
			throw new FormatException($"Missing or invalid octave in '{name}'.");
		}

		int midi = ((octave + 1) * 12) + letterPc + accidental;
		return Create(midi);
	}

	/// <summary>Returns a pitch transposed by a number of semitones.</summary>
	/// <param name="semitones">The signed semitone offset.</param>
	/// <returns>The transposed pitch.</returns>
	public Pitch Transpose(int semitones) => Create(Midi + semitones);
}
