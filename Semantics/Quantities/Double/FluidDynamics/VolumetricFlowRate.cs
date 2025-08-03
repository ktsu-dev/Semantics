// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a volumetric flow rate quantity with double precision.
/// </summary>
public sealed record VolumetricFlowRate
{
	/// <summary>Gets the underlying generic volumetric flow rate instance.</summary>
	public Generic.VolumetricFlowRate<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="VolumetricFlowRate"/> class.
	/// </summary>
	public VolumetricFlowRate() { }

	/// <summary>
	/// Creates a new VolumetricFlowRate from a value in cubic meters per second.
	/// </summary>
	/// <param name="cubicMetersPerSecond">The value in cubic meters per second.</param>
	/// <returns>A new VolumetricFlowRate instance.</returns>
	public static VolumetricFlowRate FromCubicMetersPerSecond(double cubicMetersPerSecond) => new() { Value = Generic.VolumetricFlowRate<double>.FromCubicMetersPerSecond(cubicMetersPerSecond) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
