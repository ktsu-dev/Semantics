// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a sound intensity level quantity with compile-time dimensional safety.
/// Sound intensity level (IL) is a logarithmic measure of sound intensity relative to 10⁻¹² W/m².
/// </summary>
public sealed record SoundIntensityLevel<T> : PhysicalQuantity<SoundIntensityLevel<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of sound intensity level [1].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.SoundIntensityLevel;

	/// <summary>
	/// Initializes a new instance of the SoundIntensityLevel class.
	/// </summary>
	public SoundIntensityLevel() : base() { }

	/// <summary>
	/// Creates a new SoundIntensityLevel from a value in decibels IL.
	/// </summary>
	/// <param name="decibels">The sound intensity level in dB IL.</param>
	/// <returns>A new SoundIntensityLevel instance.</returns>
	public static SoundIntensityLevel<T> FromDecibels(T decibels) => Create(decibels);

	/// <summary>
	/// Creates a SoundIntensityLevel from a SoundIntensity value using IL formula.
	/// IL = 10 * log₁₀(I / I₀) where I₀ = 10⁻¹² W/m²
	/// </summary>
	/// <param name="soundIntensity">The sound intensity.</param>
	/// <returns>The corresponding sound intensity level in dB IL.</returns>
	public static SoundIntensityLevel<T> FromSoundIntensity(SoundIntensity<T> soundIntensity)
	{
		Guard.NotNull(soundIntensity);

		// Reference intensity: 10⁻¹² W/m²
		T referenceIntensity = T.CreateChecked(1e-12);
		T ratio = soundIntensity.Value / referenceIntensity;
		T decibels = T.CreateChecked(10.0 * Math.Log10(double.CreateChecked(ratio)));
		return Create(decibels);
	}

	/// <summary>
	/// Converts this SoundIntensityLevel back to SoundIntensity.
	/// I = I₀ * 10^(IL/10)
	/// </summary>
	/// <returns>The corresponding sound intensity.</returns>
	public SoundIntensity<T> ToSoundIntensity()
	{
		T referenceIntensity = T.CreateChecked(1e-12); // 10⁻¹² W/m²
		T exponent = Value / T.CreateChecked(10.0);
		T intensity = referenceIntensity * T.CreateChecked(Math.Pow(10.0, double.CreateChecked(exponent)));
		return SoundIntensity<T>.Create(intensity);
	}

	/// <summary>
	/// Calculates the combined intensity level of multiple sources.
	/// IL_total = 10 * log₁₀(10^(IL₁/10) + 10^(IL₂/10) + ...)
	/// </summary>
	/// <param name="other">Another sound intensity level.</param>
	/// <returns>The combined sound intensity level.</returns>
	public SoundIntensityLevel<T> CombineWith(SoundIntensityLevel<T> other)
	{
		Guard.NotNull(other);

		T exp1 = T.CreateChecked(Math.Pow(10.0, double.CreateChecked(Value / T.CreateChecked(10.0))));
		T exp2 = T.CreateChecked(Math.Pow(10.0, double.CreateChecked(other.Value / T.CreateChecked(10.0))));
		T combined = T.CreateChecked(10.0 * Math.Log10(double.CreateChecked(exp1 + exp2)));
		return Create(combined);
	}
}
