// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;

/// <summary>
/// A musical time signature, with bar and beat lengths in whole-note fractions.
/// </summary>
public sealed record TimeSignature
{
	/// <summary>Gets the number of beats per bar (the upper number).</summary>
	public int Beats { get; private init; } = 4;

	/// <summary>Gets the beat unit (the lower number): 4 = quarter, 8 = eighth.</summary>
	public int BeatUnit { get; private init; } = 4;

	/// <summary>Gets the length of one bar as a fraction of a whole note.</summary>
	public Duration BarDuration => Duration.Create(Beats, BeatUnit);

	/// <summary>Gets the length of one beat as a fraction of a whole note.</summary>
	public Duration BeatDuration => Duration.Create(1, BeatUnit);

	/// <summary>Creates a time signature.</summary>
	/// <param name="beats">Beats per bar; must be positive.</param>
	/// <param name="beatUnit">The beat unit (denominator); must be positive.</param>
	/// <returns>A new time signature.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when either argument is not positive.</exception>
	public static TimeSignature Create(int beats, int beatUnit)
	{
		if (beats <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(beats), "Beats must be positive.");
		}

		if (beatUnit <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(beatUnit), "Beat unit must be positive.");
		}

		return new() { Beats = beats, BeatUnit = beatUnit };
	}
}
