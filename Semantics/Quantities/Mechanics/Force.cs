// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a force physical quantity.
/// </summary>
[SIUnit("N", "newton", "newtons")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
public sealed record Force
	: PhysicalQuantity<Force>
	, IDerivativeOperators<Force, Mass, Acceleration>
	, IDerivativeOperators<Force, Acceleration, Mass>
	, IIntegralOperators<Force, Length, Energy>
	, IDerivativeOperators<Force, Area, Pressure>
{
	/// <summary>
	/// Divides a <see cref="Force"/> by a <see cref="Mass"/> to compute an <see cref="Acceleration"/>.
	/// </summary>
	/// <param name="left">The force operand.</param>
	/// <param name="right">The mass operand.</param>
	/// <returns>An <see cref="Acceleration"/> representing the result of the division.</returns>
	public static Acceleration operator /(Force left, Mass right) =>
		IDerivativeOperators<Force, Mass, Acceleration>.Derive(left, right);

	/// <summary>
	/// Divides a <see cref="Force"/> by an <see cref="Acceleration"/> to compute a <see cref="Mass"/>.
	/// </summary>
	/// <param name="left">The force operand.</param>
	/// <param name="right">The acceleration operand.</param>
	/// <returns>A <see cref="Mass"/> representing the result of the division.</returns>
	public static Mass operator /(Force left, Acceleration right) =>
		IDerivativeOperators<Force, Acceleration, Mass>.Derive(left, right);

	/// <summary>
	/// Multiplies a <see cref="Force"/> by a <see cref="Length"/> to compute an <see cref="Energy"/>.
	/// </summary>
	/// <param name="left">The force operand.</param>
	/// <param name="right">The length operand.</param>
	/// <returns>An <see cref="Energy"/> representing the result of the multiplication.</returns>
	public static Energy operator *(Force left, Length right) =>
		IIntegralOperators<Force, Length, Energy>.Integrate(left, right);

	/// <summary>
	/// Divides a <see cref="Force"/> by an <see cref="Area"/> to compute a <see cref="Pressure"/>.
	/// </summary>
	/// <param name="left">The force operand.</param>
	/// <param name="right">The area operand.</param>
	/// <returns>A <see cref="Pressure"/> representing the result of the division.</returns>
	public static Pressure operator /(Force left, Area right) =>
		IDerivativeOperators<Force, Area, Pressure>.Derive(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Force"/>.
/// </summary>
public static class ForceConversions
{
	/// <summary>
	/// Converts a value to newtons.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Force"/> representing the value in newtons.</returns>
	public static Force Newtons<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Force>();

	/// <summary>
	/// Converts a <see cref="Force"/> to a numeric value in newtons.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Force"/> to convert.</param>
	/// <returns>The numeric value in newtons.</returns>
	public static TNumber Newtons<TNumber>(this Force value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to kilonewtons.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Force"/> representing the value in kilonewtons.</returns>
	public static Force Kilonewtons<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Force>(PhysicalConstants.Kilo);

	/// <summary>
	/// Converts a <see cref="Force"/> to a numeric value in kilonewtons.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Force"/> to convert.</param>
	/// <returns>The numeric value in kilonewtons.</returns>
	public static TNumber Kilonewtons<TNumber>(this Force value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Kilo));

	/// <summary>
	/// Converts a value to millinewtons.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Force"/> representing the value in millinewtons.</returns>
	public static Force Millinewtons<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Force>(PhysicalConstants.Milli);

	/// <summary>
	/// Converts a <see cref="Force"/> to a numeric value in millinewtons.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Force"/> to convert.</param>
	/// <returns>The numeric value in millinewtons.</returns>
	public static TNumber Millinewtons<TNumber>(this Force value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Milli));

	/// <summary>
	/// Converts a value to micronewtons.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Force"/> representing the value in micronewtons.</returns>
	public static Force Micronewtons<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Force>(PhysicalConstants.Micro);

	/// <summary>
	/// Converts a <see cref="Force"/> to a numeric value in micronewtons.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Force"/> to convert.</param>
	/// <returns>The numeric value in micronewtons.</returns>
	public static TNumber Micronewtons<TNumber>(this Force value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Micro));

	/// <summary>
	/// Converts a value to pounds-force.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Force"/> representing the value in pounds-force.</returns>
	public static Force PoundsForce<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Force>(PhysicalConstants.PoundsForceToNewtonsFactor);

	/// <summary>
	/// Converts a <see cref="Force"/> to a numeric value in pounds-force.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Force"/> to convert.</param>
	/// <returns>The numeric value in pounds-force.</returns>
	public static TNumber PoundsForce<TNumber>(this Force value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.PoundsForceToNewtonsFactor));
}
