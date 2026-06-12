// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Globalization;
using System.Numerics;

/// <summary>
/// Represents a musical pitch interval measured in semitones (twelve-tone equal temperament).
/// </summary>
/// <remarks>
/// One semitone is a frequency ratio of <c>2^(1/12)</c>; twelve semitones make one octave (a ratio of
/// two). Semitones are the natural unit for pitch-shift, detune, and transpose parameters and convert
/// to and from <see cref="Cents{T}"/> (1 semitone = 100 cents) and frequency ratios.
/// </remarks>
/// <typeparam name="T">The floating-point storage type.</typeparam>
/// <param name="Value">The interval in semitones.</param>
public readonly record struct Semitones<T>(T Value) : IComparable<Semitones<T>>
	where T : struct, INumber<T>
{
	/// <summary>Gets a unison interval (zero semitones).</summary>
	public static Semitones<T> Unison => new(T.Zero);

	/// <summary>Gets an octave interval (twelve semitones).</summary>
	public static Semitones<T> Octave => new(T.CreateChecked(12));

	/// <summary>
	/// Creates an interval from a raw semitone value.
	/// </summary>
	/// <param name="value">The interval in semitones.</param>
	/// <returns>A new <see cref="Semitones{T}"/>.</returns>
	public static Semitones<T> Create(T value) => new(value);

	/// <summary>
	/// Creates an interval from cents (100 cents → 1 semitone).
	/// </summary>
	/// <param name="cents">The interval in cents.</param>
	/// <returns>A new <see cref="Semitones{T}"/>.</returns>
	public static Semitones<T> FromCents(Cents<T> cents) => cents.ToSemitones();

	/// <summary>
	/// Creates an interval from a frequency ratio using <c>semitones = 12·log2(ratio)</c>.
	/// </summary>
	/// <param name="ratio">The frequency ratio.</param>
	/// <returns>A new <see cref="Semitones{T}"/>.</returns>
	public static Semitones<T> FromFrequencyRatio(Ratio<T> ratio)
	{
		double r = double.CreateChecked(ratio.Value);
		double semitones = 12.0 * Math.Log2(r);
		return new(T.CreateChecked(semitones));
	}

	/// <summary>
	/// Converts this interval to cents (1 semitone → 100 cents).
	/// </summary>
	/// <returns>The equivalent <see cref="Cents{T}"/>.</returns>
	public Cents<T> ToCents() => new(Value * T.CreateChecked(100));

	/// <summary>
	/// Converts this interval to a frequency ratio using <c>ratio = 2^(semitones/12)</c>.
	/// </summary>
	/// <returns>The equivalent frequency <see cref="Ratio{T}"/>.</returns>
	public Ratio<T> ToFrequencyRatio()
	{
		double semitones = double.CreateChecked(Value);
		double ratio = Math.Pow(2.0, semitones / 12.0);
		return new(T.CreateChecked(ratio));
	}

	/// <summary>Adds two intervals.</summary>
	/// <param name="left">The first interval.</param>
	/// <param name="right">The second interval.</param>
	/// <returns>The combined interval.</returns>
	public static Semitones<T> operator +(Semitones<T> left, Semitones<T> right) => new(left.Value + right.Value);

	/// <summary>Subtracts one interval from another.</summary>
	/// <param name="left">The interval to subtract from.</param>
	/// <param name="right">The interval to subtract.</param>
	/// <returns>The difference interval.</returns>
	public static Semitones<T> operator -(Semitones<T> left, Semitones<T> right) => new(left.Value - right.Value);

	/// <summary>Adds two intervals (friendly alternate for <c>operator +</c>).</summary>
	/// <param name="left">The first interval.</param>
	/// <param name="right">The second interval.</param>
	/// <returns>The combined interval.</returns>
	public static Semitones<T> Add(Semitones<T> left, Semitones<T> right) => left + right;

	/// <summary>Subtracts one interval from another (friendly alternate for <c>operator -</c>).</summary>
	/// <param name="left">The interval to subtract from.</param>
	/// <param name="right">The interval to subtract.</param>
	/// <returns>The difference interval.</returns>
	public static Semitones<T> Subtract(Semitones<T> left, Semitones<T> right) => left - right;

	/// <inheritdoc/>
	public int CompareTo(Semitones<T> other) => Value.CompareTo(other.Value);

	/// <summary>Determines whether one interval is less than another.</summary>
	/// <param name="left">The left interval.</param>
	/// <param name="right">The right interval.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
	public static bool operator <(Semitones<T> left, Semitones<T> right) => left.CompareTo(right) < 0;

	/// <summary>Determines whether one interval is greater than another.</summary>
	/// <param name="left">The left interval.</param>
	/// <param name="right">The right interval.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
	public static bool operator >(Semitones<T> left, Semitones<T> right) => left.CompareTo(right) > 0;

	/// <summary>Determines whether one interval is less than or equal to another.</summary>
	/// <param name="left">The left interval.</param>
	/// <param name="right">The right interval.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
	public static bool operator <=(Semitones<T> left, Semitones<T> right) => left.CompareTo(right) <= 0;

	/// <summary>Determines whether one interval is greater than or equal to another.</summary>
	/// <param name="left">The left interval.</param>
	/// <param name="right">The right interval.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
	public static bool operator >=(Semitones<T> left, Semitones<T> right) => left.CompareTo(right) >= 0;

	/// <summary>Returns a culture-invariant string representation of this interval.</summary>
	/// <returns>The interval formatted with an <c> st</c> suffix.</returns>
	public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"{Value} st");
}
