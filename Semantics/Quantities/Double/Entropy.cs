// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an entropy quantity with double precision.
/// </summary>
public sealed record Entropy
{
	/// <summary>Gets the underlying generic entropy instance.</summary>
	public Generic.Entropy<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="Entropy"/> class.
	/// </summary>
	public Entropy() { }

	/// <summary>
	/// Creates a new Entropy from a value in joules per kelvin.
	/// </summary>
	/// <param name="joulesPerKelvin">The value in joules per kelvin.</param>
	/// <returns>A new Entropy instance.</returns>
	public static Entropy FromJoulesPerKelvin(double joulesPerKelvin) => new() { Value = Generic.Entropy<double>.FromJoulesPerKelvin(joulesPerKelvin) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
