// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an electric flux quantity with double precision.
/// </summary>
public sealed record ElectricFlux : Generic.ElectricFlux<double>
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
	public static new ElectricFlux FromVoltMeters(double voltMeters) => new() { Value = voltMeters };
}
