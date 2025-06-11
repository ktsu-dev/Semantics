// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an illuminance physical quantity.
/// </summary>
[SIUnit(typeof(SIUnits), nameof(SIUnits.Lux))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]

public sealed record Illuminance
	: PhysicalQuantity<Illuminance>
	, IIntegralOperators<Illuminance, Area, LuminousFlux>
{
	/// <summary>
	/// Multiplies an <see cref="Illuminance"/> by an <see cref="Area"/> to compute a <see cref="LuminousFlux"/>.
	/// </summary>
	/// <param name="left">The illuminance operand.</param>
	/// <param name="right">The area operand.</param>
	/// <returns>A <see cref="LuminousFlux"/> representing the result of the multiplication.</returns>
	public static LuminousFlux operator *(Illuminance left, Area right) =>
		IIntegralOperators<Illuminance, Area, LuminousFlux>.Integrate(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="Illuminance"/>.
/// </summary>
public static class IlluminanceConversions
{
	/// <summary>
	/// Converts a value to lux.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Illuminance"/> representing the value in lux.</returns>
	public static Illuminance Lux<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Illuminance>();

	/// <summary>
	/// Converts an <see cref="Illuminance"/> to a numeric value in lux.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Illuminance"/> to convert.</param>
	/// <returns>The numeric value in lux.</returns>
	public static TNumber Lux<TNumber>(this Illuminance value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to foot-candles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="Illuminance"/> representing the value in foot-candles.</returns>
	public static Illuminance FootCandle<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, Illuminance>(PhysicalConstants.FootCandleToLuxFactor);

	/// <summary>
	/// Converts an <see cref="Illuminance"/> to a numeric value in foot-candles.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="Illuminance"/> to convert.</param>
	/// <returns>The numeric value in foot-candles.</returns>
	public static TNumber FootCandle<TNumber>(this Illuminance value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.FootCandleToLuxFactor));
}
