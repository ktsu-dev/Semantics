// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Globalization;
using System.Numerics;

/// <summary>
/// Represents a percentage (a ratio scaled by 100).
/// </summary>
/// <remarks>
/// <c>100</c> percent corresponds to a <see cref="Ratio{T}"/> of one. This type exists so that
/// user-facing parameters (mix, depth, drive) can be expressed in the units a musician expects while
/// still round-tripping losslessly to and from a normalized ratio.
/// </remarks>
/// <typeparam name="T">The floating-point storage type.</typeparam>
/// <param name="Value">The percentage value.</param>
public readonly record struct Percent<T>(T Value) : IComparable<Percent<T>>
	where T : struct, INumber<T>
{
	/// <summary>
	/// Creates a percentage from a raw value.
	/// </summary>
	/// <param name="value">The percentage value (100 = unity).</param>
	/// <returns>A new <see cref="Percent{T}"/>.</returns>
	public static Percent<T> Create(T value) => new(value);

	/// <summary>
	/// Creates a percentage from a fraction in the range <c>[0, 1]</c> (0.5 → 50%).
	/// </summary>
	/// <param name="fraction">The fraction to scale by 100.</param>
	/// <returns>A new <see cref="Percent{T}"/>.</returns>
	public static Percent<T> FromFraction(T fraction) => new(fraction * T.CreateChecked(100));

	/// <summary>
	/// Creates a percentage from a ratio (1 → 100%).
	/// </summary>
	/// <param name="ratio">The ratio to convert.</param>
	/// <returns>A new <see cref="Percent{T}"/>.</returns>
	public static Percent<T> FromRatio(Ratio<T> ratio)
	{
		ArgumentNullException.ThrowIfNull(ratio);
		return FromFraction(ratio.Value);
	}

	/// <summary>
	/// Converts this percentage to a fraction in the range <c>[0, 1]</c> (50% → 0.5).
	/// </summary>
	/// <returns>The fractional value.</returns>
	public T ToFraction() => Value / T.CreateChecked(100);

	/// <summary>
	/// Converts this percentage to a ratio (100% → 1).
	/// </summary>
	/// <returns>The equivalent <see cref="Ratio{T}"/>.</returns>
	public Ratio<T> ToRatio() => Ratio<T>.Create(ToFraction());

	/// <inheritdoc/>
	public int CompareTo(Percent<T> other) => Value.CompareTo(other.Value);

	/// <summary>Determines whether one percentage is less than another.</summary>
	/// <param name="left">The left percentage.</param>
	/// <param name="right">The right percentage.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
	public static bool operator <(Percent<T> left, Percent<T> right) => left.CompareTo(right) < 0;

	/// <summary>Determines whether one percentage is greater than another.</summary>
	/// <param name="left">The left percentage.</param>
	/// <param name="right">The right percentage.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
	public static bool operator >(Percent<T> left, Percent<T> right) => left.CompareTo(right) > 0;

	/// <summary>Determines whether one percentage is less than or equal to another.</summary>
	/// <param name="left">The left percentage.</param>
	/// <param name="right">The right percentage.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
	public static bool operator <=(Percent<T> left, Percent<T> right) => left.CompareTo(right) <= 0;

	/// <summary>Determines whether one percentage is greater than or equal to another.</summary>
	/// <param name="left">The left percentage.</param>
	/// <param name="right">The right percentage.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
	public static bool operator >=(Percent<T> left, Percent<T> right) => left.CompareTo(right) >= 0;

	/// <summary>Returns a culture-invariant string representation of this percentage.</summary>
	/// <returns>The percentage formatted with a <c>%</c> suffix.</returns>
	public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"{Value}%");
}
