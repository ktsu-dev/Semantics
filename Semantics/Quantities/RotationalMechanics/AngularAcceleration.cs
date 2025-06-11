// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an angular acceleration physical quantity.
/// </summary>
[SIUnit("rad/s²", "radian per second squared", "radians per second squared")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
[Obsolete]
public sealed record AngularAcceleration
	: PhysicalQuantity<AngularAcceleration>
	, IIntegralOperators<AngularAcceleration, Time, AngularVelocity>
{
	/// <summary>
	/// Multiplies an <see cref="AngularAcceleration"/> by a <see cref="Time"/> to compute an <see cref="AngularVelocity"/>.
	/// </summary>
	/// <param name="left">The angular acceleration operand.</param>
	/// <param name="right">The time operand.</param>
	/// <returns>An <see cref="AngularVelocity"/> representing the result of the multiplication.</returns>
	public static AngularVelocity operator *(AngularAcceleration left, Time right) =>
		IIntegralOperators<AngularAcceleration, Time, AngularVelocity>.Integrate(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="AngularAcceleration"/>.
/// </summary>
public static class AngularAccelerationConversions
{
	/// <summary>
	/// Converts a value to radians per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AngularAcceleration"/> representing the value in radians per second squared.</returns>
	public static AngularAcceleration RadiansPerSecondSquared<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AngularAcceleration>();

	/// <summary>
	/// Converts an <see cref="AngularAcceleration"/> to a numeric value in radians per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AngularAcceleration"/> to convert.</param>
	/// <returns>The numeric value in radians per second squared.</returns>
	public static TNumber RadiansPerSecondSquared<TNumber>(this AngularAcceleration value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to degrees per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AngularAcceleration"/> representing the value in degrees per second squared.</returns>
	public static AngularAcceleration DegreesPerSecondSquared<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AngularAcceleration>(PhysicalConstants.DegreesToRadiansFactor);

	/// <summary>
	/// Converts an <see cref="AngularAcceleration"/> to a numeric value in degrees per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AngularAcceleration"/> to convert.</param>
	/// <returns>The numeric value in degrees per second squared.</returns>
	public static TNumber DegreesPerSecondSquared<TNumber>(this AngularAcceleration value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.DegreesToRadiansFactor));

	/// <summary>
	/// Converts a value to gradians per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AngularAcceleration"/> representing the value in gradians per second squared.</returns>
	public static AngularAcceleration GradiansPerSecondSquared<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AngularAcceleration>(PhysicalConstants.GradiansToRadiansFactor);

	/// <summary>
	/// Converts an <see cref="AngularAcceleration"/> to a numeric value in gradians per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AngularAcceleration"/> to convert.</param>
	/// <returns>The numeric value in gradians per second squared.</returns>
	public static TNumber GradiansPerSecondSquared<TNumber>(this AngularAcceleration value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.GradiansToRadiansFactor));

	/// <summary>
	/// Converts a value to revolutions per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AngularAcceleration"/> representing the value in revolutions per second squared.</returns>
	public static AngularAcceleration RevolutionsPerSecondSquared<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AngularAcceleration>(PhysicalConstants.RevolutionsToRadiansFactor);

	/// <summary>
	/// Converts an <see cref="AngularAcceleration"/> to a numeric value in revolutions per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AngularAcceleration"/> to convert.</param>
	/// <returns>The numeric value in revolutions per second squared.</returns>
	public static TNumber RevolutionsPerSecondSquared<TNumber>(this AngularAcceleration value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.RevolutionsToRadiansFactor));

	/// <summary>
	/// Converts a value to cycles per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AngularAcceleration"/> representing the value in cycles per second squared.</returns>
	public static AngularAcceleration CyclesPerSecondSquared<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AngularAcceleration>(PhysicalConstants.CyclesToRadiansFactor);

	/// <summary>
	/// Converts an <see cref="AngularAcceleration"/> to a numeric value in cycles per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AngularAcceleration"/> to convert.</param>
	/// <returns>The numeric value in cycles per second squared.</returns>
	public static TNumber CyclesPerSecondSquared<TNumber>(this AngularAcceleration value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.CyclesToRadiansFactor));

	/// <summary>
	/// Converts a value to turns per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AngularAcceleration"/> representing the value in turns per second squared.</returns>
	public static AngularAcceleration TurnsPerSecondSquared<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AngularAcceleration>(PhysicalConstants.TurnsToRadiansFactor);

	/// <summary>
	/// Converts an <see cref="AngularAcceleration"/> to a numeric value in turns per second squared.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AngularAcceleration"/> to convert.</param>
	/// <returns>The numeric value in turns per second squared.</returns>
	public static TNumber TurnsPerSecondSquared<TNumber>(this AngularAcceleration value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.TurnsToRadiansFactor));
}
