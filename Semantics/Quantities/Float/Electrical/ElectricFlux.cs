// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an electric flux quantity with float precision.
/// </summary>
public sealed record ElectricFlux : Generic.ElectricFlux<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricFlux"/> class.
	/// </summary>
	public ElectricFlux() : base() { }

	/// <summary>
	/// Creates a new ElectricFlux from a value in volt-meters.
	/// </summary>
	/// <param name="voltMeters">The value in volt-meters.</param>
	/// <returns>A new ElectricFlux instance.</returns>
	public static new ElectricFlux FromVoltMeters(float voltMeters) => new() { Value = voltMeters };
}
