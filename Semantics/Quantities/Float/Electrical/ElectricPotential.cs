// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an electric potential quantity with float precision.
/// </summary>
public sealed record ElectricPotential : Generic.ElectricPotential<float>
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
	public static new ElectricPotential FromVolts(float volts) => new() { Value = volts };
}
