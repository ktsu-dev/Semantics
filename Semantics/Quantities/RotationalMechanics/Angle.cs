// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an angle physical quantity.
/// </summary>
[SIUnit(typeof(SIUnits), nameof(SIUnits.Radian))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]

public sealed record Angle
	: PhysicalQuantity<Angle>
	, IDerivativeOperators<Angle, Time, AngularVelocity>
{
	/// <summary>
	/// Divides an <see cref="Angle"/> by a <see cref="Time"/> to compute an <see cref="AngularVelocity"/>.
	/// </summary>
	/// <param name="left">The angle operand.</param>
	/// <param name="right">The time operand.</param>
	/// <returns>An <see cref="AngularVelocity"/> representing the result of the division.</returns>
	public static AngularVelocity operator /(Angle left, Time right) =>
		IDerivativeOperators<Angle, Time, AngularVelocity>.Derive(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Angle"/>.
/// </summary>
public static class AngleConversions
{
	private static Conversions.IConversionCalculator<Angle> Calculator => Conversions.ConversionRegistry.GetCalculator<Angle>();

	/// <summary>
	/// Converts a value to radians.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Angle"/> representing the value in radians.</returns>
	public static Angle Radians<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "radians");

	/// <summary>
	/// Converts an <see cref="Angle"/> to a numeric value in radians.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Angle"/> to convert.</param>
	/// <returns>The numeric value in radians.</returns>
	public static TNumber Radians<TNumber>(this Angle value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "radians");

	/// <summary>
	/// Converts a value to degrees.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Angle"/> representing the value in degrees.</returns>
	public static Angle Degrees<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "degrees");

	/// <summary>
	/// Converts an <see cref="Angle"/> to a numeric value in degrees.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Angle"/> to convert.</param>
	/// <returns>The numeric value in degrees.</returns>
	public static TNumber Degrees<TNumber>(this Angle value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "degrees");

	/// <summary>
	/// Converts a value to gradians.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Angle"/> representing the value in gradians.</returns>
	public static Angle Gradians<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "gradians");

	/// <summary>
	/// Converts an <see cref="Angle"/> to a numeric value in gradians.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Angle"/> to convert.</param>
	/// <returns>The numeric value in gradians.</returns>
	public static TNumber Gradians<TNumber>(this Angle value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "gradians");

	/// <summary>
	/// Converts a value to arcminutes.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Angle"/> representing the value in arcminutes.</returns>
	public static Angle ArcMinutes<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "arcminutes");

	/// <summary>
	/// Converts an <see cref="Angle"/> to a numeric value in arcminutes.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Angle"/> to convert.</param>
	/// <returns>The numeric value in arcminutes.</returns>
	public static TNumber ArcMinutes<TNumber>(this Angle value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "arcminutes");

	/// <summary>
	/// Converts a value to arcseconds.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Angle"/> representing the value in arcseconds.</returns>
	public static Angle ArcSeconds<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "arcseconds");

	/// <summary>
	/// Converts an <see cref="Angle"/> to a numeric value in arcseconds.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Angle"/> to convert.</param>
	/// <returns>The numeric value in arcseconds.</returns>
	public static TNumber ArcSeconds<TNumber>(this Angle value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "arcseconds");

	/// <summary>
	/// Converts a value to revolutions.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Angle"/> representing the value in revolutions.</returns>
	public static Angle Revolutions<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "revolutions");

	/// <summary>
	/// Converts an <see cref="Angle"/> to a numeric value in revolutions.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Angle"/> to convert.</param>
	/// <returns>The numeric value in revolutions.</returns>
	public static TNumber Revolutions<TNumber>(this Angle value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "revolutions");

	/// <summary>
	/// Converts a value to cycles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Angle"/> representing the value in cycles.</returns>
	public static Angle Cycles<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "cycles");

	/// <summary>
	/// Converts an <see cref="Angle"/> to a numeric value in cycles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Angle"/> to convert.</param>
	/// <returns>The numeric value in cycles.</returns>
	public static TNumber Cycles<TNumber>(this Angle value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "cycles");

	/// <summary>
	/// Converts a value to turns.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Angle"/> representing the value in turns.</returns>
	public static Angle Turns<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "turns");

	/// <summary>
	/// Converts an <see cref="Angle"/> to a numeric value in turns.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Angle"/> to convert.</param>
	/// <returns>The numeric value in turns.</returns>
	public static TNumber Turns<TNumber>(this Angle value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "turns");
}
