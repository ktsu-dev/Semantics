// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an acceleration quantity with float precision.
/// </summary>
public sealed record Acceleration : Generic.Acceleration<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Acceleration"/> class.
	/// </summary>
	public Acceleration() : base() { }

	/// <summary>
	/// Creates a new Acceleration from a value in meters per second squared.
	/// </summary>
	/// <param name="metersPerSecondSquared">The value in meters per second squared.</param>
	/// <returns>A new Acceleration instance.</returns>
	public static new Acceleration FromMetersPerSecondSquared(float metersPerSecondSquared) => new() { Value = metersPerSecondSquared };


}
