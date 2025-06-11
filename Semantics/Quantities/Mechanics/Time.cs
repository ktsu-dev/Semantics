// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a time physical quantity.
/// </summary>
[SIUnit("s", "second", "seconds")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
public sealed record Time
	: PhysicalQuantity<Time>
{
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Time"/>.
/// </summary>
public static class TimeConversions
{
	private static Conversions.IConversionCalculator<Time> Calculator => Conversions.ConversionRegistry.GetCalculator<Time>();

	/// <summary>
	/// Converts a value to seconds.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Time"/> representing the value in seconds.</returns>
	public static Time Seconds<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "seconds");

	/// <summary>
	/// Converts a <see cref="Time"/> to a numeric value in seconds.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Time"/> to convert.</param>
	/// <returns>The numeric value in seconds.</returns>
	public static TNumber Seconds<TNumber>(this Time value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "seconds");

	/// <summary>
	/// Converts a value to milliseconds.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Time"/> representing the value in milliseconds.</returns>
	public static Time Milliseconds<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "milliseconds");

	/// <summary>
	/// Converts a <see cref="Time"/> to a numeric value in milliseconds.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Time"/> to convert.</param>
	/// <returns>The numeric value in milliseconds.</returns>
	public static TNumber Milliseconds<TNumber>(this Time value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "milliseconds");

	/// <summary>
	/// Converts a value to microseconds.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Time"/> representing the value in microseconds.</returns>
	public static Time Microseconds<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "microseconds");

	/// <summary>
	/// Converts a <see cref="Time"/> to a numeric value in microseconds.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Time"/> to convert.</param>
	/// <returns>The numeric value in microseconds.</returns>
	public static TNumber Microseconds<TNumber>(this Time value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "microseconds");

	/// <summary>
	/// Converts a value to nanoseconds.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Time"/> representing the value in nanoseconds.</returns>
	public static Time Nanoseconds<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "nanoseconds");

	/// <summary>
	/// Converts a <see cref="Time"/> to a numeric value in nanoseconds.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Time"/> to convert.</param>
	/// <returns>The numeric value in nanoseconds.</returns>
	public static TNumber Nanoseconds<TNumber>(this Time value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "nanoseconds");

	/// <summary>
	/// Converts a value to minutes.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Time"/> representing the value in minutes.</returns>
	public static Time Minutes<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "minutes");

	/// <summary>
	/// Converts a <see cref="Time"/> to a numeric value in minutes.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Time"/> to convert.</param>
	/// <returns>The numeric value in minutes.</returns>
	public static TNumber Minutes<TNumber>(this Time value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "minutes");

	/// <summary>
	/// Converts a value to hours.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Time"/> representing the value in hours.</returns>
	public static Time Hours<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "hours");

	/// <summary>
	/// Converts a <see cref="Time"/> to a numeric value in hours.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Time"/> to convert.</param>
	/// <returns>The numeric value in hours.</returns>
	public static TNumber Hours<TNumber>(this Time value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "hours");

	/// <summary>
	/// Converts a value to days.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Time"/> representing the value in days.</returns>
	public static Time Days<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "days");

	/// <summary>
	/// Converts a <see cref="Time"/> to a numeric value in days.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Time"/> to convert.</param>
	/// <returns>The numeric value in days.</returns>
	public static TNumber Days<TNumber>(this Time value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "days");

	/// <summary>
	/// Converts a value to years.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Time"/> representing the value in years.</returns>
	public static Time Years<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "years");

	/// <summary>
	/// Converts a <see cref="Time"/> to a numeric value in years.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Time"/> to convert.</param>
	/// <returns>The numeric value in years.</returns>
	public static TNumber Years<TNumber>(this Time value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "years");
}
