// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a jerk physical quantity.
/// </summary>
[SIUnit("m/s³", "meter per second cubed", "meters per second cubed")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
[Obsolete]
public sealed record Jerk
	: PhysicalQuantity<Jerk>
	, IIntegralOperators<Jerk, Time, Acceleration>
{
	/// <summary>
	/// Multiplies a <see cref="Jerk"/> by a <see cref="Time"/> to compute an <see cref="Acceleration"/>.
	/// </summary>
	/// <param name="left">The jerk operand.</param>
	/// <param name="right">The time operand.</param>
	/// <returns>An <see cref="Acceleration"/> representing the result of the multiplication.</returns>
	public static Acceleration operator *(Jerk left, Time right) =>
		IIntegralOperators<Jerk, Time, Acceleration>.Integrate(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Jerk"/>.
/// </summary>
public static class JerkConversions
{
	/// <summary>
	/// Converts a value to meters per second cubed.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Jerk"/> representing the value in meters per second cubed.</returns>
	public static Jerk MetersPerSecondCubed<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Jerk>();

	/// <summary>
	/// Converts a <see cref="Jerk"/> to a numeric value in meters per second cubed.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Jerk"/> to convert.</param>
	/// <returns>The numeric value in meters per second cubed.</returns>
	public static TNumber MetersPerSecondCubed<TNumber>(this Jerk value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to feet per second cubed.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Jerk"/> representing the value in feet per second cubed.</returns>
	public static Jerk FeetPerSecondCubed<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Jerk>(PhysicalConstants.FeetToMetersFactor);

	/// <summary>
	/// Converts a <see cref="Jerk"/> to a numeric value in feet per second cubed.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Jerk"/> to convert.</param>
	/// <returns>The numeric value in feet per second cubed.</returns>
	public static TNumber FeetPerSecondCubed<TNumber>(this Jerk value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.FeetToMetersFactor));

	/// <summary>
	/// Converts a value to inches per second cubed.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Jerk"/> representing the value in inches per second cubed.</returns>
	public static Jerk InchesPerSecondCubed<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Jerk>(PhysicalConstants.InchesToMetersFactor);

	/// <summary>
	/// Converts a <see cref="Jerk"/> to a numeric value in inches per second cubed.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Jerk"/> to convert.</param>
	/// <returns>The numeric value in inches per second cubed.</returns>
	public static TNumber InchesPerSecondCubed<TNumber>(this Jerk value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.InchesToMetersFactor));

	/// <summary>
	/// Converts a value to yards per second cubed.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Jerk"/> representing the value in yards per second cubed.</returns>
	public static Jerk YardsPerSecondCubed<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Jerk>(PhysicalConstants.YardsToMetersFactor);

	/// <summary>
	/// Converts a <see cref="Jerk"/> to a numeric value in yards per second cubed.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Jerk"/> to convert.</param>
	/// <returns>The numeric value in yards per second cubed.</returns>
	public static TNumber YardsPerSecondCubed<TNumber>(this Jerk value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.YardsToMetersFactor));

	/// <summary>
	/// Converts a value to miles per second cubed.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Jerk"/> representing the value in miles per second cubed.</returns>
	public static Jerk MilesPerSecondCubed<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Jerk>(PhysicalConstants.MilesToMetersFactor);

	/// <summary>
	/// Converts a <see cref="Jerk"/> to a numeric value in miles per second cubed.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Jerk"/> to convert.</param>
	/// <returns>The numeric value in miles per second cubed.</returns>
	public static TNumber MilesPerSecondCubed<TNumber>(this Jerk value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.MilesToMetersFactor));
}
