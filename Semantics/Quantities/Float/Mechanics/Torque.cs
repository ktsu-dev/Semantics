// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a torque quantity with float precision.
/// </summary>
public sealed record Torque : Generic.Torque<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Torque"/> class.
	/// </summary>
	public Torque() : base() { }

	/// <summary>
	/// Creates a new Torque from a value in newton-meters.
	/// </summary>
	/// <param name="newtonMeters">The value in newton-meters.</param>
	/// <returns>A new Torque instance.</returns>
	public static new Torque FromNewtonMeters(float newtonMeters) => new() { Quantity = newtonMeters };
}
