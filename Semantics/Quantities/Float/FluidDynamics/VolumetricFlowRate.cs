// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a volumetric flow rate quantity with float precision.
/// </summary>
public sealed record VolumetricFlowRate : Generic.VolumetricFlowRate<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="VolumetricFlowRate"/> class.
	/// </summary>
	public VolumetricFlowRate() : base() { }

	/// <summary>
	/// Creates a new VolumetricFlowRate from a value in cubic meters per second.
	/// </summary>
	/// <param name="cubicMetersPerSecond">The value in cubic meters per second.</param>
	/// <returns>A new VolumetricFlowRate instance.</returns>
	public static new VolumetricFlowRate FromCubicMetersPerSecond(float cubicMetersPerSecond) => new() { Value = cubicMetersPerSecond };
}
