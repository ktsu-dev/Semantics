// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a momentum quantity with float precision.
/// </summary>
public sealed record Momentum
{
	/// <summary>Gets the underlying generic momentum instance.</summary>
	public Generic.Momentum<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="Momentum"/> class.
	/// </summary>
	public Momentum() { }

	/// <summary>
	/// Creates a new Momentum from a value in kilogram-meters per second.
	/// </summary>
	/// <param name="kilogramMetersPerSecond">The value in kilogram-meters per second.</param>
	/// <returns>A new Momentum instance.</returns>
	public static Momentum FromKilogramMetersPerSecond(float kilogramMetersPerSecond) => new() { Value = Generic.Momentum<float>.FromKilogramMetersPerSecond(kilogramMetersPerSecond) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
