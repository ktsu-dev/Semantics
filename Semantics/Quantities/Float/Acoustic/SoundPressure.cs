// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a sound pressure quantity with float precision.
/// </summary>
public sealed record SoundPressure : Generic.SoundPressure<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SoundPressure"/> class.
	/// </summary>
	public SoundPressure() : base() { }

	/// <summary>
	/// Creates a new SoundPressure from a value in pascals.
	/// </summary>
	/// <param name="pascals">The value in pascals.</param>
	/// <returns>A new SoundPressure instance.</returns>
	public static new SoundPressure FromPascals(float pascals) => new() { Quantity = pascals };

	/// <summary>
	/// Creates a new SoundPressure from a value in micropascals.
	/// </summary>
	/// <param name="micropascals">The value in micropascals.</param>
	/// <returns>A new SoundPressure instance.</returns>
	public static new SoundPressure FromMicropascals(float micropascals) => new() { Quantity = micropascals };

	/// <summary>
	/// Creates a new SoundPressure from a value in bars.
	/// </summary>
	/// <param name="bars">The value in bars.</param>
	/// <returns>A new SoundPressure instance.</returns>
	public static new SoundPressure FromBars(float bars) => new() { Quantity = bars };
}
