// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Globalization;
using System.Numerics;

/// <summary>
/// Represents a sound pressure level (SPL) in decibels relative to the 20 µPa
/// threshold of hearing.
/// </summary>
/// <remarks>
/// SPL is a logarithmic field quantity: <c>SPL = 20·log10(p / p₀)</c> with
/// <c>p₀ = 20 µPa</c>. Logarithmic scales don't fit the linear
/// <see cref="PhysicalQuantity{TSelf, T}"/> model (their addition is not linear
/// addition), so SPL is a hand-written companion that converts to and from the
/// linear <see cref="SoundPressure{T}"/> quantity.
/// </remarks>
/// <typeparam name="T">The floating-point storage type.</typeparam>
/// <param name="Value">The level in decibels.</param>
public readonly record struct SoundPressureLevel<T>(T Value) : IComparable<SoundPressureLevel<T>>
	where T : struct, INumber<T>
{
	/// <summary>
	/// Creates a level from a raw decibel value.
	/// </summary>
	/// <param name="decibels">The level in dB re 20 µPa.</param>
	/// <returns>A new <see cref="SoundPressureLevel{T}"/>.</returns>
	public static SoundPressureLevel<T> FromDecibels(T decibels) => new(decibels);

	/// <summary>
	/// Creates a level from a linear sound pressure using <c>SPL = 20·log10(p / p₀)</c>.
	/// </summary>
	/// <param name="pressure">The RMS sound pressure.</param>
	/// <returns>A new <see cref="SoundPressureLevel{T}"/>. Zero pressure maps to negative infinity.</returns>
	public static SoundPressureLevel<T> FromSoundPressure(SoundPressure<T> pressure)
	{
		ArgumentNullException.ThrowIfNull(pressure);
		double p = double.CreateChecked(pressure.Value);
		double p0 = PhysicalConstants.Generic.ReferenceSoundPressure<double>();
		return new(T.CreateChecked(20.0 * Math.Log10(p / p0)));
	}

	/// <summary>
	/// Converts this level to the equivalent linear sound pressure using <c>p = p₀·10^(SPL/20)</c>.
	/// </summary>
	/// <returns>The RMS <see cref="SoundPressure{T}"/>.</returns>
	public SoundPressure<T> ToSoundPressure()
	{
		double db = double.CreateChecked(Value);
		double p0 = PhysicalConstants.Generic.ReferenceSoundPressure<double>();
		return SoundPressure<T>.Create(T.CreateChecked(p0 * Math.Pow(10.0, db / 20.0)));
	}

	/// <summary>Adds two levels in decibel space (cascading gains).</summary>
	/// <param name="left">The first level.</param>
	/// <param name="right">The second level.</param>
	/// <returns>The summed level.</returns>
	public static SoundPressureLevel<T> operator +(SoundPressureLevel<T> left, SoundPressureLevel<T> right) => new(left.Value + right.Value);

	/// <summary>Subtracts one level from another in decibel space.</summary>
	/// <param name="left">The level to subtract from.</param>
	/// <param name="right">The level to subtract.</param>
	/// <returns>The difference level.</returns>
	public static SoundPressureLevel<T> operator -(SoundPressureLevel<T> left, SoundPressureLevel<T> right) => new(left.Value - right.Value);

	/// <summary>Adds two levels (friendly alternate for <c>operator +</c>).</summary>
	/// <param name="left">The first level.</param>
	/// <param name="right">The second level.</param>
	/// <returns>The summed level.</returns>
	public static SoundPressureLevel<T> Add(SoundPressureLevel<T> left, SoundPressureLevel<T> right) => left + right;

	/// <summary>Subtracts one level from another (friendly alternate for <c>operator -</c>).</summary>
	/// <param name="left">The level to subtract from.</param>
	/// <param name="right">The level to subtract.</param>
	/// <returns>The difference level.</returns>
	public static SoundPressureLevel<T> Subtract(SoundPressureLevel<T> left, SoundPressureLevel<T> right) => left - right;

	/// <inheritdoc/>
	public int CompareTo(SoundPressureLevel<T> other) => Value.CompareTo(other.Value);

	/// <summary>Determines whether one level is less than another.</summary>
	/// <param name="left">The left level.</param>
	/// <param name="right">The right level.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
	public static bool operator <(SoundPressureLevel<T> left, SoundPressureLevel<T> right) => left.CompareTo(right) < 0;

	/// <summary>Determines whether one level is greater than another.</summary>
	/// <param name="left">The left level.</param>
	/// <param name="right">The right level.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
	public static bool operator >(SoundPressureLevel<T> left, SoundPressureLevel<T> right) => left.CompareTo(right) > 0;

	/// <summary>Determines whether one level is less than or equal to another.</summary>
	/// <param name="left">The left level.</param>
	/// <param name="right">The right level.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
	public static bool operator <=(SoundPressureLevel<T> left, SoundPressureLevel<T> right) => left.CompareTo(right) <= 0;

	/// <summary>Determines whether one level is greater than or equal to another.</summary>
	/// <param name="left">The left level.</param>
	/// <param name="right">The right level.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
	public static bool operator >=(SoundPressureLevel<T> left, SoundPressureLevel<T> right) => left.CompareTo(right) >= 0;

	/// <summary>Returns a culture-invariant string representation of this level.</summary>
	/// <returns>The level formatted with a <c> dB SPL</c> suffix.</returns>
	public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"{Value} dB SPL");
}
