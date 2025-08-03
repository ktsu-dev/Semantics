// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents a thermal diffusivity quantity with compile-time dimensional safety.
/// Thermal diffusivity measures how quickly heat diffuses through a material.
/// </summary>
public record ThermalDiffusivity<T> : PhysicalQuantity<ThermalDiffusivity<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of thermal diffusivity [L² T⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.ThermalDiffusivity;

	/// <summary>
	/// Initializes a new instance of the ThermalDiffusivity class.
	/// </summary>
	public ThermalDiffusivity() : base() { }

	/// <summary>
	/// Creates a new ThermalDiffusivity from a value in square meters per second.
	/// </summary>
	/// <param name="squareMetersPerSecond">The thermal diffusivity value in m²/s.</param>
	/// <returns>A new ThermalDiffusivity instance.</returns>
	public static ThermalDiffusivity<T> FromSquareMetersPerSecond(T squareMetersPerSecond) => Create(squareMetersPerSecond);

	/// <summary>
	/// Creates a new ThermalDiffusivity from a value in square feet per hour.
	/// </summary>
	/// <param name="squareFeetPerHour">The thermal diffusivity value in ft²/h.</param>
	/// <returns>A new ThermalDiffusivity instance.</returns>
	public static ThermalDiffusivity<T> FromSquareFeetPerHour(T squareFeetPerHour) =>
		Create(squareFeetPerHour * T.CreateChecked(2.581e-5));

	/// <summary>
	/// Creates a new ThermalDiffusivity from a value in square centimeters per second.
	/// </summary>
	/// <param name="squareCentimetersPerSecond">The thermal diffusivity value in cm²/s.</param>
	/// <returns>A new ThermalDiffusivity instance.</returns>
	public static ThermalDiffusivity<T> FromSquareCentimetersPerSecond(T squareCentimetersPerSecond) =>
		Create(squareCentimetersPerSecond * T.CreateChecked(1e-4));

	/// <summary>Gets the thermal diffusivity in square meters per second.</summary>
	/// <returns>The thermal diffusivity in m²/s.</returns>
	public T SquareMetersPerSecond => Value;

	/// <summary>Gets the thermal diffusivity in square feet per hour.</summary>
	/// <returns>The thermal diffusivity in ft²/h.</returns>
	public T SquareFeetPerHour => Value / T.CreateChecked(2.581e-5);

	/// <summary>Gets the thermal diffusivity in square centimeters per second.</summary>
	/// <returns>The thermal diffusivity in cm²/s.</returns>
	public T SquareCentimetersPerSecond => Value / T.CreateChecked(1e-4);

	/// <summary>
	/// Calculates thermal diffusivity from material properties: α = k/(ρ·cp).
	/// </summary>
	/// <param name="thermalConductivity">The thermal conductivity.</param>
	/// <param name="density">The material density.</param>
	/// <param name="specificHeat">The specific heat capacity.</param>
	/// <returns>The thermal diffusivity.</returns>
	public static ThermalDiffusivity<T> FromMaterialProperties(
		ThermalConductivity<T> thermalConductivity,
		Density<T> density,
		SpecificHeat<T> specificHeat)
	{
		ArgumentNullException.ThrowIfNull(thermalConductivity);
		ArgumentNullException.ThrowIfNull(density);
		ArgumentNullException.ThrowIfNull(specificHeat);
		return Create(thermalConductivity.Value / (density.Value * specificHeat.Value));
	}

	/// <summary>
	/// Calculates the characteristic time for heat diffusion: t = L²/α.
	/// </summary>
	/// <param name="characteristicLength">The characteristic length scale.</param>
	/// <returns>The characteristic diffusion time.</returns>
	public Time<T> CalculateDiffusionTime(Length<T> characteristicLength)
	{
		ArgumentNullException.ThrowIfNull(characteristicLength);
		T lengthSquared = characteristicLength.Value * characteristicLength.Value;
		return Time<T>.Create(lengthSquared / Value);
	}

	/// <summary>
	/// Calculates the thermal penetration depth: δ = √(α·t).
	/// </summary>
	/// <param name="time">The time duration.</param>
	/// <returns>The thermal penetration depth.</returns>
	public Length<T> CalculatePenetrationDepth(Time<T> time)
	{
		ArgumentNullException.ThrowIfNull(time);
		return Length<T>.Create(T.CreateChecked(Math.Sqrt(double.CreateChecked(Value * time.Value))));
	}

	/// <summary>
	/// Calculates thermal conductivity from diffusivity: k = α·ρ·cp.
	/// </summary>
	/// <param name="density">The material density.</param>
	/// <param name="specificHeat">The specific heat capacity.</param>
	/// <returns>The thermal conductivity.</returns>
	public ThermalConductivity<T> CalculateThermalConductivity(Density<T> density, SpecificHeat<T> specificHeat)
	{
		ArgumentNullException.ThrowIfNull(density);
		ArgumentNullException.ThrowIfNull(specificHeat);
		return ThermalConductivity<T>.Create(Value * density.Value * specificHeat.Value);
	}

	/// <summary>
	/// Common thermal diffusivity values for reference (at room temperature).
	/// </summary>
	public static class CommonValues
	{
		/// <summary>Water: 1.43 × 10⁻⁷ m²/s</summary>
		public static ThermalDiffusivity<T> Water => FromSquareMetersPerSecond(T.CreateChecked(1.43e-7));

		/// <summary>Air: 2.2 × 10⁻⁵ m²/s</summary>
		public static ThermalDiffusivity<T> Air => FromSquareMetersPerSecond(T.CreateChecked(2.2e-5));

		/// <summary>Aluminum: 9.7 × 10⁻⁵ m²/s</summary>
		public static ThermalDiffusivity<T> Aluminum => FromSquareMetersPerSecond(T.CreateChecked(9.7e-5));

		/// <summary>Steel: 1.2 × 10⁻⁵ m²/s</summary>
		public static ThermalDiffusivity<T> Steel => FromSquareMetersPerSecond(T.CreateChecked(1.2e-5));

		/// <summary>Copper: 1.11 × 10⁻⁴ m²/s</summary>
		public static ThermalDiffusivity<T> Copper => FromSquareMetersPerSecond(T.CreateChecked(1.11e-4));

		/// <summary>Concrete: 5.0 × 10⁻⁷ m²/s</summary>
		public static ThermalDiffusivity<T> Concrete => FromSquareMetersPerSecond(T.CreateChecked(5.0e-7));

		/// <summary>Glass: 3.4 × 10⁻⁷ m²/s</summary>
		public static ThermalDiffusivity<T> Glass => FromSquareMetersPerSecond(T.CreateChecked(3.4e-7));
	}
}
