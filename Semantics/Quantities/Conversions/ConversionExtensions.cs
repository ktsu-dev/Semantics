// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

using System.Numerics;

/// <summary>
/// Provides extension methods for generic unit conversions.
/// </summary>
public static class ConversionExtensions
{
	/// <summary>
	/// Converts a numeric value to a specific physical quantity using the specified unit.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
	/// <param name="value">The numeric value to convert.</param>
	/// <param name="unit">The unit to convert from.</param>
	/// <returns>A physical quantity representing the value in the specified unit.</returns>
	public static TQuantity ToQuantity<TNumber, TQuantity>(this TNumber value, string unit)
		where TNumber : INumber<TNumber>
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		IConversionCalculator<TQuantity> calculator = ConversionRegistry.GetCalculator<TQuantity>();
		return calculator.FromUnit(value, unit);
	}

	/// <summary>
	/// Converts a physical quantity to a numeric value in the specified unit.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
	/// <param name="quantity">The physical quantity to convert.</param>
	/// <param name="unit">The unit to convert to.</param>
	/// <returns>The numeric value in the specified unit.</returns>
	public static TNumber ToUnit<TNumber, TQuantity>(this TQuantity quantity, string unit)
		where TNumber : INumber<TNumber>
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		IConversionCalculator<TQuantity> calculator = ConversionRegistry.GetCalculator<TQuantity>();
		return calculator.ToUnit<TNumber>(quantity, unit);
	}

	/// <summary>
	/// Gets the available units for the specified quantity type.
	/// </summary>
	/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
	/// <returns>An enumerable of available unit names.</returns>
	public static IEnumerable<string> GetAvailableUnits<TQuantity>()
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		IConversionCalculator<TQuantity> calculator = ConversionRegistry.GetCalculator<TQuantity>();
		return calculator.GetAvailableUnits();
	}

	/// <summary>
	/// Gets the base SI unit for the specified quantity type.
	/// </summary>
	/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
	/// <returns>The base SI unit name.</returns>
	public static string GetBaseUnit<TQuantity>()
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		IConversionCalculator<TQuantity> calculator = ConversionRegistry.GetCalculator<TQuantity>();
		return calculator.GetBaseUnit();
	}
}
