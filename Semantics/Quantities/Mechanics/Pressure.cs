// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a pressure physical quantity.
/// </summary>
[SIUnit(typeof(SIUnits), nameof(SIUnits.Pascal))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]

public sealed record Pressure
	: PhysicalQuantity<Pressure>
	, IIntegralOperators<Pressure, Area, Force>
{
	/// <summary>
	/// Multiplies a <see cref="Pressure"/> by an <see cref="Area"/> to compute a <see cref="Force"/>.
	/// </summary>
	/// <param name="left">The pressure operand.</param>
	/// <param name="right">The area operand.</param>
	/// <returns>A <see cref="Force"/> representing the result of the multiplication.</returns>
	public static Force operator *(Pressure left, Area right) =>
		IIntegralOperators<Pressure, Area, Force>.Integrate(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Pressure"/>.
/// </summary>
public static class PressureConversions
{
	/// <summary>
	/// Converts a value to pascals.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Pressure"/> representing the value in pascals.</returns>
	public static Pressure Pascals<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Pressure>();

	/// <summary>
	/// Converts a <see cref="Pressure"/> to a numeric value in pascals.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Pressure"/> to convert.</param>
	/// <returns>The numeric value in pascals.</returns>
	public static TNumber Pascals<TNumber>(this Pressure value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to bars.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Pressure"/> representing the value in bars.</returns>
	public static Pressure Bars<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Pressure>(PhysicalConstants.BarToPascalsFactor);

	/// <summary>
	/// Converts a <see cref="Pressure"/> to a numeric value in bars.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Pressure"/> to convert.</param>
	/// <returns>The numeric value in bars.</returns>
	public static TNumber Bars<TNumber>(this Pressure value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.BarToPascalsFactor));

	/// <summary>
	/// Converts a value to psi (pounds per square inch).
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Pressure"/> representing the value in psi.</returns>
	public static Pressure Psi<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Pressure>(PhysicalConstants.PsiToPascalsFactor);

	/// <summary>
	/// Converts a <see cref="Pressure"/> to a numeric value in psi (pounds per square inch).
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Pressure"/> to convert.</param>
	/// <returns>The numeric value in psi.</returns>
	public static TNumber Psi<TNumber>(this Pressure value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.PsiToPascalsFactor));

	/// <summary>
	/// Converts a value to atmospheres.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Pressure"/> representing the value in atmospheres.</returns>
	public static Pressure Atmospheres<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Pressure>(PhysicalConstants.AtmToPascalsFactor);

	/// <summary>
	/// Converts a <see cref="Pressure"/> to a numeric value in atmospheres.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Pressure"/> to convert.</param>
	/// <returns>The numeric value in atmospheres.</returns>
	public static TNumber Atmospheres<TNumber>(this Pressure value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.AtmToPascalsFactor));

	/// <summary>
	/// Converts a value to torr.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Pressure"/> representing the value in torr.</returns>
	public static Pressure Torr<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Pressure>(PhysicalConstants.TorrToPascalsFactor);

	/// <summary>
	/// Converts a <see cref="Pressure"/> to a numeric value in torr.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Pressure"/> to convert.</param>
	/// <returns>The numeric value in torr.</returns>
	public static TNumber Torr<TNumber>(this Pressure value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.TorrToPascalsFactor));
}
