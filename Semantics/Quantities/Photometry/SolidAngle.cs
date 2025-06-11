// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a solid angle physical quantity.
/// </summary>
[SIUnit(typeof(SIUnits), nameof(SIUnits.Steradian))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]

public sealed record SolidAngle
	: PhysicalQuantity<SolidAngle>
	, IIntegralOperators<SolidAngle, LuminousIntensity, LuminousFlux>
{
	/// <summary>
	/// Multiplies a <see cref="SolidAngle"/> by a <see cref="LuminousIntensity"/> to compute a <see cref="LuminousFlux"/>.
	/// </summary>
	/// <param name="left">The solid angle operand.</param>
	/// <param name="right">The luminous intensity operand.</param>
	/// <returns>A <see cref="LuminousFlux"/> representing the result of the multiplication.</returns>
	public static LuminousFlux operator *(SolidAngle left, LuminousIntensity right) =>
		IIntegralOperators<SolidAngle, LuminousIntensity, LuminousFlux>.Integrate(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="SolidAngle"/>.
/// </summary>
public static class SolidAngleConversions
{
	/// <summary>
	/// Converts a value to steradians.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="SolidAngle"/> representing the value in steradians.</returns>
	public static SolidAngle Steradians<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, SolidAngle>();

	/// <summary>
	/// Converts a <see cref="SolidAngle"/> to a numeric value in steradians.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="SolidAngle"/> to convert.</param>
	/// <returns>The numeric value in steradians.</returns>
	public static TNumber Steradians<TNumber>(this SolidAngle value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to square degrees.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="SolidAngle"/> representing the value in square degrees.</returns>
	public static SolidAngle SquareDegrees<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, SolidAngle>(PhysicalConstants.SquareDegreesToSteradiansFactor);

	/// <summary>
	/// Converts a <see cref="SolidAngle"/> to a numeric value in square degrees.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="SolidAngle"/> to convert.</param>
	/// <returns>The numeric value in square degrees.</returns>
	public static TNumber SquareDegrees<TNumber>(this SolidAngle value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.SquareDegreesToSteradiansFactor));
}
