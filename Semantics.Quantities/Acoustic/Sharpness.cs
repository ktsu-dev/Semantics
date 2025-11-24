// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a sharpness quantity with compile-time dimensional safety.
/// Sharpness is a perceptual measure of the high-frequency content of sound, measured in acums.
/// </summary>
public sealed record Sharpness<T> : PhysicalQuantity<Sharpness<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of sharpness [1].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Sharpness;

	/// <summary>
	/// Initializes a new instance of the Sharpness class.
	/// </summary>
	public Sharpness() : base() { }

	/// <summary>
	/// Creates a new Sharpness from a value in acums.
	/// </summary>
	/// <param name="acums">The sharpness in acums.</param>
	/// <returns>A new Sharpness instance.</returns>
	public static Sharpness<T> FromAcums(T acums) => Create(acums);

	/// <summary>
	/// Gets sharpness as a multiple of the reference (1 acum = narrow band noise at 1 kHz and 60 dB).
	/// </summary>
	/// <returns>The sharpness ratio.</returns>
	public T ToSharpnessRatio() => Value;

	/// <summary>
	/// Gets the sharpness category based on acum value.
	/// </summary>
	/// <returns>A string describing the sharpness level.</returns>
	public string GetSharpnessCategory() => double.CreateChecked(Value) switch
	{
		< 0.5 => "Very Dull",
		< 1.0 => "Dull",
		< 1.5 => "Moderate",
		< 2.5 => "Sharp",
		< 4.0 => "Very Sharp",
		_ => "Extremely Sharp"
	};

	/// <summary>
	/// Estimates perceived sound quality based on sharpness.
	/// </summary>
	/// <returns>A string describing the perceived quality.</returns>
	public string GetPerceivedQuality() => double.CreateChecked(Value) switch
	{
		< 0.8 => "Warm and Pleasant",
		< 1.2 => "Balanced",
		< 2.0 => "Bright",
		< 3.0 => "Harsh",
		_ => "Piercing"
	};

	/// <summary>
	/// Combines sharpness values (simplified linear model).
	/// Note: Actual sharpness combination is complex and frequency-dependent.
	/// </summary>
	/// <param name="other">Another sharpness value.</param>
	/// <returns>The combined sharpness.</returns>
	public Sharpness<T> CombineWith(Sharpness<T> other)
	{
		Guard.NotNull(other);

		// Simplified linear combination
		// Real sharpness calculation requires detailed spectral analysis
		T combined = Value + other.Value;
		return FromAcums(combined);
	}

	/// <summary>
	/// Estimates the dominant frequency content based on sharpness.
	/// This is a rough approximation.
	/// </summary>
	/// <returns>A string describing the frequency content.</returns>
	public string GetFrequencyContent() => double.CreateChecked(Value) switch
	{
		< 0.5 => "Low frequency dominant",
		< 1.0 => "Mid frequency dominant",
		< 2.0 => "Upper mid frequency dominant",
		< 3.0 => "High frequency dominant",
		_ => "Very high frequency dominant"
	};
}
