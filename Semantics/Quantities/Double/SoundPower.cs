// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a sound power quantity with double precision.
/// </summary>
public sealed record SoundPower
{
	/// <summary>Gets the underlying generic sound power instance.</summary>
	public Generic.SoundPower<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="SoundPower"/> class.
	/// </summary>
	public SoundPower() { }

	/// <summary>
	/// Creates a new SoundPower from a value in watts.
	/// </summary>
	/// <param name="watts">The value in watts.</param>
	/// <returns>A new SoundPower instance.</returns>
	public static SoundPower FromWatts(double watts) => new() { Value = Generic.SoundPower<double>.FromWatts(watts) };

	/// <summary>
	/// Creates a new SoundPower from a value in milliwatts.
	/// </summary>
	/// <param name="milliwatts">The value in milliwatts.</param>
	/// <returns>A new SoundPower instance.</returns>
	public static SoundPower FromMilliwatts(double milliwatts) => new() { Value = Generic.SoundPower<double>.FromMilliwatts(milliwatts) };

	/// <summary>
	/// Creates a new SoundPower from a value in acoustic watts.
	/// </summary>
	/// <param name="acousticWatts">The value in acoustic watts.</param>
	/// <returns>A new SoundPower instance.</returns>
	public static SoundPower FromAcousticWatts(double acousticWatts) => new() { Value = Generic.SoundPower<double>.FromAcousticWatts(acousticWatts) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
