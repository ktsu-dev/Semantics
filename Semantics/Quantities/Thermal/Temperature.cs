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
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Temperature;

	/// <summary>
	/// Initializes a new instance of the Temperature class.
	/// </summary>
	public Temperature() : base() { }

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
	public static Temperature<T> FromCelsius(T celsius) => Create(celsius + T.CreateChecked(273.15));

	/// <summary>
	/// Creates a new Temperature from a value in Fahrenheit.
	/// </summary>
	/// <param name="fahrenheit">The value in Fahrenheit.</param>
	/// <returns>A new Temperature instance.</returns>
	public static Temperature<T> FromFahrenheit(T fahrenheit) => Create(((fahrenheit - T.CreateChecked(32)) * T.CreateChecked(5.0 / 9.0)) + T.CreateChecked(273.15));

	/// <summary>
	/// Creates a new Temperature from a value in Rankine.
	/// </summary>
	/// <param name="rankine">The value in Rankine.</param>
	/// <returns>A new Temperature instance.</returns>
	public static Temperature<T> FromRankine(T rankine) => Create(rankine * T.CreateChecked(5.0 / 9.0));

	/// <summary>
	/// Converts to Kelvin.
	/// </summary>
	/// <returns>The temperature in Kelvin.</returns>
	public T ToKelvin() => Value;

	/// <summary>
	/// Converts to Celsius.
	/// </summary>
	/// <returns>The temperature in Celsius.</returns>
	public T ToCelsius() => Value - T.CreateChecked(273.15);

	/// <summary>
	/// Converts to Fahrenheit.
	/// </summary>
	/// <returns>The temperature in Fahrenheit.</returns>
	public T ToFahrenheit() => ((Value - T.CreateChecked(273.15)) * T.CreateChecked(9.0 / 5.0)) + T.CreateChecked(32);

	/// <summary>
	/// Converts to Rankine.
	/// </summary>
	/// <returns>The temperature in Rankine.</returns>
	public T ToRankine() => Value * T.CreateChecked(9.0 / 5.0);

	/// <summary>
	/// Calculates absolute zero in this temperature scale.
	/// </summary>
	/// <returns>Absolute zero temperature.</returns>
	public static Temperature<T> AbsoluteZero => Create(T.Zero);

	/// <summary>
	/// Calculates water freezing point (273.15 K, 0°C).
	/// </summary>
	/// <returns>Water freezing point.</returns>
	public static Temperature<T> WaterFreezingPoint => Create(T.CreateChecked(273.15));

	/// <summary>
	/// Calculates water boiling point (373.15 K, 100°C).
	/// </summary>
	/// <returns>Water boiling point.</returns>
	public static Temperature<T> WaterBoilingPoint => Create(T.CreateChecked(373.15));
}
