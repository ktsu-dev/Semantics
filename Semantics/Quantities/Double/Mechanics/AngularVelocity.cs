// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an angular velocity quantity with double precision.
/// </summary>
public sealed record AngularVelocity : Generic.AngularVelocity<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AngularVelocity"/> class.
	/// </summary>
	public AngularVelocity() : base() { }

	/// <summary>
	/// Creates a new AngularVelocity from a value in radians per second.
	/// </summary>
	/// <param name="radiansPerSecond">The value in radians per second.</param>
	/// <returns>A new AngularVelocity instance.</returns>
	public static new AngularVelocity FromRadiansPerSecond(double radiansPerSecond) => new() { Value = radiansPerSecond };
}
