// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an angular velocity quantity with float precision.
/// </summary>
public sealed record AngularVelocity : Generic.AngularVelocity<float>
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
	public static new AngularVelocity FromRadiansPerSecond(float radiansPerSecond) => new() { Quantity = radiansPerSecond };
}
