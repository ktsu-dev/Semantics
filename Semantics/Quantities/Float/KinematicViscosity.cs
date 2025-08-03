// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a kinematic viscosity quantity with float precision.
/// </summary>
public sealed record KinematicViscosity
{
	/// <summary>Gets the underlying generic kinematic viscosity instance.</summary>
	public Generic.KinematicViscosity<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="KinematicViscosity"/> class.
	/// </summary>
	public KinematicViscosity() { }

	/// <summary>
	/// Creates a new KinematicViscosity from a value in square meters per second.
	/// </summary>
	/// <param name="squareMetersPerSecond">The value in square meters per second.</param>
	/// <returns>A new KinematicViscosity instance.</returns>
	public static new KinematicViscosity FromSquareMetersPerSecond(float squareMetersPerSecond) => new() { Value = Generic.KinematicViscosity<float>.FromSquareMetersPerSecond(squareMetersPerSecond) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
