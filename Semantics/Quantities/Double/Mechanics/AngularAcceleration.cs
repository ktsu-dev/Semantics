// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an angular acceleration quantity with double precision.
/// </summary>
public sealed record AngularAcceleration : Generic.AngularAcceleration<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AngularAcceleration"/> class.
	/// </summary>
	public AngularAcceleration() : base() { }

	/// <summary>
	/// Creates a new AngularAcceleration from a value in radians per second squared.
	/// </summary>
	/// <param name="radiansPerSecondSquared">The value in radians per second squared.</param>
	/// <returns>A new AngularAcceleration instance.</returns>
	public static new AngularAcceleration FromRadiansPerSecondSquared(double radiansPerSecondSquared) => new() { Quantity = radiansPerSecondSquared };
}
