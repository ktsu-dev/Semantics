// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a sound power level quantity with double precision.
/// </summary>
public sealed record SoundPowerLevel
{
	/// <summary>Gets the underlying generic sound power level instance.</summary>
	public Generic.SoundPowerLevel<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="SoundPowerLevel"/> class.
	/// </summary>
	public SoundPowerLevel() { }

	/// <summary>
	/// Creates a new SoundPowerLevel from a value in decibels PWL.
	/// </summary>
	/// <param name="decibels">The sound power level in dB PWL.</param>
	/// <returns>A new SoundPowerLevel instance.</returns>
	public static SoundPowerLevel FromDecibels(double decibels) => new() { Value = Generic.SoundPowerLevel<double>.FromDecibels(decibels) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
