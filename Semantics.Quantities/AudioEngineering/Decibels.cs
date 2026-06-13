// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Globalization;
using System.Numerics;

/// <summary>
/// Represents a logarithmic level in decibels (dB).
/// </summary>
/// <remarks>
/// Decibels express ratios on a logarithmic scale. Two conventions are supported:
/// <list type="bullet">
///   <item><b>Amplitude / field</b> quantities (voltage, sample value): <c>dB = 20·log10(ratio)</c>.</item>
///   <item><b>Power</b> quantities (energy, intensity): <c>dB = 10·log10(ratio)</c>.</item>
/// </list>
/// Use <see cref="ToAmplitude"/>/<see cref="FromAmplitude"/> for gains applied to samples, and
/// <see cref="ToPower"/>/<see cref="FromPower"/> for power ratios. A level of <c>0 dB</c> is unity.
/// </remarks>
/// <typeparam name="T">The floating-point storage type.</typeparam>
/// <param name="Value">The level in decibels.</param>
public readonly record struct Decibels<T>(T Value) : IComparable<Decibels<T>>
	where T : struct, INumber<T>
{
	/// <summary>Gets a level of zero decibels (unity).</summary>
	public static Decibels<T> Unity => new(T.Zero);

	/// <summary>
	/// Creates a level from a raw decibel value.
	/// </summary>
	/// <param name="value">The level in decibels.</param>
	/// <returns>A new <see cref="Decibels{T}"/>.</returns>
	public static Decibels<T> Create(T value) => new(value);

	/// <summary>
	/// Creates a level from a linear amplitude ratio using <c>dB = 20·log10(ratio)</c>.
	/// </summary>
	/// <param name="amplitude">The linear amplitude ratio.</param>
	/// <returns>A new <see cref="Decibels{T}"/>. An amplitude of zero maps to negative infinity.</returns>
	public static Decibels<T> FromAmplitude(T amplitude)
	{
		double linear = double.CreateChecked(amplitude);
		double db = 20.0 * Math.Log10(linear);
		return new(T.CreateChecked(db));
	}

	/// <summary>
	/// Creates a level from a linear power ratio using <c>dB = 10·log10(ratio)</c>.
	/// </summary>
	/// <param name="power">The linear power ratio.</param>
	/// <returns>A new <see cref="Decibels{T}"/>. A power of zero maps to negative infinity.</returns>
	public static Decibels<T> FromPower(T power)
	{
		double linear = double.CreateChecked(power);
		double db = 10.0 * Math.Log10(linear);
		return new(T.CreateChecked(db));
	}

	/// <summary>
	/// Creates a level from an amplitude <see cref="Gain{T}"/>.
	/// </summary>
	/// <param name="gain">The linear amplitude gain.</param>
	/// <returns>A new <see cref="Decibels{T}"/>.</returns>
	public static Decibels<T> FromGain(Gain<T> gain) => FromAmplitude(gain.Value);

	/// <summary>
	/// Converts this level to a linear amplitude gain using <c>gain = 10^(dB/20)</c>.
	/// </summary>
	/// <returns>The equivalent amplitude <see cref="Gain{T}"/>.</returns>
	public Gain<T> ToAmplitude()
	{
		double db = double.CreateChecked(Value);
		double linear = Math.Pow(10.0, db / 20.0);
		return new(T.CreateChecked(linear));
	}

	/// <summary>
	/// Converts this level to a linear power ratio using <c>ratio = 10^(dB/10)</c>.
	/// </summary>
	/// <returns>The equivalent power <see cref="Ratio{T}"/>.</returns>
	public Ratio<T> ToPower()
	{
		double db = double.CreateChecked(Value);
		double linear = Math.Pow(10.0, db / 10.0);
		return Ratio<T>.Create(T.CreateChecked(linear));
	}

	/// <summary>Adds two decibel levels (cascading two stages multiplies their linear gains).</summary>
	/// <param name="left">The first level.</param>
	/// <param name="right">The second level.</param>
	/// <returns>The summed level.</returns>
	public static Decibels<T> operator +(Decibels<T> left, Decibels<T> right) => new(left.Value + right.Value);

	/// <summary>Subtracts one decibel level from another.</summary>
	/// <param name="left">The level to subtract from.</param>
	/// <param name="right">The level to subtract.</param>
	/// <returns>The difference level.</returns>
	public static Decibels<T> operator -(Decibels<T> left, Decibels<T> right) => new(left.Value - right.Value);

	/// <summary>Adds two decibel levels (friendly alternate for <c>operator +</c>).</summary>
	/// <param name="left">The first level.</param>
	/// <param name="right">The second level.</param>
	/// <returns>The summed level.</returns>
	public static Decibels<T> Add(Decibels<T> left, Decibels<T> right) => left + right;

	/// <summary>Subtracts one decibel level from another (friendly alternate for <c>operator -</c>).</summary>
	/// <param name="left">The level to subtract from.</param>
	/// <param name="right">The level to subtract.</param>
	/// <returns>The difference level.</returns>
	public static Decibels<T> Subtract(Decibels<T> left, Decibels<T> right) => left - right;

	/// <inheritdoc/>
	public int CompareTo(Decibels<T> other) => Value.CompareTo(other.Value);

	/// <summary>Determines whether one level is less than another.</summary>
	/// <param name="left">The left level.</param>
	/// <param name="right">The right level.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
	public static bool operator <(Decibels<T> left, Decibels<T> right) => left.CompareTo(right) < 0;

	/// <summary>Determines whether one level is greater than another.</summary>
	/// <param name="left">The left level.</param>
	/// <param name="right">The right level.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
	public static bool operator >(Decibels<T> left, Decibels<T> right) => left.CompareTo(right) > 0;

	/// <summary>Determines whether one level is less than or equal to another.</summary>
	/// <param name="left">The left level.</param>
	/// <param name="right">The right level.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
	public static bool operator <=(Decibels<T> left, Decibels<T> right) => left.CompareTo(right) <= 0;

	/// <summary>Determines whether one level is greater than or equal to another.</summary>
	/// <param name="left">The left level.</param>
	/// <param name="right">The right level.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
	public static bool operator >=(Decibels<T> left, Decibels<T> right) => left.CompareTo(right) >= 0;

	/// <summary>Returns a culture-invariant string representation of this level.</summary>
	/// <returns>The level formatted with a <c> dB</c> suffix.</returns>
	public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"{Value} dB");
}
