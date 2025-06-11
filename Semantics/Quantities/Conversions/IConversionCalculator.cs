// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

using System.Numerics;

/// <summary>
/// Defines the interface for conversion calculators that handle unit conversions.
/// </summary>
/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
public interface IConversionCalculator<TQuantity>
	where TQuantity : PhysicalQuantity<TQuantity>, new()
{
	/// <summary>
	/// Converts a numeric value from a specific unit to the base SI unit.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The numeric value to convert.</param>
	/// <param name="unit">The unit to convert from.</param>
	/// <returns>A physical quantity representing the value in base SI unit.</returns>
	public TQuantity FromUnit<TNumber>(TNumber value, string unit)
		where TNumber : INumber<TNumber>;

	/// <summary>
	/// Converts a physical quantity to a numeric value in a specific unit.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="quantity">The physical quantity to convert.</param>
	/// <param name="unit">The unit to convert to.</param>
	/// <returns>The numeric value in the specified unit.</returns>
	public TNumber ToUnit<TNumber>(TQuantity quantity, string unit)
		where TNumber : INumber<TNumber>;

	/// <summary>
	/// Gets the available units for this quantity type.
	/// </summary>
	/// <returns>An enumerable of available unit names.</returns>
	public IEnumerable<string> GetAvailableUnits();

	/// <summary>
	/// Gets the base SI unit for this quantity type.
	/// </summary>
	/// <returns>The base SI unit name.</returns>
	public string GetBaseUnit();
}
