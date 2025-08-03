// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a moment of inertia quantity with float precision.
/// </summary>
public sealed record MomentOfInertia
{
	/// <summary>Gets the underlying generic moment of inertia instance.</summary>
	public Generic.MomentOfInertia<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="MomentOfInertia"/> class.
	/// </summary>
	public MomentOfInertia() { }

	/// <summary>
	/// Creates a new MomentOfInertia from a value in kilogram-square meters.
	/// </summary>
	/// <param name="kilogramSquareMeters">The value in kilogram-square meters.</param>
	/// <returns>A new MomentOfInertia instance.</returns>
	public static new MomentOfInertia FromKilogramSquareMeters(float kilogramSquareMeters) => new() { Value = Generic.MomentOfInertia<float>.FromKilogramSquareMeters(kilogramSquareMeters) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
