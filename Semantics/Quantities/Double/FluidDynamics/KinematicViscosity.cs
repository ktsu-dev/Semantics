// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a kinematic viscosity quantity with double precision.
/// </summary>
public sealed record KinematicViscosity
{
	/// <summary>Gets the underlying generic kinematic viscosity instance.</summary>
	public Generic.KinematicViscosity<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="KinematicViscosity"/> class.
	/// </summary>
	public KinematicViscosity() { }

	/// <summary>
	/// Creates a new KinematicViscosity from a value in square meters per second.
	/// </summary>
	/// <param name="squareMetersPerSecond">The value in square meters per second.</param>
	/// <returns>A new KinematicViscosity instance.</returns>
	public static KinematicViscosity FromSquareMetersPerSecond(double squareMetersPerSecond) => new() { Value = Generic.KinematicViscosity<double>.FromSquareMetersPerSecond(squareMetersPerSecond) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
