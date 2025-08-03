// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an electric capacitance quantity with double precision.
/// </summary>
public sealed record ElectricCapacitance
{
	/// <summary>Gets the underlying generic electric capacitance instance.</summary>
	public Generic.ElectricCapacitance<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricCapacitance"/> class.
	/// </summary>
	public ElectricCapacitance() { }

	/// <summary>
	/// Creates a new ElectricCapacitance from a value in farads.
	/// </summary>
	/// <param name="farads">The value in farads.</param>
	/// <returns>A new ElectricCapacitance instance.</returns>
	public static ElectricCapacitance FromFarads(double farads) => new() { Value = Generic.ElectricCapacitance<double>.FromFarads(farads) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
