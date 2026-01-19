// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a heat capacity quantity with compile-time dimensional safety.
/// Heat capacity is the amount of heat required to change the temperature of an object by one degree.
/// </summary>
public sealed record HeatCapacity<T> : PhysicalQuantity<HeatCapacity<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of heat capacity [M L² T⁻² Θ⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.HeatCapacity;

	/// <summary>
	/// Initializes a new instance of the HeatCapacity class.
	/// </summary>
	public HeatCapacity() : base() { }

	/// <summary>
	/// Creates a new HeatCapacity from a value in joules per kelvin.
	/// </summary>
	/// <param name="joulesPerKelvin">The heat capacity value in J/K.</param>
	/// <returns>A new HeatCapacity instance.</returns>
	public static HeatCapacity<T> FromJoulesPerKelvin(T joulesPerKelvin) => Create(joulesPerKelvin);

	/// <summary>
	/// Creates a new HeatCapacity from a value in calories per kelvin.
	/// </summary>
	/// <param name="caloriesPerKelvin">The heat capacity value in cal/K.</param>
	/// <returns>A new HeatCapacity instance.</returns>
	public static HeatCapacity<T> FromCaloriesPerKelvin(T caloriesPerKelvin) =>
		Create(caloriesPerKelvin * PhysicalConstants.Generic.CalorieToJoule<T>());

	/// <summary>
	/// Creates a new HeatCapacity from a value in BTU per Fahrenheit.
	/// </summary>
	/// <param name="btuPerFahrenheit">The heat capacity value in BTU/°F.</param>
	/// <returns>A new HeatCapacity instance.</returns>
	public static HeatCapacity<T> FromBtuPerFahrenheit(T btuPerFahrenheit) =>
		Create(btuPerFahrenheit * PhysicalConstants.Generic.BtuPerFahrenheitToJoulesPerKelvin<T>());

	/// <summary>Gets the heat capacity in joules per kelvin.</summary>
	/// <returns>The heat capacity in J/K.</returns>
	public T JoulesPerKelvin => Value;

	/// <summary>Gets the heat capacity in calories per kelvin.</summary>
	/// <returns>The heat capacity in cal/K.</returns>
	public T CaloriesPerKelvin => Value / PhysicalConstants.Generic.CalorieToJoule<T>();

	/// <summary>Gets the heat capacity in BTU per Fahrenheit.</summary>
	/// <returns>The heat capacity in BTU/°F.</returns>
	public T BtuPerFahrenheit => Value / PhysicalConstants.Generic.BtuPerFahrenheitToJoulesPerKelvin<T>();

	/// <summary>
	/// Calculates heat required for temperature change: Q = C·ΔT.
	/// </summary>
	/// <param name="temperatureChange">The temperature change.</param>
	/// <returns>The heat required.</returns>
	public Heat<T> CalculateHeatRequired(Temperature<T> temperatureChange)
	{
		Ensure.NotNull(temperatureChange);
		return Heat<T>.Create(Value * temperatureChange.Value);
	}

	/// <summary>
	/// Calculates temperature change from heat: ΔT = Q/C.
	/// </summary>
	/// <param name="heat">The heat added or removed.</param>
	/// <returns>The temperature change.</returns>
	public Temperature<T> CalculateTemperatureChange(Heat<T> heat)
	{
		Ensure.NotNull(heat);
		return Temperature<T>.Create(heat.Value / Value);
	}

	/// <summary>
	/// Calculates specific heat capacity: c = C/m.
	/// </summary>
	/// <param name="mass">The mass of the object.</param>
	/// <returns>The specific heat capacity.</returns>
	public SpecificHeat<T> CalculateSpecificHeat(Mass<T> mass)
	{
		Ensure.NotNull(mass);
		return SpecificHeat<T>.Create(Value / mass.Value);
	}
}
