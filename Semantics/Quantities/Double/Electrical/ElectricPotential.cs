// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an electric potential quantity with double precision.
/// </summary>
public sealed record ElectricPotential
{
	/// <summary>Gets the underlying generic electric potential instance.</summary>
	public Generic.ElectricPotential<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricPotential"/> class.
	/// </summary>
	public ElectricPotential() { }

	/// <summary>
	/// Creates a new ElectricPotential from a value in volts.
	/// </summary>
	/// <param name="volts">The value in volts.</param>
	/// <returns>A new ElectricPotential instance.</returns>
	public static ElectricPotential FromVolts(double volts) => new() { Value = Generic.ElectricPotential<double>.FromVolts(volts) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
