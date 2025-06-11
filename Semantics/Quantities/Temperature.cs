// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a temperature physical quantity.
/// </summary>
[SIUnit("K", "kelvin", "kelvins")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
public sealed record Temperature
	: PhysicalQuantity<Temperature>
{
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Temperature"/>.
/// </summary>
public static class TemperatureConversions
{
	/// <summary>
	/// Converts a value to Kelvin.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Temperature"/> representing the value in Kelvin.</returns>
	public static Temperature Kelvin<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Temperature>();

	/// <summary>
	/// Converts a <see cref="Temperature"/> to a numeric value in Kelvin.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Temperature"/> to convert.</param>
	/// <returns>The numeric value in Kelvin.</returns>
	public static TNumber Kelvin<TNumber>(this Temperature value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to degrees Celsius.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Temperature"/> representing the value in degrees Celsius.</returns>
	public static Temperature Celsius<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Temperature>(PhysicalConstants.CelsiusToKelvinFactor, PhysicalConstants.CelsiusToKelvinOffset);

	/// <summary>
	/// Converts a <see cref="Temperature"/> to a numeric value in degrees Celsius.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Temperature"/> to convert.</param>
	/// <returns>The numeric value in degrees Celsius.</returns>
	public static TNumber Celsius<TNumber>(this Temperature value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.CelsiusToKelvinFactor, PhysicalConstants.CelsiusToKelvinOffset));

	/// <summary>
	/// Converts a value to degrees Fahrenheit.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Temperature"/> representing the value in degrees Fahrenheit.</returns>
	public static Temperature Fahrenheit<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
	{
		// Convert Fahrenheit to Celsius first, then to Kelvin
		double celsius = (Convert.ToDouble(value) - PhysicalConstants.FahrenheitToCelsiusOffset) / PhysicalConstants.FahrenheitToCelsiusFactor;
		return celsius.Celsius();
	}

	/// <summary>
	/// Converts a <see cref="Temperature"/> to a numeric value in degrees Fahrenheit.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Temperature"/> to convert.</param>
	/// <returns>The numeric value in degrees Fahrenheit.</returns>
	public static TNumber Fahrenheit<TNumber>(this Temperature value)
		where TNumber : INumber<TNumber>
	{
		// Convert Kelvin to Celsius first, then to Fahrenheit
		double celsius = value.Celsius<double>();
		double fahrenheit = (celsius * PhysicalConstants.FahrenheitToCelsiusFactor) + PhysicalConstants.FahrenheitToCelsiusOffset;
		return TNumber.CreateChecked(fahrenheit);
	}
}
