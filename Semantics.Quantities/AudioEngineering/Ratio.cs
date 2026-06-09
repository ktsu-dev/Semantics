// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Globalization;
using System.Numerics;

/// <summary>
/// Represents a dimensionless ratio between two like quantities.
/// </summary>
/// <remarks>
/// A ratio of <c>1</c> means the two quantities are equal. Ratios are the natural currency of audio
/// engineering parameters (gain, mix, feedback, depth) and convert cleanly to and from
/// <see cref="Percent{T}"/>.
/// </remarks>
/// <typeparam name="T">The floating-point storage type.</typeparam>
/// <param name="Value">The ratio value.</param>
public readonly record struct Ratio<T>(T Value) : IComparable<Ratio<T>>
	where T : struct, INumber<T>
{
	/// <summary>Gets a ratio of one (unity).</summary>
	public static Ratio<T> Unity => new(T.One);

	/// <summary>Gets a ratio of zero.</summary>
	public static Ratio<T> Zero => new(T.Zero);

	/// <summary>
	/// Creates a ratio from a raw value.
	/// </summary>
	/// <param name="value">The ratio value (1 = unity).</param>
	/// <returns>A new <see cref="Ratio{T}"/>.</returns>
	public static Ratio<T> Create(T value) => new(value);

	/// <summary>
	/// Creates a ratio from a percentage.
	/// </summary>
	/// <param name="percent">The percentage (100% = unity).</param>
	/// <returns>A new <see cref="Ratio{T}"/>.</returns>
	public static Ratio<T> FromPercent(Percent<T> percent) => percent.ToRatio();

	/// <summary>
	/// Converts this ratio to a percentage (1 → 100%).
	/// </summary>
	/// <returns>The equivalent <see cref="Percent{T}"/>.</returns>
	public Percent<T> ToPercent() => new(Value * T.CreateChecked(100));

	/// <summary>Multiplies two ratios.</summary>
	/// <param name="left">The left ratio.</param>
	/// <param name="right">The right ratio.</param>
	/// <returns>The product ratio.</returns>
	public static Ratio<T> operator *(Ratio<T> left, Ratio<T> right) => new(left.Value * right.Value);

	/// <summary>Divides two ratios.</summary>
	/// <param name="left">The numerator ratio.</param>
	/// <param name="right">The denominator ratio.</param>
	/// <returns>The quotient ratio.</returns>
	public static Ratio<T> operator /(Ratio<T> left, Ratio<T> right) => new(left.Value / right.Value);

	/// <summary>Multiplies two ratios (friendly alternate for <c>operator *</c>).</summary>
	/// <param name="left">The left ratio.</param>
	/// <param name="right">The right ratio.</param>
	/// <returns>The product ratio.</returns>
	public static Ratio<T> Multiply(Ratio<T> left, Ratio<T> right) => left * right;

	/// <summary>Divides two ratios (friendly alternate for <c>operator /</c>).</summary>
	/// <param name="left">The numerator ratio.</param>
	/// <param name="right">The denominator ratio.</param>
	/// <returns>The quotient ratio.</returns>
	public static Ratio<T> Divide(Ratio<T> left, Ratio<T> right) => left / right;

	/// <inheritdoc/>
	public int CompareTo(Ratio<T> other) => Value.CompareTo(other.Value);

	/// <summary>Determines whether one ratio is less than another.</summary>
	/// <param name="left">The left ratio.</param>
	/// <param name="right">The right ratio.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
	public static bool operator <(Ratio<T> left, Ratio<T> right) => left.CompareTo(right) < 0;

	/// <summary>Determines whether one ratio is greater than another.</summary>
	/// <param name="left">The left ratio.</param>
	/// <param name="right">The right ratio.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
	public static bool operator >(Ratio<T> left, Ratio<T> right) => left.CompareTo(right) > 0;

	/// <summary>Determines whether one ratio is less than or equal to another.</summary>
	/// <param name="left">The left ratio.</param>
	/// <param name="right">The right ratio.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
	public static bool operator <=(Ratio<T> left, Ratio<T> right) => left.CompareTo(right) <= 0;

	/// <summary>Determines whether one ratio is greater than or equal to another.</summary>
	/// <param name="left">The left ratio.</param>
	/// <param name="right">The right ratio.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
	public static bool operator >=(Ratio<T> left, Ratio<T> right) => left.CompareTo(right) >= 0;

	/// <summary>Returns a culture-invariant string representation of this ratio.</summary>
	/// <returns>The ratio formatted with an <c>x</c> suffix.</returns>
	public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"{Value}x");
}
