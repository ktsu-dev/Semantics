// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an area physical quantity.
/// </summary>
[SIUnit("m²", "square meter", "square meters")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
public sealed record Area
	: PhysicalQuantity<Area>
	, IIntegralOperators<Area, Illuminance, LuminousFlux>
{
	/// <summary>
	/// Multiplies an <see cref="Area"/> and a <see cref="Length"/> to compute a <see cref="Volume"/>.
	/// </summary>
	/// <param name="left">The <see cref="Area"/> operand.</param>
	/// <param name="right">The <see cref="Length"/> operand.</param>
	/// <returns>A <see cref="Volume"/> representing the product of the area and length.</returns>
	public static Volume operator *(Area left, Length right) =>
		Multiply<Volume>(left, right);

	/// <summary>
	/// Multiplies an <see cref="Area"/> by an <see cref="Illuminance"/> to compute a <see cref="LuminousFlux"/>.
	/// </summary>
	/// <param name="left">The area operand.</param>
	/// <param name="right">The illuminance operand.</param>
	/// <returns>A <see cref="LuminousFlux"/> representing the result of the multiplication.</returns>
	public static LuminousFlux operator *(Area left, Illuminance right) =>
		IIntegralOperators<Area, Illuminance, LuminousFlux>.Integrate(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Area"/>.
/// </summary>
public static class AreaConversions
{
	private static Conversions.IConversionCalculator<Area> Calculator => Conversions.ConversionRegistry.GetCalculator<Area>();

	/// <summary>
	/// Converts a value to square meters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Area"/> representing the value in square meters.</returns>
	public static Area SquareMeters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "square_meters");

	/// <summary>
	/// Converts an <see cref="Area"/> to a numeric value in square meters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Area"/> to convert.</param>
	/// <returns>The numeric value in square meters.</returns>
	public static TNumber SquareMeters<TNumber>(this Area value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "square_meters");

	/// <summary>
	/// Converts a value to square kilometers.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Area"/> representing the value in square kilometers.</returns>
	public static Area SquareKilometers<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "square_kilometers");

	/// <summary>
	/// Converts an <see cref="Area"/> to a numeric value in square kilometers.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Area"/> to convert.</param>
	/// <returns>The numeric value in square kilometers.</returns>
	public static TNumber SquareKilometers<TNumber>(this Area value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "square_kilometers");

	/// <summary>
	/// Converts a value to square centimeters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Area"/> representing the value in square centimeters.</returns>
	public static Area SquareCentimeters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "square_centimeters");

	/// <summary>
	/// Converts an <see cref="Area"/> to a numeric value in square centimeters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Area"/> to convert.</param>
	/// <returns>The numeric value in square centimeters.</returns>
	public static TNumber SquareCentimeters<TNumber>(this Area value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "square_centimeters");

	/// <summary>
	/// Converts a value to square millimeters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Area"/> representing the value in square millimeters.</returns>
	public static Area SquareMillimeters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "square_millimeters");

	/// <summary>
	/// Converts an <see cref="Area"/> to a numeric value in square millimeters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Area"/> to convert.</param>
	/// <returns>The numeric value in square millimeters.</returns>
	public static TNumber SquareMillimeters<TNumber>(this Area value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "square_millimeters");

	// Imperial and other conversions
	/// <summary>
	/// Converts a value to square feet.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Area"/> representing the value in square feet.</returns>
	public static Area SquareFeet<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "square_feet");

	/// <summary>
	/// Converts an <see cref="Area"/> to a numeric value in square feet.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Area"/> to convert.</param>
	/// <returns>The numeric value in square feet.</returns>
	public static TNumber SquareFeet<TNumber>(this Area value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "square_feet");

	/// <summary>
	/// Converts a value to square inches.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Area"/> representing the value in square inches.</returns>
	public static Area SquareInches<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "square_inches");

	/// <summary>
	/// Converts an <see cref="Area"/> to a numeric value in square inches.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Area"/> to convert.</param>
	/// <returns>The numeric value in square inches.</returns>
	public static TNumber SquareInches<TNumber>(this Area value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "square_inches");

	/// <summary>
	/// Converts a value to square yards.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Area"/> representing the value in square yards.</returns>
	public static Area SquareYards<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "square_yards");

	/// <summary>
	/// Converts an <see cref="Area"/> to a numeric value in square yards.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Area"/> to convert.</param>
	/// <returns>The numeric value in square yards.</returns>
	public static TNumber SquareYards<TNumber>(this Area value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "square_yards");

	/// <summary>
	/// Converts a value to acres.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Area"/> representing the value in acres.</returns>
	public static Area Acres<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "acres");

	/// <summary>
	/// Converts an <see cref="Area"/> to a numeric value in acres.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Area"/> to convert.</param>
	/// <returns>The numeric value in acres.</returns>
	public static TNumber Acres<TNumber>(this Area value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "acres");

	/// <summary>
	/// Converts a value to hectares.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Area"/> representing the value in hectares.</returns>
	public static Area Hectares<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "hectares");

	/// <summary>
	/// Converts an <see cref="Area"/> to a numeric value in hectares.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Area"/> to convert.</param>
	/// <returns>The numeric value in hectares.</returns>
	public static TNumber Hectares<TNumber>(this Area value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "hectares");
}
