// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Globalization;
using System.Numerics;

/// <summary>
/// Represents a linear amplitude gain factor.
/// </summary>
/// <remarks>
/// A gain of <c>1</c> is unity (no change), <c>0</c> is silence, and <c>2</c> doubles the amplitude
/// (≈ +6.02 dB). Gain is the value you multiply a sample by on the audio path; <see cref="Decibels{T}"/>
/// is its logarithmic counterpart for display and user input. Conversions use the amplitude (field)
/// convention <c>dB = 20·log10(gain)</c>.
/// </remarks>
/// <typeparam name="T">The floating-point storage type.</typeparam>
/// <param name="Value">The linear gain factor.</param>
public readonly record struct Gain<T>(T Value) : IComparable<Gain<T>>
	where T : struct, INumber<T>
{
	/// <summary>Gets unity gain (a factor of one).</summary>
	public static Gain<T> Unity => new(T.One);

	/// <summary>Gets silence (a factor of zero).</summary>
	public static Gain<T> Silence => new(T.Zero);

	/// <summary>
	/// Creates a gain from a raw linear factor.
	/// </summary>
	/// <param name="value">The linear gain factor (1 = unity).</param>
	/// <returns>A new <see cref="Gain{T}"/>.</returns>
	public static Gain<T> Create(T value) => new(value);

	/// <summary>
	/// Creates a gain from a level in decibels using the amplitude convention <c>gain = 10^(dB/20)</c>.
	/// </summary>
	/// <param name="decibels">The level in decibels.</param>
	/// <returns>A new <see cref="Gain{T}"/>.</returns>
	public static Gain<T> FromDecibels(Decibels<T> decibels) => decibels.ToAmplitude();

	/// <summary>
	/// Converts this gain to a level in decibels using the amplitude convention <c>dB = 20·log10(gain)</c>.
	/// </summary>
	/// <returns>The equivalent <see cref="Decibels{T}"/>. Silence maps to negative infinity.</returns>
	public Decibels<T> ToDecibels() => Decibels<T>.FromAmplitude(Value);

	/// <summary>Multiplies two gains (cascading two stages).</summary>
	/// <param name="left">The first gain.</param>
	/// <param name="right">The second gain.</param>
	/// <returns>The combined gain.</returns>
	public static Gain<T> operator *(Gain<T> left, Gain<T> right) => new(left.Value * right.Value);

	/// <summary>Multiplies two gains (friendly alternate for <c>operator *</c>).</summary>
	/// <param name="left">The first gain.</param>
	/// <param name="right">The second gain.</param>
	/// <returns>The combined gain.</returns>
	public static Gain<T> Multiply(Gain<T> left, Gain<T> right) => left * right;

	/// <inheritdoc/>
	public int CompareTo(Gain<T> other) => Value.CompareTo(other.Value);

	/// <summary>Determines whether one gain is less than another.</summary>
	/// <param name="left">The left gain.</param>
	/// <param name="right">The right gain.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
	public static bool operator <(Gain<T> left, Gain<T> right) => left.CompareTo(right) < 0;

	/// <summary>Determines whether one gain is greater than another.</summary>
	/// <param name="left">The left gain.</param>
	/// <param name="right">The right gain.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
	public static bool operator >(Gain<T> left, Gain<T> right) => left.CompareTo(right) > 0;

	/// <summary>Determines whether one gain is less than or equal to another.</summary>
	/// <param name="left">The left gain.</param>
	/// <param name="right">The right gain.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
	public static bool operator <=(Gain<T> left, Gain<T> right) => left.CompareTo(right) <= 0;

	/// <summary>Determines whether one gain is greater than or equal to another.</summary>
	/// <param name="left">The left gain.</param>
	/// <param name="right">The right gain.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
	public static bool operator >=(Gain<T> left, Gain<T> right) => left.CompareTo(right) >= 0;

	/// <summary>Returns a culture-invariant string representation of this gain.</summary>
	/// <returns>The gain formatted with an <c>x</c> suffix.</returns>
	public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"{Value}x");
}
