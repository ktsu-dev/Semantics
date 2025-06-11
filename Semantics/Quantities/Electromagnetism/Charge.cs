// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an electric charge physical quantity.
/// </summary>
[SIUnit("C", "coulomb", "coulombs")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
[Obsolete]
public sealed record Charge
	: PhysicalQuantity<Charge>
	, IDerivativeOperators<Charge, Time, ElectricCurrent>
	, IIntegralOperators<Charge, ElectricPotential, Energy>
{
	/// <summary>
	/// Divides a <see cref="Charge"/> by a <see cref="Time"/> to compute an <see cref="ElectricCurrent"/>.
	/// </summary>
	/// <param name="left">The charge operand.</param>
	/// <param name="right">The time operand.</param>
	/// <returns>An <see cref="ElectricCurrent"/> representing the result of the division.</returns>
	public static ElectricCurrent operator /(Charge left, Time right) =>
		IDerivativeOperators<Charge, Time, ElectricCurrent>.Derive(left, right);

	/// <summary>
	/// Multiplies a <see cref="Charge"/> by an <see cref="ElectricPotential"/> to compute an <see cref="Energy"/>.
	/// </summary>
	/// <param name="left">The charge operand.</param>
	/// <param name="right">The electric potential operand.</param>
	/// <returns>An <see cref="Energy"/> representing the result of the multiplication.</returns>
	public static Energy operator *(Charge left, ElectricPotential right) =>
		IIntegralOperators<Charge, ElectricPotential, Energy>.Integrate(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Charge"/>.
/// </summary>
public static class ChargeConversions
{
	/// <summary>
	/// Converts a value to coulombs.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Charge"/> representing the value in coulombs.</returns>
	public static Charge Coulombs<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Charge>();

	/// <summary>
	/// Converts a <see cref="Charge"/> to a numeric value in coulombs.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Charge"/> to convert.</param>
	/// <returns>The numeric value in coulombs.</returns>
	public static TNumber Coulombs<TNumber>(this Charge value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to milliampere-hours.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Charge"/> representing the value in milliampere-hours.</returns>
	public static Charge MilliampereHours<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Charge>(PhysicalConstants.MilliampereHoursToCoulombsFactor);

	/// <summary>
	/// Converts a <see cref="Charge"/> to a numeric value in milliampere-hours.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Charge"/> to convert.</param>
	/// <returns>The numeric value in milliampere-hours.</returns>
	public static TNumber MilliampereHours<TNumber>(this Charge value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.MilliampereHoursToCoulombsFactor));

	/// <summary>
	/// Converts a value to ampere-hours.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Charge"/> representing the value in ampere-hours.</returns>
	public static Charge AmpereHours<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Charge>(PhysicalConstants.AmpereHoursToCoulombsFactor);

	/// <summary>
	/// Converts a <see cref="Charge"/> to a numeric value in ampere-hours.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Charge"/> to convert.</param>
	/// <returns>The numeric value in ampere-hours.</returns>
	public static TNumber AmpereHours<TNumber>(this Charge value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.AmpereHoursToCoulombsFactor));
}
