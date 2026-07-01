// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;

/// <summary>The chromatic classification of a chord at a position within a progression.</summary>
public sealed record ChromaticAnalysis
{
	/// <summary>Gets the zero-based index of the chord within the progression.</summary>
	public int Index { get; private init; }

	/// <summary>Gets the chromatic category.</summary>
	public ChromaticKind Kind { get; private init; }

	/// <summary>Gets an optional human-readable detail (e.g. "V/V", "bII"), or null.</summary>
	public string? Detail { get; private init; }

	/// <summary>Creates a chromatic analysis result.</summary>
	/// <param name="index">The zero-based chord index; must be non-negative.</param>
	/// <param name="kind">The chromatic category.</param>
	/// <param name="detail">An optional descriptive detail.</param>
	/// <returns>A new chromatic analysis result.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is negative.</exception>
	public static ChromaticAnalysis Create(int index, ChromaticKind kind, string? detail)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be non-negative.");
		}

		return new() { Index = index, Kind = kind, Detail = detail };
	}
}
