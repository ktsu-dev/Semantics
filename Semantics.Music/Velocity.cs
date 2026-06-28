// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;

/// <summary>
/// A MIDI note velocity (0..127), modelling dynamic intensity.
/// </summary>
public sealed record Velocity
{
	/// <summary>Gets the velocity value, 0 (silent) through 127 (loudest).</summary>
	public int Value { get; private init; } = 64;

	/// <summary>Pianissimo (very soft).</summary>
	public static Velocity Pianissimo => Create(24);

	/// <summary>Piano (soft).</summary>
	public static Velocity Piano => Create(48);

	/// <summary>Mezzo-piano (medium soft).</summary>
	public static Velocity MezzoPiano => Create(64);

	/// <summary>Mezzo-forte (medium loud).</summary>
	public static Velocity MezzoForte => Create(80);

	/// <summary>Forte (loud).</summary>
	public static Velocity Forte => Create(96);

	/// <summary>Fortissimo (very loud).</summary>
	public static Velocity Fortissimo => Create(112);

	/// <summary>Creates a velocity.</summary>
	/// <param name="value">The velocity value, 0..127.</param>
	/// <returns>A new velocity.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is outside 0..127.</exception>
	public static Velocity Create(int value)
	{
		if (value is < 0 or > 127)
		{
			throw new ArgumentOutOfRangeException(nameof(value), value, "Velocity must be in 0..127.");
		}

		return new() { Value = value };
	}
}
