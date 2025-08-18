// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a specific heat capacity quantity with compile-time dimensional safety.
/// Specific heat is the amount of heat required to change the temperature of one unit mass by one degree.
/// </summary>
public sealed record SpecificHeat<T> : PhysicalQuantity<SpecificHeat<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of specific heat [L² T⁻² Θ⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.SpecificHeat;

	/// <summary>
	/// Initializes a new instance of the SpecificHeat class.
	/// </summary>
	public SpecificHeat() : base() { }

	/// <summary>
	/// Creates a new SpecificHeat from a value in joules per kilogram-kelvin.
	/// </summary>
	/// <param name="joulesPerKilogramKelvin">The specific heat value in J/(kg·K).</param>
	/// <returns>A new SpecificHeat instance.</returns>
	public static SpecificHeat<T> FromJoulesPerKilogramKelvin(T joulesPerKilogramKelvin) => Create(joulesPerKilogramKelvin);

	/// <summary>
	/// Creates a new SpecificHeat from a value in calories per gram-kelvin.
	/// </summary>
	/// <param name="caloriesPerGramKelvin">The specific heat value in cal/(g·K).</param>
	/// <returns>A new SpecificHeat instance.</returns>
	public static SpecificHeat<T> FromCaloriesPerGramKelvin(T caloriesPerGramKelvin) =>
		Create(caloriesPerGramKelvin * T.CreateChecked(4184));

	/// <summary>
	/// Creates a new SpecificHeat from a value in BTU per pound-Fahrenheit.
	/// </summary>
	/// <param name="btuPerPoundFahrenheit">The specific heat value in BTU/(lb·°F).</param>
	/// <returns>A new SpecificHeat instance.</returns>
	public static SpecificHeat<T> FromBtuPerPoundFahrenheit(T btuPerPoundFahrenheit) =>
		Create(btuPerPoundFahrenheit * PhysicalConstants.Generic.BtuPerPoundFahrenheitToJoulesPerKilogramKelvin<T>());

	/// <summary>Gets the specific heat in joules per kilogram-kelvin.</summary>
	/// <returns>The specific heat in J/(kg·K).</returns>
	public T JoulesPerKilogramKelvin => Value;

	/// <summary>Gets the specific heat in calories per gram-kelvin.</summary>
	/// <returns>The specific heat in cal/(g·K).</returns>
	public T CaloriesPerGramKelvin => Value / T.CreateChecked(4184);

	/// <summary>Gets the specific heat in BTU per pound-Fahrenheit.</summary>
	/// <returns>The specific heat in BTU/(lb·°F).</returns>
	public T BtuPerPoundFahrenheit => Value / PhysicalConstants.Generic.BtuPerPoundFahrenheitToJoulesPerKilogramKelvin<T>();

	/// <summary>
	/// Calculates heat required for temperature change: Q = m·c·ΔT.
	/// </summary>
	/// <param name="mass">The mass of the substance.</param>
	/// <param name="temperatureChange">The temperature change.</param>
	/// <returns>The heat required.</returns>
	public Heat<T> CalculateHeatRequired(Mass<T> mass, Temperature<T> temperatureChange)
	{
		ArgumentNullException.ThrowIfNull(mass);
		ArgumentNullException.ThrowIfNull(temperatureChange);
		return Heat<T>.Create(mass.Value * Value * temperatureChange.Value);
	}

	/// <summary>
	/// Calculates temperature change from heat: ΔT = Q/(m·c).
	/// </summary>
	/// <param name="heat">The heat added or removed.</param>
	/// <param name="mass">The mass of the substance.</param>
	/// <returns>The temperature change.</returns>
	public Temperature<T> CalculateTemperatureChange(Heat<T> heat, Mass<T> mass)
	{
		ArgumentNullException.ThrowIfNull(heat);
		ArgumentNullException.ThrowIfNull(mass);
		return Temperature<T>.Create(heat.Value / (mass.Value * Value));
	}

	/// <summary>
	/// Calculates total heat capacity: C = m·c.
	/// </summary>
	/// <param name="mass">The mass of the object.</param>
	/// <returns>The total heat capacity.</returns>
	public HeatCapacity<T> CalculateHeatCapacity(Mass<T> mass)
	{
		ArgumentNullException.ThrowIfNull(mass);
		return HeatCapacity<T>.Create(mass.Value * Value);
	}

	/// <summary>
	/// Common specific heat values for reference (at room temperature).
	/// </summary>
	public static class CommonValues
	{
		/// <summary>Water: 4184 J/(kg·K)</summary>
		public static SpecificHeat<T> Water => FromJoulesPerKilogramKelvin(T.CreateChecked(4184));

		/// <summary>Ice: 2090 J/(kg·K)</summary>
		public static SpecificHeat<T> Ice => FromJoulesPerKilogramKelvin(T.CreateChecked(2090));

		/// <summary>Aluminum: 897 J/(kg·K)</summary>
		public static SpecificHeat<T> Aluminum => FromJoulesPerKilogramKelvin(T.CreateChecked(897));

		/// <summary>Copper: 385 J/(kg·K)</summary>
		public static SpecificHeat<T> Copper => FromJoulesPerKilogramKelvin(T.CreateChecked(385));

		/// <summary>Iron: 449 J/(kg·K)</summary>
		public static SpecificHeat<T> Iron => FromJoulesPerKilogramKelvin(T.CreateChecked(449));

		/// <summary>Air: 1005 J/(kg·K)</summary>
		public static SpecificHeat<T> Air => FromJoulesPerKilogramKelvin(T.CreateChecked(1005));
	}
}
