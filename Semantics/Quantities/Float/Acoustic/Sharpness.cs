// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a sharpness quantity with float precision.
/// </summary>
public sealed record Sharpness
{
	/// <summary>Gets the underlying generic sharpness instance.</summary>
	public Generic.Sharpness<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="Sharpness"/> class.
	/// </summary>
	public Sharpness() { }

	/// <summary>
	/// Creates a new Sharpness from a value in acums.
	/// </summary>
	/// <param name="acums">The sharpness in acums.</param>
	/// <returns>A new Sharpness instance.</returns>
	public static Sharpness FromAcums(float acums) => new() { Value = Generic.Sharpness<float>.FromAcums(acums) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
