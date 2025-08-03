// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a loudness quantity with float precision.
/// </summary>
public sealed record Loudness : Generic.Loudness<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Loudness"/> class.
	/// </summary>
	public Loudness() : base() { }

	/// <summary>
	/// Creates a new Loudness from a value in sones.
	/// </summary>
	/// <param name="sones">The loudness in sones.</param>
	/// <returns>A new Loudness instance.</returns>
	public static new Loudness FromSones(float sones) => new() { Value = sones };

	/// <summary>
	/// Creates a new Loudness from a value in phons (loudness level).
	/// </summary>
	/// <param name="phons">The loudness level in phons.</param>
	/// <returns>A new Loudness instance.</returns>
	public static new Loudness FromPhons(float phons) => new() { Value = phons };
}
