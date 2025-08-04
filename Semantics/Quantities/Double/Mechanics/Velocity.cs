// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a velocity quantity with double precision.
/// </summary>
public sealed record Velocity : Generic.Velocity<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Velocity"/> class.
	/// </summary>
	public Velocity() : base() { }

	/// <summary>
	/// Creates a new Velocity from a value in meters per second.
	/// </summary>
	/// <param name="metersPerSecond">The value in meters per second.</param>
	/// <returns>A new Velocity instance.</returns>
	public static new Velocity FromMetersPerSecond(double metersPerSecond) => new() { Quantity = metersPerSecond };
}
