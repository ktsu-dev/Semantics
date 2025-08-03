// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a sharpness quantity with float precision.
/// </summary>
public sealed record Sharpness : Generic.Sharpness<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Sharpness"/> class.
	/// </summary>
	public Sharpness() : base() { }

	/// <summary>
	/// Creates a new Sharpness from a value in acums.
	/// </summary>
	/// <param name="acums">The sharpness in acums.</param>
	/// <returns>A new Sharpness instance.</returns>
	public static new Sharpness FromAcums(float acums) => new() { Value = acums };
}
