// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a kinematic viscosity quantity with double precision.
/// </summary>
public sealed record KinematicViscosity : Generic.KinematicViscosity<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="KinematicViscosity"/> class.
	/// </summary>
	public KinematicViscosity() : base() { }

	/// <summary>
	/// Creates a new KinematicViscosity from a value in square meters per second.
	/// </summary>
	/// <param name="squareMetersPerSecond">The value in square meters per second.</param>
	/// <returns>A new KinematicViscosity instance.</returns>
	public static new KinematicViscosity FromSquareMetersPerSecond(double squareMetersPerSecond) => new() { Value = squareMetersPerSecond };
}
