// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Globalization;
using System.Numerics;

/// <summary>
/// Represents a fine musical pitch interval measured in cents.
/// </summary>
/// <remarks>
/// One cent is one hundredth of a <see cref="Semitones{T}">semitone</see>, so 1200 cents make one
/// octave. Cents are the conventional unit for fine detune and tuning parameters.
/// </remarks>
/// <typeparam name="T">The floating-point storage type.</typeparam>
/// <param name="Value">The interval in cents.</param>
public readonly record struct Cents<T>(T Value) : IComparable<Cents<T>>
	where T : struct, INumber<T>
{
	/// <summary>Gets a unison interval (zero cents).</summary>
	public static Cents<T> Unison => new(T.Zero);

	/// <summary>
	/// Creates an interval from a raw cents value.
	/// </summary>
	/// <param name="value">The interval in cents.</param>
	/// <returns>A new <see cref="Cents{T}"/>.</returns>
	public static Cents<T> Create(T value) => new(value);

	/// <summary>
	/// Creates an interval from semitones (1 semitone → 100 cents).
	/// </summary>
	/// <param name="semitones">The interval in semitones.</param>
	/// <returns>A new <see cref="Cents{T}"/>.</returns>
	public static Cents<T> FromSemitones(Semitones<T> semitones) => semitones.ToCents();

	/// <summary>
	/// Creates an interval from a frequency ratio using <c>cents = 1200·log2(ratio)</c>.
	/// </summary>
	/// <param name="ratio">The frequency ratio.</param>
	/// <returns>A new <see cref="Cents{T}"/>.</returns>
	public static Cents<T> FromFrequencyRatio(Ratio<T> ratio)
	{
		ArgumentNullException.ThrowIfNull(ratio);
		double r = double.CreateChecked(ratio.Value);
		double cents = 1200.0 * Math.Log2(r);
		return new(T.CreateChecked(cents));
	}

	/// <summary>
	/// Converts this interval to semitones (100 cents → 1 semitone).
	/// </summary>
	/// <returns>The equivalent <see cref="Semitones{T}"/>.</returns>
	public Semitones<T> ToSemitones() => new(Value / T.CreateChecked(100));

	/// <summary>
	/// Converts this interval to a frequency ratio using <c>ratio = 2^(cents/1200)</c>.
	/// </summary>
	/// <returns>The equivalent frequency <see cref="Ratio{T}"/>.</returns>
	public Ratio<T> ToFrequencyRatio()
	{
		double cents = double.CreateChecked(Value);
		double ratio = Math.Pow(2.0, cents / 1200.0);
		return Ratio<T>.Create(T.CreateChecked(ratio));
	}

	/// <summary>Adds two intervals.</summary>
	/// <param name="left">The first interval.</param>
	/// <param name="right">The second interval.</param>
	/// <returns>The combined interval.</returns>
	public static Cents<T> operator +(Cents<T> left, Cents<T> right) => new(left.Value + right.Value);

	/// <summary>Subtracts one interval from another.</summary>
	/// <param name="left">The interval to subtract from.</param>
	/// <param name="right">The interval to subtract.</param>
	/// <returns>The difference interval.</returns>
	public static Cents<T> operator -(Cents<T> left, Cents<T> right) => new(left.Value - right.Value);

	/// <summary>Adds two intervals (friendly alternate for <c>operator +</c>).</summary>
	/// <param name="left">The first interval.</param>
	/// <param name="right">The second interval.</param>
	/// <returns>The combined interval.</returns>
	public static Cents<T> Add(Cents<T> left, Cents<T> right) => left + right;

	/// <summary>Subtracts one interval from another (friendly alternate for <c>operator -</c>).</summary>
	/// <param name="left">The interval to subtract from.</param>
	/// <param name="right">The interval to subtract.</param>
	/// <returns>The difference interval.</returns>
	public static Cents<T> Subtract(Cents<T> left, Cents<T> right) => left - right;

	/// <inheritdoc/>
	public int CompareTo(Cents<T> other) => Value.CompareTo(other.Value);

	/// <summary>Determines whether one interval is less than another.</summary>
	/// <param name="left">The left interval.</param>
	/// <param name="right">The right interval.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
	public static bool operator <(Cents<T> left, Cents<T> right) => left.CompareTo(right) < 0;

	/// <summary>Determines whether one interval is greater than another.</summary>
	/// <param name="left">The left interval.</param>
	/// <param name="right">The right interval.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
	public static bool operator >(Cents<T> left, Cents<T> right) => left.CompareTo(right) > 0;

	/// <summary>Determines whether one interval is less than or equal to another.</summary>
	/// <param name="left">The left interval.</param>
	/// <param name="right">The right interval.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
	public static bool operator <=(Cents<T> left, Cents<T> right) => left.CompareTo(right) <= 0;

	/// <summary>Determines whether one interval is greater than or equal to another.</summary>
	/// <param name="left">The left interval.</param>
	/// <param name="right">The right interval.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
	public static bool operator >=(Cents<T> left, Cents<T> right) => left.CompareTo(right) >= 0;

	/// <summary>Returns a culture-invariant string representation of this interval.</summary>
	/// <returns>The interval formatted with a <c> ct</c> suffix.</returns>
	public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"{Value} ct");
}
