// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a molar mass physical quantity.
/// </summary>
[SIUnit("kg/mol", "kilogram per mole", "kilograms per mole")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
[Obsolete]
public sealed record MolarMass
	: PhysicalQuantity<MolarMass>
	, IIntegralOperators<MolarMass, AmountOfSubstance, Mass>
{
	/// <summary>
	/// Multiplies a <see cref="MolarMass"/> by an <see cref="AmountOfSubstance"/> to compute a <see cref="Mass"/>.
	/// </summary>
	/// <param name="left">The molar mass operand.</param>
	/// <param name="right">The amount of substance operand.</param>
	/// <returns>A <see cref="Mass"/> representing the result of the multiplication.</returns>
	public static Mass operator *(MolarMass left, AmountOfSubstance right) =>
		IIntegralOperators<MolarMass, AmountOfSubstance, Mass>.Integrate(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="MolarMass"/>.
/// </summary>
public static class MolarMassConversions
{
	/// <summary>
	/// Converts a value to kilograms per mole.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="MolarMass"/> representing the value in kilograms per mole.</returns>
	public static MolarMass KilogramsPerMole<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, MolarMass>();

	/// <summary>
	/// Converts a <see cref="MolarMass"/> to a numeric value in kilograms per mole.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="MolarMass"/> to convert.</param>
	/// <returns>The numeric value in kilograms per mole.</returns>
	public static TNumber KilogramsPerMole<TNumber>(this MolarMass value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to grams per mole.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="MolarMass"/> representing the value in grams per mole.</returns>
	public static MolarMass GramsPerMole<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, MolarMass>(PhysicalConstants.Milli);

	/// <summary>
	/// Converts a <see cref="MolarMass"/> to a numeric value in grams per mole.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="MolarMass"/> to convert.</param>
	/// <returns>The numeric value in grams per mole.</returns>
	public static TNumber GramsPerMole<TNumber>(this MolarMass value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Milli));

	/// <summary>
	/// Converts a value to milligrams per mole.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="MolarMass"/> representing the value in milligrams per mole.</returns>
	public static MolarMass MilligramsPerMole<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, MolarMass>(PhysicalConstants.Micro);

	/// <summary>
	/// Converts a <see cref="MolarMass"/> to a numeric value in milligrams per mole.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="MolarMass"/> to convert.</param>
	/// <returns>The numeric value in milligrams per mole.</returns>
	public static TNumber MilligramsPerMole<TNumber>(this MolarMass value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Micro));
}
