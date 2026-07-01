// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;

/// <summary>A cadence located within a progression at the position of its resolution chord.</summary>
public sealed record CadenceInstance
{
	/// <summary>Gets the index of the resolution (second) chord of the cadence.</summary>
	public int Index { get; private init; }

	/// <summary>Gets the cadence type.</summary>
	public Cadence Type { get; private init; }

	/// <summary>Creates a cadence instance.</summary>
	/// <param name="index">The zero-based index of the resolution chord; must be non-negative.</param>
	/// <param name="type">The cadence type.</param>
	/// <returns>A new cadence instance.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is negative.</exception>
	public static CadenceInstance Create(int index, Cadence type)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be non-negative.");
		}

		return new() { Index = index, Type = type };
	}
}
