// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Globalization;
using System.Numerics;

/// <summary>
/// Represents a sound intensity level (SIL) in decibels relative to the
/// 10⁻¹² W/m² threshold of hearing.
/// </summary>
/// <remarks>
/// SIL is a logarithmic power quantity: <c>SIL = 10·log10(I / I₀)</c> with
/// <c>I₀ = 10⁻¹² W/m²</c>. Logarithmic scales are hand-written companions to the
/// linear generated quantities — see <see cref="SoundIntensity{T}"/>.
/// </remarks>
/// <typeparam name="T">The floating-point storage type.</typeparam>
/// <param name="Value">The level in decibels.</param>
public readonly record struct SoundIntensityLevel<T>(T Value) : IComparable<SoundIntensityLevel<T>>
	where T : struct, INumber<T>
{
	/// <summary>
	/// Creates a level from a raw decibel value.
	/// </summary>
	/// <param name="decibels">The level in dB re 10⁻¹² W/m².</param>
	/// <returns>A new <see cref="SoundIntensityLevel{T}"/>.</returns>
	public static SoundIntensityLevel<T> FromDecibels(T decibels) => new(decibels);

	/// <summary>
	/// Creates a level from a linear sound intensity using <c>SIL = 10·log10(I / I₀)</c>.
	/// </summary>
	/// <param name="intensity">The sound intensity.</param>
	/// <returns>A new <see cref="SoundIntensityLevel{T}"/>. Zero intensity maps to negative infinity.</returns>
	public static SoundIntensityLevel<T> FromSoundIntensity(SoundIntensity<T> intensity)
	{
		ArgumentNullException.ThrowIfNull(intensity);
		double i = double.CreateChecked(intensity.Value);
		double i0 = PhysicalConstants.Generic.ReferenceSoundIntensity<double>();
		return new(T.CreateChecked(10.0 * Math.Log10(i / i0)));
	}

	/// <summary>
	/// Converts this level to the equivalent linear sound intensity using <c>I = I₀·10^(SIL/10)</c>.
	/// </summary>
	/// <returns>The <see cref="SoundIntensity{T}"/>.</returns>
	public SoundIntensity<T> ToSoundIntensity()
	{
		double db = double.CreateChecked(Value);
		double i0 = PhysicalConstants.Generic.ReferenceSoundIntensity<double>();
		return SoundIntensity<T>.Create(T.CreateChecked(i0 * Math.Pow(10.0, db / 10.0)));
	}

	/// <summary>Adds two levels in decibel space.</summary>
	/// <param name="left">The first level.</param>
	/// <param name="right">The second level.</param>
	/// <returns>The summed level.</returns>
	public static SoundIntensityLevel<T> operator +(SoundIntensityLevel<T> left, SoundIntensityLevel<T> right) => new(left.Value + right.Value);

	/// <summary>Subtracts one level from another in decibel space.</summary>
	/// <param name="left">The level to subtract from.</param>
	/// <param name="right">The level to subtract.</param>
	/// <returns>The difference level.</returns>
	public static SoundIntensityLevel<T> operator -(SoundIntensityLevel<T> left, SoundIntensityLevel<T> right) => new(left.Value - right.Value);

	/// <summary>Adds two levels (friendly alternate for <c>operator +</c>).</summary>
	/// <param name="left">The first level.</param>
	/// <param name="right">The second level.</param>
	/// <returns>The summed level.</returns>
	public static SoundIntensityLevel<T> Add(SoundIntensityLevel<T> left, SoundIntensityLevel<T> right) => left + right;

	/// <summary>Subtracts one level from another (friendly alternate for <c>operator -</c>).</summary>
	/// <param name="left">The level to subtract from.</param>
	/// <param name="right">The level to subtract.</param>
	/// <returns>The difference level.</returns>
	public static SoundIntensityLevel<T> Subtract(SoundIntensityLevel<T> left, SoundIntensityLevel<T> right) => left - right;

	/// <inheritdoc/>
	public int CompareTo(SoundIntensityLevel<T> other) => Value.CompareTo(other.Value);

	/// <summary>Determines whether one level is less than another.</summary>
	/// <param name="left">The left level.</param>
	/// <param name="right">The right level.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
	public static bool operator <(SoundIntensityLevel<T> left, SoundIntensityLevel<T> right) => left.CompareTo(right) < 0;

	/// <summary>Determines whether one level is greater than another.</summary>
	/// <param name="left">The left level.</param>
	/// <param name="right">The right level.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
	public static bool operator >(SoundIntensityLevel<T> left, SoundIntensityLevel<T> right) => left.CompareTo(right) > 0;

	/// <summary>Determines whether one level is less than or equal to another.</summary>
	/// <param name="left">The left level.</param>
	/// <param name="right">The right level.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
	public static bool operator <=(SoundIntensityLevel<T> left, SoundIntensityLevel<T> right) => left.CompareTo(right) <= 0;

	/// <summary>Determines whether one level is greater than or equal to another.</summary>
	/// <param name="left">The left level.</param>
	/// <param name="right">The right level.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
	public static bool operator >=(SoundIntensityLevel<T> left, SoundIntensityLevel<T> right) => left.CompareTo(right) >= 0;

	/// <summary>Returns a culture-invariant string representation of this level.</summary>
	/// <returns>The level formatted with a <c> dB SIL</c> suffix.</returns>
	public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"{Value} dB SIL");
}
