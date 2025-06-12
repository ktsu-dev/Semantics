// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a loudness quantity with compile-time dimensional safety.
/// Loudness is a perceptual measure of sound strength, typically measured in sones.
/// </summary>
public sealed record Loudness<T> : PhysicalQuantity<Loudness<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of loudness [1].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Loudness;

	/// <summary>
	/// Initializes a new instance of the Loudness class.
	/// </summary>
	public Loudness() : base() { }

	/// <summary>
	/// Creates a new Loudness from a value in sones.
	/// </summary>
	/// <param name="sones">The loudness in sones.</param>
	/// <returns>A new Loudness instance.</returns>
	public static Loudness<T> FromSones(T sones) => Create(sones);

	/// <summary>
	/// Creates a new Loudness from a value in phons (loudness level).
	/// This uses Stevens' power law: S = k * (φ - φ₀)^n, where n ≈ 0.3 for loudness
	/// </summary>
	/// <param name="phons">The loudness level in phons.</param>
	/// <returns>A new Loudness instance.</returns>
	public static Loudness<T> FromPhons(T phons)
	{
		// Stevens' power law approximation for loudness
		// At 40 phons = 1 sone (reference)
		T phonDifference = phons - T.CreateChecked(40.0);
		T sones = T.CreateChecked(Math.Pow(2.0, double.CreateChecked(phonDifference) / 10.0));
		return FromSones(sones);
	}

	/// <summary>
	/// Converts loudness to phons (loudness level).
	/// </summary>
	/// <returns>The loudness level in phons.</returns>
	public T ToPhons()
	{
		// Inverse of Stevens' power law
		T logRatio = T.CreateChecked(Math.Log2(double.CreateChecked(Value)));
		return T.CreateChecked(40.0) + (logRatio * T.CreateChecked(10.0));
	}

	/// <summary>
	/// Gets loudness as a multiple of the reference (1 sone).
	/// </summary>
	/// <returns>The loudness ratio.</returns>
	public T ToLoudnessRatio() => Value;

	/// <summary>
	/// Calculates the combined loudness of multiple sound sources.
	/// Combined loudness is not simply additive due to masking effects.
	/// This uses a simplified model: L_total = (L₁^α + L₂^α + ...)^(1/α) where α ≈ 0.3
	/// </summary>
	/// <param name="other">Another loudness value.</param>
	/// <returns>The combined loudness.</returns>
	public Loudness<T> CombineWith(Loudness<T> other)
	{
		ArgumentNullException.ThrowIfNull(other);

		// Simplified combination using power law
		double alpha = 0.3;
		double l1 = Math.Pow(double.CreateChecked(Value), alpha);
		double l2 = Math.Pow(double.CreateChecked(other.Value), alpha);
		double combined = Math.Pow(l1 + l2, 1.0 / alpha);

		return FromSones(T.CreateChecked(combined));
	}

	/// <summary>
	/// Gets the loudness category based on sone value.
	/// </summary>
	/// <returns>A string describing the loudness level.</returns>
	public string GetLoudnessCategory() => double.CreateChecked(Value) switch
	{
		< 0.1 => "Very Quiet",
		< 0.25 => "Quiet",
		< 1.0 => "Moderate",
		< 4.0 => "Loud",
		< 16.0 => "Very Loud",
		_ => "Extremely Loud"
	};

	/// <summary>
	/// Estimates the sound pressure level that would produce this loudness for a 1 kHz tone.
	/// This is an approximation based on the equal-loudness contours.
	/// </summary>
	/// <returns>The estimated SPL in dB.</returns>
	public T EstimateSPLAt1kHz()
	{
		T phons = ToPhons();
		// For 1 kHz pure tone, phons ≈ dB SPL
		return phons;
	}
}
