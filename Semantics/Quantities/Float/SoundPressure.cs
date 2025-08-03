// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a sound pressure quantity with float precision.
/// </summary>
public sealed record SoundPressure
{
	/// <summary>Gets the underlying generic sound pressure instance.</summary>
	public Generic.SoundPressure<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="SoundPressure"/> class.
	/// </summary>
	public SoundPressure() { }

	/// <summary>
	/// Creates a new SoundPressure from a value in pascals.
	/// </summary>
	/// <param name="pascals">The value in pascals.</param>
	/// <returns>A new SoundPressure instance.</returns>
	public static new SoundPressure FromPascals(float pascals) => new() { Value = Generic.SoundPressure<float>.FromPascals(pascals) };

	/// <summary>
	/// Creates a new SoundPressure from a value in micropascals.
	/// </summary>
	/// <param name="micropascals">The value in micropascals.</param>
	/// <returns>A new SoundPressure instance.</returns>
	public static new SoundPressure FromMicropascals(float micropascals) => new() { Value = Generic.SoundPressure<float>.FromMicropascals(micropascals) };

	/// <summary>
	/// Creates a new SoundPressure from a value in bars.
	/// </summary>
	/// <param name="bars">The value in bars.</param>
	/// <returns>A new SoundPressure instance.</returns>
	public static new SoundPressure FromBars(float bars) => new() { Value = Generic.SoundPressure<float>.FromBars(bars) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
