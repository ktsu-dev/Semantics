// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a velocity quantity with float precision.
/// </summary>
public sealed record Velocity : Generic.Velocity<float>
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
	public static new Velocity FromMetersPerSecond(float metersPerSecond) => new() { Quantity = metersPerSecond };

}
