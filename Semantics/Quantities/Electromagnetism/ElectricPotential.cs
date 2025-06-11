// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an electric potential physical quantity.
/// </summary>
[SIUnit("V", "volt", "volts")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
[Obsolete]
public sealed record ElectricPotential
	: PhysicalQuantity<ElectricPotential>
	, IIntegralOperators<ElectricPotential, ElectricCurrent, Power>
	, IIntegralOperators<ElectricPotential, Charge, Energy>
	, IDerivativeOperators<ElectricPotential, ElectricCurrent, Resistance>
	, IDerivativeOperators<ElectricPotential, Resistance, ElectricCurrent>
{
	/// <summary>
	/// Multiplies an <see cref="ElectricPotential"/> by an <see cref="ElectricCurrent"/> to compute a <see cref="Power"/>.
	/// </summary>
	/// <param name="left">The electric potential operand.</param>
	/// <param name="right">The electric current operand.</param>
	/// <returns>A <see cref="Power"/> representing the result of the multiplication.</returns>
	public static Power operator *(ElectricPotential left, ElectricCurrent right) =>
		IIntegralOperators<ElectricPotential, ElectricCurrent, Power>.Integrate(left, right);

	/// <summary>
	/// Multiplies an <see cref="ElectricPotential"/> by a <see cref="Charge"/> to compute an <see cref="Energy"/>.
	/// </summary>
	/// <param name="left">The electric potential operand.</param>
	/// <param name="right">The charge operand.</param>
	/// <returns>An <see cref="Energy"/> representing the result of the multiplication.</returns>
	public static Energy operator *(ElectricPotential left, Charge right) =>
		IIntegralOperators<ElectricPotential, Charge, Energy>.Integrate(left, right);

	/// <summary>
	/// Divides an <see cref="ElectricPotential"/> by an <see cref="ElectricCurrent"/> to compute a <see cref="Resistance"/>.
	/// </summary>
	/// <param name="left">The electric potential operand.</param>
	/// <param name="right">The electric current operand.</param>
	/// <returns>A <see cref="Resistance"/> representing the result of the division.</returns>
	public static Resistance operator /(ElectricPotential left, ElectricCurrent right) =>
		IDerivativeOperators<ElectricPotential, ElectricCurrent, Resistance>.Derive(left, right);

	/// <summary>
	/// Divides an <see cref="ElectricPotential"/> by a <see cref="Resistance"/> to compute an <see cref="ElectricCurrent"/>.
	/// </summary>
	/// <param name="left">The electric potential operand.</param>
	/// <param name="right">The resistance operand.</param>
	/// <returns>An <see cref="ElectricCurrent"/> representing the result of the division.</returns>
	public static ElectricCurrent operator /(ElectricPotential left, Resistance right) =>
		IDerivativeOperators<ElectricPotential, Resistance, ElectricCurrent>.Derive(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="ElectricPotential"/>.
/// </summary>
public static class ElectricPotentialConversions
{
	/// <summary>
	/// Converts a value to volts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="ElectricPotential"/> representing the value in volts.</returns>
	public static ElectricPotential Volts<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, ElectricPotential>();

	/// <summary>
	/// Converts an <see cref="ElectricPotential"/> to a numeric value in volts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="ElectricPotential"/> to convert.</param>
	/// <returns>The numeric value in volts.</returns>
	public static TNumber Volts<TNumber>(this ElectricPotential value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to millivolts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="ElectricPotential"/> representing the value in millivolts.</returns>
	public static ElectricPotential Millivolts<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, ElectricPotential>(PhysicalConstants.Milli);

	/// <summary>
	/// Converts an <see cref="ElectricPotential"/> to a numeric value in millivolts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="ElectricPotential"/> to convert.</param>
	/// <returns>The numeric value in millivolts.</returns>
	public static TNumber Millivolts<TNumber>(this ElectricPotential value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Milli));

	/// <summary>
	/// Converts a value to microvolts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="ElectricPotential"/> representing the value in microvolts.</returns>
	public static ElectricPotential Microvolts<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, ElectricPotential>(PhysicalConstants.Micro);

	/// <summary>
	/// Converts an <see cref="ElectricPotential"/> to a numeric value in microvolts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="ElectricPotential"/> to convert.</param>
	/// <returns>The numeric value in microvolts.</returns>
	public static TNumber Microvolts<TNumber>(this ElectricPotential value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Micro));

	/// <summary>
	/// Converts a value to nanovolts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="ElectricPotential"/> representing the value in nanovolts.</returns>
	public static ElectricPotential Nanovolts<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, ElectricPotential>(PhysicalConstants.Nano);

	/// <summary>
	/// Converts an <see cref="ElectricPotential"/> to a numeric value in nanovolts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="ElectricPotential"/> to convert.</param>
	/// <returns>The numeric value in nanovolts.</returns>
	public static TNumber Nanovolts<TNumber>(this ElectricPotential value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Nano));

	/// <summary>
	/// Converts a value to kilovolts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="ElectricPotential"/> representing the value in kilovolts.</returns>
	public static ElectricPotential Kilovolts<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, ElectricPotential>(PhysicalConstants.Kilo);

	/// <summary>
	/// Converts an <see cref="ElectricPotential"/> to a numeric value in kilovolts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="ElectricPotential"/> to convert.</param>
	/// <returns>The numeric value in kilovolts.</returns>
	public static TNumber Kilovolts<TNumber>(this ElectricPotential value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Kilo));

	/// <summary>
	/// Converts a value to megavolts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="ElectricPotential"/> representing the value in megavolts.</returns>
	public static ElectricPotential Megavolts<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, ElectricPotential>(PhysicalConstants.Mega);

	/// <summary>
	/// Converts an <see cref="ElectricPotential"/> to a numeric value in megavolts.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="ElectricPotential"/> to convert.</param>
	/// <returns>The numeric value in megavolts.</returns>
	public static TNumber Megavolts<TNumber>(this ElectricPotential value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Mega));
}
