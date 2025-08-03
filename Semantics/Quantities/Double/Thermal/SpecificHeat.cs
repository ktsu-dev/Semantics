// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a specific heat quantity with double precision.
/// </summary>
public sealed record SpecificHeat
{
	/// <summary>Gets the underlying generic specific heat instance.</summary>
	public Generic.SpecificHeat<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="SpecificHeat"/> class.
	/// </summary>
	public SpecificHeat() { }

	/// <summary>
	/// Creates a new SpecificHeat from a value in joules per kilogram-kelvin.
	/// </summary>
	/// <param name="joulesPerKilogramKelvin">The value in joules per kilogram-kelvin.</param>
	/// <returns>A new SpecificHeat instance.</returns>
	public static SpecificHeat FromJoulesPerKilogramKelvin(double joulesPerKilogramKelvin) => new() { Value = Generic.SpecificHeat<double>.FromJoulesPerKilogramKelvin(joulesPerKilogramKelvin) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
