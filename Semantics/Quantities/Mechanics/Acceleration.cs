// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an acceleration physical quantity.
/// </summary>
[SIUnit("m/s²", "meter per second squared", "meters per second squared")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
public sealed record Acceleration
	: PhysicalQuantity<Acceleration>
	, IIntegralOperators<Acceleration, Mass, Force>
	, IDerivativeOperators<Acceleration, Time, Jerk>
	, IIntegralOperators<Acceleration, Time, Velocity>
{
	/// <summary>
	/// Multiplies an <see cref="Acceleration"/> by a <see cref="Mass"/> to compute a <see cref="Force"/>.
	/// </summary>
	/// <param name="left">The acceleration operand.</param>
	/// <param name="right">The mass operand.</param>
	/// <returns>A <see cref="Force"/> representing the result of the multiplication.</returns>
	public static Force operator *(Acceleration left, Mass right) =>
		IIntegralOperators<Acceleration, Mass, Force>.Integrate(left, right);

	/// <summary>
	/// Divides an <see cref="Acceleration"/> by a <see cref="Time"/> to compute a <see cref="Jerk"/>.
	/// </summary>
	/// <param name="left">The acceleration operand.</param>
	/// <param name="right">The time operand.</param>
	/// <returns>A <see cref="Jerk"/> representing the result of the division.</returns>
	public static Jerk operator /(Acceleration left, Time right) =>
		IDerivativeOperators<Acceleration, Time, Jerk>.Derive(left, right);

	/// <summary>
	/// Multiplies an <see cref="Acceleration"/> by a <see cref="Time"/> to compute a <see cref="Velocity"/>.
	/// </summary>
	/// <param name="left">The acceleration operand.</param>
	/// <param name="right">The time operand.</param>
	/// <returns>A <see cref="Velocity"/> representing the result of the multiplication.</returns>
	public static Velocity operator *(Acceleration left, Time right) =>
		IIntegralOperators<Acceleration, Time, Velocity>.Integrate(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Acceleration"/>.
/// </summary>
public static class AccelerationConversions
{
	private static Conversions.IConversionCalculator<Acceleration> Calculator => Conversions.ConversionRegistry.GetCalculator<Acceleration>();

	/// <summary>
	/// Converts a value to meters per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Acceleration"/> representing the value in meters per second squared.</returns>
	public static Acceleration MetersPerSecondSquared<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "meters_per_second_squared");

	/// <summary>
	/// Converts an <see cref="Acceleration"/> to a numeric value in meters per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Acceleration"/> to convert.</param>
	/// <returns>The numeric value in meters per second squared.</returns>
	public static TNumber MetersPerSecondSquared<TNumber>(this Acceleration value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "meters_per_second_squared");

	/// <summary>
	/// Converts a value to feet per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Acceleration"/> representing the value in feet per second squared.</returns>
	public static Acceleration FeetPerSecondSquared<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "feet_per_second_squared");

	/// <summary>
	/// Converts an <see cref="Acceleration"/> to a numeric value in feet per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Acceleration"/> to convert.</param>
	/// <returns>The numeric value in feet per second squared.</returns>
	public static TNumber FeetPerSecondSquared<TNumber>(this Acceleration value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "feet_per_second_squared");

	/// <summary>
	/// Converts a value to inches per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Acceleration"/> representing the value in inches per second squared.</returns>
	public static Acceleration InchesPerSecondSquared<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "inches_per_second_squared");

	/// <summary>
	/// Converts an <see cref="Acceleration"/> to a numeric value in inches per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Acceleration"/> to convert.</param>
	/// <returns>The numeric value in inches per second squared.</returns>
	public static TNumber InchesPerSecondSquared<TNumber>(this Acceleration value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "inches_per_second_squared");

	/// <summary>
	/// Converts a value to standard gravity (g).
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Acceleration"/> representing the value in standard gravitational acceleration.</returns>
	public static Acceleration StandardGravity<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "g");

	/// <summary>
	/// Converts an <see cref="Acceleration"/> to a numeric value in standard gravity (g).
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Acceleration"/> to convert.</param>
	/// <returns>The numeric value in standard gravitational acceleration.</returns>
	public static TNumber StandardGravity<TNumber>(this Acceleration value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "g");

	/// <summary>
	/// Converts a value to yards per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Acceleration"/> representing the value in yards per second squared.</returns>
	public static Acceleration YardsPerSecondSquared<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "yards_per_second_squared");

	/// <summary>
	/// Converts an <see cref="Acceleration"/> to a numeric value in yards per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Acceleration"/> to convert.</param>
	/// <returns>The numeric value in yards per second squared.</returns>
	public static TNumber YardsPerSecondSquared<TNumber>(this Acceleration value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "yards_per_second_squared");

	/// <summary>
	/// Converts a value to miles per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Acceleration"/> representing the value in miles per second squared.</returns>
	public static Acceleration MilesPerSecondSquared<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "miles_per_second_squared");

	/// <summary>
	/// Converts an <see cref="Acceleration"/> to a numeric value in miles per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Acceleration"/> to convert.</param>
	/// <returns>The numeric value in miles per second squared.</returns>
	public static TNumber MilesPerSecondSquared<TNumber>(this Acceleration value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "miles_per_second_squared");

	/// <summary>
	/// Converts a value to nautical miles per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Acceleration"/> representing the value in nautical miles per second squared.</returns>
	public static Acceleration NauticalMilesPerSecondSquared<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "nautical_miles_per_second_squared");

	/// <summary>
	/// Converts an <see cref="Acceleration"/> to a numeric value in nautical miles per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Acceleration"/> to convert.</param>
	/// <returns>The numeric value in nautical miles per second squared.</returns>
	public static TNumber NauticalMilesPerSecondSquared<TNumber>(this Acceleration value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "nautical_miles_per_second_squared");

	/// <summary>
	/// Converts a value to fathoms per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Acceleration"/> representing the value in fathoms per second squared.</returns>
	public static Acceleration FathomsPerSecondSquared<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "fathoms_per_second_squared");

	/// <summary>
	/// Converts an <see cref="Acceleration"/> to a numeric value in fathoms per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Acceleration"/> to convert.</param>
	/// <returns>The numeric value in fathoms per second squared.</returns>
	public static TNumber FathomsPerSecondSquared<TNumber>(this Acceleration value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "fathoms_per_second_squared");
}
