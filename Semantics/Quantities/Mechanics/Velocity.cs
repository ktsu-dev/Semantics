// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a velocity physical quantity.
/// </summary>
[SIUnit(typeof(SIUnits), nameof(SIUnits.MeterPerSecond))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]

public sealed record Velocity
	: PhysicalQuantity<Velocity>
	, IDerivativeOperators<Velocity, Time, Acceleration>
	, IIntegralOperators<Velocity, Time, Length>
	, IIntegralOperators<Velocity, Mass, Momentum>
{
	/// <summary>
	/// Divides a <see cref="Velocity"/> by a <see cref="Time"/> to compute an <see cref="Acceleration"/>.
	/// </summary>
	/// <param name="left">The velocity operand.</param>
	/// <param name="right">The time operand.</param>
	/// <returns>An <see cref="Acceleration"/> representing the result of the division.</returns>
	public static Acceleration operator /(Velocity left, Time right) =>
		IDerivativeOperators<Velocity, Time, Acceleration>.Derive(left, right);

	/// <summary>
	/// Multiplies a <see cref="Velocity"/> by a <see cref="Time"/> to compute a <see cref="Length"/>.
	/// </summary>
	/// <param name="left">The velocity operand.</param>
	/// <param name="right">The time operand.</param>
	/// <returns>A <see cref="Length"/> representing the result of the multiplication.</returns>
	public static Length operator *(Velocity left, Time right) =>
		IIntegralOperators<Velocity, Time, Length>.Integrate(left, right);

	/// <summary>
	/// Multiplies a <see cref="Time"/> by a <see cref="Velocity"/> to compute a <see cref="Length"/>.
	/// </summary>
	/// <param name="left">The time operand.</param>
	/// <param name="right">The velocity operand.</param>
	/// <returns>A <see cref="Length"/> representing the result of the multiplication.</returns>
	public static Length operator *(Time left, Velocity right) =>
		IIntegralOperators<Velocity, Time, Length>.Integrate(right, left);

	/// <summary>
	/// Multiplies a <see cref="Velocity"/> by a <see cref="Mass"/> to compute a <see cref="Momentum"/>.
	/// </summary>
	/// <param name="left">The velocity operand.</param>
	/// <param name="right">The mass operand.</param>
	/// <returns>A <see cref="Momentum"/> representing the result of the multiplication.</returns>
	public static Momentum operator *(Velocity left, Mass right) =>
		IIntegralOperators<Velocity, Mass, Momentum>.Integrate(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Velocity"/>.
/// </summary>
public static class VelocityConversions
{
	private static Conversions.IConversionCalculator<Velocity> Calculator => Conversions.ConversionRegistry.GetCalculator<Velocity>();

	/// <summary>
	/// Converts a value to meters per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Velocity"/> representing the value in meters per second.</returns>
	public static Velocity MetersPerSecond<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "meters_per_second");

	/// <summary>
	/// Converts a <see cref="Velocity"/> to a numeric value in meters per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Velocity"/> to convert.</param>
	/// <returns>The numeric value in meters per second.</returns>
	public static TNumber MetersPerSecond<TNumber>(this Velocity value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "meters_per_second");

	/// <summary>
	/// Converts a value to feet per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Velocity"/> representing the value in feet per second.</returns>
	public static Velocity FeetPerSecond<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "feet_per_second");

	/// <summary>
	/// Converts a <see cref="Velocity"/> to a numeric value in feet per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Velocity"/> to convert.</param>
	/// <returns>The numeric value in feet per second.</returns>
	public static TNumber FeetPerSecond<TNumber>(this Velocity value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "feet_per_second");

	/// <summary>
	/// Converts a value to inches per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Velocity"/> representing the value in inches per second.</returns>
	public static Velocity InchesPerSecond<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "inches_per_second");

	/// <summary>
	/// Converts a <see cref="Velocity"/> to a numeric value in inches per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Velocity"/> to convert.</param>
	/// <returns>The numeric value in inches per second.</returns>
	public static TNumber InchesPerSecond<TNumber>(this Velocity value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "inches_per_second");

	/// <summary>
	/// Converts a value to miles per hour.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Velocity"/> representing the value in miles per hour.</returns>
	public static Velocity MilesPerHour<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "miles_per_hour");

	/// <summary>
	/// Converts a <see cref="Velocity"/> to a numeric value in miles per hour.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Velocity"/> to convert.</param>
	/// <returns>The numeric value in miles per hour.</returns>
	public static TNumber MilesPerHour<TNumber>(this Velocity value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "miles_per_hour");

	/// <summary>
	/// Converts a value to kilometers per hour.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Velocity"/> representing the value in kilometers per hour.</returns>
	public static Velocity KilometersPerHour<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "kilometers_per_hour");

	/// <summary>
	/// Converts a <see cref="Velocity"/> to a numeric value in kilometers per hour.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Velocity"/> to convert.</param>
	/// <returns>The numeric value in kilometers per hour.</returns>
	public static TNumber KilometersPerHour<TNumber>(this Velocity value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "kilometers_per_hour");

	/// <summary>
	/// Converts a value to knots.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Velocity"/> representing the value in knots.</returns>
	public static Velocity Knots<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "knots");

	/// <summary>
	/// Converts a <see cref="Velocity"/> to a numeric value in knots.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Velocity"/> to convert.</param>
	/// <returns>The numeric value in knots.</returns>
	public static TNumber Knots<TNumber>(this Velocity value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "knots");
}
