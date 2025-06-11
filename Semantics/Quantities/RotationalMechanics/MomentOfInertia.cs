// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a moment of inertia physical quantity.
/// </summary>
[SIUnit("kg⋅m²", "kilogram square meter", "kilogram square meters")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
public sealed record MomentOfInertia
	: PhysicalQuantity<MomentOfInertia>
	, IIntegralOperators<MomentOfInertia, AngularVelocity, AngularMomentum>
{
	/// <summary>
	/// Multiplies a <see cref="MomentOfInertia"/> by an <see cref="AngularVelocity"/> to compute an <see cref="AngularMomentum"/>.
	/// </summary>
	/// <param name="left">The moment of inertia operand.</param>
	/// <param name="right">The angular velocity operand.</param>
	/// <returns>An <see cref="AngularMomentum"/> representing the result of the multiplication.</returns>
	public static AngularMomentum operator *(MomentOfInertia left, AngularVelocity right) =>
		IIntegralOperators<MomentOfInertia, AngularVelocity, AngularMomentum>.Integrate(left, right);

	/// <summary>
	/// Creates a <see cref="MomentOfInertia"/> from a given mass and radius.
	/// </summary>
	/// <param name="mass">The mass of the object.</param>
	/// <param name="radius">The radius of rotation.</param>
	/// <returns>A new instance of <see cref="MomentOfInertia"/>.</returns>
	public static MomentOfInertia FromMassAndRadius(Mass mass, Length radius)
	{
		ArgumentNullException.ThrowIfNull(mass);
		ArgumentNullException.ThrowIfNull(radius);
		Area area = radius * radius; // This creates an Area (Length × Length)
		return Create(mass.Quantity * area.Quantity);
	}
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="MomentOfInertia"/>.
/// </summary>
public static class MomentOfInertiaConversions
{
	/// <summary>
	/// Converts a value to kilogram square meters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="MomentOfInertia"/> representing the value in kilogram square meters.</returns>
	public static MomentOfInertia KilogramSquareMeters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, MomentOfInertia>();

	/// <summary>
	/// Converts a <see cref="MomentOfInertia"/> to a numeric value in kilogram square meters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="MomentOfInertia"/> to convert.</param>
	/// <returns>The numeric value in kilogram square meters.</returns>
	public static TNumber KilogramSquareMeters<TNumber>(this MomentOfInertia value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to gram square meters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="MomentOfInertia"/> representing the value in gram square meters.</returns>
	public static MomentOfInertia GramSquareMeters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, MomentOfInertia>(PhysicalConstants.Milli);

	/// <summary>
	/// Converts a <see cref="MomentOfInertia"/> to a numeric value in gram square meters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="MomentOfInertia"/> to convert.</param>
	/// <returns>The numeric value in gram square meters.</returns>
	public static TNumber GramSquareMeters<TNumber>(this MomentOfInertia value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Milli));

	/// <summary>
	/// Converts a value to milligram square meters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>A <see cref="MomentOfInertia"/> representing the value in milligram square meters.</returns>
	public static MomentOfInertia MilligramSquareMeters<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, MomentOfInertia>(PhysicalConstants.Micro);

	/// <summary>
	/// Converts a <see cref="MomentOfInertia"/> to a numeric value in milligram square meters.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="MomentOfInertia"/> to convert.</param>
	/// <returns>The numeric value in milligram square meters.</returns>
	public static TNumber MilligramSquareMeters<TNumber>(this MomentOfInertia value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Micro));
}
