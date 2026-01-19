// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a thermal conductivity quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record ThermalConductivity<T> : PhysicalQuantity<ThermalConductivity<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of thermalconductivity [M L T⁻³ Θ⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.ThermalConductivity;

	/// <summary>
	/// Initializes a new instance of the <see cref="ThermalConductivity{T}"/> class.
	/// </summary>
	public ThermalConductivity() : base() { }

	/// <summary>
	/// Creates a new ThermalConductivity from a value in watts per meter-kelvin.
	/// </summary>
	/// <param name="wattsPerMeterKelvin">The value in W/(m·K).</param>
	/// <returns>A new ThermalConductivity instance.</returns>
	public static ThermalConductivity<T> FromWattsPerMeterKelvin(T wattsPerMeterKelvin) => Create(wattsPerMeterKelvin);

	/// <summary>
	/// Creates a new ThermalConductivity from a value in BTU per hour-foot-Fahrenheit.
	/// </summary>
	/// <param name="btuPerHourFootFahrenheit">The value in BTU/(h·ft·°F).</param>
	/// <returns>A new ThermalConductivity instance.</returns>
	public static ThermalConductivity<T> FromBtuPerHourFootFahrenheit(T btuPerHourFootFahrenheit) =>
		Create(btuPerHourFootFahrenheit * PhysicalConstants.Generic.BtuPerHourFootFahrenheitToWattsPerMeterKelvin<T>());

	/// <summary>
	/// Converts to watts per meter-kelvin.
	/// </summary>
	/// <returns>The thermal conductivity in W/(m·K).</returns>
	public T ToWattsPerMeterKelvin() => Value;

	/// <summary>
	/// Converts to BTU per hour-foot-Fahrenheit.
	/// </summary>
	/// <returns>The thermal conductivity in BTU/(h·ft·°F).</returns>
	public T ToBtuPerHourFootFahrenheit() => Value / PhysicalConstants.Generic.BtuPerHourFootFahrenheitToWattsPerMeterKelvin<T>();

	/// <summary>
	/// Calculates heat flow rate using Fourier's law: Q̇ = k·A·ΔT/L.
	/// </summary>
	/// <param name="area">The cross-sectional area.</param>
	/// <param name="temperatureDifference">The temperature difference.</param>
	/// <param name="length">The length/thickness.</param>
	/// <returns>The heat flow rate.</returns>
	public Power<T> CalculateHeatFlow(Area<T> area, Temperature<T> temperatureDifference, Length<T> length)
	{
		Ensure.NotNull(area);
		Ensure.NotNull(temperatureDifference);
		Ensure.NotNull(length);
		return Power<T>.Create(Value * area.Value * temperatureDifference.Value / length.Value);
	}

	/// <summary>
	/// Calculates thermal resistance: R = L/(k·A).
	/// </summary>
	/// <param name="length">The length/thickness.</param>
	/// <param name="area">The cross-sectional area.</param>
	/// <returns>The thermal resistance.</returns>
	public ThermalResistance<T> CalculateThermalResistance(Length<T> length, Area<T> area)
	{
		Ensure.NotNull(length);
		Ensure.NotNull(area);
		return ThermalResistance<T>.Create(length.Value / (Value * area.Value));
	}
}
