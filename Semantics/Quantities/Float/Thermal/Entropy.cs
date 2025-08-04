// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an entropy quantity with float precision.
/// </summary>
public sealed record Entropy : Generic.Entropy<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Entropy"/> class.
	/// </summary>
	public Entropy() : base() { }

	/// <summary>
	/// Creates a new Entropy from a value in joules per kelvin.
	/// </summary>
	/// <param name="joulesPerKelvin">The value in joules per kelvin.</param>
	/// <returns>A new Entropy instance.</returns>
	public static new Entropy FromJoulesPerKelvin(float joulesPerKelvin) => new() { Quantity = joulesPerKelvin };
}
