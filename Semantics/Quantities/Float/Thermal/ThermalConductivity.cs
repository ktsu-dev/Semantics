// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a thermal conductivity quantity with float precision.
/// </summary>
public sealed record ThermalConductivity : Generic.ThermalConductivity<float>
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
	public static new ThermalConductivity FromWattsPerMeterKelvin(float wattsPerMeterKelvin) => new() { Value = wattsPerMeterKelvin };

	/// <summary>
	/// Creates a new ThermalConductivity from a value in BTU per hour-foot-Fahrenheit.
	/// </summary>
	/// <param name="btuPerHourFootFahrenheit">The value in BTU/(h·ft·°F).</param>
	/// <returns>A new ThermalConductivity instance.</returns>
	public static new ThermalConductivity FromBtuPerHourFootFahrenheit(float btuPerHourFootFahrenheit) => new() { Value = btuPerHourFootFahrenheit };
}
