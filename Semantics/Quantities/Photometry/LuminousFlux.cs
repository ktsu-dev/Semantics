// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a luminous flux physical quantity.
/// </summary>
[SIUnit(typeof(SIUnits), nameof(SIUnits.Lumen))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]

public sealed record LuminousFlux
	: PhysicalQuantity<LuminousFlux>
	, IDerivativeOperators<LuminousFlux, SolidAngle, LuminousIntensity>
	, IDerivativeOperators<LuminousFlux, Area, Illuminance>
{
	/// <summary>
	/// Divides a <see cref="LuminousFlux"/> by a <see cref="SolidAngle"/> to compute a <see cref="LuminousIntensity"/>.
	/// </summary>
	/// <param name="left">The luminous flux operand.</param>
	/// <param name="right">The solid angle operand.</param>
	/// <returns>A <see cref="LuminousIntensity"/> representing the result of the division.</returns>
	public static LuminousIntensity operator /(LuminousFlux left, SolidAngle right) =>
		IDerivativeOperators<LuminousFlux, SolidAngle, LuminousIntensity>.Derive(left, right);

	/// <summary>
	/// Divides a <see cref="LuminousFlux"/> by an <see cref="Area"/> to compute an <see cref="Illuminance"/>.
	/// </summary>
	/// <param name="left">The luminous flux operand.</param>
	/// <param name="right">The area operand.</param>
	/// <returns>An <see cref="Illuminance"/> representing the result of the division.</returns>
	public static Illuminance operator /(LuminousFlux left, Area right) =>
		IDerivativeOperators<LuminousFlux, Area, Illuminance>.Derive(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="LuminousFlux"/>.
/// </summary>
public static class LuminousFluxConversions
{
	/// <summary>
	/// Converts a value to lumens.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="LuminousFlux"/> representing the value in lumens.</returns>
	public static LuminousFlux Lumens<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, LuminousFlux>();

	/// <summary>
	/// Converts a <see cref="LuminousFlux"/> to a numeric value in lumens.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="LuminousFlux"/> to convert.</param>
	/// <returns>The numeric value in lumens.</returns>
	public static TNumber Lumens<TNumber>(this LuminousFlux value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());
}
