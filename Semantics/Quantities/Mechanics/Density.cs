// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a density physical quantity.
/// </summary>
[SIUnit(typeof(SIUnits), nameof(SIUnits.KilogramPerCubicMeter))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]

public sealed record Density
	: PhysicalQuantity<Density>
	, IIntegralOperators<Density, Volume, Mass>
{
	/// <summary>
	/// Multiplies a <see cref="Density"/> by a <see cref="Volume"/> to compute a <see cref="Mass"/>.
	/// </summary>
	/// <param name="left">The density operand.</param>
	/// <param name="right">The volume operand.</param>
	/// <returns>A <see cref="Mass"/> representing the result of the multiplication.</returns>
	public static Mass operator *(Density left, Volume right) =>
		IIntegralOperators<Density, Volume, Mass>.Integrate(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Density"/>.
/// </summary>
public static class DensityConversions
{
	/// <summary>
	/// Converts a value to kilograms per cubic meter.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Density"/> representing the value in kilograms per cubic meter.</returns>
	public static Density KilogramsPerCubicMeter<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Density>();

	/// <summary>
	/// Converts a <see cref="Density"/> to a numeric value in kilograms per cubic meter.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Density"/> to convert.</param>
	/// <returns>The numeric value in kilograms per cubic meter.</returns>
	public static TNumber KilogramsPerCubicMeter<TNumber>(this Density value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to grams per cubic centimeter.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Density"/> representing the value in grams per cubic centimeter.</returns>
	public static Density GramsPerCubicCentimeter<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Density>(PhysicalConstants.Kilo);

	/// <summary>
	/// Converts a <see cref="Density"/> to a numeric value in grams per cubic centimeter.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Density"/> to convert.</param>
	/// <returns>The numeric value in grams per cubic centimeter.</returns>
	public static TNumber GramsPerCubicCentimeter<TNumber>(this Density value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Kilo));

	/// <summary>
	/// Converts a value to milligrams per cubic meter.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Density"/> representing the value in milligrams per cubic meter.</returns>
	public static Density MilligramsPerCubicMeter<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Density>(PhysicalConstants.Milli);

	/// <summary>
	/// Converts a <see cref="Density"/> to a numeric value in milligrams per cubic meter.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Density"/> to convert.</param>
	/// <returns>The numeric value in milligrams per cubic meter.</returns>
	public static TNumber MilligramsPerCubicMeter<TNumber>(this Density value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Milli));

	/// <summary>
	/// Converts a value to micrograms per cubic meter.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Density"/> representing the value in micrograms per cubic meter.</returns>
	public static Density MicrogramsPerCubicMeter<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Density>(PhysicalConstants.Micro);

	/// <summary>
	/// Converts a <see cref="Density"/> to a numeric value in micrograms per cubic meter.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Density"/> to convert.</param>
	/// <returns>The numeric value in micrograms per cubic meter.</returns>
	public static TNumber MicrogramsPerCubicMeter<TNumber>(this Density value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Micro));

	/// <summary>
	/// Converts a value to nanograms per cubic meter.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="Density"/> representing the value in nanograms per cubic meter.</returns>
	public static Density NanogramsPerCubicMeter<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Density>(PhysicalConstants.Nano);

	/// <summary>
	/// Converts a <see cref="Density"/> to a numeric value in nanograms per cubic meter.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Density"/> to convert.</param>
	/// <returns>The numeric value in nanograms per cubic meter.</returns>
	public static TNumber NanogramsPerCubicMeter<TNumber>(this Density value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Nano));
}
