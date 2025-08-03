// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a temperature quantity with double precision.
/// </summary>
public sealed record Temperature
{
	/// <summary>Gets the underlying generic temperature instance.</summary>
	public Generic.Temperature<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="Temperature"/> class.
	/// </summary>
	public Temperature() { }

	/// <summary>
	/// Creates a new Temperature from a value in kelvin.
	/// </summary>
	/// <param name="kelvin">The value in kelvin.</param>
	/// <returns>A new Temperature instance.</returns>
	public static Temperature FromKelvin(double kelvin) => new() { Value = Generic.Temperature<double>.FromKelvin(kelvin) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
