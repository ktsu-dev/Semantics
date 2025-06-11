// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an angular momentum physical quantity.
/// </summary>
[SIUnit("kg⋅m²/s", "kilogram square meter per second", "kilogram square meters per second")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Physical quantity operations")]
public sealed record AngularMomentum
	: PhysicalQuantity<AngularMomentum>
	, IDerivativeOperators<AngularMomentum, AngularVelocity, MomentOfInertia>
	, IDerivativeOperators<AngularMomentum, MomentOfInertia, AngularVelocity>
{
	/// <summary>
	/// Divides an <see cref="AngularMomentum"/> by an <see cref="AngularVelocity"/> to compute a <see cref="MomentOfInertia"/>.
	/// </summary>
	/// <param name="left">The angular momentum operand.</param>
	/// <param name="right">The angular velocity operand.</param>
	/// <returns>A <see cref="MomentOfInertia"/> representing the result of the division.</returns>
	public static MomentOfInertia operator /(AngularMomentum left, AngularVelocity right) =>
		IDerivativeOperators<AngularMomentum, AngularVelocity, MomentOfInertia>.Derive(left, right);

	/// <summary>
	/// Divides an <see cref="AngularMomentum"/> by a <see cref="MomentOfInertia"/> to compute an <see cref="AngularVelocity"/>.
	/// </summary>
	/// <param name="left">The angular momentum operand.</param>
	/// <param name="right">The moment of inertia operand.</param>
	/// <returns>An <see cref="AngularVelocity"/> representing the result of the division.</returns>
	public static AngularVelocity operator /(AngularMomentum left, MomentOfInertia right) =>
		IDerivativeOperators<AngularMomentum, MomentOfInertia, AngularVelocity>.Derive(left, right);
}

/// <summary>
/// Provides extension methods for converting values to and from <see cref="AngularMomentum"/>.
/// </summary>
public static class AngularMomentumConversions
{
	/// <summary>
	/// Converts a value to kilogram square meters per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AngularMomentum"/> representing the value in kilogram square meters per second.</returns>
	public static AngularMomentum KilogramSquareMetersPerSecond<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AngularMomentum>();

	/// <summary>
	/// Converts an <see cref="AngularMomentum"/> to a numeric value in kilogram square meters per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AngularMomentum"/> to convert.</param>
	/// <returns>The numeric value in kilogram square meters per second.</returns>
	public static TNumber KilogramSquareMetersPerSecond<TNumber>(this AngularMomentum value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber());

	/// <summary>
	/// Converts a value to gram square meters per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AngularMomentum"/> representing the value in gram square meters per second.</returns>
	public static AngularMomentum GramSquareMetersPerSecond<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AngularMomentum>(PhysicalConstants.Milli);

	/// <summary>
	/// Converts an <see cref="AngularMomentum"/> to a numeric value in gram square meters per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AngularMomentum"/> to convert.</param>
	/// <returns>The numeric value in gram square meters per second.</returns>
	public static TNumber GramSquareMetersPerSecond<TNumber>(this AngularMomentum value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Milli));

	/// <summary>
	/// Converts a value to milligram square meters per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to convert.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <returns>An <see cref="AngularMomentum"/> representing the value in milligram square meters per second.</returns>
	public static AngularMomentum MilligramSquareMetersPerSecond<TNumber>(this TNumber value)
		where TNumber : INumber<TNumber>
		=> value.ConvertToQuantity<TNumber, AngularMomentum>(PhysicalConstants.Micro);

	/// <summary>
	/// Converts an <see cref="AngularMomentum"/> to a numeric value in milligram square meters per second.
	/// </summary>
	/// <typeparam name="TNumber">The type of the numeric value.</typeparam>
	/// <param name="value">The <see cref="AngularMomentum"/> to convert.</param>
	/// <returns>The numeric value in milligram square meters per second.</returns>
	public static TNumber MilligramSquareMetersPerSecond<TNumber>(this AngularMomentum value)
		where TNumber : INumber<TNumber>
		=> TNumber.CreateChecked(value.ConvertToNumber(PhysicalConstants.Micro));
}
