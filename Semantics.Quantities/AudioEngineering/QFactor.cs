// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Globalization;
using System.Numerics;

/// <summary>
/// Represents the quality factor (Q) of a resonant filter or EQ band.
/// </summary>
/// <remarks>
/// Q is the dimensionless ratio of a filter's centre frequency to its bandwidth: a higher Q means a
/// narrower, more resonant response. This type provides the standard conversions between Q, absolute
/// bandwidth (in Hz), and bandwidth expressed in octaves, which is how parametric EQs typically expose
/// the control.
/// </remarks>
/// <typeparam name="T">The floating-point storage type.</typeparam>
/// <param name="Value">The quality factor.</param>
public readonly record struct QFactor<T>(T Value) : IComparable<QFactor<T>>
	where T : struct, INumber<T>
{
	/// <summary>
	/// Creates a quality factor from a raw value.
	/// </summary>
	/// <param name="value">The quality factor.</param>
	/// <returns>A new <see cref="QFactor{T}"/>.</returns>
	public static QFactor<T> Create(T value) => new(value);

	/// <summary>
	/// Creates a quality factor from a centre frequency and an absolute bandwidth using <c>Q = fc / bw</c>.
	/// </summary>
	/// <param name="centerFrequency">The centre frequency, in Hz.</param>
	/// <param name="bandwidth">The bandwidth, in Hz.</param>
	/// <returns>A new <see cref="QFactor{T}"/>.</returns>
	public static QFactor<T> FromBandwidth(T centerFrequency, T bandwidth) => new(centerFrequency / bandwidth);

	/// <summary>
	/// Creates a quality factor from a bandwidth expressed in octaves using
	/// <c>Q = √(2^N) / (2^N − 1)</c>.
	/// </summary>
	/// <param name="octaves">The bandwidth, in octaves.</param>
	/// <returns>A new <see cref="QFactor{T}"/>.</returns>
	public static QFactor<T> FromBandwidthInOctaves(T octaves)
	{
		double n = double.CreateChecked(octaves);
		double twoToN = Math.Pow(2.0, n);
		double q = Math.Sqrt(twoToN) / (twoToN - 1.0);
		return new(T.CreateChecked(q));
	}

	/// <summary>
	/// Computes the absolute bandwidth (in Hz) of a band centred at <paramref name="centerFrequency"/>
	/// using <c>bw = fc / Q</c>.
	/// </summary>
	/// <param name="centerFrequency">The centre frequency, in Hz.</param>
	/// <returns>The bandwidth, in Hz.</returns>
	public T BandwidthAt(T centerFrequency) => centerFrequency / Value;

	/// <summary>
	/// Computes the bandwidth in octaves corresponding to this Q.
	/// </summary>
	/// <returns>The bandwidth, in octaves.</returns>
	/// <remarks>Inverts <see cref="FromBandwidthInOctaves"/> via <c>N = log2(1 + 1/(2Q²) + √((1 + 1/(2Q²))² − 1))</c>.</remarks>
	public T ToBandwidthInOctaves()
	{
		double q = double.CreateChecked(Value);
		double a = 1.0 + (1.0 / (2.0 * q * q));
		double n = Math.Log2(a + Math.Sqrt((a * a) - 1.0));
		return T.CreateChecked(n);
	}

	/// <inheritdoc/>
	public int CompareTo(QFactor<T> other) => Value.CompareTo(other.Value);

	/// <summary>Determines whether one Q is less than another.</summary>
	/// <param name="left">The left Q.</param>
	/// <param name="right">The right Q.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
	public static bool operator <(QFactor<T> left, QFactor<T> right) => left.CompareTo(right) < 0;

	/// <summary>Determines whether one Q is greater than another.</summary>
	/// <param name="left">The left Q.</param>
	/// <param name="right">The right Q.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
	public static bool operator >(QFactor<T> left, QFactor<T> right) => left.CompareTo(right) > 0;

	/// <summary>Determines whether one Q is less than or equal to another.</summary>
	/// <param name="left">The left Q.</param>
	/// <param name="right">The right Q.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
	public static bool operator <=(QFactor<T> left, QFactor<T> right) => left.CompareTo(right) <= 0;

	/// <summary>Determines whether one Q is greater than or equal to another.</summary>
	/// <param name="left">The left Q.</param>
	/// <param name="right">The right Q.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
	public static bool operator >=(QFactor<T> left, QFactor<T> right) => left.CompareTo(right) >= 0;

	/// <summary>Returns a culture-invariant string representation of this quality factor.</summary>
	/// <returns>The quality factor formatted with a <c>Q </c> prefix.</returns>
	public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"Q {Value}");
}
