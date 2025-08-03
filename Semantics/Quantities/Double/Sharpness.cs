// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a sharpness quantity with double precision.
/// </summary>
public sealed record Sharpness
{
	/// <summary>Gets the underlying generic sharpness instance.</summary>
	public Generic.Sharpness<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="Sharpness"/> class.
	/// </summary>
	public Sharpness() { }

	/// <summary>
	/// Creates a new Sharpness from a value in acums.
	/// </summary>
	/// <param name="acums">The sharpness in acums.</param>
	/// <returns>A new Sharpness instance.</returns>
	public static Sharpness FromAcums(double acums) => new() { Value = Generic.Sharpness<double>.FromAcums(acums) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
