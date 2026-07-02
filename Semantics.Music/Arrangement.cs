// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

/// <summary>The ordered realization of a piece: a sequence of sections in performance order.</summary>
public sealed record Arrangement
{
	/// <summary>Gets the home key used for whole-piece analysis; a section may override it locally.</summary>
	public Key Key { get; private init; } = Key.Create(PitchClass.Create(0), Mode.Major);

	/// <summary>Gets the sections in performance order; repetition is expressed by repeated entries.</summary>
	public IReadOnlyList<Section> Sections { get; private init; } = [];

	/// <summary>Determines structural equality: the home key and ordered sections all match.</summary>
	/// <param name="other">The arrangement to compare.</param>
	/// <returns><see langword="true"/> when the arrangements are value-equal.</returns>
	public bool Equals(Arrangement? other) =>
		other is not null
		&& Key == other.Key
		&& Sections.SequenceEqual(other.Sections);

	/// <summary>Returns a hash code consistent with <see cref="Equals(Arrangement)"/>.</summary>
	/// <returns>The hash code.</returns>
	public override int GetHashCode() => HashCode.Combine(Key, Sections.Count);

	/// <summary>Gets the total length of the arrangement in bars.</summary>
	public double TotalBars => Sections.Sum(section => section.Bars);

	/// <summary>Gets the structural form of the arrangement.</summary>
	public Form Form => Form.Of(this);

	/// <summary>Creates an arrangement.</summary>
	/// <param name="key">The home key.</param>
	/// <param name="sections">The sections in performance order; must be non-empty.</param>
	/// <returns>A new arrangement.</returns>
	/// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="sections"/> is empty.</exception>
	public static Arrangement Create(Key key, IEnumerable<Section> sections)
	{
		Ensure.NotNull(key);
		Ensure.NotNull(sections);
		List<Section> list = [.. sections];
		if (list.Count == 0)
		{
			throw new ArgumentException("An arrangement must contain at least one section.", nameof(sections));
		}

		return new() { Key = key, Sections = list };
	}

	/// <summary>Returns the chart: a home-key line then blank-line-separated section blocks.</summary>
	/// <returns>The canonical arrangement text.</returns>
	public override string ToString()
	{
		StringBuilder sb = new();
		_ = sb.Append(Key);
		foreach (Section section in Sections)
		{
			_ = sb.Append("\n\n").Append(section);
		}

		return sb.ToString();
	}

	/// <summary>Parses a chart-style arrangement.</summary>
	/// <param name="text">The arrangement text.</param>
	/// <returns>The parsed arrangement.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static Arrangement Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Arrangement? result)
			? result
			: throw new FormatException($"Invalid arrangement '{text}'.");
	}

	/// <summary>Tries to parse a chart-style arrangement.</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed arrangement, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [NotNullWhen(true)] out Arrangement? result)
	{
		result = null;
		if (text is null)
		{
			return false;
		}

		string[] blocks = text.Replace("\r", string.Empty).Split(["\n\n"], System.StringSplitOptions.RemoveEmptyEntries);
		if (blocks.Length < 2 || !Key.TryParse(blocks[0].Trim(), out Key? key))
		{
			return false;
		}

		List<Section> sections = [];
		for (int i = 1; i < blocks.Length; i++)
		{
			if (!Section.TryParse(blocks[i], out Section? section))
			{
				return false;
			}

			sections.Add(section);
		}

		result = Create(key, sections);
		return true;
	}
}
