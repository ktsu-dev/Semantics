// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an angular velocity physical quantity.
/// </summary>
[SIUnit(typeof(SIUnits), nameof(SIUnits.RadianPerSecond))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]

public sealed record AngularVelocity
	: PhysicalQuantity<AngularVelocity>
	, IIntegralOperators<AngularVelocity, Time, Angle>
	, IDerivativeOperators<AngularVelocity, Time, AngularAcceleration>
{
	/// <summary>
	/// Multiplies an <see cref="AngularVelocity"/> by a <see cref="Time"/> to compute an <see cref="Angle"/>.
	/// </summary>
	/// <param name="left">The angular velocity operand.</param>
	/// <param name="right">The time operand.</param>
	/// <returns>An <see cref="Angle"/> representing the result of the multiplication.</returns>
	public static Angle operator *(AngularVelocity left, Time right) =>
		IIntegralOperators<AngularVelocity, Time, Angle>.Integrate(left, right);

	/// <summary>
	/// Divides an <see cref="AngularVelocity"/> by a <see cref="Time"/> to compute an <see cref="AngularAcceleration"/>.
	/// </summary>
	/// <param name="left">The angular velocity operand.</param>
	/// <param name="right">The time operand.</param>
	/// <returns>An <see cref="AngularAcceleration"/> representing the result of the division.</returns>
	public static AngularAcceleration operator /(AngularVelocity left, Time right) =>
		IDerivativeOperators<AngularVelocity, Time, AngularAcceleration>.Derive(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="AngularVelocity"/>.
/// </summary>
public static class AngularVelocityConversions
{
	/// <summary>
	/// Converts a value to radians per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AngularVelocity"/> representing the value in radians per second.</returns>
	public static AngularVelocity RadiansPerSecond<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AngularVelocity>();

	/// <summary>
	/// Converts an <see cref="AngularVelocity"/> to a numeric value in radians per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AngularVelocity"/> to convert.</param>
	/// <returns>The numeric value in radians per second.</returns>
	public static TNumber RadiansPerSecond<TNumber>(this AngularVelocity value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to degrees per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AngularVelocity"/> representing the value in degrees per second.</returns>
	public static AngularVelocity DegreesPerSecond<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AngularVelocity>(PhysicalConstants.DegreesToRadiansFactor);

	/// <summary>
	/// Converts an <see cref="AngularVelocity"/> to a numeric value in degrees per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AngularVelocity"/> to convert.</param>
	/// <returns>The numeric value in degrees per second.</returns>
	public static TNumber DegreesPerSecond<TNumber>(this AngularVelocity value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.DegreesToRadiansFactor));

	/// <summary>
	/// Converts a value to gradians per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AngularVelocity"/> representing the value in gradians per second.</returns>
	public static AngularVelocity GradiansPerSecond<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AngularVelocity>(PhysicalConstants.GradiansToRadiansFactor);

	/// <summary>
	/// Converts an <see cref="AngularVelocity"/> to a numeric value in gradians per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AngularVelocity"/> to convert.</param>
	/// <returns>The numeric value in gradians per second.</returns>
	public static TNumber GradiansPerSecond<TNumber>(this AngularVelocity value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.GradiansToRadiansFactor));

	/// <summary>
	/// Converts a value to revolutions per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AngularVelocity"/> representing the value in revolutions per second.</returns>
	public static AngularVelocity RevolutionsPerSecond<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AngularVelocity>(PhysicalConstants.RevolutionsToRadiansFactor);

	/// <summary>
	/// Converts an <see cref="AngularVelocity"/> to a numeric value in revolutions per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AngularVelocity"/> to convert.</param>
	/// <returns>The numeric value in revolutions per second.</returns>
	public static TNumber RevolutionsPerSecond<TNumber>(this AngularVelocity value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.RevolutionsToRadiansFactor));

	/// <summary>
	/// Converts a value to cycles per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AngularVelocity"/> representing the value in cycles per second.</returns>
	public static AngularVelocity CyclesPerSecond<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AngularVelocity>(PhysicalConstants.CyclesToRadiansFactor);

	/// <summary>
	/// Converts an <see cref="AngularVelocity"/> to a numeric value in cycles per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AngularVelocity"/> to convert.</param>
	/// <returns>The numeric value in cycles per second.</returns>
	public static TNumber CyclesPerSecond<TNumber>(this AngularVelocity value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.CyclesToRadiansFactor));

	/// <summary>
	/// Converts a value to turns per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AngularVelocity"/> representing the value in turns per second.</returns>
	public static AngularVelocity TurnsPerSecond<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AngularVelocity>(PhysicalConstants.TurnsToRadiansFactor);

	/// <summary>
	/// Converts an <see cref="AngularVelocity"/> to a numeric value in turns per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AngularVelocity"/> to convert.</param>
	/// <returns>The numeric value in turns per second.</returns>
	public static TNumber TurnsPerSecond<TNumber>(this AngularVelocity value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.TurnsToRadiansFactor));
}
