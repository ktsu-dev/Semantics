// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a mass physical quantity.
/// </summary>
[SIUnit(typeof(SIUnits), nameof(SIUnits.Kilogram))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]

public sealed record Mass
	: PhysicalQuantity<Mass>
	, IIntegralOperators<Mass, Acceleration, Force>
	, IDerivativeOperators<Mass, Volume, Density>
	, IDerivativeOperators<Mass, Density, Volume>
	, IIntegralOperators<Mass, Velocity, Momentum>
	, IDerivativeOperators<Mass, MolarMass, AmountOfSubstance>
	, IDerivativeOperators<Mass, AmountOfSubstance, MolarMass>
{
	/// <summary>
	/// Multiplies a <see cref="Mass"/> by an <see cref="Acceleration"/> to compute a <see cref="Force"/>.
	/// </summary>
	/// <param name="left">The mass operand.</param>
	/// <param name="right">The acceleration operand.</param>
	/// <returns>A <see cref="Force"/> representing the result of the multiplication.</returns>
	public static Force operator *(Mass left, Acceleration right) =>
		IIntegralOperators<Mass, Acceleration, Force>.Integrate(left, right);

	/// <summary>
	/// Divides a <see cref="Mass"/> by a <see cref="Volume"/> to compute a <see cref="Density"/>.
	/// </summary>
	/// <param name="left">The mass operand.</param>
	/// <param name="right">The volume operand.</param>
	/// <returns>A <see cref="Density"/> representing the result of the division.</returns>
	public static Density operator /(Mass left, Volume right) =>
		IDerivativeOperators<Mass, Volume, Density>.Derive(left, right);

	/// <summary>
	/// Divides a <see cref="Mass"/> by a <see cref="Density"/> to compute a <see cref="Volume"/>.
	/// </summary>
	/// <param name="left">The mass operand.</param>
	/// <param name="right">The density operand.</param>
	/// <returns>A <see cref="Volume"/> representing the result of the division.</returns>
	public static Volume operator /(Mass left, Density right) =>
		IDerivativeOperators<Mass, Density, Volume>.Derive(left, right);

	/// <summary>
	/// Multiplies a <see cref="Mass"/> by a <see cref="Velocity"/> to compute a <see cref="Momentum"/>.
	/// </summary>
	/// <param name="left">The mass operand.</param>
	/// <param name="right">The velocity operand.</param>
	/// <returns>A <see cref="Momentum"/> representing the result of the multiplication.</returns>
	public static Momentum operator *(Mass left, Velocity right) =>
		IIntegralOperators<Mass, Velocity, Momentum>.Integrate(left, right);

	/// <summary>
	/// Divides a <see cref="Mass"/> by a <see cref="MolarMass"/> to compute an <see cref="AmountOfSubstance"/>.
	/// </summary>
	/// <param name="left">The mass operand.</param>
	/// <param name="right">The molar mass operand.</param>
	/// <returns>An <see cref="AmountOfSubstance"/> representing the result of the division.</returns>
	public static AmountOfSubstance operator /(Mass left, MolarMass right) =>
		IDerivativeOperators<Mass, MolarMass, AmountOfSubstance>.Derive(left, right);

	/// <summary>
	/// Divides a <see cref="Mass"/> by an <see cref="AmountOfSubstance"/> to compute a <see cref="MolarMass"/>.
	/// </summary>
	/// <param name="left">The mass operand.</param>
	/// <param name="right">The amount of substance operand.</param>
	/// <returns>A <see cref="MolarMass"/> representing the result of the division.</returns>
	public static MolarMass operator /(Mass left, AmountOfSubstance right) =>
		IDerivativeOperators<Mass, AmountOfSubstance, MolarMass>.Derive(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Mass"/>.
/// </summary>
public static class MassConversions
{
	private static Conversions.IConversionCalculator<Mass> Calculator => Conversions.ConversionRegistry.GetCalculator<Mass>();

	/// <summary>
	/// Converts a value to kilograms.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Mass"/> representing the value in kilograms.</returns>
	public static Mass Kilograms<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "kilograms");

	/// <summary>
	/// Converts a <see cref="Mass"/> to a numeric value in kilograms.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Mass"/> to convert.</param>
	/// <returns>The numeric value in kilograms.</returns>
	public static TNumber Kilograms<TNumber>(this Mass value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "kilograms");

	/// <summary>
	/// Converts a value to grams.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Mass"/> representing the value in grams.</returns>
	public static Mass Grams<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "grams");

	/// <summary>
	/// Converts a <see cref="Mass"/> to a numeric value in grams.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Mass"/> to convert.</param>
	/// <returns>The numeric value in grams.</returns>
	public static TNumber Grams<TNumber>(this Mass value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "grams");

	/// <summary>
	/// Converts a value to milligrams.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Mass"/> representing the value in milligrams.</returns>
	public static Mass Milligrams<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "milligrams");

	/// <summary>
	/// Converts a <see cref="Mass"/> to a numeric value in milligrams.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Mass"/> to convert.</param>
	/// <returns>The numeric value in milligrams.</returns>
	public static TNumber Milligrams<TNumber>(this Mass value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "milligrams");

	/// <summary>
	/// Converts a value to pounds.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Mass"/> representing the value in pounds.</returns>
	public static Mass Pounds<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "pounds");

	/// <summary>
	/// Converts a <see cref="Mass"/> to a numeric value in pounds.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Mass"/> to convert.</param>
	/// <returns>The numeric value in pounds.</returns>
	public static TNumber Pounds<TNumber>(this Mass value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "pounds");

	/// <summary>
	/// Converts a value to ounces.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Mass"/> representing the value in ounces.</returns>
	public static Mass Ounces<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "ounces");

	/// <summary>
	/// Converts a <see cref="Mass"/> to a numeric value in ounces.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Mass"/> to convert.</param>
	/// <returns>The numeric value in ounces.</returns>
	public static TNumber Ounces<TNumber>(this Mass value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "ounces");

	/// <summary>
	/// Converts a value to stones.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Mass"/> representing the value in stones.</returns>
	public static Mass Stones<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "stones");

	/// <summary>
	/// Converts a <see cref="Mass"/> to a numeric value in stones.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Mass"/> to convert.</param>
	/// <returns>The numeric value in stones.</returns>
	public static TNumber Stones<TNumber>(this Mass value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "stones");

	/// <summary>
	/// Converts a value to metric tons.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Mass"/> representing the value in metric tons.</returns>
	public static Mass MetricTons<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "metric_tons");

	/// <summary>
	/// Converts a <see cref="Mass"/> to a numeric value in metric tons.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Mass"/> to convert.</param>
	/// <returns>The numeric value in metric tons.</returns>
	public static TNumber MetricTons<TNumber>(this Mass value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "metric_tons");
}
