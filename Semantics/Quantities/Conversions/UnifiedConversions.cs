// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

using System.Numerics;
using System.Reflection;

/// <summary>
/// Provides unified conversion methods for all physical quantities using strongly-typed units.
/// Replaces the scattered static conversion classes.
/// </summary>
public static class UnifiedConversions
{
	/// <summary>
	/// Converts a numeric value to a physical quantity using the specified SI unit.
	/// </summary>
	/// <typeparam name="TNumber">The type of the input number.</typeparam>
	/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <param name="unit">The SI unit to convert from.</param>
	/// <returns>A quantity representing the value in the base unit.</returns>
	public static TQuantity FromSIUnit<TNumber, TQuantity>(this TNumber value, SIUnit unit)
		where TNumber : INumber<TNumber>
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		ITypedConversionCalculator<TQuantity> calculator = GetTypedCalculator<TQuantity>();
		return calculator.FromSIUnit(value, unit);
	}

	/// <summary>
	/// Converts a numeric value to a physical quantity using the specified imperial unit.
	/// </summary>
	/// <typeparam name="TNumber">The type of the input number.</typeparam>
	/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <param name="unit">The imperial unit to convert from.</param>
	/// <returns>A quantity representing the value in the base unit.</returns>
	public static TQuantity FromImperialUnit<TNumber, TQuantity>(this TNumber value, ImperialUnit unit)
		where TNumber : INumber<TNumber>
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		ITypedConversionCalculator<TQuantity> calculator = GetTypedCalculator<TQuantity>();
		return calculator.FromImperialUnit(value, unit);
	}

	/// <summary>
	/// Converts a physical quantity to a numeric value using the specified SI unit.
	/// </summary>
	/// <typeparam name="TNumber">The type of the output number.</typeparam>
	/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
	/// <param name="quantity">The quantity to convert.</param>
	/// <param name="unit">The SI unit to convert to.</param>
	/// <returns>The value in the specified unit.</returns>
	public static TNumber ToSIUnit<TNumber, TQuantity>(this TQuantity quantity, SIUnit unit)
		where TNumber : INumber<TNumber>
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		ITypedConversionCalculator<TQuantity> calculator = GetTypedCalculator<TQuantity>();
		return calculator.ToSIUnit<TNumber>(quantity, unit);
	}

	/// <summary>
	/// Converts a physical quantity to a numeric value using the specified imperial unit.
	/// </summary>
	/// <typeparam name="TNumber">The type of the output number.</typeparam>
	/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
	/// <param name="quantity">The quantity to convert.</param>
	/// <param name="unit">The imperial unit to convert to.</param>
	/// <returns>The value in the specified unit.</returns>
	public static TNumber ToImperialUnit<TNumber, TQuantity>(this TQuantity quantity, ImperialUnit unit)
		where TNumber : INumber<TNumber>
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		ITypedConversionCalculator<TQuantity> calculator = GetTypedCalculator<TQuantity>();
		return calculator.ToImperialUnit<TNumber>(quantity, unit);
	}

	/// <summary>
	/// Gets the available SI units for the specified quantity type.
	/// </summary>
	/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
	/// <returns>An enumerable of available SI units.</returns>
	public static IEnumerable<SIUnit> GetAvailableSIUnits<TQuantity>()
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		ITypedConversionCalculator<TQuantity> calculator = GetTypedCalculator<TQuantity>();
		return calculator.GetAvailableSIUnits();
	}

	/// <summary>
	/// Gets the available imperial units for the specified quantity type.
	/// </summary>
	/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
	/// <returns>An enumerable of available imperial units.</returns>
	public static IEnumerable<ImperialUnit> GetAvailableImperialUnits<TQuantity>()
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		ITypedConversionCalculator<TQuantity> calculator = GetTypedCalculator<TQuantity>();
		return calculator.GetAvailableImperialUnits();
	}

	/// <summary>
	/// Gets the base SI unit for the specified quantity type.
	/// </summary>
	/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
	/// <returns>The base SI unit.</returns>
	public static SIUnit GetBaseSIUnit<TQuantity>()
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		ITypedConversionCalculator<TQuantity> calculator = GetTypedCalculator<TQuantity>();
		return calculator.GetBaseSIUnit();
	}

	private static ITypedConversionCalculator<TQuantity> GetTypedCalculator<TQuantity>()
		where TQuantity : PhysicalQuantity<TQuantity>, new() => TypedConversionRegistry.GetCalculator<TQuantity>();
}

/// <summary>
/// Registry for typed conversion calculators.
/// </summary>
public static class TypedConversionRegistry
{
	private static readonly Dictionary<Type, object> _calculators = [];

	/// <summary>
	/// Gets the typed conversion calculator for the specified quantity type.
	/// </summary>
	/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
	/// <returns>The typed conversion calculator.</returns>
	public static ITypedConversionCalculator<TQuantity> GetCalculator<TQuantity>()
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		Type type = typeof(TQuantity);

		if (_calculators.TryGetValue(type, out object? calculator))
		{
			return (ITypedConversionCalculator<TQuantity>)calculator;
		}

		// Create calculator based on quantity type
		ITypedConversionCalculator<TQuantity> newCalculator = CreateCalculator<TQuantity>();
		_calculators[type] = newCalculator;
		return newCalculator;
	}

	private static ITypedConversionCalculator<TQuantity> CreateCalculator<TQuantity>()
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		Type quantityType = typeof(TQuantity);

		// Try to find an existing typed calculator
		return quantityType.Name switch
		{
			nameof(Force) => (ITypedConversionCalculator<TQuantity>)TypedForceConversionCalculator.Instance,
			nameof(Energy) => (ITypedConversionCalculator<TQuantity>)TypedEnergyConversionCalculator.Instance,
			_ => new GenericTypedConversionCalculator<TQuantity>()
		};
	}
}

/// <summary>
/// A generic typed conversion calculator that can work with any quantity type using its SI unit attribute.
/// </summary>
/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
internal sealed class GenericTypedConversionCalculator<TQuantity> : TypedConversionCalculator<TQuantity>
	where TQuantity : PhysicalQuantity<TQuantity>, new()
{
	/// <summary>
	/// Initializes a new instance of the <see cref="GenericTypedConversionCalculator{TQuantity}"/> class.
	/// </summary>
	public GenericTypedConversionCalculator() : base(GetBaseUnit(), GetRelatedSIUnits(), GetRelatedImperialUnits())
	{
	}

	private static SIUnit GetBaseUnit()
	{
		// Get the base SI unit from the quantity's SIUnitAttribute
		SIUnitAttribute? unitAttribute = typeof(TQuantity).GetCustomAttribute<SIUnitAttribute>();
		return unitAttribute?.Unit ?? throw new InvalidOperationException($"Quantity type {typeof(TQuantity).Name} must have an SIUnitAttribute.");
	}

	private static IEnumerable<SIUnit> GetRelatedSIUnits() =>
		// For now, return empty - can be expanded to automatically discover related units
		[];

	private static IEnumerable<ImperialUnit> GetRelatedImperialUnits() =>
		// For now, return empty - can be expanded to automatically discover related units
		[];
}
