// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents a heat transfer coefficient quantity with compile-time dimensional safety.
/// Heat transfer coefficient characterizes convective heat transfer between a surface and a fluid.
/// </summary>
public record HeatTransferCoefficient<T> : PhysicalQuantity<HeatTransferCoefficient<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of heat transfer coefficient [M T⁻³ Θ⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.HeatTransferCoefficient;

	/// <summary>
	/// Initializes a new instance of the HeatTransferCoefficient class.
	/// </summary>
	public HeatTransferCoefficient() : base() { }

	/// <summary>
	/// Creates a new HeatTransferCoefficient from a value in watts per square meter-kelvin.
	/// </summary>
	/// <param name="wattsPerSquareMeterKelvin">The heat transfer coefficient value in W/(m²·K).</param>
	/// <returns>A new HeatTransferCoefficient instance.</returns>
	public static HeatTransferCoefficient<T> FromWattsPerSquareMeterKelvin(T wattsPerSquareMeterKelvin) => Create(wattsPerSquareMeterKelvin);

	/// <summary>
	/// Creates a new HeatTransferCoefficient from a value in BTU per hour-square foot-Fahrenheit.
	/// </summary>
	/// <param name="btuPerHourSquareFootFahrenheit">The heat transfer coefficient value in BTU/(h·ft²·°F).</param>
	/// <returns>A new HeatTransferCoefficient instance.</returns>
	public static HeatTransferCoefficient<T> FromBtuPerHourSquareFootFahrenheit(T btuPerHourSquareFootFahrenheit) =>
		Create(btuPerHourSquareFootFahrenheit * PhysicalConstants.Generic.BtuPerHourSquareFootFahrenheitToWattsPerSquareMeterKelvin<T>());

	/// <summary>
	/// Creates a new HeatTransferCoefficient from a value in calories per second-square centimeter-kelvin.
	/// </summary>
	/// <param name="caloriesPerSecondSquareCentimeterKelvin">The heat transfer coefficient value in cal/(s·cm²·K).</param>
	/// <returns>A new HeatTransferCoefficient instance.</returns>
	public static HeatTransferCoefficient<T> FromCaloriesPerSecondSquareCentimeterKelvin(T caloriesPerSecondSquareCentimeterKelvin) =>
		Create(caloriesPerSecondSquareCentimeterKelvin * T.CreateChecked(41840));

	/// <summary>Gets the heat transfer coefficient in watts per square meter-kelvin.</summary>
	/// <returns>The heat transfer coefficient in W/(m²·K).</returns>
	public T WattsPerSquareMeterKelvin => Value;

	/// <summary>Gets the heat transfer coefficient in BTU per hour-square foot-Fahrenheit.</summary>
	/// <returns>The heat transfer coefficient in BTU/(h·ft²·°F).</returns>
	public T BtuPerHourSquareFootFahrenheit => Value / PhysicalConstants.Generic.BtuPerHourSquareFootFahrenheitToWattsPerSquareMeterKelvin<T>();

	/// <summary>Gets the heat transfer coefficient in calories per second-square centimeter-kelvin.</summary>
	/// <returns>The heat transfer coefficient in cal/(s·cm²·K).</returns>
	public T CaloriesPerSecondSquareCentimeterKelvin => Value / T.CreateChecked(41840);

	/// <summary>
	/// Calculates convective heat transfer rate using Newton's law of cooling: Q̇ = h·A·ΔT.
	/// </summary>
	/// <param name="area">The heat transfer surface area.</param>
	/// <param name="temperatureDifference">The temperature difference between surface and fluid.</param>
	/// <returns>The heat transfer rate.</returns>
	public Power<T> CalculateHeatTransferRate(Area<T> area, Temperature<T> temperatureDifference)
	{
		ArgumentNullException.ThrowIfNull(area);
		ArgumentNullException.ThrowIfNull(temperatureDifference);
		return Power<T>.Create(Value * area.Value * temperatureDifference.Value);
	}

	/// <summary>
	/// Calculates convective thermal resistance: R = 1/(h·A).
	/// </summary>
	/// <param name="area">The heat transfer surface area.</param>
	/// <returns>The convective thermal resistance.</returns>
	public ThermalResistance<T> CalculateThermalResistance(Area<T> area)
	{
		ArgumentNullException.ThrowIfNull(area);
		return ThermalResistance<T>.Create(T.One / (Value * area.Value));
	}

	/// <summary>
	/// Typical heat transfer coefficient ranges for reference.
	/// </summary>
	public static class TypicalValues
	{
		/// <summary>Natural convection in air: 5-25 W/(m²·K)</summary>
		public static HeatTransferCoefficient<T> NaturalConvectionAir => FromWattsPerSquareMeterKelvin(T.CreateChecked(15));

		/// <summary>Forced convection in air: 25-250 W/(m²·K)</summary>
		public static HeatTransferCoefficient<T> ForcedConvectionAir => FromWattsPerSquareMeterKelvin(T.CreateChecked(100));

		/// <summary>Natural convection in water: 500-1000 W/(m²·K)</summary>
		public static HeatTransferCoefficient<T> NaturalConvectionWater => FromWattsPerSquareMeterKelvin(T.CreateChecked(750));

		/// <summary>Forced convection in water: 1000-15000 W/(m²·K)</summary>
		public static HeatTransferCoefficient<T> ForcedConvectionWater => FromWattsPerSquareMeterKelvin(T.CreateChecked(5000));

		/// <summary>Boiling water: 2500-35000 W/(m²·K)</summary>
		public static HeatTransferCoefficient<T> BoilingWater => FromWattsPerSquareMeterKelvin(T.CreateChecked(15000));

		/// <summary>Condensing steam: 5000-100000 W/(m²·K)</summary>
		public static HeatTransferCoefficient<T> CondensingSteam => FromWattsPerSquareMeterKelvin(T.CreateChecked(25000));
	}
}
