// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents one of the twelve pitch classes (C..B), octave-folded to 0..11.
/// </summary>
public sealed record PitchClass
{
	private static readonly string[] Names =
		["C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"];

	/// <summary>Gets the pitch class value, 0 (C) through 11 (B).</summary>
	public int Value { get; private init; }

	/// <summary>Gets the sharp-spelled name of this pitch class.</summary>
	public string Name => Names[Value];

	/// <summary>Creates a pitch class, folding any integer into the range 0..11.</summary>
	/// <param name="value">Any integer; reduced modulo 12.</param>
	/// <returns>A pitch class in 0..11.</returns>
	public static PitchClass Create(int value) => new() { Value = ((value % 12) + 12) % 12 };

	/// <summary>Creates a pitch class from a note letter and accidental.</summary>
	/// <param name="letter">The natural note letter.</param>
	/// <param name="accidental">The accidental.</param>
	/// <returns>The folded pitch class.</returns>
	public static PitchClass Create(NoteLetter letter, Accidental accidental) =>
		Create((int)letter + (int)accidental);

	/// <summary>Parses a pitch-class name such as "C", "F#", or "Bb".</summary>
	/// <param name="text">A note letter A-G with optional accidentals; no octave.</param>
	/// <returns>The parsed pitch class.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static PitchClass Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out PitchClass? result)
			? result
			: throw new FormatException($"Invalid pitch class '{text}'.");
	}

	/// <summary>Tries to parse a pitch-class name.</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed pitch class, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [NotNullWhen(true)] out PitchClass? result)
	{
		result = null;
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}

		int index = 0;
		if (!Notation.TryReadNoteLetter(text![index], out NoteLetter letter))
		{
			return false;
		}

		index++;
		int accidental = Notation.ReadAccidentalOffset(text, ref index);
		if (index != text.Length)
		{
			return false;
		}

		result = Create((int)letter + accidental);
		return true;
	}

	/// <summary>Returns the sharp-spelled pitch-class name.</summary>
	/// <returns>The canonical name (e.g. "C#").</returns>
	public override string ToString() => Name;
}
