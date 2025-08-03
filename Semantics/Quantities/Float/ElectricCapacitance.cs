// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an electric capacitance quantity with float precision.
/// </summary>
public sealed record ElectricCapacitance : Generic.ElectricCapacitance<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricCapacitance"/> class.
	/// </summary>
	public ElectricCapacitance() : base() { }

	/// <summary>
	/// Creates a new ElectricCapacitance from a value in farads.
	/// </summary>
	/// <param name="farads">The value in farads.</param>
	/// <returns>A new ElectricCapacitance instance.</returns>
	public static new ElectricCapacitance FromFarads(float farads) => new() { Value = farads };
}
