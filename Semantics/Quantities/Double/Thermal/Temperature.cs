// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a temperature quantity with double precision.
/// </summary>
public sealed record Temperature : Generic.Temperature<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Temperature"/> class.
	/// </summary>
	public Temperature() : base() { }

	/// <summary>
	/// Creates a new Temperature from a value in kelvin.
	/// </summary>
	/// <param name="kelvin">The value in kelvin.</param>
	/// <returns>A new Temperature instance.</returns>
	public static new Temperature FromKelvin(double kelvin) => new() { Value = kelvin };
}
