// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a sound pressure level quantity with float precision.
/// </summary>
public sealed record SoundPressureLevel
{
	/// <summary>Gets the underlying generic sound pressure level instance.</summary>
	public Generic.SoundPressureLevel<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="SoundPressureLevel"/> class.
	/// </summary>
	public SoundPressureLevel() { }

	/// <summary>
	/// Creates a new SoundPressureLevel from a value in decibels SPL.
	/// </summary>
	/// <param name="decibels">The sound pressure level in dB SPL.</param>
	/// <returns>A new SoundPressureLevel instance.</returns>
	public static SoundPressureLevel FromDecibels(float decibels) => new() { Value = Generic.SoundPressureLevel<float>.FromDecibels(decibels) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
