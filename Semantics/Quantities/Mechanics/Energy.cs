// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an energy physical quantity.
/// </summary>
[SIUnit("J", "joule", "joules")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
public sealed record Energy
	: PhysicalQuantity<Energy>
	, IDerivativeOperators<Energy, Force, Length>
	, IDerivativeOperators<Energy, Length, Force>
	, IDerivativeOperators<Energy, Time, Power>
	, IDerivativeOperators<Energy, ElectricPotential, Charge>
	, IDerivativeOperators<Energy, Charge, ElectricPotential>
	, IDerivativeOperators<Energy, Torque, Angle>
	, IDerivativeOperators<Energy, Angle, Torque>
{
	/// <summary>
	/// Divides an <see cref="Energy"/> by a <see cref="Force"/> to compute a <see cref="Length"/>.
	/// </summary>
	/// <param name="left">The energy operand.</param>
	/// <param name="right">The force operand.</param>
	/// <returns>A <see cref="Length"/> representing the result of the division.</returns>
	public static Length operator /(Energy left, Force right) =>
		IDerivativeOperators<Energy, Force, Length>.Derive(left, right);

	/// <summary>
	/// Divides an <see cref="Energy"/> by a <see cref="Length"/> to compute a <see cref="Force"/>.
	/// </summary>
	/// <param name="left">The energy operand.</param>
	/// <param name="right">The length operand.</param>
	/// <returns>A <see cref="Force"/> representing the result of the division.</returns>
	public static Force operator /(Energy left, Length right) =>
		IDerivativeOperators<Energy, Length, Force>.Derive(left, right);

	/// <summary>
	/// Divides an <see cref="Energy"/> by a <see cref="Time"/> to compute a <see cref="Power"/>.
	/// </summary>
	/// <param name="left">The energy operand.</param>
	/// <param name="right">The time operand.</param>
	/// <returns>A <see cref="Power"/> representing the result of the division.</returns>
	public static Power operator /(Energy left, Time right) =>
		IDerivativeOperators<Energy, Time, Power>.Derive(left, right);

	/// <summary>
	/// Divides an <see cref="Energy"/> by an <see cref="ElectricPotential"/> to compute a <see cref="Charge"/>.
	/// </summary>
	/// <param name="left">The energy operand.</param>
	/// <param name="right">The electric potential operand.</param>
	/// <returns>A <see cref="Charge"/> representing the result of the division.</returns>
	public static Charge operator /(Energy left, ElectricPotential right) =>
		IDerivativeOperators<Energy, ElectricPotential, Charge>.Derive(left, right);

	/// <summary>
	/// Divides an <see cref="Energy"/> by a <see cref="Charge"/> to compute an <see cref="ElectricPotential"/>.
	/// </summary>
	/// <param name="left">The energy operand.</param>
	/// <param name="right">The charge operand.</param>
	/// <returns>An <see cref="ElectricPotential"/> representing the result of the division.</returns>
	public static ElectricPotential operator /(Energy left, Charge right) =>
		IDerivativeOperators<Energy, Charge, ElectricPotential>.Derive(left, right);

	/// <summary>
	/// Divides an <see cref="Energy"/> by a <see cref="Torque"/> to compute an <see cref="Angle"/>.
	/// </summary>
	/// <param name="left">The energy operand.</param>
	/// <param name="right">The torque operand.</param>
	/// <returns>An <see cref="Angle"/> representing the result of the division.</returns>
	public static Angle operator /(Energy left, Torque right) =>
		IDerivativeOperators<Energy, Torque, Angle>.Derive(left, right);

	/// <summary>
	/// Divides an <see cref="Energy"/> by an <see cref="Angle"/> to compute a <see cref="Torque"/>.
	/// </summary>
	/// <param name="left">The energy operand.</param>
	/// <param name="right">The angle operand.</param>
	/// <returns>A <see cref="Torque"/> representing the result of the division.</returns>
	public static Torque operator /(Energy left, Angle right) =>
		IDerivativeOperators<Energy, Angle, Torque>.Derive(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Energy"/>.
/// </summary>
public static class EnergyConversions
{
	private static Conversions.IConversionCalculator<Energy> Calculator => Conversions.ConversionRegistry.GetCalculator<Energy>();

	/// <summary>
	/// Converts a value to joules.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Energy"/> representing the value in joules.</returns>
	public static Energy Joules<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "joules");

	/// <summary>
	/// Converts an <see cref="Energy"/> to a numeric value in joules.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Energy"/> to convert.</param>
	/// <returns>The numeric value in joules.</returns>
	public static TNumber Joules<TNumber>(this Energy value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "joules");

	/// <summary>
	/// Converts a value to millijoules.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Energy"/> representing the value in millijoules.</returns>
	public static Energy Millijoules<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "millijoules");

	/// <summary>
	/// Converts an <see cref="Energy"/> to a numeric value in millijoules.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Energy"/> to convert.</param>
	/// <returns>The numeric value in millijoules.</returns>
	public static TNumber Millijoules<TNumber>(this Energy value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "millijoules");

	/// <summary>
	/// Converts a value to microjoules.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Energy"/> representing the value in microjoules.</returns>
	public static Energy Microjoules<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "microjoules");

	/// <summary>
	/// Converts an <see cref="Energy"/> to a numeric value in microjoules.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Energy"/> to convert.</param>
	/// <returns>The numeric value in microjoules.</returns>
	public static TNumber Microjoules<TNumber>(this Energy value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "microjoules");

	/// <summary>
	/// Converts a value to nanojoules.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Energy"/> representing the value in nanojoules.</returns>
	public static Energy Nanojoules<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "nanojoules");

	/// <summary>
	/// Converts an <see cref="Energy"/> to a numeric value in nanojoules.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Energy"/> to convert.</param>
	/// <returns>The numeric value in nanojoules.</returns>
	public static TNumber Nanojoules<TNumber>(this Energy value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "nanojoules");

	/// <summary>
	/// Converts a value to kilojoules.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Energy"/> representing the value in kilojoules.</returns>
	public static Energy Kilojoules<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "kilojoules");

	/// <summary>
	/// Converts an <see cref="Energy"/> to a numeric value in kilojoules.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Energy"/> to convert.</param>
	/// <returns>The numeric value in kilojoules.</returns>
	public static TNumber Kilojoules<TNumber>(this Energy value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "kilojoules");

	/// <summary>
	/// Converts a value to megajoules.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Energy"/> representing the value in megajoules.</returns>
	public static Energy Megajoules<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "megajoules");

	/// <summary>
	/// Converts an <see cref="Energy"/> to a numeric value in megajoules.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Energy"/> to convert.</param>
	/// <returns>The numeric value in megajoules.</returns>
	public static TNumber Megajoules<TNumber>(this Energy value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "megajoules");

	/// <summary>
	/// Converts a value to gigajoules.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Energy"/> representing the value in gigajoules.</returns>
	public static Energy Gigajoules<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "gigajoules");

	/// <summary>
	/// Converts an <see cref="Energy"/> to a numeric value in gigajoules.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Energy"/> to convert.</param>
	/// <returns>The numeric value in gigajoules.</returns>
	public static TNumber Gigajoules<TNumber>(this Energy value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "gigajoules");

	/// <summary>
	/// Converts a value to calories.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Energy"/> representing the value in calories.</returns>
	public static Energy Calories<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "calories");

	/// <summary>
	/// Converts an <see cref="Energy"/> to a numeric value in calories.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Energy"/> to convert.</param>
	/// <returns>The numeric value in calories.</returns>
	public static TNumber Calories<TNumber>(this Energy value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "calories");

	/// <summary>
	/// Converts a value to kilocalories.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Energy"/> representing the value in kilocalories.</returns>
	public static Energy Kilocalories<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "kilocalories");

	/// <summary>
	/// Converts an <see cref="Energy"/> to a numeric value in kilocalories.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Energy"/> to convert.</param>
	/// <returns>The numeric value in kilocalories.</returns>
	public static TNumber Kilocalories<TNumber>(this Energy value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "kilocalories");

	/// <summary>
	/// Converts a value to watt hours.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Energy"/> representing the value in watt hours.</returns>
	public static Energy WattHours<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "watt_hours");

	/// <summary>
	/// Converts an <see cref="Energy"/> to a numeric value in watt hours.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Energy"/> to convert.</param>
	/// <returns>The numeric value in watt hours.</returns>
	public static TNumber WattHours<TNumber>(this Energy value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "watt_hours");

	/// <summary>
	/// Converts a value to kilowatt hours.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Energy"/> representing the value in kilowatt hours.</returns>
	public static Energy KilowattHours<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "kilowatt_hours");

	/// <summary>
	/// Converts an <see cref="Energy"/> to a numeric value in kilowatt hours.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Energy"/> to convert.</param>
	/// <returns>The numeric value in kilowatt hours.</returns>
	public static TNumber KilowattHours<TNumber>(this Energy value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "kilowatt_hours");

	/// <summary>
	/// Converts a value to BTUs (British Thermal Units).
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Energy"/> representing the value in BTUs.</returns>
	public static Energy BTUs<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> Calculator.FromUnit(value, "btus");

	/// <summary>
	/// Converts an <see cref="Energy"/> to a numeric value in BTUs (British Thermal Units).
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Energy"/> to convert.</param>
	/// <returns>The numeric value in BTUs.</returns>
	public static TNumber BTUs<TNumber>(this Energy value)
		where TNumber : INumber<TNumber>
		=> Calculator.ToUnit<TNumber>(value, "btus");
}
