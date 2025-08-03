// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a torque quantity with double precision.
/// </summary>
public sealed record Torque : Generic.Torque<double>
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
	public static new Torque FromNewtonMeters(double newtonMeters) => new() { Value = newtonMeters };
}
