// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Music;

using System;
using System.Globalization;

/// <summary>Shared note-letter and accidental scanning used by pitch, chord, and roman-numeral parsing.</summary>
internal static class Notation
{
	/// <summary>Throws when a note letter or accidental is not a defined value, so a cast int can't silently name another note.</summary>
	/// <param name="letter">The note letter; its parameter name is the one the public factories use.</param>
	/// <param name="accidental">The accidental; its parameter name is the one the public factories use.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when either value is undefined.</exception>
	internal static void EnsureDefined(NoteLetter letter, Accidental accidental)
	{
		if (!Enum.IsDefined(letter))
		{
			throw new ArgumentOutOfRangeException(nameof(letter), letter, "Note letter must be a defined NoteLetter value.");
		}

		if (!Enum.IsDefined(accidental))
		{
			throw new ArgumentOutOfRangeException(nameof(accidental), accidental, "Accidental must be a defined Accidental value.");
		}
	}

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

	/// <summary>
	/// Parses an integer written the way the music types print one: digits only, with no surrounding
	/// whitespace and no leading '+', so a parsed value always prints back as the text it came from.
	/// </summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="allowNegative">Whether a leading '-' is accepted.</param>
	/// <param name="value">The parsed value, or 0 on failure.</param>
	/// <returns><see langword="true"/> when the text is a canonical integer.</returns>
	internal static bool TryParseInteger(string text, bool allowNegative, out int value)
	{
		value = 0;
		if (text.Length == 0 || text[0] == '+')
		{
			return false;
		}

		NumberStyles styles = allowNegative ? NumberStyles.AllowLeadingSign : NumberStyles.None;
		return int.TryParse(text, styles, CultureInfo.InvariantCulture, out value);
	}
}
