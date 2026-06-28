// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;

/// <summary>
/// Represents a melodic/harmonic interval measured in signed semitones.
/// </summary>
public sealed record Interval
{
	/// <summary>Gets the signed interval size in semitones (positive ascending, negative descending).</summary>
	public int Semitones { get; private init; }

	/// <summary>Gets the direction of the interval: +1 ascending, -1 descending, 0 unison.</summary>
	public int Direction => Math.Sign(Semitones);

	/// <summary>Gets the interval reduced to within one octave, preserving direction.</summary>
	public Interval Folded => Create(Direction * (Math.Abs(Semitones) % 12));

	/// <summary>Gets the interval size in cents (100 cents per equal-tempered semitone).</summary>
	public double Cents => Semitones * 100.0;

	/// <summary>Creates an interval of the given signed semitone size.</summary>
	/// <param name="semitones">The signed semitone size.</param>
	/// <returns>A new interval.</returns>
	public static Interval Create(int semitones) => new() { Semitones = semitones };

	/// <summary>Creates the interval that moves from one pitch to another.</summary>
	/// <param name="from">The starting pitch.</param>
	/// <param name="to">The ending pitch.</param>
	/// <returns>The signed interval between them.</returns>
	/// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
	public static Interval Between(Pitch from, Pitch to)
	{
		Ensure.NotNull(from);
		Ensure.NotNull(to);
		return Create(to.Midi - from.Midi);
	}
}
