// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

/// <summary>Shared note-letter and accidental scanning used by pitch, chord, and roman-numeral parsing.</summary>
internal static class Notation
{
	/// <summary>Maps a note-letter character (case-insensitive) to a <see cref="NoteLetter"/>.</summary>
	/// <param name="c">The character to read.</param>
	/// <param name="letter">The parsed note letter, or default on failure.</param>
	/// <returns><see langword="true"/> when the character is a note letter A-G.</returns>
	internal static bool TryReadNoteLetter(char c, out NoteLetter letter)
	{
		NoteLetter? mapped = char.ToUpperInvariant(c) switch
		{
			'C' => NoteLetter.C,
			'D' => NoteLetter.D,
			'E' => NoteLetter.E,
			'F' => NoteLetter.F,
			'G' => NoteLetter.G,
			'A' => NoteLetter.A,
			'B' => NoteLetter.B,
			_ => null,
		};

		letter = mapped ?? default;
		return mapped is not null;
	}

	/// <summary>Consumes any run of accidental characters (# b ♯ ♭) from <paramref name="index"/>, returning the net semitone offset.</summary>
	/// <param name="text">The text being scanned.</param>
	/// <param name="index">The current index; advanced past any accidental characters.</param>
	/// <returns>The net semitone offset of the consumed accidentals.</returns>
	internal static int ReadAccidentalOffset(string text, ref int index)
	{
		int offset = 0;
		while (index < text.Length && (text[index] is '#' or 'b' or '♯' or '♭'))
		{
			offset += text[index] is '#' or '♯' ? 1 : -1;
			index++;
		}

		return offset;
	}
}
