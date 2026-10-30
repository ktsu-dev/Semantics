// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Music;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

/// <summary>A labeled structural unit of a piece: a role plus its harmonic content.</summary>
public sealed record Section
{
	/// <summary>Gets the structural role of the section.</summary>
	public SectionType Type { get; private init; } = SectionType.Other;

	/// <summary>Gets an optional human label distinguishing repeated instances (e.g. "Verse 1").</summary>
	public string? Label { get; private init; }

	/// <summary>Gets the harmonic content of the section.</summary>
	public Progression Progression { get; private init; } = new();

	/// <summary>Gets an optional local key for a section that modulates; null inherits the arrangement key.</summary>
	public Key? Key { get; private init; }

	/// <summary>Gets the length of the section in bars, derived from its progression.</summary>
	public double Bars => Progression.TotalBars;

	/// <summary>Creates a section.</summary>
	/// <param name="type">The structural role.</param>
	/// <param name="progression">The harmonic content.</param>
	/// <param name="label">An optional human label.</param>
	/// <param name="key">An optional local key.</param>
	/// <returns>A new section.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="progression"/> is null.</exception>
	public static Section Create(SectionType type, Progression progression, string? label = null, Key? key = null)
	{
		Ensure.NotNull(progression);
		return new() { Type = type, Progression = progression, Label = label, Key = key };
	}

	/// <summary>Returns whether another section has the same role and ordered chord content.</summary>
	/// <param name="other">The section to compare.</param>
	/// <returns><see langword="true"/> when the type and chord sequence match; label and key are ignored.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
	public bool IsSameStructure(Section other)
	{
		Ensure.NotNull(other);
		if (Type != other.Type)
		{
			return false;
		}

		IReadOnlyList<ChordEvent> mine = Progression.Chords;
		IReadOnlyList<ChordEvent> theirs = other.Progression.Chords;
		if (mine.Count != theirs.Count)
		{
			return false;
		}

		for (int i = 0; i < mine.Count; i++)
		{
			if (mine[i].Chord != theirs[i].Chord)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>Returns the chart-style section block: a bracket header then the progression line.</summary>
	/// <returns>The canonical section text.</returns>
	public override string ToString()
	{
		StringBuilder sb = new();
		_ = sb.Append('[').Append(Type).Append(']');
		if (Label is not null)
		{
			_ = sb.Append(" \"");
			AppendEscapedLabel(sb, Label);
			_ = sb.Append('"');
		}

		if (Key is not null)
		{
			_ = sb.Append(" (").Append(Key).Append(')');
		}

		_ = sb.Append('\n').Append(Progression);
		return sb.ToString();
	}

	/// <summary>Parses a chart-style section block.</summary>
	/// <param name="text">The section text.</param>
	/// <returns>The parsed section.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static Section Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Section? result)
			? result
			: throw new FormatException($"Invalid section '{text}'.");
	}

	/// <summary>Tries to parse a chart-style section block.</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed section, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [NotNullWhen(true)] out Section? result)
	{
		result = null;
		if (text is null)
		{
			return false;
		}

		string[] lines = text.Replace("\r", string.Empty).Split('\n');
		if (lines.Length < 2 || lines[0].Length == 0 || lines[0][0] != '[')
		{
			return false;
		}

		string header = lines[0];
		int close = header.IndexOf(']');
		if (close < 1)
		{
			return false;
		}

		if (!TryParseSectionType(header[1..close], out SectionType type))
		{
			return false;
		}

		if (!TryParseHeaderTail(header, close + 1, out string? label, out Key? key))
		{
			return false;
		}

		if (!Progression.TryParse(lines[1], out Progression? progression))
		{
			return false;
		}

		if (!OnlyBlankLinesFollowProgression(lines))
		{
			return false;
		}

		result = Create(type, progression, label, key);
		return true;
	}

	// After "]" the header is exactly one of: nothing, ` "<label>"`, ` (<key>)` or ` "<label>" (<key>)`.
	// Anything else is refused, so a key written before the label cannot be dropped and junk cannot hide
	// in the header.
	private static bool TryParseHeaderTail(string header, int position, out string? label, out Key? key)
	{
		label = null;
		key = null;
		if (position == header.Length)
		{
			return true;
		}

		if (!TrySkipSeparator(header, ref position))
		{
			return false;
		}

		if (header[position] == '"')
		{
			if (!TryReadEscapedLabel(header, position + 1, out label, out int endQuote))
			{
				return false;
			}

			position = endQuote + 1;

			// Only a key may follow the label, after a single space.
			if (position == header.Length)
			{
				return true;
			}

			if (!TrySkipSeparator(header, ref position))
			{
				return false;
			}
		}

		return TryReadKey(header, position, out key);
	}

	// Steps over the single space between header parts, which has to be followed by something.
	private static bool TrySkipSeparator(string header, ref int position)
	{
		if (header[position] != ' ' || position + 1 >= header.Length)
		{
			return false;
		}

		position++;
		return true;
	}

	// Reads ` (<key>)` from its opening parenthesis to the end of the header line. The closing
	// parenthesis has to end the line, so nothing can trail the key.
	private static bool TryReadKey(string header, int open, [NotNullWhen(true)] out Key? key)
	{
		key = null;
		return header[open] == '('
			&& header[^1] == ')'
			&& Key.TryParse(header[(open + 1)..^1], out key);
	}

	// The progression is the one line after the header. Anything else is refused rather than
	// dropped, so a section written across several lines cannot parse as a shorter progression.
	// Trailing blank lines, such as a file's final newline, carry nothing and are allowed.
	private static bool OnlyBlankLinesFollowProgression(string[] lines) =>
		lines.Skip(2).All(string.IsNullOrWhiteSpace);

	// The label sits between double quotes on the one header line, so a quote, a backslash or a line
	// break inside it is written as a backslash escape that TryReadEscapedLabel reverses.
	private static void AppendEscapedLabel(StringBuilder sb, string label)
	{
		foreach (char c in label)
		{
			_ = c switch
			{
				'"' => sb.Append("\\\""),
				'\\' => sb.Append("\\\\"),
				'\n' => sb.Append("\\n"),
				'\r' => sb.Append("\\r"),
				_ => sb.Append(c),
			};
		}
	}

	// Reads an escaped label starting just after its opening quote, returning the index of the
	// closing quote. A backslash before anything other than a written escape is kept as it is, so
	// a label written before escaping existed still reads back the same.
	private static bool TryReadEscapedLabel(string header, int start, [NotNullWhen(true)] out string? label, out int endQuote)
	{
		label = null;
		endQuote = -1;
		StringBuilder sb = new();
		int i = start;
		while (i < header.Length)
		{
			char c = header[i];
			if (c == '"')
			{
				label = sb.ToString();
				endQuote = i;
				return true;
			}

			char next = i + 1 < header.Length ? header[i + 1] : '\0';
			if (c == '\\' && next is '"' or '\\' or 'n' or 'r')
			{
				_ = sb.Append(next switch
				{
					'n' => '\n',
					'r' => '\r',
					_ => next,
				});
				i += 2;
				continue;
			}

			_ = sb.Append(c);
			i++;
		}

		return false;
	}

	// Enum.TryParse alone also takes integers, comma-separated name lists and padded names, all of
	// which ToString never writes. Only the exact name of one declared member is a section type.
	private static bool TryParseSectionType(string name, out SectionType type)
	{
		type = default;
		if (name.Length == 0)
		{
			return false;
		}

		foreach (char c in name)
		{
			if (c is not ((>= 'A' and <= 'Z') or (>= 'a' and <= 'z')))
			{
				return false;
			}
		}

		return Enum.TryParse(name, out type) && Enum.IsDefined(type);
	}
}
