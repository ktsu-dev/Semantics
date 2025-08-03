// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a molar mass quantity with double precision.
/// </summary>
public sealed record MolarMass : Generic.MolarMass<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MolarMass"/> class.
	/// </summary>
	public MolarMass() : base() { }

	/// <summary>
	/// Creates a new MolarMass from a value in grams per mole.
	/// </summary>
	/// <param name="gramsPerMole">The value in grams per mole.</param>
	/// <returns>A new MolarMass instance.</returns>
	public static new MolarMass FromGramsPerMole(double gramsPerMole) => new() { Value = gramsPerMole };
}
