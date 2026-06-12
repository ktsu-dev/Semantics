// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Globalization;
using System.Numerics;

/// <summary>
/// Represents a directivity index (DI) in decibels — how much more intense a
/// source or receiver is on-axis than its spherical average.
/// </summary>
/// <remarks>
/// <c>DI = 10·log10(I_axis / I_average)</c>. Like all decibel scales it is a
/// hand-written companion rather than a generated linear quantity.
/// </remarks>
/// <typeparam name="T">The floating-point storage type.</typeparam>
/// <param name="Value">The index in decibels.</param>
public readonly record struct DirectionalityIndex<T>(T Value) : IComparable<DirectionalityIndex<T>>
	where T : struct, INumber<T>
{
	/// <summary>Gets the index of an omnidirectional source (0 dB).</summary>
	public static DirectionalityIndex<T> Omnidirectional => new(T.Zero);

	/// <summary>
	/// Creates an index from a raw decibel value.
	/// </summary>
	/// <param name="decibels">The index in decibels.</param>
	/// <returns>A new <see cref="DirectionalityIndex{T}"/>.</returns>
	public static DirectionalityIndex<T> FromDecibels(T decibels) => new(decibels);

	/// <summary>
	/// Creates an index from the linear on-axis-to-average intensity ratio using <c>DI = 10·log10(ratio)</c>.
	/// </summary>
	/// <param name="ratio">The intensity ratio.</param>
	/// <returns>A new <see cref="DirectionalityIndex{T}"/>.</returns>
	public static DirectionalityIndex<T> FromIntensityRatio(Ratio<T> ratio)
	{
		ArgumentNullException.ThrowIfNull(ratio);
		double linear = double.CreateChecked(ratio.Value);
		return new(T.CreateChecked(10.0 * Math.Log10(linear)));
	}

	/// <summary>
	/// Converts this index to the linear intensity ratio using <c>ratio = 10^(DI/10)</c>.
	/// </summary>
	/// <returns>The on-axis-to-average intensity <see cref="Ratio{T}"/>.</returns>
	public Ratio<T> ToIntensityRatio()
	{
		double db = double.CreateChecked(Value);
		return Ratio<T>.Create(T.CreateChecked(Math.Pow(10.0, db / 10.0)));
	}

	/// <inheritdoc/>
	public int CompareTo(DirectionalityIndex<T> other) => Value.CompareTo(other.Value);

	/// <summary>Determines whether one index is less than another.</summary>
	/// <param name="left">The left index.</param>
	/// <param name="right">The right index.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
	public static bool operator <(DirectionalityIndex<T> left, DirectionalityIndex<T> right) => left.CompareTo(right) < 0;

	/// <summary>Determines whether one index is greater than another.</summary>
	/// <param name="left">The left index.</param>
	/// <param name="right">The right index.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
	public static bool operator >(DirectionalityIndex<T> left, DirectionalityIndex<T> right) => left.CompareTo(right) > 0;

	/// <summary>Determines whether one index is less than or equal to another.</summary>
	/// <param name="left">The left index.</param>
	/// <param name="right">The right index.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
	public static bool operator <=(DirectionalityIndex<T> left, DirectionalityIndex<T> right) => left.CompareTo(right) <= 0;

	/// <summary>Determines whether one index is greater than or equal to another.</summary>
	/// <param name="left">The left index.</param>
	/// <param name="right">The right index.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
	public static bool operator >=(DirectionalityIndex<T> left, DirectionalityIndex<T> right) => left.CompareTo(right) >= 0;

	/// <summary>Returns a culture-invariant string representation of this index.</summary>
	/// <returns>The index formatted with a <c> dB</c> suffix.</returns>
	public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"{Value} dB");
}
