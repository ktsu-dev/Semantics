// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>The structural form of a piece: its section letter-pattern and any recognized named form.</summary>
public sealed record Form
{
	private static readonly int[][] BluesTemplates =
	[
		[1, 1, 1, 1, 4, 4, 1, 1, 5, 4, 1, 1],
		[1, 1, 1, 1, 4, 4, 1, 1, 5, 4, 1, 5],
		[1, 4, 1, 1, 4, 4, 1, 1, 5, 4, 1, 1],
		[1, 4, 1, 1, 4, 4, 1, 1, 5, 4, 1, 5],
	];

	/// <summary>Gets the section letter-pattern (e.g. "AABA").</summary>
	public string Pattern { get; private init; } = "";

	/// <summary>Gets the per-section letters, aligned to arrangement order.</summary>
	public IReadOnlyList<char> Letters { get; private init; } = [];

	/// <summary>Gets the recognized named form, or null.</summary>
	public NamedForm? Name { get; private init; }

	/// <summary>Determines structural equality: the pattern, named form, and ordered letters all match.</summary>
	/// <param name="other">The form to compare.</param>
	/// <returns><see langword="true"/> when the forms are value-equal.</returns>
	public bool Equals(Form? other) =>
		other is not null
		&& Pattern == other.Pattern
		&& Name == other.Name
		&& Letters.SequenceEqual(other.Letters);

	/// <summary>Returns a hash code consistent with <see cref="Equals(Form)"/>.</summary>
	/// <returns>The hash code.</returns>
	public override int GetHashCode() => (Pattern, Name).GetHashCode();

	/// <summary>Derives the form of an arrangement from its sections.</summary>
	/// <param name="arrangement">The arrangement to analyze.</param>
	/// <returns>The derived form.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="arrangement"/> is null.</exception>
	public static Form Of(Arrangement arrangement)
	{
		Ensure.NotNull(arrangement);
		IReadOnlyList<Section> sections = arrangement.Sections;

		List<char> letters = [];
		List<Section> representatives = [];
		foreach (Section section in sections)
		{
			int index = representatives.FindIndex(representative => representative.IsSameStructure(section));
			if (index < 0)
			{
				representatives.Add(section);
				index = representatives.Count - 1;
			}

			letters.Add((char)('A' + index));
		}

		string pattern = new([.. letters]);
		NamedForm name = sections.Any(section => IsTwelveBarBlues(section, section.Key ?? arrangement.Key))
			? NamedForm.TwelveBarBlues
			: RecognizePattern(pattern);

		return new() { Pattern = pattern, Letters = letters, Name = name };
	}

	/// <summary>Parses a form from a letter-pattern string.</summary>
	/// <param name="pattern">A pattern of upper-case letters A-Z (e.g. "ABACA").</param>
	/// <returns>The form for the pattern.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="pattern"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the pattern is empty or contains a non-upper-case-letter.</exception>
	public static Form Parse(string pattern)
	{
		Ensure.NotNull(pattern);
		return TryParse(pattern, out Form? result)
			? result
			: throw new FormatException($"Invalid form pattern '{pattern}'.");
	}

	/// <summary>Tries to parse a form from a letter-pattern string.</summary>
	/// <param name="pattern">The text to parse.</param>
	/// <param name="result">The parsed form, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? pattern, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Form? result)
	{
		result = null;
		if (pattern is null || pattern.Length == 0 || !pattern.All(c => c is >= 'A' and <= 'Z'))
		{
			return false;
		}

		result = new() { Pattern = pattern, Letters = [.. pattern], Name = RecognizePattern(pattern) };
		return true;
	}

	/// <summary>Returns the letter-pattern (e.g. "AABA").</summary>
	/// <returns>The canonical form text.</returns>
	public override string ToString() => Pattern;

	private static NamedForm RecognizePattern(string pattern) => pattern switch
	{
		"AABA" => NamedForm.ThirtyTwoBarAABA,
		"AB" => NamedForm.Binary,
		"ABA" => NamedForm.Ternary,
		_ when IsRondo(pattern) => NamedForm.Rondo,
		_ when pattern.Length > 1 && pattern.All(letter => letter == pattern[0]) => NamedForm.Strophic,
		_ when pattern.Distinct().Count() == pattern.Length => NamedForm.ThroughComposed,
		_ => NamedForm.Unknown,
	};

	private static bool IsRondo(string pattern)
	{
		if (pattern.Length < 5 || pattern.Length % 2 == 0)
		{
			return false;
		}

		for (int i = 0; i < pattern.Length; i++)
		{
			bool refrainPosition = i % 2 == 0;
			if (refrainPosition != (pattern[i] == 'A'))
			{
				return false;
			}
		}

		return true;
	}

	private static bool IsTwelveBarBlues(Section section, Key key)
	{
		IReadOnlyList<ChordEvent> chords = section.Progression.Chords;
		if (chords.Count != 12)
		{
			return false;
		}

		int[] degrees = new int[12];
		for (int i = 0; i < 12; i++)
		{
			ScaleDegree degree = key.FunctionOf(chords[i].Chord.Root);
			if (degree.Alteration != 0)
			{
				return false;
			}

			degrees[i] = degree.Degree;
		}

		return BluesTemplates.Any(template => template.SequenceEqual(degrees));
	}
}
