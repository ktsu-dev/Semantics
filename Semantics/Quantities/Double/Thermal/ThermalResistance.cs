// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a thermal resistance quantity with double precision.
/// </summary>
public sealed record ThermalResistance
{
	/// <summary>Gets the underlying generic thermal resistance instance.</summary>
	public Generic.ThermalResistance<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ThermalResistance"/> class.
	/// </summary>
	public ThermalResistance() { }

	/// <summary>
	/// Creates a new ThermalResistance from a value in kelvin per watt.
	/// </summary>
	/// <param name="kelvinPerWatt">The value in K/W.</param>
	/// <returns>A new ThermalResistance instance.</returns>
	public static ThermalResistance FromKelvinPerWatt(double kelvinPerWatt) => new() { Value = Generic.ThermalResistance<double>.FromKelvinPerWatt(kelvinPerWatt) };

	/// <summary>
	/// Creates a new ThermalResistance from a value in Fahrenheit-hour per BTU.
	/// </summary>
	/// <param name="fahrenheitHourPerBtu">The value in °F·h/BTU.</param>
	/// <returns>A new ThermalResistance instance.</returns>
	public static ThermalResistance FromFahrenheitHourPerBtu(double fahrenheitHourPerBtu) => new() { Value = Generic.ThermalResistance<double>.FromFahrenheitHourPerBtu(fahrenheitHourPerBtu) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
