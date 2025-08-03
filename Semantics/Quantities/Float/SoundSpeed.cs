// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a sound speed quantity with float precision.
/// </summary>
public sealed record SoundSpeed
{
	/// <summary>Gets the underlying generic sound speed instance.</summary>
	public Generic.SoundSpeed<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="SoundSpeed"/> class.
	/// </summary>
	public SoundSpeed() { }

	/// <summary>
	/// Creates a new SoundSpeed from a value in meters per second.
	/// </summary>
	/// <param name="metersPerSecond">The value in meters per second.</param>
	/// <returns>A new SoundSpeed instance.</returns>
	public static new SoundSpeed FromMetersPerSecond(float metersPerSecond) => new() { Value = Generic.SoundSpeed<float>.FromMetersPerSecond(metersPerSecond) };

	/// <summary>
	/// Creates a new SoundSpeed from a value in feet per second.
	/// </summary>
	/// <param name="feetPerSecond">The value in feet per second.</param>
	/// <returns>A new SoundSpeed instance.</returns>
	public static new SoundSpeed FromFeetPerSecond(float feetPerSecond) => new() { Value = Generic.SoundSpeed<float>.FromFeetPerSecond(feetPerSecond) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
