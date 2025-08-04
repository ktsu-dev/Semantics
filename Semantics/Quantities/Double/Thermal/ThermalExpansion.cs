// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a thermal expansion coefficient quantity with double precision.
/// </summary>
public sealed record ThermalExpansion : Generic.ThermalExpansion<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ThermalExpansion"/> class.
	/// </summary>
	public ThermalExpansion() : base() { }

	/// <summary>
	/// Creates a new ThermalExpansion from a value in per kelvin.
	/// </summary>
	/// <param name="perKelvin">The thermal expansion coefficient value in K⁻¹.</param>
	/// <returns>A new ThermalExpansion instance.</returns>
	public static new ThermalExpansion FromPerKelvin(double perKelvin) => new() { Quantity = perKelvin };

	/// <summary>
	/// Creates a new ThermalExpansion from a value in per Celsius.
	/// </summary>
	/// <param name="perCelsius">The thermal expansion coefficient value in °C⁻¹.</param>
	/// <returns>A new ThermalExpansion instance.</returns>
	public static new ThermalExpansion FromPerCelsius(double perCelsius) => new() { Quantity = perCelsius };

	/// <summary>
	/// Creates a new ThermalExpansion from a value in per Fahrenheit.
	/// </summary>
	/// <param name="perFahrenheit">The thermal expansion coefficient value in °F⁻¹.</param>
	/// <returns>A new ThermalExpansion instance.</returns>
	public static new ThermalExpansion FromPerFahrenheit(double perFahrenheit) => new() { Quantity = perFahrenheit };
}
