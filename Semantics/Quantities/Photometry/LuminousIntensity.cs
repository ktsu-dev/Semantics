// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a luminous intensity physical quantity.
/// </summary>
[SIUnit("cd", "candela", "candelas")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
[Obsolete]
public sealed record LuminousIntensity
	: PhysicalQuantity<LuminousIntensity>
	, IIntegralOperators<LuminousIntensity, SolidAngle, LuminousFlux>
{
	/// <summary>
	/// Multiplies a <see cref="LuminousIntensity"/> by a <see cref="SolidAngle"/> to compute a <see cref="LuminousFlux"/>.
	/// </summary>
	/// <param name="left">The luminous intensity operand.</param>
	/// <param name="right">The solid angle operand.</param>
	/// <returns>A <see cref="LuminousFlux"/> representing the result of the multiplication.</returns>
	public static LuminousFlux operator *(LuminousIntensity left, SolidAngle right) =>
		IIntegralOperators<LuminousIntensity, SolidAngle, LuminousFlux>.Integrate(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="LuminousIntensity"/>.
/// </summary>
public static class LuminousIntensityConversions
{
	/// <summary>
	/// Converts a value to candelas.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="LuminousIntensity"/> representing the value in candelas.</returns>
	public static LuminousIntensity Candelas<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, LuminousIntensity>();

	/// <summary>
	/// Converts a <see cref="LuminousIntensity"/> to a numeric value in candelas.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="LuminousIntensity"/> to convert.</param>
	/// <returns>The numeric value in candelas.</returns>
	public static TNumber Candelas<TNumber>(this LuminousIntensity value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());
}
