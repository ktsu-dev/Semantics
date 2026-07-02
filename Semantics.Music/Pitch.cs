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

	/// <summary>Creates a pitch from a note letter, accidental, and octave (MIDI 60 = C4).</summary>
	/// <param name="letter">The natural note letter.</param>
	/// <param name="accidental">The accidental.</param>
	/// <param name="octave">The octave number.</param>
	/// <returns>A new pitch.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the resulting MIDI number is outside 0..127.</exception>
	public static Pitch Create(NoteLetter letter, Accidental accidental, int octave) =>
		Create(((octave + 1) * 12) + (int)letter + (int)accidental);

	/// <summary>Parses a note name such as "C4", "F#3", or "Bb5".</summary>
	/// <param name="name">A letter A-G, optional accidentals, then an octave integer.</param>
	/// <returns>The parsed pitch.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the name cannot be parsed.</exception>
	public static Pitch Parse(string name)
	{
		Ensure.NotNull(name);
		return TryParse(name, out Pitch? result)
			? result
			: throw new FormatException($"Invalid note name '{name}'.");
	}

	/// <summary>Tries to parse a note name.</summary>
	/// <param name="name">The text to parse.</param>
	/// <param name="result">The parsed pitch, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? name, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Pitch? result)
	{
		result = null;
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}

		int index = 0;
		if (!Notation.TryReadNoteLetter(name![index], out NoteLetter letter))
		{
			return false;
		}

		index++;
		int accidental = Notation.ReadAccidentalOffset(name, ref index);
		if (!int.TryParse(name[index..], NumberStyles.Integer, CultureInfo.InvariantCulture, out int octave))
		{
			return false;
		}

		int midi = ((octave + 1) * 12) + (int)letter + accidental;
		if (midi is < 0 or > 127)
		{
			return false;
		}

		result = new() { Midi = midi };
		return true;
	}

	/// <summary>Returns the canonical note name (e.g. "C4").</summary>
	/// <returns>The note name.</returns>
	public override string ToString() => Name;

	/// <summary>Returns a pitch transposed by a number of semitones.</summary>
	/// <param name="semitones">The signed semitone offset.</param>
	/// <returns>The transposed pitch.</returns>
	public Pitch Transpose(int semitones) => Create(Midi + semitones);

	/// <summary>The reference tuning: A4 (MIDI 69) in hertz.</summary>
	public const double ConcertPitchHz = 440.0;

	/// <summary>Gets the fundamental frequency in hertz, using 12-tone equal temperament at A4 = 440 Hz.</summary>
	public double FrequencyHz => ConcertPitchHz * Math.Pow(2.0, (Midi - 69) / 12.0);

	/// <summary>Returns the pitch whose equal-tempered frequency is nearest to the given frequency.</summary>
	/// <param name="hertz">The frequency in hertz; must be positive.</param>
	/// <returns>The nearest pitch.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown when <paramref name="hertz"/> is not positive or maps outside the MIDI range 0..127.
	/// </exception>
	public static Pitch FromFrequency(double hertz)
	{
		if (hertz <= 0.0 || double.IsNaN(hertz))
		{
			throw new ArgumentOutOfRangeException(nameof(hertz), hertz, "Frequency must be positive.");
		}

		int midi = (int)Math.Round(69 + (12.0 * Math.Log(hertz / ConcertPitchHz, 2.0)), MidpointRounding.AwayFromZero);
		return Create(midi);
	}
}
