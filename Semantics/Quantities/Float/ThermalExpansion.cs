// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a thermal expansion coefficient quantity with float precision.
/// </summary>
public sealed record ThermalExpansion
{
	/// <summary>Gets the underlying generic thermal expansion instance.</summary>
	public Generic.ThermalExpansion<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ThermalExpansion"/> class.
	/// </summary>
	public ThermalExpansion() { }

	/// <summary>
	/// Creates a new ThermalExpansion from a value in per kelvin.
	/// </summary>
	/// <param name="perKelvin">The thermal expansion coefficient value in K⁻¹.</param>
	/// <returns>A new ThermalExpansion instance.</returns>
	public static new ThermalExpansion FromPerKelvin(float perKelvin) => new() { Value = Generic.ThermalExpansion<float>.FromPerKelvin(perKelvin) };

	/// <summary>
	/// Creates a new ThermalExpansion from a value in per Celsius.
	/// </summary>
	/// <param name="perCelsius">The thermal expansion coefficient value in °C⁻¹.</param>
	/// <returns>A new ThermalExpansion instance.</returns>
	public static new ThermalExpansion FromPerCelsius(float perCelsius) => new() { Value = Generic.ThermalExpansion<float>.FromPerCelsius(perCelsius) };

	/// <summary>
	/// Creates a new ThermalExpansion from a value in per Fahrenheit.
	/// </summary>
	/// <param name="perFahrenheit">The thermal expansion coefficient value in °F⁻¹.</param>
	/// <returns>A new ThermalExpansion instance.</returns>
	public static new ThermalExpansion FromPerFahrenheit(float perFahrenheit) => new() { Value = Generic.ThermalExpansion<float>.FromPerFahrenheit(perFahrenheit) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
