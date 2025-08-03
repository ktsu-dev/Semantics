// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a sound pressure level quantity with float precision.
/// </summary>
public sealed record SoundPressureLevel : Generic.SoundPressureLevel<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SoundPressureLevel"/> class.
	/// </summary>
	public SoundPressureLevel() : base() { }

	/// <summary>
	/// Creates a new SoundPressureLevel from a value in decibels SPL.
	/// </summary>
	/// <param name="decibels">The sound pressure level in dB SPL.</param>
	/// <returns>A new SoundPressureLevel instance.</returns>
	public static new SoundPressureLevel FromDecibels(float decibels) => new() { Value = decibels };
}
