// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a thermal expansion coefficient quantity with compile-time dimensional safety.
/// Thermal expansion coefficient describes how much a material expands per unit temperature change.
/// </summary>
public sealed record ThermalExpansion<T> : PhysicalQuantity<ThermalExpansion<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of thermal expansion coefficient [Θ⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.ThermalExpansion;

	/// <summary>
	/// Initializes a new instance of the ThermalExpansion class.
	/// </summary>
	public ThermalExpansion() : base() { }

	/// <summary>
	/// Creates a new ThermalExpansion from a value in per kelvin.
	/// </summary>
	/// <param name="perKelvin">The thermal expansion coefficient value in K⁻¹.</param>
	/// <returns>A new ThermalExpansion instance.</returns>
	public static ThermalExpansion<T> FromPerKelvin(T perKelvin) => Create(perKelvin);

	/// <summary>
	/// Creates a new ThermalExpansion from a value in per Celsius.
	/// </summary>
	/// <param name="perCelsius">The thermal expansion coefficient value in °C⁻¹.</param>
	/// <returns>A new ThermalExpansion instance.</returns>
	public static ThermalExpansion<T> FromPerCelsius(T perCelsius) => Create(perCelsius);

	/// <summary>
	/// Creates a new ThermalExpansion from a value in per Fahrenheit.
	/// </summary>
	/// <param name="perFahrenheit">The thermal expansion coefficient value in °F⁻¹.</param>
	/// <returns>A new ThermalExpansion instance.</returns>
	public static ThermalExpansion<T> FromPerFahrenheit(T perFahrenheit) =>
		Create(perFahrenheit * PhysicalConstants.Generic.CelsiusToFahrenheitSlope<T>());

	/// <summary>Gets the thermal expansion coefficient in per kelvin.</summary>
	/// <returns>The thermal expansion coefficient in K⁻¹.</returns>
	public T PerKelvin => Value;

	/// <summary>Gets the thermal expansion coefficient in per Celsius.</summary>
	/// <returns>The thermal expansion coefficient in °C⁻¹.</returns>
	public T PerCelsius => Value;

	/// <summary>Gets the thermal expansion coefficient in per Fahrenheit.</summary>
	/// <returns>The thermal expansion coefficient in °F⁻¹.</returns>
	public T PerFahrenheit => Value / PhysicalConstants.Generic.CelsiusToFahrenheitSlope<T>();

	/// <summary>
	/// Calculates linear expansion: ΔL = α·L₀·ΔT.
	/// </summary>
	/// <param name="originalLength">The original length.</param>
	/// <param name="temperatureChange">The temperature change.</param>
	/// <returns>The change in length.</returns>
	public Length<T> CalculateLinearExpansion(Length<T> originalLength, Temperature<T> temperatureChange)
	{
		Guard.NotNull(originalLength);
		Guard.NotNull(temperatureChange);
		return Length<T>.Create(Value * originalLength.Value * temperatureChange.Value);
	}

	/// <summary>
	/// Calculates final length after thermal expansion: L = L₀·(1 + α·ΔT).
	/// </summary>
	/// <param name="originalLength">The original length.</param>
	/// <param name="temperatureChange">The temperature change.</param>
	/// <returns>The final length.</returns>
	public Length<T> CalculateFinalLength(Length<T> originalLength, Temperature<T> temperatureChange)
	{
		Guard.NotNull(originalLength);
		Guard.NotNull(temperatureChange);
		T expansionFactor = T.One + (Value * temperatureChange.Value);
		return Length<T>.Create(originalLength.Value * expansionFactor);
	}

	/// <summary>
	/// Calculates area expansion coefficient (approximately 2α for small expansions).
	/// </summary>
	/// <returns>The area expansion coefficient.</returns>
	public ThermalExpansion<T> CalculateAreaExpansionCoefficient() => Create(Value * T.CreateChecked(2));

	/// <summary>
	/// Calculates volume expansion coefficient (approximately 3α for small expansions).
	/// </summary>
	/// <returns>The volume expansion coefficient.</returns>
	public ThermalExpansion<T> CalculateVolumeExpansionCoefficient() => Create(Value * T.CreateChecked(3));

	/// <summary>
	/// Common thermal expansion coefficients for reference (at room temperature).
	/// </summary>
	public static class CommonValues
	{
		/// <summary>Aluminum: 23.1 × 10⁻⁶ K⁻¹</summary>
		public static ThermalExpansion<T> Aluminum => FromPerKelvin(T.CreateChecked(23.1e-6));

		/// <summary>Steel: 11.0 × 10⁻⁶ K⁻¹</summary>
		public static ThermalExpansion<T> Steel => FromPerKelvin(T.CreateChecked(11.0e-6));

		/// <summary>Copper: 16.5 × 10⁻⁶ K⁻¹</summary>
		public static ThermalExpansion<T> Copper => FromPerKelvin(T.CreateChecked(16.5e-6));

		/// <summary>Concrete: 10.0 × 10⁻⁶ K⁻¹</summary>
		public static ThermalExpansion<T> Concrete => FromPerKelvin(T.CreateChecked(10.0e-6));

		/// <summary>Glass: 9.0 × 10⁻⁶ K⁻¹</summary>
		public static ThermalExpansion<T> Glass => FromPerKelvin(T.CreateChecked(9.0e-6));

		/// <summary>PVC: 52.0 × 10⁻⁶ K⁻¹</summary>
		public static ThermalExpansion<T> PVC => FromPerKelvin(T.CreateChecked(52.0e-6));
	}
}
