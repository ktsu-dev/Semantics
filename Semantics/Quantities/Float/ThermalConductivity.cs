// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a thermal conductivity quantity with float precision.
/// </summary>
public sealed record ThermalConductivity
{
	/// <summary>Gets the underlying generic thermal conductivity instance.</summary>
	public Generic.ThermalConductivity<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ThermalConductivity"/> class.
	/// </summary>
	public ThermalConductivity() { }

	/// <summary>
	/// Creates a new ThermalConductivity from a value in watts per meter-kelvin.
	/// </summary>
	/// <param name="wattsPerMeterKelvin">The value in W/(m·K).</param>
	/// <returns>A new ThermalConductivity instance.</returns>
	public static new ThermalConductivity FromWattsPerMeterKelvin(float wattsPerMeterKelvin) => new() { Value = Generic.ThermalConductivity<float>.FromWattsPerMeterKelvin(wattsPerMeterKelvin) };

	/// <summary>
	/// Creates a new ThermalConductivity from a value in BTU per hour-foot-Fahrenheit.
	/// </summary>
	/// <param name="btuPerHourFootFahrenheit">The value in BTU/(h·ft·°F).</param>
	/// <returns>A new ThermalConductivity instance.</returns>
	public static new ThermalConductivity FromBtuPerHourFootFahrenheit(float btuPerHourFootFahrenheit) => new() { Value = Generic.ThermalConductivity<float>.FromBtuPerHourFootFahrenheit(btuPerHourFootFahrenheit) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
