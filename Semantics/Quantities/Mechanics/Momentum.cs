// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a momentum physical quantity.
/// </summary>
[SIUnit("kg·m/s", "kilogram meter per second", "kilogram meters per second")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
public sealed record Momentum
	: PhysicalQuantity<Momentum>
	, IDerivativeOperators<Momentum, Velocity, Mass>
	, IDerivativeOperators<Momentum, Mass, Velocity>
{
	/// <summary>
	/// Divides a <see cref="Momentum"/> by a <see cref="Velocity"/> to compute a <see cref="Mass"/>.
	/// </summary>
	/// <param name="left">The momentum operand.</param>
	/// <param name="right">The velocity operand.</param>
	/// <returns>A <see cref="Mass"/> representing the result of the division.</returns>
	public static Mass operator /(Momentum left, Velocity right) =>
		IDerivativeOperators<Momentum, Velocity, Mass>.Derive(left, right);

	/// <summary>
	/// Divides a <see cref="Momentum"/> by a <see cref="Mass"/> to compute a <see cref="Velocity"/>.
	/// </summary>
	/// <param name="left">The momentum operand.</param>
	/// <param name="right">The mass operand.</param>
	/// <returns>A <see cref="Velocity"/> representing the result of the division.</returns>
	public static Velocity operator /(Momentum left, Mass right) =>
		IDerivativeOperators<Momentum, Mass, Velocity>.Derive(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Momentum"/>.
/// </summary>
public static class MomentumConversions
{
	/// <summary>
	/// Converts a value to kilogram meters per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Momentum"/> representing the value in kilogram meters per second.</returns>
	public static Momentum KilogramMetersPerSecond<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Momentum>();

	/// <summary>
	/// Converts a <see cref="Momentum"/> to a numeric value in kilogram meters per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Momentum"/> to convert.</param>
	/// <returns>The numeric value in kilogram meters per second.</returns>
	public static TNumber KilogramMetersPerSecond<TNumber>(this Momentum value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to gram meters per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Momentum"/> representing the value in gram meters per second.</returns>
	public static Momentum GramMetersPerSecond<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Momentum>(PhysicalConstants.Milli);

	/// <summary>
	/// Converts a <see cref="Momentum"/> to a numeric value in gram meters per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Momentum"/> to convert.</param>
	/// <returns>The numeric value in gram meters per second.</returns>
	public static TNumber GramMetersPerSecond<TNumber>(this Momentum value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Milli));

	/// <summary>
	/// Converts a value to milligram meters per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Momentum"/> representing the value in milligram meters per second.</returns>
	public static Momentum MilligramMetersPerSecond<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Momentum>(PhysicalConstants.Micro);

	/// <summary>
	/// Converts a <see cref="Momentum"/> to a numeric value in milligram meters per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Momentum"/> to convert.</param>
	/// <returns>The numeric value in milligram meters per second.</returns>
	public static TNumber MilligramMetersPerSecond<TNumber>(this Momentum value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Micro));
}
