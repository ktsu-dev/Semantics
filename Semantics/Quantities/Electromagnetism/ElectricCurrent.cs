// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an electric current physical quantity.
/// </summary>
[SIUnit("A", "ampere", "amperes")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
[Obsolete]
public sealed record ElectricCurrent
	: PhysicalQuantity<ElectricCurrent>
	, IIntegralOperators<ElectricCurrent, Time, Charge>
	, IIntegralOperators<ElectricCurrent, ElectricPotential, Power>
	, IIntegralOperators<ElectricCurrent, Resistance, ElectricPotential>
{
	/// <summary>
	/// Multiplies an <see cref="ElectricCurrent"/> by a <see cref="Time"/> to compute a <see cref="Charge"/>.
	/// </summary>
	/// <param name="left">The electric current operand.</param>
	/// <param name="right">The time operand.</param>
	/// <returns>A <see cref="Charge"/> representing the result of the multiplication.</returns>
	public static Charge operator *(ElectricCurrent left, Time right) =>
		IIntegralOperators<ElectricCurrent, Time, Charge>.Integrate(left, right);

	/// <summary>
	/// Multiplies an <see cref="ElectricCurrent"/> by an <see cref="ElectricPotential"/> to compute a <see cref="Power"/>.
	/// </summary>
	/// <param name="left">The electric current operand.</param>
	/// <param name="right">The electric potential operand.</param>
	/// <returns>A <see cref="Power"/> representing the result of the multiplication.</returns>
	public static Power operator *(ElectricCurrent left, ElectricPotential right) =>
		IIntegralOperators<ElectricCurrent, ElectricPotential, Power>.Integrate(left, right);

	/// <summary>
	/// Multiplies an <see cref="ElectricCurrent"/> by a <see cref="Resistance"/> to compute an <see cref="ElectricPotential"/>.
	/// </summary>
	/// <param name="left">The electric current operand.</param>
	/// <param name="right">The resistance operand.</param>
	/// <returns>An <see cref="ElectricPotential"/> representing the result of the multiplication.</returns>
	public static ElectricPotential operator *(ElectricCurrent left, Resistance right) =>
		IIntegralOperators<ElectricCurrent, Resistance, ElectricPotential>.Integrate(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="ElectricCurrent"/>.
/// </summary>
public static class ElectricCurrentConversions
{
	/// <summary>
	/// Converts a value to amperes.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="ElectricCurrent"/> representing the value in amperes.</returns>
	public static ElectricCurrent Amperes<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, ElectricCurrent>();

	/// <summary>
	/// Converts an <see cref="ElectricCurrent"/> to a numeric value in amperes.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="ElectricCurrent"/> to convert.</param>
	/// <returns>The numeric value in amperes.</returns>
	public static TNumber Amperes<TNumber>(this ElectricCurrent value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to milliamperes.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="ElectricCurrent"/> representing the value in milliamperes.</returns>
	public static ElectricCurrent Milliamperes<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, ElectricCurrent>(PhysicalConstants.Milli);

	/// <summary>
	/// Converts an <see cref="ElectricCurrent"/> to a numeric value in milliamperes.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="ElectricCurrent"/> to convert.</param>
	/// <returns>The numeric value in milliamperes.</returns>
	public static TNumber Milliamperes<TNumber>(this ElectricCurrent value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Milli));

	/// <summary>
	/// Converts a value to microamperes.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="ElectricCurrent"/> representing the value in microamperes.</returns>
	public static ElectricCurrent Microamperes<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, ElectricCurrent>(PhysicalConstants.Micro);

	/// <summary>
	/// Converts an <see cref="ElectricCurrent"/> to a numeric value in microamperes.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="ElectricCurrent"/> to convert.</param>
	/// <returns>The numeric value in microamperes.</returns>
	public static TNumber Microamperes<TNumber>(this ElectricCurrent value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Micro));

	/// <summary>
	/// Converts a value to nanoamperes.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="ElectricCurrent"/> representing the value in nanoamperes.</returns>
	public static ElectricCurrent Nanoamperes<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, ElectricCurrent>(PhysicalConstants.Nano);

	/// <summary>
	/// Converts an <see cref="ElectricCurrent"/> to a numeric value in nanoamperes.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="ElectricCurrent"/> to convert.</param>
	/// <returns>The numeric value in nanoamperes.</returns>
	public static TNumber Nanoamperes<TNumber>(this ElectricCurrent value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Nano));

	/// <summary>
	/// Converts a value to kiloamperes.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="ElectricCurrent"/> representing the value in kiloamperes.</returns>
	public static ElectricCurrent Kiloamperes<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, ElectricCurrent>(PhysicalConstants.Kilo);

	/// <summary>
	/// Converts an <see cref="ElectricCurrent"/> to a numeric value in kiloamperes.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="ElectricCurrent"/> to convert.</param>
	/// <returns>The numeric value in kiloamperes.</returns>
	public static TNumber Kiloamperes<TNumber>(this ElectricCurrent value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Kilo));

	/// <summary>
	/// Converts a value to megaamperes.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="ElectricCurrent"/> representing the value in megaamperes.</returns>
	public static ElectricCurrent Megaamperes<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, ElectricCurrent>(PhysicalConstants.Mega);

	/// <summary>
	/// Converts an <see cref="ElectricCurrent"/> to a numeric value in megaamperes.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="ElectricCurrent"/> to convert.</param>
	/// <returns>The numeric value in megaamperes.</returns>
	public static TNumber Megaamperes<TNumber>(this ElectricCurrent value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Mega));
}
