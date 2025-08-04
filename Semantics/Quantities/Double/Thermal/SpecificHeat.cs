// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a specific heat quantity with double precision.
/// </summary>
public sealed record SpecificHeat : Generic.SpecificHeat<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SpecificHeat"/> class.
	/// </summary>
	public SpecificHeat() : base() { }

	/// <summary>
	/// Creates a new SpecificHeat from a value in joules per kilogram-kelvin.
	/// </summary>
	/// <param name="joulesPerKilogramKelvin">The value in joules per kilogram-kelvin.</param>
	/// <returns>A new SpecificHeat instance.</returns>
	public static new SpecificHeat FromJoulesPerKilogramKelvin(double joulesPerKilogramKelvin) => new() { Quantity = joulesPerKilogramKelvin };
}
