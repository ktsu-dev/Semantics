// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a molar mass quantity with float precision.
/// </summary>
public sealed record MolarMass : Generic.MolarMass<float>
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
	public static MolarMass FromGramsPerMole(float gramsPerMole) => new() { Quantity = gramsPerMole };
}
