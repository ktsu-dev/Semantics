// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a specific heat quantity with float precision.
/// </summary>
public sealed record SpecificHeat
{
	/// <summary>Gets the underlying generic specific heat instance.</summary>
	public Generic.SpecificHeat<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="SpecificHeat"/> class.
	/// </summary>
	public SpecificHeat() { }

	/// <summary>
	/// Creates a new SpecificHeat from a value in joules per kilogram-kelvin.
	/// </summary>
	/// <param name="joulesPerKilogramKelvin">The value in joules per kilogram-kelvin.</param>
	/// <returns>A new SpecificHeat instance.</returns>
	public static new SpecificHeat FromJoulesPerKilogramKelvin(float joulesPerKilogramKelvin) => new() { Value = Generic.SpecificHeat<float>.FromJoulesPerKilogramKelvin(joulesPerKilogramKelvin) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
