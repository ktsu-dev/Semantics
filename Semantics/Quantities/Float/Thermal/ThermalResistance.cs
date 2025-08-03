// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a thermal resistance quantity with float precision.
/// </summary>
public sealed record ThermalResistance : Generic.ThermalResistance<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ThermalResistance"/> class.
	/// </summary>
	public ThermalResistance() : base() { }

	/// <summary>
	/// Creates a new ThermalResistance from a value in kelvin per watt.
	/// </summary>
	/// <param name="kelvinPerWatt">The value in K/W.</param>
	/// <returns>A new ThermalResistance instance.</returns>
	public static new ThermalResistance FromKelvinPerWatt(float kelvinPerWatt) => new() { Value = kelvinPerWatt };

	/// <summary>
	/// Creates a new ThermalResistance from a value in Fahrenheit-hour per BTU.
	/// </summary>
	/// <param name="fahrenheitHourPerBtu">The value in °F·h/BTU.</param>
	/// <returns>A new ThermalResistance instance.</returns>
	public static new ThermalResistance FromFahrenheitHourPerBtu(float fahrenheitHourPerBtu) => new() { Value = fahrenheitHourPerBtu };
}
