// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;
using System.Reflection;

// TODO: support both direct use and dependency injection for physical quantities

/// <summary>
/// Represents a physical quantity with a specific unit of measurement.
/// </summary>
/// <typeparam name="TSelf">The type of the derived class.</typeparam>
public abstract record PhysicalQuantity<TSelf>
	: SemanticQuantity<TSelf, double> // TODO: support other numeric types implementing INumber through dependency injection
	, IComparable<TSelf>
	where TSelf : PhysicalQuantity<TSelf>, new()
{
	/// <summary>
	/// Gets the SI unit attribute associated with the derived class.
	/// </summary>
	private static SIUnitAttribute SIUnitAttribute { get; } = typeof(TSelf).GetCustomAttribute<SIUnitAttribute>() ?? new SIUnitAttribute(new SIUnit(string.Empty, string.Empty, string.Empty));

	/// <summary>
	/// Gets the SI unit for this physical quantity type.
	/// </summary>
	protected static SIUnit Unit => SIUnitAttribute.Unit;

	/// <summary>
	/// Compares the current physical quantity to another instance of the same type.
	/// </summary>
	/// <param name="other">The other physical quantity to compare to.</param>
	/// <returns>
	/// A value less than zero if this instance is less than <paramref name="other"/>,
	/// zero if they are equal, or a value greater than zero if this instance is greater than <paramref name="other"/>.
	/// </returns>
	public int CompareTo(TSelf? other) => other is null ? 1 : Quantity.CompareTo(other.Quantity);

	/// <summary>
	/// Returns a string representation of the physical quantity, including its unit symbol and name.
	/// </summary>
	/// <returns>A string that represents the physical quantity.</returns>
	public sealed override string ToString()
	{
		string symbolComponent = string.IsNullOrWhiteSpace(Unit.Symbol) ? string.Empty : $" {Unit.Symbol}";
		double absQuantity = Math.Abs(Quantity);
		bool isPlural = absQuantity != 1;
		string pluralComponent = isPlural ? Unit.Plural : Unit.Singular;
		string nameComponent = string.IsNullOrWhiteSpace(pluralComponent) ? string.Empty : $" ({pluralComponent})";
		return $"{Quantity}{symbolComponent}{nameComponent}";
	}

	/// <summary>
	/// Determines whether one physical quantity is less than another.
	/// </summary>
	/// <param name="left">The first physical quantity.</param>
	/// <param name="right">The second physical quantity.</param>
	/// <returns>True if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, false.</returns>
	public static bool operator <(PhysicalQuantity<TSelf> left, TSelf right) =>
		left is null ? right is not null : left.CompareTo(right) < 0;

	/// <summary>
	/// Determines whether one physical quantity is less than or equal to another.
	/// </summary>
	/// <param name="left">The first physical quantity.</param>
	/// <param name="right">The second physical quantity.</param>
	/// <returns>True if <paramref name="left"/> is less than or equal to <paramref name="right"/>; otherwise, false.</returns>
	public static bool operator <=(PhysicalQuantity<TSelf> left, TSelf right) =>
		left is null || left.CompareTo(right) <= 0;

	/// <summary>
	/// Determines whether one physical quantity is greater than another.
	/// </summary>
	/// <param name="left">The first physical quantity.</param>
	/// <param name="right">The second physical quantity.</param>
	/// <returns>True if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, false.</returns>
	public static bool operator >(PhysicalQuantity<TSelf> left, TSelf right) =>
		left is not null && left.CompareTo(right) > 0;

	/// <summary>
	/// Determines whether one physical quantity is greater than or equal to another.
	/// </summary>
	/// <param name="left">The first physical quantity.</param>
	/// <param name="right">The second physical quantity.</param>
	/// <returns>True if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, false.</returns>
	public static bool operator >=(PhysicalQuantity<TSelf> left, TSelf right) =>
		left is null ? right is null : left.CompareTo(right) >= 0;

	/// <summary>
	/// Raises the physical quantity to the specified power.
	/// </summary>
	/// <typeparam name="TPower">The type of the power value.</typeparam>
	/// <param name="power">The power to raise the quantity to.</param>
	/// <returns>A new instance of the physical quantity raised to the specified power.</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="power"/> is null.</exception>
	public TSelf Pow<TPower>(TPower power)
		where TPower : INumber<TPower>
	{
		ArgumentNullException.ThrowIfNull(power);
		return Create(Math.Pow(Quantity, Convert.ToDouble(power)));
	}

	/// <summary>
	/// Returns the absolute value of the physical quantity.
	/// </summary>
	/// <returns>A new instance of the physical quantity with an absolute value.</returns>
	public TSelf Abs() => Create(Math.Abs(Quantity));

	/// <summary>
	/// Clamps the physical quantity to the specified minimum and maximum values.
	/// </summary>
	/// <typeparam name="T1">The type of the minimum value.</typeparam>
	/// <typeparam name="T2">The type of the maximum value.</typeparam>
	/// <param name="min">The minimum value.</param>
	/// <param name="max">The maximum value.</param>
	/// <returns>A new instance of the physical quantity clamped to the specified range.</returns>
	public TSelf Clamp<T1, T2>(T1 min, T2 max)
		where T1 : INumber<T1>
		where T2 : INumber<T2>
		=> Create(Math.Clamp(Quantity, Convert.ToDouble(min), Convert.ToDouble(max)));

	/// <summary>
	/// Returns the minimum of this physical quantity and another.
	/// </summary>
	/// <param name="other">The other physical quantity to compare with.</param>
	/// <returns>A new instance of the physical quantity representing the minimum value.</returns>
	public TSelf Min(TSelf other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Create(Math.Min(Quantity, other.Quantity));
	}

	/// <summary>
	/// Returns the maximum of this physical quantity and another.
	/// </summary>
	/// <param name="other">The other physical quantity to compare with.</param>
	/// <returns>A new instance of the physical quantity representing the maximum value.</returns>
	public TSelf Max(TSelf other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Create(Math.Max(Quantity, other.Quantity));
	}

	/// <summary>
	/// Adds a specified value to the physical quantity during a unit conversion.
	/// </summary>
	/// <param name="other">The value to add.</param>
	/// <returns>A new instance of the physical quantity with the added value.</returns>
	protected TSelf ConversionAdd(double other) => Create(Quantity + other);
}

/// <summary>
/// Provides static methods for converting values to and from physical quantities.
/// </summary>
public static class PhysicalQuantity
{
	/// <summary>
	/// Converts a numeric value to a specific physical quantity type with a given conversion factor and offset.
	/// </summary>
	/// <typeparam name="TInput">The type of the input numeric value.</typeparam>
	/// <typeparam name="TQuantity">The type of the physical quantity to convert to.</typeparam>
	/// <param name="value">The numeric value to convert.</param>
	/// <param name="factor">The conversion factor to apply.</param>
	/// <param name="offset">The conversion offset to apply.</param>
	/// <returns>A new instance of the specified physical quantity type.</returns>
	public static TQuantity ConvertToQuantity<TInput, TQuantity>(this TInput value, double factor, double offset)
		where TQuantity : PhysicalQuantity<TQuantity>, new()
		where TInput : INumber<TInput>
		=> PhysicalQuantity<TQuantity>.Create((Convert.ToDouble(value) * factor) + offset);

	/// <summary>
	/// Converts a numeric value to a specific physical quantity type with a given conversion factor.
	/// </summary>
	/// <typeparam name="TInput">The type of the input numeric value.</typeparam>
	/// <typeparam name="TQuantity">The type of the physical quantity to convert to.</typeparam>
	/// <param name="value">The numeric value to convert.</param>
	/// <param name="factor">The conversion factor to apply.</param>
	/// <returns>A new instance of the specified physical quantity type.</returns>
	public static TQuantity ConvertToQuantity<TInput, TQuantity>(this TInput value, double factor)
		where TQuantity : PhysicalQuantity<TQuantity>, new()
		where TInput : INumber<TInput>
		=> PhysicalQuantity<TQuantity>.Create(Convert.ToDouble(value) * factor);

	/// <summary>
	/// Converts a numeric value to a specific physical quantity type without any conversion factor or offset.
	/// </summary>
	/// <typeparam name="TInput">The type of the input numeric value.</typeparam>
	/// <typeparam name="TQuantity">The type of the physical quantity to convert to.</typeparam>
	/// <param name="value">The numeric value to convert.</param>
	/// <returns>A new instance of the specified physical quantity type.</returns>
	public static TQuantity ConvertToQuantity<TInput, TQuantity>(this TInput value)
		where TQuantity : PhysicalQuantity<TQuantity>, new()
		where TInput : INumber<TInput>
		=> PhysicalQuantity<TQuantity>.Create(Convert.ToDouble(value));

	/// <summary>
	/// Converts a physical quantity to a numeric value with a given conversion factor and offset.
	/// </summary>
	/// <typeparam name="TQuantity">The type of the physical quantity to convert from.</typeparam>
	/// <param name="value">The physical quantity to convert.</param>
	/// <param name="factor">The conversion factor to apply.</param>
	/// <param name="offset">The conversion offset to apply.</param>
	/// <returns>The numeric value representing the converted physical quantity.</returns>
	public static double ConvertToNumber<TQuantity>(this TQuantity value, double factor, double offset)
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		ArgumentNullException.ThrowIfNull(value);
		return (value.Quantity - offset) / factor;
	}

	/// <summary>
	/// Converts a physical quantity to a numeric value with a given conversion factor.
	/// </summary>
	/// <typeparam name="TQuantity">The type of the physical quantity to convert from.</typeparam>
	/// <param name="value">The physical quantity to convert.</param>
	/// <param name="factor">The conversion factor to apply.</param>
	/// <returns>The numeric value representing the converted physical quantity.</returns>
	public static double ConvertToNumber<TQuantity>(this TQuantity value, double factor)
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		ArgumentNullException.ThrowIfNull(value);
		return value.Quantity / factor;
	}

	/// <summary>
	/// Converts a physical quantity to a numeric value without any conversion factor or offset.
	/// </summary>
	/// <typeparam name="TQuantity">The type of the physical quantity to convert from.</typeparam>
	/// <param name="value">The physical quantity to convert.</param>
	/// <returns>The numeric value representing the converted physical quantity.</returns>
	public static double ConvertToNumber<TQuantity>(this TQuantity value)
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		ArgumentNullException.ThrowIfNull(value);
		return value.Quantity;
	}
}
