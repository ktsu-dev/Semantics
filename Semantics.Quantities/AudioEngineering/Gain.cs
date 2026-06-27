// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Numerics;

/// <summary>
/// Bespoke members of <see cref="Gain{T}"/>; the quantity itself is generated from
/// <c>dimensions.json</c> as a semantic overload of the Dimensionless <see cref="Ratio{T}"/>.
/// </summary>
/// <remarks>
/// A gain of <c>1</c> is unity (no change), <c>0</c> is silence, and <c>2</c> doubles the
/// amplitude (≈ +6.02 dB). Gain is the value you multiply a sample by on the audio path;
/// <see cref="Decibels{T}"/> is its logarithmic counterpart for display and user input,
/// using the amplitude (field) convention <c>dB = 20·log10(gain)</c>. As a Vector0
/// magnitude, gain is non-negative — polarity inversion is a separate concern.
/// </remarks>
public partial record Gain<T>
	where T : struct, INumber<T>
{
	/// <summary>Gets unity gain (a factor of one).</summary>
	public static Gain<T> Unity => Create(T.One);

	/// <summary>Gets silence (a factor of zero).</summary>
	public static Gain<T> Silence => Create(T.Zero);

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
	public Decibels<T> ToDecibels() => Decibels<T>.FromGain(this);

	/// <summary>Multiplies two gains (cascading two stages).</summary>
	/// <param name="left">The first gain.</param>
	/// <param name="right">The second gain.</param>
	/// <returns>The combined gain.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Usage", "CA2225:Operator overloads have named alternates",
		Justification = "Multiply is provided as the named alternate.")]
	public static Gain<T> operator *(Gain<T> left, Gain<T> right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value * right.Value);
	}

	/// <summary>Multiplies two gains (friendly alternate for <c>operator *</c>).</summary>
	/// <param name="left">The first gain.</param>
	/// <param name="right">The second gain.</param>
	/// <returns>The combined gain.</returns>
	public static Gain<T> Multiply(Gain<T> left, Gain<T> right) => left * right;
}
