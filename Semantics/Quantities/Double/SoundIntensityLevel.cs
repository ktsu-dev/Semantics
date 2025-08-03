// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a sound intensity level quantity with double precision.
/// </summary>
public sealed record SoundIntensityLevel
{
	/// <summary>Gets the underlying generic sound intensity level instance.</summary>
	public Generic.SoundIntensityLevel<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="SoundIntensityLevel"/> class.
	/// </summary>
	public SoundIntensityLevel() { }

	/// <summary>
	/// Creates a new SoundIntensityLevel from a value in decibels IL.
	/// </summary>
	/// <param name="decibels">The sound intensity level in dB IL.</param>
	/// <returns>A new SoundIntensityLevel instance.</returns>
	public static SoundIntensityLevel FromDecibels(double decibels) => new() { Value = Generic.SoundIntensityLevel<double>.FromDecibels(decibels) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
