// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a length physical quantity.
/// </summary>
[SIUnit(typeof(SIUnits), nameof(SIUnits.Meter))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
public sealed record Length
	: PhysicalQuantity<Length>
	, IDerivativeOperators<Length, Time, Velocity>
{
	/// <summary>
	/// Multiplies two <see cref="Length"/> instances to compute an <see cref="Area"/>.
	/// </summary>
	/// <param name="left">The first <see cref="Length"/> operand.</param>
	/// <param name="right">The second <see cref="Length"/> operand.</param>
	/// <returns>An <see cref="Area"/> representing the product of the two lengths.</returns>
	public static Area operator *(Length left, Length right) =>
		Multiply<Area>(left, right);

	/// <summary>
	/// Divides a <see cref="Length"/> by a <see cref="Time"/> to compute a <see cref="Velocity"/>.
	/// </summary>
	/// <param name="left">The length operand.</param>
	/// <param name="right">The time operand.</param>
	/// <returns>A <see cref="Velocity"/> representing the result of the division.</returns>
	public static Velocity operator /(Length left, Time right) =>
		IDerivativeOperators<Length, Time, Velocity>.Derive(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Length"/>.
/// </summary>
public static class LengthConversions
{
	private static Conversions.IConversionCalculator<Length> Calculator => Conversions.ConversionRegistry.GetCalculator<Length>();

	/// <summary>
	/// Converts a value to meters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in meters.</returns>
	public static Length Meters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "meters");

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in meters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in meters.</returns>
	public static TNumber Meters<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "meters");

	// Metric prefixes
	/// <summary>
	/// Converts a value to kilometers.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in kilometers.</returns>
	public static Length Kilometers<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "kilometers");

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in kilometers.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in kilometers.</returns>
	public static TNumber Kilometers<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "kilometers");

	/// <summary>
	/// Converts a value to centimeters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in centimeters.</returns>
	public static Length Centimeters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "centimeters");

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in centimeters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in centimeters.</returns>
	public static TNumber Centimeters<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "centimeters");

	/// <summary>
	/// Converts a value to millimeters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in millimeters.</returns>
	public static Length Millimeters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "millimeters");

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in millimeters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in millimeters.</returns>
	public static TNumber Millimeters<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "millimeters");

	/// <summary>
	/// Converts a value to micrometers.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in micrometers.</returns>
	public static Length Micrometers<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "micrometers");

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in micrometers.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in micrometers.</returns>
	public static TNumber Micrometers<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "micrometers");

	/// <summary>
	/// Converts a value to nanometers.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in nanometers.</returns>
	public static Length Nanometers<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "nanometers");

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in nanometers.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in nanometers.</returns>
	public static TNumber Nanometers<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "nanometers");

	// Imperial and other conversions
	/// <summary>
	/// Converts a value to feet.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in feet.</returns>
	public static Length Feet<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "feet");

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in feet.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in feet.</returns>
	public static TNumber Feet<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "feet");

	/// <summary>
	/// Converts a value to inches.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in inches.</returns>
	public static Length Inches<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "inches");

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in inches.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in inches.</returns>
	public static TNumber Inches<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "inches");

	/// <summary>
	/// Converts a value to yards.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in yards.</returns>
	public static Length Yards<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "yards");

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in yards.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in yards.</returns>
	public static TNumber Yards<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "yards");

	/// <summary>
	/// Converts a value to miles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in miles.</returns>
	public static Length Miles<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "miles");

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in miles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in miles.</returns>
	public static TNumber Miles<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "miles");

	/// <summary>
	/// Converts a value to nautical miles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in nautical miles.</returns>
	public static Length NauticalMiles<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "nautical_miles");

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in nautical miles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in nautical miles.</returns>
	public static TNumber NauticalMiles<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "nautical_miles");

	/// <summary>
	/// Converts a value to fathoms.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Length"/> representing the value in fathoms.</returns>
	public static Length Fathoms<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "fathoms");

	/// <summary>
	/// Converts a <see cref="Length"/> to a numeric value in fathoms.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Length"/> to convert.</param>
	/// <returns>The numeric value in fathoms.</returns>
	public static TNumber Fathoms<TNumber>(this Length value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "fathoms");
}
