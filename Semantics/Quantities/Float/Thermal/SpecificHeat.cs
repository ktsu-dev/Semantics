// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a specific heat quantity with float precision.
/// </summary>
public sealed record SpecificHeat : Generic.SpecificHeat<float>
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
	public static new SpecificHeat FromJoulesPerKilogramKelvin(float joulesPerKilogramKelvin) => new() { Quantity = joulesPerKilogramKelvin };
}
