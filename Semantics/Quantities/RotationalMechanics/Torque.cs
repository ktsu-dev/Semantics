// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a torque physical quantity.
/// </summary>
[SIUnit(typeof(SIUnits), nameof(SIUnits.NewtonMeter))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]

public sealed record Torque
	: PhysicalQuantity<Torque>
	, IIntegralOperators<Torque, Angle, Energy>
{
	/// <summary>
	/// Multiplies a <see cref="Torque"/> by an <see cref="Angle"/> to compute an <see cref="Energy"/>.
	/// </summary>
	/// <param name="left">The torque operand.</param>
	/// <param name="right">The angle operand.</param>
	/// <returns>An <see cref="Energy"/> representing the result of the multiplication.</returns>
	public static Energy operator *(Torque left, Angle right) =>
		IIntegralOperators<Torque, Angle, Energy>.Integrate(left, right);

	// Note: Additional operators for relationships with Force and Length will be added as those become available with cross-product operations
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Torque"/>.
/// </summary>
public static class TorqueConversions
{
	/// <summary>
	/// Converts a value to newton meters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Torque"/> representing the value in newton meters.</returns>
	public static Torque NewtonMeters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Torque>();

	/// <summary>
	/// Converts a <see cref="Torque"/> to a numeric value in newton meters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Torque"/> to convert.</param>
	/// <returns>The numeric value in newton meters.</returns>
	public static TNumber NewtonMeters<TNumber>(this Torque value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to foot-pounds.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Torque"/> representing the value in foot-pounds.</returns>
	public static Torque FootPounds<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Torque>(PhysicalConstants.FootPoundsToNewtonMetersFactor);

	/// <summary>
	/// Converts a <see cref="Torque"/> to a numeric value in foot-pounds.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Torque"/> to convert.</param>
	/// <returns>The numeric value in foot-pounds.</returns>
	public static TNumber FootPounds<TNumber>(this Torque value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.FootPoundsToNewtonMetersFactor));

	/// <summary>
	/// Converts a value to pound-inches.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Torque"/> representing the value in pound-inches.</returns>
	public static Torque PoundInches<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Torque>(PhysicalConstants.PoundInchesToNewtonMetersFactor);

	/// <summary>
	/// Converts a <see cref="Torque"/> to a numeric value in pound-inches.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Torque"/> to convert.</param>
	/// <returns>The numeric value in pound-inches.</returns>
	public static TNumber PoundInches<TNumber>(this Torque value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.PoundInchesToNewtonMetersFactor));
}
