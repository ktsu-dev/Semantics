// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a heat (thermal energy) quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Heat<T> : PhysicalQuantity<Heat<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Heat;

	/// <summary>
	/// Initializes a new instance of the Heat class.
	/// </summary>
	public Heat() : base() { }

	/// <summary>
	/// Creates a new Heat from a value in joules.
	/// </summary>
	/// <param name="joules">The value in joules.</param>
	/// <returns>A new Heat instance.</returns>
	public static Heat<T> FromJoules(T joules) => Create(joules);

	/// <summary>
	/// Creates a new Heat from a value in calories.
	/// </summary>
	/// <param name="calories">The value in calories.</param>
	/// <returns>A new Heat instance.</returns>
	public static Heat<T> FromCalories(T calories) => Create(calories * PhysicalConstants.Generic.CalorieToJoule<T>());

	/// <summary>
	/// Creates a new Heat from a value in BTU (British Thermal Units).
	/// </summary>
	/// <param name="btu">The value in BTU.</param>
	/// <returns>A new Heat instance.</returns>
	public static Heat<T> FromBTU(T btu) => Create(btu * PhysicalConstants.Generic.BtuToJoule<T>());

	/// <summary>
	/// Creates a new Heat from a value in kilowatt-hours.
	/// </summary>
	/// <param name="kilowattHours">The value in kilowatt-hours.</param>
	/// <returns>A new Heat instance.</returns>
	public static Heat<T> FromKilowattHours(T kilowattHours) => Create(kilowattHours * PhysicalConstants.Generic.KilowattHourToJoule<T>());

	/// <summary>
	/// Converts to joules.
	/// </summary>
	/// <returns>The heat in joules.</returns>
	public T ToJoules() => Value;

	/// <summary>
	/// Converts to calories.
	/// </summary>
	/// <returns>The heat in calories.</returns>
	public T ToCalories() => Value / PhysicalConstants.Generic.CalorieToJoule<T>();

	/// <summary>
	/// Converts to BTU.
	/// </summary>
	/// <returns>The heat in BTU.</returns>
	public T ToBTU() => Value / PhysicalConstants.Generic.BtuToJoule<T>();

	/// <summary>
	/// Converts to kilowatt-hours.
	/// </summary>
	/// <returns>The heat in kilowatt-hours.</returns>
	public T ToKilowattHours() => Value / PhysicalConstants.Generic.KilowattHourToJoule<T>();

	/// <summary>
	/// Converts Heat to Energy.
	/// </summary>
	/// <returns>The equivalent energy.</returns>
	public Energy<T> ToEnergy() => Energy<T>.Create(Value);

	/// <summary>
	/// Creates Heat from Energy.
	/// </summary>
	/// <param name="energy">The energy to convert.</param>
	/// <returns>The equivalent heat.</returns>
	public static Heat<T> FromEnergy(Energy<T> energy)
	{
		ArgumentNullException.ThrowIfNull(energy);
		return Create(energy.Value);
	}
}
