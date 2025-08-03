// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a loudness quantity with float precision.
/// </summary>
public sealed record Loudness
{
	/// <summary>Gets the underlying generic loudness instance.</summary>
	public Generic.Loudness<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="Loudness"/> class.
	/// </summary>
	public Loudness() { }

	/// <summary>
	/// Creates a new Loudness from a value in sones.
	/// </summary>
	/// <param name="sones">The loudness in sones.</param>
	/// <returns>A new Loudness instance.</returns>
	public static new Loudness FromSones(float sones) => new() { Value = Generic.Loudness<float>.FromSones(sones) };

	/// <summary>
	/// Creates a new Loudness from a value in phons (loudness level).
	/// </summary>
	/// <param name="phons">The loudness level in phons.</param>
	/// <returns>A new Loudness instance.</returns>
	public static new Loudness FromPhons(float phons) => new() { Value = Generic.Loudness<float>.FromPhons(phons) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
