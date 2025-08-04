// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a heat capacity quantity with double precision.
/// </summary>
public sealed record HeatCapacity : Generic.HeatCapacity<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="HeatCapacity"/> class.
	/// </summary>
	public HeatCapacity() : base() { }

	/// <summary>
	/// Creates a new HeatCapacity from a value in joules per kelvin.
	/// </summary>
	/// <param name="joulesPerKelvin">The value in joules per kelvin.</param>
	/// <returns>A new HeatCapacity instance.</returns>
	public static new HeatCapacity FromJoulesPerKelvin(double joulesPerKelvin) => new() { Quantity = joulesPerKelvin };
}
