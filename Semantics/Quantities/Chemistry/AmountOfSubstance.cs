// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an amount of substance physical quantity.
/// </summary>
[SIUnit("mol", "mole", "moles")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
[Obsolete]
public sealed record AmountOfSubstance
	: PhysicalQuantity<AmountOfSubstance>
	, IIntegralOperators<AmountOfSubstance, MolarMass, Mass>
{
	/// <summary>
	/// Multiplies an <see cref="AmountOfSubstance"/> by a <see cref="MolarMass"/> to compute a <see cref="Mass"/>.
	/// </summary>
	/// <param name="left">The amount of substance operand.</param>
	/// <param name="right">The molar mass operand.</param>
	/// <returns>A <see cref="Mass"/> representing the result of the multiplication.</returns>
	public static Mass operator *(AmountOfSubstance left, MolarMass right) =>
		IIntegralOperators<AmountOfSubstance, MolarMass, Mass>.Integrate(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="AmountOfSubstance"/>.
/// </summary>
public static class AmountOfSubstanceConversions
{
	/// <summary>
	/// Converts a value to moles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AmountOfSubstance"/> representing the value in moles.</returns>
	public static AmountOfSubstance Moles<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AmountOfSubstance>();

	/// <summary>
	/// Converts an <see cref="AmountOfSubstance"/> to a numeric value in moles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AmountOfSubstance"/> to convert.</param>
	/// <returns>The numeric value in moles.</returns>
	public static TNumber Moles<TNumber>(this AmountOfSubstance value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to kilomoles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AmountOfSubstance"/> representing the value in kilomoles.</returns>
	public static AmountOfSubstance Kilomoles<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AmountOfSubstance>(PhysicalConstants.Kilo);

	/// <summary>
	/// Converts an <see cref="AmountOfSubstance"/> to a numeric value in kilomoles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AmountOfSubstance"/> to convert.</param>
	/// <returns>The numeric value in kilomoles.</returns>
	public static TNumber Kilomoles<TNumber>(this AmountOfSubstance value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Kilo));

	/// <summary>
	/// Converts a value to millimoles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AmountOfSubstance"/> representing the value in millimoles.</returns>
	public static AmountOfSubstance Millimoles<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AmountOfSubstance>(PhysicalConstants.Milli);

	/// <summary>
	/// Converts an <see cref="AmountOfSubstance"/> to a numeric value in millimoles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AmountOfSubstance"/> to convert.</param>
	/// <returns>The numeric value in millimoles.</returns>
	public static TNumber Millimoles<TNumber>(this AmountOfSubstance value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Milli));

	/// <summary>
	/// Converts a value to micromoles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AmountOfSubstance"/> representing the value in micromoles.</returns>
	public static AmountOfSubstance Micromoles<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AmountOfSubstance>(PhysicalConstants.Micro);

	/// <summary>
	/// Converts an <see cref="AmountOfSubstance"/> to a numeric value in micromoles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AmountOfSubstance"/> to convert.</param>
	/// <returns>The numeric value in micromoles.</returns>
	public static TNumber Micromoles<TNumber>(this AmountOfSubstance value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Micro));

	/// <summary>
	/// Converts a value to nanomoles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AmountOfSubstance"/> representing the value in nanomoles.</returns>
	public static AmountOfSubstance Nanomoles<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AmountOfSubstance>(PhysicalConstants.Nano);

	/// <summary>
	/// Converts an <see cref="AmountOfSubstance"/> to a numeric value in nanomoles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AmountOfSubstance"/> to convert.</param>
	/// <returns>The numeric value in nanomoles.</returns>
	public static TNumber Nanomoles<TNumber>(this AmountOfSubstance value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Nano));
}
