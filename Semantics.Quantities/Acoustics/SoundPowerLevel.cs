// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Globalization;
using System.Numerics;

/// <summary>
/// Represents a sound power level (SWL) in decibels relative to the 10⁻¹² W
/// reference sound power.
/// </summary>
/// <remarks>
/// SWL is a logarithmic power quantity: <c>SWL = 10·log10(P / P₀)</c> with
/// <c>P₀ = 10⁻¹² W</c>. Logarithmic scales are hand-written companions to the
/// linear generated quantities — see <see cref="SoundPower{T}"/>.
/// </remarks>
/// <typeparam name="T">The floating-point storage type.</typeparam>
/// <param name="Value">The level in decibels.</param>
public readonly record struct SoundPowerLevel<T>(T Value) : IComparable<SoundPowerLevel<T>>
	where T : struct, INumber<T>
{
	/// <summary>
	/// Creates a level from a raw decibel value.
	/// </summary>
	/// <param name="decibels">The level in dB re 10⁻¹² W.</param>
	/// <returns>A new <see cref="SoundPowerLevel{T}"/>.</returns>
	public static SoundPowerLevel<T> FromDecibels(T decibels) => new(decibels);

	/// <summary>
	/// Creates a level from a linear sound power using <c>SWL = 10·log10(P / P₀)</c>.
	/// </summary>
	/// <param name="power">The sound power.</param>
	/// <returns>A new <see cref="SoundPowerLevel{T}"/>. Zero power maps to negative infinity.</returns>
	public static SoundPowerLevel<T> FromSoundPower(SoundPower<T> power)
	{
		ArgumentNullException.ThrowIfNull(power);
		double p = double.CreateChecked(power.Value);
		double p0 = PhysicalConstants.Generic.ReferenceSoundPower<double>();
		return new(T.CreateChecked(10.0 * Math.Log10(p / p0)));
	}

	/// <summary>
	/// Converts this level to the equivalent linear sound power using <c>P = P₀·10^(SWL/10)</c>.
	/// </summary>
	/// <returns>The <see cref="SoundPower{T}"/>.</returns>
	public SoundPower<T> ToSoundPower()
	{
		double db = double.CreateChecked(Value);
		double p0 = PhysicalConstants.Generic.ReferenceSoundPower<double>();
		return SoundPower<T>.Create(T.CreateChecked(p0 * Math.Pow(10.0, db / 10.0)));
	}

	/// <summary>Adds two levels in decibel space.</summary>
	/// <param name="left">The first level.</param>
	/// <param name="right">The second level.</param>
	/// <returns>The summed level.</returns>
	public static SoundPowerLevel<T> operator +(SoundPowerLevel<T> left, SoundPowerLevel<T> right) => new(left.Value + right.Value);

	/// <summary>Subtracts one level from another in decibel space.</summary>
	/// <param name="left">The level to subtract from.</param>
	/// <param name="right">The level to subtract.</param>
	/// <returns>The difference level.</returns>
	public static SoundPowerLevel<T> operator -(SoundPowerLevel<T> left, SoundPowerLevel<T> right) => new(left.Value - right.Value);

	/// <summary>Adds two levels (friendly alternate for <c>operator +</c>).</summary>
	/// <param name="left">The first level.</param>
	/// <param name="right">The second level.</param>
	/// <returns>The summed level.</returns>
	public static SoundPowerLevel<T> Add(SoundPowerLevel<T> left, SoundPowerLevel<T> right) => left + right;

	/// <summary>Subtracts one level from another (friendly alternate for <c>operator -</c>).</summary>
	/// <param name="left">The level to subtract from.</param>
	/// <param name="right">The level to subtract.</param>
	/// <returns>The difference level.</returns>
	public static SoundPowerLevel<T> Subtract(SoundPowerLevel<T> left, SoundPowerLevel<T> right) => left - right;

	/// <inheritdoc/>
	public int CompareTo(SoundPowerLevel<T> other) => Value.CompareTo(other.Value);

	/// <summary>Determines whether one level is less than another.</summary>
	/// <param name="left">The left level.</param>
	/// <param name="right">The right level.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
	public static bool operator <(SoundPowerLevel<T> left, SoundPowerLevel<T> right) => left.CompareTo(right) < 0;

	/// <summary>Determines whether one level is greater than another.</summary>
	/// <param name="left">The left level.</param>
	/// <param name="right">The right level.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
	public static bool operator >(SoundPowerLevel<T> left, SoundPowerLevel<T> right) => left.CompareTo(right) > 0;

	/// <summary>Determines whether one level is less than or equal to another.</summary>
	/// <param name="left">The left level.</param>
	/// <param name="right">The right level.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
	public static bool operator <=(SoundPowerLevel<T> left, SoundPowerLevel<T> right) => left.CompareTo(right) <= 0;

	/// <summary>Determines whether one level is greater than or equal to another.</summary>
	/// <param name="left">The left level.</param>
	/// <param name="right">The right level.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
	public static bool operator >=(SoundPowerLevel<T> left, SoundPowerLevel<T> right) => left.CompareTo(right) >= 0;

	/// <summary>Returns a culture-invariant string representation of this level.</summary>
	/// <returns>The level formatted with a <c> dB SWL</c> suffix.</returns>
	public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"{Value} dB SWL");
}
