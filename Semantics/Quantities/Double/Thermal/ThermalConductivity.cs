// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a thermal conductivity quantity with double precision.
/// </summary>
public sealed record ThermalConductivity : Generic.ThermalConductivity<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ThermalConductivity"/> class.
	/// </summary>
	public ThermalConductivity() : base() { }

	/// <summary>
	/// Creates a new ThermalConductivity from a value in watts per meter-kelvin.
	/// </summary>
	/// <param name="wattsPerMeterKelvin">The value in W/(m·K).</param>
	/// <returns>A new ThermalConductivity instance.</returns>
	public static new ThermalConductivity FromWattsPerMeterKelvin(double wattsPerMeterKelvin) => new() { Quantity = wattsPerMeterKelvin };

	/// <summary>
	/// Creates a new ThermalConductivity from a value in BTU per hour-foot-Fahrenheit.
	/// </summary>
	/// <param name="btuPerHourFootFahrenheit">The value in BTU/(h·ft·°F).</param>
	/// <returns>A new ThermalConductivity instance.</returns>
	public static new ThermalConductivity FromBtuPerHourFootFahrenheit(double btuPerHourFootFahrenheit) => new() { Quantity = btuPerHourFootFahrenheit };
}
