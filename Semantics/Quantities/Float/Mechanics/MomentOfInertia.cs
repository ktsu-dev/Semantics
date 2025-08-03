// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a moment of inertia quantity with float precision.
/// </summary>
public sealed record MomentOfInertia : Generic.MomentOfInertia<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MomentOfInertia"/> class.
	/// </summary>
	public MomentOfInertia() : base() { }

	/// <summary>
	/// Creates a new MomentOfInertia from a value in kilogram-square meters.
	/// </summary>
	/// <param name="kilogramSquareMeters">The value in kilogram-square meters.</param>
	/// <returns>A new MomentOfInertia instance.</returns>
	public static new MomentOfInertia FromKilogramSquareMeters(float kilogramSquareMeters) => new() { Value = kilogramSquareMeters };
}
