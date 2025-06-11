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
	/// <summary>
	/// Converts a value to square meters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Area"/> representing the value in square meters.</returns>
	public static Area SquareMeters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Area>();

	/// <summary>
	/// Converts an <see cref="Area"/> to a numeric value in square meters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Area"/> to convert.</param>
	/// <returns>The numeric value in square meters.</returns>
	public static TNumber SquareMeters<TNumber>(this Area value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to square kilometers.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Area"/> representing the value in square kilometers.</returns>
	public static Area SquareKilometers<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Area>(PhysicalConstants.Kilo * PhysicalConstants.Kilo);

	/// <summary>
	/// Converts an <see cref="Area"/> to a numeric value in square kilometers.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Area"/> to convert.</param>
	/// <returns>The numeric value in square kilometers.</returns>
	public static TNumber SquareKilometers<TNumber>(this Area value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Kilo * PhysicalConstants.Kilo));

	/// <summary>
	/// Converts a value to square centimeters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Area"/> representing the value in square centimeters.</returns>
	public static Area SquareCentimeters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Area>(PhysicalConstants.Centi * PhysicalConstants.Centi);

	/// <summary>
	/// Converts an <see cref="Area"/> to a numeric value in square centimeters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Area"/> to convert.</param>
	/// <returns>The numeric value in square centimeters.</returns>
	public static TNumber SquareCentimeters<TNumber>(this Area value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Centi * PhysicalConstants.Centi));

	/// <summary>
	/// Converts a value to square millimeters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Area"/> representing the value in square millimeters.</returns>
	public static Area SquareMillimeters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Area>(PhysicalConstants.Milli * PhysicalConstants.Milli);

	/// <summary>
	/// Converts an <see cref="Area"/> to a numeric value in square millimeters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Area"/> to convert.</param>
	/// <returns>The numeric value in square millimeters.</returns>
	public static TNumber SquareMillimeters<TNumber>(this Area value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Milli * PhysicalConstants.Milli));

	// Imperial and other conversions
	/// <summary>
	/// Converts a value to square feet.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Area"/> representing the value in square feet.</returns>
	public static Area SquareFeet<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Area>(PhysicalConstants.FeetToMetersFactor * PhysicalConstants.FeetToMetersFactor);

	/// <summary>
	/// Converts an <see cref="Area"/> to a numeric value in square feet.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Area"/> to convert.</param>
	/// <returns>The numeric value in square feet.</returns>
	public static TNumber SquareFeet<TNumber>(this Area value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.FeetToMetersFactor * PhysicalConstants.FeetToMetersFactor));

	/// <summary>
	/// Converts a value to square inches.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Area"/> representing the value in square inches.</returns>
	public static Area SquareInches<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Area>(PhysicalConstants.InchesToMetersFactor * PhysicalConstants.InchesToMetersFactor);

	/// <summary>
	/// Converts an <see cref="Area"/> to a numeric value in square inches.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Area"/> to convert.</param>
	/// <returns>The numeric value in square inches.</returns>
	public static TNumber SquareInches<TNumber>(this Area value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.InchesToMetersFactor * PhysicalConstants.InchesToMetersFactor));

	/// <summary>
	/// Converts a value to acres.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Area"/> representing the value in acres.</returns>
	public static Area Acres<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Area>(PhysicalConstants.AcresToSquareMetersFactor);

	/// <summary>
	/// Converts an <see cref="Area"/> to a numeric value in acres.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Area"/> to convert.</param>
	/// <returns>The numeric value in acres.</returns>
	public static TNumber Acres<TNumber>(this Area value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.AcresToSquareMetersFactor));
}
