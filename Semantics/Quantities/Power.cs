// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a power physical quantity.
/// </summary>
[SIUnit("W", "watt", "watts")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
public sealed record Power
	: PhysicalQuantity<Power>
	, IIntegralOperators<Power, Time, Energy>
	, IDerivativeOperators<Power, ElectricCurrent, ElectricPotential>
	, IDerivativeOperators<Power, ElectricPotential, ElectricCurrent>
{
	/// <summary>
	/// Multiplies a <see cref="Power"/> by a <see cref="Time"/> to compute an <see cref="Energy"/>.
	/// </summary>
	/// <param name="left">The power operand.</param>
	/// <param name="right">The time operand.</param>
	/// <returns>An <see cref="Energy"/> representing the result of the multiplication.</returns>
	public static Energy operator *(Power left, Time right) =>
		IIntegralOperators<Power, Time, Energy>.Integrate(left, right);

	/// <summary>
	/// Divides a <see cref="Power"/> by an <see cref="ElectricCurrent"/> to compute an <see cref="ElectricPotential"/>.
	/// </summary>
	/// <param name="left">The power operand.</param>
	/// <param name="right">The electric current operand.</param>
	/// <returns>An <see cref="ElectricPotential"/> representing the result of the division.</returns>
	public static ElectricPotential operator /(Power left, ElectricCurrent right) =>
		IDerivativeOperators<Power, ElectricCurrent, ElectricPotential>.Derive(left, right);

	/// <summary>
	/// Divides a <see cref="Power"/> by an <see cref="ElectricPotential"/> to compute an <see cref="ElectricCurrent"/>.
	/// </summary>
	/// <param name="left">The power operand.</param>
	/// <param name="right">The electric potential operand.</param>
	/// <returns>An <see cref="ElectricCurrent"/> representing the result of the division.</returns>
	public static ElectricCurrent operator /(Power left, ElectricPotential right) =>
		IDerivativeOperators<Power, ElectricPotential, ElectricCurrent>.Derive(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Power"/>.
/// </summary>
public static class PowerConversions
{
	/// <summary>
	/// Converts a value to watts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Power"/> representing the value in watts.</returns>
	public static Power Watts<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Power>();

	/// <summary>
	/// Converts a <see cref="Power"/> to a numeric value in watts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Power"/> to convert.</param>
	/// <returns>The numeric value in watts.</returns>
	public static TNumber Watts<TNumber>(this Power value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to kilowatts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Power"/> representing the value in kilowatts.</returns>
	public static Power Kilowatts<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Power>(PhysicalConstants.Kilo);

	/// <summary>
	/// Converts a <see cref="Power"/> to a numeric value in kilowatts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Power"/> to convert.</param>
	/// <returns>The numeric value in kilowatts.</returns>
	public static TNumber Kilowatts<TNumber>(this Power value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Kilo));

	/// <summary>
	/// Converts a value to megawatts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Power"/> representing the value in megawatts.</returns>
	public static Power Megawatts<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Power>(PhysicalConstants.Mega);

	/// <summary>
	/// Converts a <see cref="Power"/> to a numeric value in megawatts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Power"/> to convert.</param>
	/// <returns>The numeric value in megawatts.</returns>
	public static TNumber Megawatts<TNumber>(this Power value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Mega));

	/// <summary>
	/// Converts a value to gigawatts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Power"/> representing the value in gigawatts.</returns>
	public static Power Gigawatts<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Power>(PhysicalConstants.Giga);

	/// <summary>
	/// Converts a <see cref="Power"/> to a numeric value in gigawatts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Power"/> to convert.</param>
	/// <returns>The numeric value in gigawatts.</returns>
	public static TNumber Gigawatts<TNumber>(this Power value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Giga));

	/// <summary>
	/// Converts a value to horsepower.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Power"/> representing the value in horsepower.</returns>
	public static Power Horsepower<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Power>(PhysicalConstants.HorsepowerToWattsFactor);

	/// <summary>
	/// Converts a <see cref="Power"/> to a numeric value in horsepower.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Power"/> to convert.</param>
	/// <returns>The numeric value in horsepower.</returns>
	public static TNumber Horsepower<TNumber>(this Power value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.HorsepowerToWattsFactor));

	/// <summary>
	/// Converts a value to metric horsepower.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Power"/> representing the value in metric horsepower.</returns>
	public static Power MetricHorsepower<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Power>(PhysicalConstants.MetricHorsePowerToWattsFactor);

	/// <summary>
	/// Converts a <see cref="Power"/> to a numeric value in metric horsepower.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Power"/> to convert.</param>
	/// <returns>The numeric value in metric horsepower.</returns>
	public static TNumber MetricHorsepower<TNumber>(this Power value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.MetricHorsePowerToWattsFactor));
}
