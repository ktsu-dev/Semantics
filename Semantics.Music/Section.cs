// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System.Collections.Generic;

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
}
