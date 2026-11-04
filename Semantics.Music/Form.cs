// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Music;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>The structural form of a piece: its section letter-pattern and any recognized named form.</summary>
/// <remarks>
/// <para>
/// The name is the named form recognized from the letter pattern alone, unless <see cref="Of"/> saw more in the
/// arrangement: a twelve-bar blues section, verse and chorus sections, or AABA sections that are not eight bars each.
/// The text form carries the name only when it differs from the one the pattern implies, as <c>"A (TwelveBarBlues)"</c>
/// or <c>"AABA (Unknown)"</c>, so <c>Form.Parse(form.ToString())</c> equals <c>form</c> for every form.
/// </para>
/// </remarks>
public sealed record Form
{
	private const int BluesBars = 12;

	private const int AabaSectionBars = 8;

	private static readonly int[][] BluesTemplates =
	[
		[1, 1, 1, 1, 4, 4, 1, 1, 5, 4, 1, 1],
		[1, 1, 1, 1, 4, 4, 1, 1, 5, 4, 1, 5],
		[1, 4, 1, 1, 4, 4, 1, 1, 5, 4, 1, 1],
		[1, 4, 1, 1, 4, 4, 1, 1, 5, 4, 1, 5],
	];

	/// <summary>The number of structurally distinct sections a form can label, one per letter A-Z.</summary>
	public const int MaxDistinctSections = 26;

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
	/// <remarks>
	/// Each structurally distinct section takes the next letter from A to Z, so an arrangement can have at most
	/// <see cref="MaxDistinctSections"/> distinct sections. Beyond that there is no letter <see cref="Parse"/> accepts.
	/// </remarks>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="arrangement"/> is null.</exception>
	/// <exception cref="ArgumentException">
	/// Thrown when <paramref name="arrangement"/> has more than <see cref="MaxDistinctSections"/> structurally distinct sections.
	/// </exception>
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
				if (representatives.Count == MaxDistinctSections)
				{
					throw new ArgumentException(
						$"A form can label at most {MaxDistinctSections} structurally distinct sections (A-Z).",
						nameof(arrangement));
				}

				representatives.Add(section);
				index = representatives.Count - 1;
			}

			letters.Add((char)('A' + index));
		}

		string pattern = new([.. letters]);
		return new() { Pattern = pattern, Letters = letters, Name = Recognize(arrangement, pattern) };
	}

	// Recognition in order of how much of the arrangement each rule reads: a blues section by its bars, verse-chorus
	// by section types, AABA by the pattern and the section lengths, and everything else by the pattern alone.
	private static NamedForm Recognize(Arrangement arrangement, string pattern)
	{
		IReadOnlyList<Section> sections = arrangement.Sections;
		if (sections.Any(section => IsTwelveBarBlues(section, section.Key ?? arrangement.Key)))
		{
			return NamedForm.TwelveBarBlues;
		}

		if (IsVerseChorus(sections))
		{
			return NamedForm.VerseChorus;
		}

		if (pattern == "AABA")
		{
			// A 32-bar song form is four eight-bar sections; four sections of another length share the letters
			// but not the form.
			return sections.All(section => section.Progression.TotalDuration == section.Progression.TimeSignature.BarDuration.Multiply(AabaSectionBars))
				? NamedForm.ThirtyTwoBarAABA
				: NamedForm.Unknown;
		}

		return RecognizePattern(pattern);
	}

	// Verse-chorus form: at least two verses and two choruses, and every verse is answered by a chorus before the next
	// verse or the end. Intros, pre- and post-choruses, bridges, solos, interludes, outros and codas may sit anywhere
	// around them; a refrain or an untyped section means the piece is something else.
	private static bool IsVerseChorus(IReadOnlyList<Section> sections)
	{
		int verses = 0;
		int choruses = 0;
		bool awaitingChorus = false;
		foreach (Section section in sections)
		{
			switch (section.Type)
			{
				case SectionType.Verse:
					if (awaitingChorus)
					{
						return false;
					}

					verses++;
					awaitingChorus = true;
					break;
				case SectionType.Chorus:
					choruses++;
					awaitingChorus = false;
					break;
				case SectionType.Refrain:
				case SectionType.Other:
					return false;
				default:
					break;
			}
		}

		return !awaitingChorus && verses >= 2 && choruses >= 2;
	}

	/// <summary>Parses a form from a letter-pattern string, optionally followed by a named form in parentheses.</summary>
	/// <remarks>
	/// A bare pattern is named from its letters alone: <c>"AABA"</c> is taken as the 32-bar form, since the letters are
	/// all there is to go on. <see cref="NamedForm.TwelveBarBlues"/> and <see cref="NamedForm.VerseChorus"/> are never
	/// recognized from letters; they come back only when the text names them, as <see cref="ToString"/> writes it.
	/// </remarks>
	/// <param name="pattern">A pattern of upper-case letters A-Z (e.g. "ABACA"), or a pattern and name (e.g. "A (TwelveBarBlues)").</param>
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
		if (pattern is null)
		{
			return false;
		}

		string letters = pattern;
		NamedForm? name = null;
		int space = pattern.IndexOf(' ');
		if (space >= 0)
		{
			if (!TryReadName(pattern[(space + 1)..], out NamedForm named))
			{
				return false;
			}

			letters = pattern[..space];
			name = named;
		}

		if (letters.Length == 0 || !letters.All(c => c is >= 'A' and <= 'Z'))
		{
			return false;
		}

		result = new() { Pattern = letters, Letters = [.. letters], Name = name ?? RecognizePattern(letters) };
		return true;
	}

	// Reads "(Name)", where Name is a NamedForm member spelled exactly; Enum.TryParse alone would also take a number
	// or a comma-separated list.
	private static bool TryReadName(string text, out NamedForm name)
	{
		name = default;
		return text.Length > 2
			&& text[0] == '('
			&& text[^1] == ')'
			&& text[1..^1].All(char.IsLetter)
			&& Enum.TryParse(text[1..^1], ignoreCase: false, out name)
			&& Enum.IsDefined(name);
	}

	/// <summary>
	/// Returns the letter-pattern (e.g. "AABA"), followed by the name in parentheses when it is not the one the pattern
	/// implies (e.g. "A (TwelveBarBlues)").
	/// </summary>
	/// <returns>The canonical form text.</returns>
	public override string ToString() =>
		Name is { } name && name != RecognizePattern(Pattern)
			? $"{Pattern} ({name})"
			: Pattern;

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

	// A twelve-bar blues is judged bar by bar, not chord by chord: the section must last exactly twelve bars, and each
	// bar is represented by the chord sounding on its downbeat. A chord held across several bars counts for each of
	// them, and a chord that starts after a downbeat does not count for that bar.
	private static bool IsTwelveBarBlues(Section section, Key key)
	{
		Progression progression = section.Progression;
		Duration bar = progression.TimeSignature.BarDuration;
		if (progression.TotalDuration != bar.Multiply(BluesBars))
		{
			return false;
		}

		int[] degrees = new int[BluesBars];
		int barIndex = 0;
		Duration end = Duration.Create(0, 1);
		foreach (ChordEvent chordEvent in progression.Chords)
		{
			end = end.Add(chordEvent.Duration);

			// Every downbeat before this chord ends that an earlier chord has not already claimed falls within this chord.
			while (barIndex < BluesBars && IsBefore(bar.Multiply(barIndex), end))
			{
				ScaleDegree degree = key.FunctionOf(chordEvent.Chord.Root);
				if (degree.Alteration != 0)
				{
					return false;
				}

				degrees[barIndex++] = degree.Degree;
			}
		}

		return BluesTemplates.Any(template => template.SequenceEqual(degrees));
	}

	private static bool IsBefore(Duration left, Duration right) =>
		(long)left.Numerator * right.Denominator < (long)right.Numerator * left.Denominator;
}
