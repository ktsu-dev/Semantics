// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a sound intensity level quantity with double precision.
/// </summary>
public sealed record SoundIntensityLevel : Generic.SoundIntensityLevel<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SoundIntensityLevel"/> class.
	/// </summary>
	public SoundIntensityLevel() : base() { }

	/// <summary>
	/// Creates a new SoundIntensityLevel from a value in decibels IL.
	/// </summary>
	/// <param name="decibels">The sound intensity level in dB IL.</param>
	/// <returns>A new SoundIntensityLevel instance.</returns>
	public static new SoundIntensityLevel FromDecibels(double decibels) => new() { Quantity = decibels };
}
