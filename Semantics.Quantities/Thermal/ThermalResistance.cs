// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a thermal resistance quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record ThermalResistance<T> : PhysicalQuantity<ThermalResistance<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of thermalresistance [M⁻¹ L⁻² T³ Θ].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.ThermalResistance;

	/// <summary>
	/// Initializes a new instance of the <see cref="ThermalResistance{T}"/> class.
	/// </summary>
	public ThermalResistance() : base() { }

	/// <summary>
	/// Creates a new ThermalResistance from a value in kelvin per watt.
	/// </summary>
	/// <param name="kelvinPerWatt">The value in K/W.</param>
	/// <returns>A new ThermalResistance instance.</returns>
	public static ThermalResistance<T> FromKelvinPerWatt(T kelvinPerWatt) => Create(kelvinPerWatt);

	/// <summary>
	/// Creates a new ThermalResistance from a value in Fahrenheit-hour per BTU.
	/// </summary>
	/// <param name="fahrenheitHourPerBtu">The value in °F·h/BTU.</param>
	/// <returns>A new ThermalResistance instance.</returns>
	public static ThermalResistance<T> FromFahrenheitHourPerBtu(T fahrenheitHourPerBtu) =>
		Create(fahrenheitHourPerBtu * PhysicalConstants.Generic.FahrenheitHourPerBtuToKelvinPerWatt<T>());

	/// <summary>
	/// Converts to kelvin per watt.
	/// </summary>
	/// <returns>The thermal resistance in K/W.</returns>
	public T ToKelvinPerWatt() => Value;

	/// <summary>
	/// Converts to Fahrenheit-hour per BTU.
	/// </summary>
	/// <returns>The thermal resistance in °F·h/BTU.</returns>
	public T ToFahrenheitHourPerBtu() => Value / PhysicalConstants.Generic.FahrenheitHourPerBtuToKelvinPerWatt<T>();

	/// <summary>
	/// Calculates heat flow rate using thermal resistance: Q̇ = ΔT/R.
	/// </summary>
	/// <param name="temperatureDifference">The temperature difference.</param>
	/// <returns>The heat flow rate.</returns>
	public Power<T> CalculateHeatFlow(Temperature<T> temperatureDifference)
	{
		ArgumentNullException.ThrowIfNull(temperatureDifference);
		return Power<T>.Create(temperatureDifference.Value / Value);
	}

	/// <summary>
	/// Adds thermal resistances in series.
	/// </summary>
	/// <param name="left">The first thermal resistance.</param>
	/// <param name="right">The second thermal resistance.</param>
	/// <returns>The total thermal resistance.</returns>
	public static ThermalResistance<T> operator +(ThermalResistance<T> left, ThermalResistance<T> right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value + right.Value);
	}

	/// <summary>
	/// Adds thermal resistances in series (friendly alternate for operator +).
	/// </summary>
	/// <param name="other">The other thermal resistance.</param>
	/// <returns>The total thermal resistance.</returns>
	public ThermalResistance<T> Add(ThermalResistance<T> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return this + other;
	}

	/// <summary>
	/// Calculates parallel thermal resistance: 1/R_total = 1/R1 + 1/R2.
	/// </summary>
	/// <param name="other">The other thermal resistance.</param>
	/// <returns>The parallel thermal resistance.</returns>
	public ThermalResistance<T> InParallelWith(ThermalResistance<T> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Create(T.CreateChecked(1) / ((T.CreateChecked(1) / Value) + (T.CreateChecked(1) / other.Value)));
	}
}
