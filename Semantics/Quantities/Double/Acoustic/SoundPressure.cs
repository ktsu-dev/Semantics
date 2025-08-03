// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a sound pressure quantity with double precision.
/// </summary>
public sealed record SoundPressure : Generic.SoundPressure<double>
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
	public static new SoundPressure FromPascals(double pascals) => new() { Value = pascals };

	/// <summary>
	/// Creates a new SoundPressure from a value in micropascals.
	/// </summary>
	/// <param name="micropascals">The value in micropascals.</param>
	/// <returns>A new SoundPressure instance.</returns>
	public static new SoundPressure FromMicropascals(double micropascals) => new() { Value = micropascals };

	/// <summary>
	/// Creates a new SoundPressure from a value in bars.
	/// </summary>
	/// <param name="bars">The value in bars.</param>
	/// <returns>A new SoundPressure instance.</returns>
	public static new SoundPressure FromBars(double bars) => new() { Value = bars };
}
