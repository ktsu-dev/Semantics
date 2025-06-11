// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a volume physical quantity.
/// </summary>
[SIUnit("m³", "cubic meter", "cubic meters")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
public sealed record Volume
	: PhysicalQuantity<Volume>
	, IIntegralOperators<Volume, Density, Mass>
{
	/// <summary>
	/// Multiplies a <see cref="Volume"/> by a <see cref="Density"/> to compute a <see cref="Mass"/>.
	/// </summary>
	/// <param name="left">The volume operand.</param>
	/// <param name="right">The density operand.</param>
	/// <returns>A <see cref="Mass"/> representing the result of the multiplication.</returns>
	public static Mass operator *(Volume left, Density right) =>
		IIntegralOperators<Volume, Density, Mass>.Integrate(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Volume"/>.
/// </summary>
public static class VolumeConversions
{
	/// <summary>
	/// Converts a value to cubic meters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Volume"/> representing the value in cubic meters.</returns>
	public static Volume CubicMeters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Volume>();

	/// <summary>
	/// Converts a <see cref="Volume"/> to a numeric value in cubic meters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Volume"/> to convert.</param>
	/// <returns>The numeric value in cubic meters.</returns>
	public static TNumber CubicMeters<TNumber>(this Volume value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to liters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Volume"/> representing the value in liters.</returns>
	public static Volume Liters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Volume>(0.001);

	/// <summary>
	/// Converts a <see cref="Volume"/> to a numeric value in liters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Volume"/> to convert.</param>
	/// <returns>The numeric value in liters.</returns>
	public static TNumber Liters<TNumber>(this Volume value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(0.001));

	/// <summary>
	/// Converts a value to milliliters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Volume"/> representing the value in milliliters.</returns>
	public static Volume Milliliters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Volume>(1e-6);

	/// <summary>
	/// Converts a <see cref="Volume"/> to a numeric value in milliliters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Volume"/> to convert.</param>
	/// <returns>The numeric value in milliliters.</returns>
	public static TNumber Milliliters<TNumber>(this Volume value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(1e-6));

	/// <summary>
	/// Converts a value to cubic centimeters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Volume"/> representing the value in cubic centimeters.</returns>
	public static Volume CubicCentimeters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Volume>(PhysicalConstants.Centi * PhysicalConstants.Centi * PhysicalConstants.Centi);

	/// <summary>
	/// Converts a <see cref="Volume"/> to a numeric value in cubic centimeters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Volume"/> to convert.</param>
	/// <returns>The numeric value in cubic centimeters.</returns>
	public static TNumber CubicCentimeters<TNumber>(this Volume value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Centi * PhysicalConstants.Centi * PhysicalConstants.Centi));

	// Imperial conversions
	/// <summary>
	/// Converts a value to cubic feet.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Volume"/> representing the value in cubic feet.</returns>
	public static Volume CubicFeet<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Volume>(Math.Pow(PhysicalConstants.FeetToMetersFactor, 3));

	/// <summary>
	/// Converts a <see cref="Volume"/> to a numeric value in cubic feet.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Volume"/> to convert.</param>
	/// <returns>The numeric value in cubic feet.</returns>
	public static TNumber CubicFeet<TNumber>(this Volume value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(Math.Pow(PhysicalConstants.FeetToMetersFactor, 3)));

	/// <summary>
	/// Converts a value to cubic inches.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Volume"/> representing the value in cubic inches.</returns>
	public static Volume CubicInches<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Volume>(Math.Pow(PhysicalConstants.InchesToMetersFactor, 3));

	/// <summary>
	/// Converts a <see cref="Volume"/> to a numeric value in cubic inches.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Volume"/> to convert.</param>
	/// <returns>The numeric value in cubic inches.</returns>
	public static TNumber CubicInches<TNumber>(this Volume value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(Math.Pow(PhysicalConstants.InchesToMetersFactor, 3)));

	/// <summary>
	/// Converts a value to gallons (US).
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Volume"/> representing the value in gallons.</returns>
	public static Volume Gallons<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Volume>(0.003785411784);

	/// <summary>
	/// Converts a <see cref="Volume"/> to a numeric value in gallons (US).
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Volume"/> to convert.</param>
	/// <returns>The numeric value in gallons.</returns>
	public static TNumber Gallons<TNumber>(this Volume value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(0.003785411784));
}
