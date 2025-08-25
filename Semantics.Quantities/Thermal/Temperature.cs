// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a temperature quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Temperature<T> : PhysicalQuantity<Temperature<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of temperature [Θ].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Temperature;

	/// <summary>
	/// Initializes a new instance of the <see cref="Temperature{T}"/> class.
	/// </summary>
	public Temperature() : base() { }

	/// <summary>
	/// Creates a new instance with the specified value.
	/// </summary>
	/// <param name="value">The value for the quantity.</param>
	/// <returns>A new instance of the quantity.</returns>
	/// <exception cref="ArgumentException">Thrown when the temperature is below absolute zero (0 K).</exception>
	public static new Temperature<T> Create(T value)
	{
		if (T.IsNegative(value))
		{
			throw new ArgumentException("Temperature cannot be below absolute zero (0 K).", nameof(value));
		}

		return new Temperature<T>() with { Quantity = value };
	}

	/// <summary>
	/// Creates a new Temperature from a value in Kelvin.
	/// </summary>
	/// <param name="kelvin">The value in Kelvin.</param>
	/// <returns>A new Temperature instance.</returns>
	public static Temperature<T> FromKelvin(T kelvin) => Create(kelvin);

	/// <summary>
	/// Creates a new Temperature from a value in Celsius.
	/// </summary>
	/// <param name="celsius">The value in Celsius.</param>
	/// <returns>A new Temperature instance.</returns>
	public static Temperature<T> FromCelsius(T celsius) => Create(celsius + PhysicalConstants.Generic.AbsoluteZeroInCelsius<T>());

	/// <summary>
	/// Creates a new Temperature from a value in Fahrenheit.
	/// </summary>
	/// <param name="fahrenheit">The value in Fahrenheit.</param>
	/// <returns>A new Temperature instance.</returns>
	public static Temperature<T> FromFahrenheit(T fahrenheit) => Create(((fahrenheit - PhysicalConstants.Generic.FahrenheitOffset<T>()) * PhysicalConstants.Generic.FahrenheitToCelsiusSlope<T>()) + PhysicalConstants.Generic.AbsoluteZeroInCelsius<T>());

	/// <summary>
	/// Creates a new Temperature from a value in Rankine.
	/// </summary>
	/// <param name="rankine">The value in Rankine.</param>
	/// <returns>A new Temperature instance.</returns>
	public static Temperature<T> FromRankine(T rankine) => Create(rankine * PhysicalConstants.Generic.FahrenheitToCelsiusSlope<T>());

	/// <summary>
	/// Converts to Kelvin.
	/// </summary>
	/// <returns>The temperature in Kelvin.</returns>
	public T ToKelvin() => Value;

	/// <summary>
	/// Converts to Celsius.
	/// </summary>
	/// <returns>The temperature in Celsius.</returns>
	public T ToCelsius() => Value - PhysicalConstants.Generic.AbsoluteZeroInCelsius<T>();

	/// <summary>
	/// Converts to Fahrenheit.
	/// </summary>
	/// <returns>The temperature in Fahrenheit.</returns>
	public T ToFahrenheit() => ((Value - PhysicalConstants.Generic.AbsoluteZeroInCelsius<T>()) * PhysicalConstants.Generic.CelsiusToFahrenheitSlope<T>()) + PhysicalConstants.Generic.FahrenheitOffset<T>();

	/// <summary>
	/// Converts to Rankine.
	/// </summary>
	/// <returns>The temperature in Rankine.</returns>
	public T ToRankine() => Value * PhysicalConstants.Generic.CelsiusToFahrenheitSlope<T>();

	/// <summary>
	/// Calculates absolute zero in this temperature scale.
	/// </summary>
	/// <returns>Absolute zero temperature.</returns>
	public static Temperature<T> AbsoluteZero => Create(T.Zero);

	/// <summary>
	/// Calculates water freezing point (273.15 K, 0°C).
	/// </summary>
	/// <returns>Water freezing point.</returns>
	public static Temperature<T> WaterFreezingPoint => Create(PhysicalConstants.Generic.StandardTemperature<T>());

	/// <summary>
	/// Calculates water boiling point (373.15 K, 100°C).
	/// </summary>
	/// <returns>Water boiling point.</returns>
	public static Temperature<T> WaterBoilingPoint => Create(PhysicalConstants.Generic.WaterBoilingPoint<T>());
}
