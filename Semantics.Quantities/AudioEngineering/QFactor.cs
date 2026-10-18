// Copyright (c) 2023-2026 ktsu-dev contributors

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
/// <param name="Value">The quality factor; must be finite and strictly positive.</param>
/// <exception cref="ArgumentException">Thrown when <paramref name="Value"/> is zero, negative, or not finite.</exception>
public readonly record struct QFactor<T>(T Value) : IComparable<QFactor<T>>
	where T : struct, INumber<T>
{
	/// <summary>Gets the quality factor, which is always finite and strictly positive.</summary>
	/// <remarks>The setter validates too, so a <c>with</c> expression cannot produce an invalid Q.</remarks>
	public T Value { get; init => field = EnsurePositiveFinite(value, nameof(Value)); } = EnsurePositiveFinite(Value, nameof(Value));

	/// <summary>
	/// Creates a quality factor from a raw value.
	/// </summary>
	/// <param name="value">The quality factor; must be finite and strictly positive.</param>
	/// <returns>A new <see cref="QFactor{T}"/>.</returns>
	/// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is zero, negative, or not finite.</exception>
	public static QFactor<T> Create(T value) => new(EnsurePositiveFinite(value, nameof(value)));

	/// <summary>
	/// Creates a quality factor from a centre frequency and an absolute bandwidth using <c>Q = fc / bw</c>.
	/// </summary>
	/// <param name="centerFrequency">The centre frequency, in Hz; must be finite and strictly positive.</param>
	/// <param name="bandwidth">The bandwidth, in Hz; must be finite and strictly positive.</param>
	/// <returns>A new <see cref="QFactor{T}"/>.</returns>
	/// <exception cref="ArgumentException">Thrown when either argument is zero, negative, or not finite.</exception>
	public static QFactor<T> FromBandwidth(T centerFrequency, T bandwidth)
	{
		EnsurePositiveFinite(centerFrequency, nameof(centerFrequency));
		EnsurePositiveFinite(bandwidth, nameof(bandwidth));
		return new(centerFrequency / bandwidth);
	}

	/// <summary>
	/// Creates a quality factor from a bandwidth expressed in octaves using
	/// <c>Q = √(2^N) / (2^N − 1)</c>.
	/// </summary>
	/// <param name="octaves">The bandwidth, in octaves; must be finite and strictly positive.</param>
	/// <returns>A new <see cref="QFactor{T}"/>.</returns>
	/// <exception cref="ArgumentException">
	/// Thrown when <paramref name="octaves"/> is zero, negative, not finite, or so small that the Q it
	/// describes is not finite.
	/// </exception>
	public static QFactor<T> FromBandwidthInOctaves(T octaves)
	{
		EnsurePositiveFinite(octaves, nameof(octaves));

		double n = double.CreateChecked(octaves);
		double twoToN = Math.Pow(2.0, n);
		double q = Math.Sqrt(twoToN) / (twoToN - 1.0);

		// A positive bandwidth too narrow for 2^N to differ from 1 in double precision gives Q = ∞.
		if (!double.IsFinite(q))
		{
			throw new ArgumentException($"Bandwidth is too narrow to give a finite Q; received {octaves} octaves.", nameof(octaves));
		}

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

	/// <summary>
	/// Throws <see cref="ArgumentException"/> unless <paramref name="value"/> is finite and strictly
	/// positive. A zero or negative Q, bandwidth or frequency has no meaning for a filter, and passing one
	/// through returns a nonsensical Q or an <see cref="OverflowException"/> far from the cause.
	/// </summary>
	private static T EnsurePositiveFinite(T value, string paramName)
	{
		if (!T.IsFinite(value) || value <= T.Zero)
		{
			throw new ArgumentException($"Value must be finite and strictly positive; received {value}.", paramName);
		}

		return value;
	}

	/// <summary>Returns a culture-invariant string representation of this quality factor.</summary>
	/// <returns>The quality factor formatted with a <c>Q </c> prefix.</returns>
	public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"Q {Value}");
}
