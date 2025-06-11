// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an electrical resistance physical quantity.
/// </summary>
[SIUnit(typeof(SIUnits), nameof(SIUnits.Ohm))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]

public sealed record Resistance
	: PhysicalQuantity<Resistance>
	, IIntegralOperators<Resistance, ElectricCurrent, ElectricPotential>
{
	/// <summary>
	/// Multiplies a <see cref="Resistance"/> by an <see cref="ElectricCurrent"/> to compute an <see cref="ElectricPotential"/>.
	/// </summary>
	/// <param name="left">The resistance operand.</param>
	/// <param name="right">The electric current operand.</param>
	/// <returns>An <see cref="ElectricPotential"/> representing the result of the multiplication.</returns>
	public static ElectricPotential operator *(Resistance left, ElectricCurrent right) =>
		IIntegralOperators<Resistance, ElectricCurrent, ElectricPotential>.Integrate(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Resistance"/>.
/// </summary>
public static class ResistanceConversions
{
	/// <summary>
	/// Converts a value to ohms.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Resistance"/> representing the value in ohms.</returns>
	public static Resistance Ohms<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Resistance>();

	/// <summary>
	/// Converts a <see cref="Resistance"/> to a numeric value in ohms.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Resistance"/> to convert.</param>
	/// <returns>The numeric value in ohms.</returns>
	public static TNumber Ohms<TNumber>(this Resistance value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to milliohms.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Resistance"/> representing the value in milliohms.</returns>
	public static Resistance Milliohms<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Resistance>(PhysicalConstants.Milli);

	/// <summary>
	/// Converts a <see cref="Resistance"/> to a numeric value in milliohms.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Resistance"/> to convert.</param>
	/// <returns>The numeric value in milliohms.</returns>
	public static TNumber Milliohms<TNumber>(this Resistance value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Milli));

	/// <summary>
	/// Converts a value to kiloohms.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Resistance"/> representing the value in kiloohms.</returns>
	public static Resistance Kiloohms<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Resistance>(PhysicalConstants.Kilo);

	/// <summary>
	/// Converts a <see cref="Resistance"/> to a numeric value in kiloohms.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Resistance"/> to convert.</param>
	/// <returns>The numeric value in kiloohms.</returns>
	public static TNumber Kiloohms<TNumber>(this Resistance value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Kilo));

	/// <summary>
	/// Converts a value to megaohms.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Resistance"/> representing the value in megaohms.</returns>
	public static Resistance Megaohms<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Resistance>(PhysicalConstants.Mega);

	/// <summary>
	/// Converts a <see cref="Resistance"/> to a numeric value in megaohms.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Resistance"/> to convert.</param>
	/// <returns>The numeric value in megaohms.</returns>
	public static TNumber Megaohms<TNumber>(this Resistance value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Mega));
}
