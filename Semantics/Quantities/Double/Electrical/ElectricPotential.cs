// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an electric potential quantity with double precision.
/// </summary>
public sealed record ElectricPotential : Generic.ElectricPotential<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricPotential"/> class.
	/// </summary>
	public ElectricPotential() : base() { }

	/// <summary>
	/// Creates a new ElectricPotential from a value in volts.
	/// </summary>
	/// <param name="volts">The value in volts.</param>
	/// <returns>A new ElectricPotential instance.</returns>
	public static new ElectricPotential FromVolts(double volts) => new() { Quantity = volts };
}
