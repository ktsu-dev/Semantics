// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
			_ = sb.Append(" \"").Append(Label).Append('"');
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

		if (!Enum.TryParse<SectionType>(header[1..close], out SectionType type))
		{
			return false;
		}

		string? label = null;
		int quote = header.IndexOf('"', close);
		if (quote >= 0)
		{
			int endQuote = header.IndexOf('"', quote + 1);
			if (endQuote < 0)
			{
				return false;
			}

			label = header[(quote + 1)..endQuote];
		}

		Key? key = null;
		int open = header.IndexOf('(', close);
		if (open >= 0)
		{
			int endParen = header.IndexOf(')', open + 1);
			if (endParen < 0 || !Key.TryParse(header[(open + 1)..endParen], out key))
			{
				return false;
			}
		}

		if (!Progression.TryParse(lines[1], out Progression? progression))
		{
			return false;
		}

		result = Create(type, progression, label, key);
		return true;
	}
}
