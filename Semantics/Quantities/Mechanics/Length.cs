// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a length physical quantity.
/// </summary>
[SIUnit("m", "meter", "meters")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
public sealed record Length
	: PhysicalQuantity<Length>
{
	/// <summary>
	/// Multiplies two <see cref="Length"/> instances to compute an <see cref="Area"/>.
	/// </summary>
	/// <param name="left">The first <see cref="Length"/> operand.</param>
	/// <param name="right">The second <see cref="Length"/> operand.</param>
	/// <returns>An <see cref="Area"/> representing the product of the two lengths.</returns>
	public static Area operator *(Length left, Length right) =>
		Multiply<Area>(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Length"/>.
/// </summary>
public static class LengthConversions
{
	/// <summary>
	/// Converts a value to meters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in meters.</returns>
	public static Length Meters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Length>();

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in meters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in meters.</returns>
	public static TNumber Meters<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	// Metric prefixes
	/// <summary>
	/// Converts a value to kilometers.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in kilometers.</returns>
	public static Length Kilometers<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Length>(PhysicalConstants.Kilo);

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in kilometers.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in kilometers.</returns>
	public static TNumber Kilometers<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Kilo));

	/// <summary>
	/// Converts a value to centimeters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in centimeters.</returns>
	public static Length Centimeters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Length>(PhysicalConstants.Centi);

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in centimeters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in centimeters.</returns>
	public static TNumber Centimeters<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Centi));

	/// <summary>
	/// Converts a value to millimeters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in millimeters.</returns>
	public static Length Millimeters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Length>(PhysicalConstants.Milli);

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in millimeters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in millimeters.</returns>
	public static TNumber Millimeters<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Milli));

	/// <summary>
	/// Converts a value to micrometers.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in micrometers.</returns>
	public static Length Micrometers<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Length>(PhysicalConstants.Micro);

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in micrometers.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in micrometers.</returns>
	public static TNumber Micrometers<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Micro));

	/// <summary>
	/// Converts a value to nanometers.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in nanometers.</returns>
	public static Length Nanometers<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Length>(PhysicalConstants.Nano);

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in nanometers.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in nanometers.</returns>
	public static TNumber Nanometers<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Nano));

	// Imperial and other conversions
	/// <summary>
	/// Converts a value to feet.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in feet.</returns>
	public static Length Feet<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Length>(PhysicalConstants.FeetToMetersFactor);

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in feet.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in feet.</returns>
	public static TNumber Feet<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.FeetToMetersFactor));

	/// <summary>
	/// Converts a value to inches.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in inches.</returns>
	public static Length Inches<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Length>(PhysicalConstants.InchesToMetersFactor);

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in inches.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in inches.</returns>
	public static TNumber Inches<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.InchesToMetersFactor));

	/// <summary>
	/// Converts a value to yards.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in yards.</returns>
	public static Length Yards<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Length>(PhysicalConstants.YardsToMetersFactor);

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in yards.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in yards.</returns>
	public static TNumber Yards<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.YardsToMetersFactor));

	/// <summary>
	/// Converts a value to miles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in miles.</returns>
	public static Length Miles<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Length>(PhysicalConstants.MilesToMetersFactor);

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in miles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in miles.</returns>
	public static TNumber Miles<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.MilesToMetersFactor));

	/// <summary>
	/// Converts a value to nautical miles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in nautical miles.</returns>
	public static Length NauticalMiles<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Length>(PhysicalConstants.NauticalMilesToMetersFactor);

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in nautical miles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in nautical miles.</returns>
	public static TNumber NauticalMiles<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.NauticalMilesToMetersFactor));

	/// <summary>
	/// Converts a value to fathoms.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in fathoms.</returns>
	public static Length Fathoms<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Length>(PhysicalConstants.FathomsToMetersFactor);

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in fathoms.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in fathoms.</returns>
	public static TNumber Fathoms<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.FathomsToMetersFactor));
}
