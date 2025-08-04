// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a concentration quantity with double precision.
/// </summary>
public sealed record Concentration : Generic.Concentration<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Concentration"/> class.
	/// </summary>
	public Concentration() : base() { }

	/// <summary>
	/// Creates a new Concentration from a value in molar (mol/L).
	/// </summary>
	/// <param name="molar">The value in molar.</param>
	/// <returns>A new Concentration instance.</returns>
	public static Concentration FromMolar(double molar) => new() { Quantity = molar };

	/// <summary>
	/// Creates a new Concentration from a molar concentration value.
	/// </summary>
	/// <param name="molarConcentration">The molar concentration value.</param>
	/// <returns>A new Concentration instance.</returns>
	public static Concentration FromMolarConcentration(double molarConcentration) => new() { Quantity = molarConcentration };
}
