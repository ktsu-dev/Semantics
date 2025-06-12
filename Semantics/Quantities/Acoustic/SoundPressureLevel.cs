// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a sound pressure level quantity with compile-time dimensional safety.
/// Sound pressure level (SPL) is a logarithmic measure of sound pressure relative to 20 μPa.
/// </summary>
public sealed record SoundPressureLevel<T> : PhysicalQuantity<SoundPressureLevel<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of sound pressure level [1].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.SoundPressureLevel;

	/// <summary>
	/// Initializes a new instance of the SoundPressureLevel class.
	/// </summary>
	public SoundPressureLevel() : base() { }

	/// <summary>
	/// Creates a new SoundPressureLevel from a value in decibels SPL.
	/// </summary>
	/// <param name="decibels">The sound pressure level in dB SPL.</param>
	/// <returns>A new SoundPressureLevel instance.</returns>
	public static SoundPressureLevel<T> FromDecibels(T decibels) => Create(decibels);

	/// <summary>
	/// Creates a SoundPressureLevel from a SoundPressure value using SPL formula.
	/// SPL = 20 * log₁₀(p / p₀) where p₀ = 20 μPa
	/// </summary>
	/// <param name="soundPressure">The sound pressure.</param>
	/// <returns>The corresponding sound pressure level in dB SPL.</returns>
	public static SoundPressureLevel<T> FromSoundPressure(SoundPressure<T> soundPressure)
	{
		ArgumentNullException.ThrowIfNull(soundPressure);
		// Reference pressure: 20 μPa = 20e-6 Pa
		T referencePressure = T.CreateChecked(20e-6);
		T ratio = soundPressure.Value / referencePressure;
		T decibels = T.CreateChecked(20.0 * Math.Log10(double.CreateChecked(ratio)));
		return FromDecibels(decibels);
	}

	/// <summary>
	/// Converts this SoundPressureLevel back to SoundPressure.
	/// p = p₀ * 10^(SPL/20)
	/// </summary>
	/// <returns>The corresponding sound pressure.</returns>
	public SoundPressure<T> ToSoundPressure()
	{
		T referencePressure = T.CreateChecked(20e-6); // 20 μPa
		T exponent = Value / T.CreateChecked(20.0);
		T pressure = referencePressure * T.CreateChecked(Math.Pow(10.0, double.CreateChecked(exponent)));
		return SoundPressure<T>.Create(pressure);
	}

	/// <summary>
	/// Gets the A-weighted sound pressure level (common for human hearing assessment).
	/// </summary>
	/// <returns>The A-weighted SPL (approximation).</returns>
	public SoundPressureLevel<T> AWeighted() => FromDecibels(Value); // Simplified - actual A-weighting is frequency-dependent

	/// <summary>
	/// Calculates the equivalent sound level for intermittent sounds.
	/// </summary>
	/// <param name="duration">Duration of measurement.</param>
	/// <param name="totalTime">Total time period.</param>
	/// <returns>The equivalent continuous sound level.</returns>
	public SoundPressureLevel<T> EquivalentLevel(Time<T> duration, Time<T> totalTime)
	{
		ArgumentNullException.ThrowIfNull(duration);
		ArgumentNullException.ThrowIfNull(totalTime);
		T ratio = duration.Value / totalTime.Value;
		T correction = T.CreateChecked(10.0 * Math.Log10(double.CreateChecked(ratio)));
		return FromDecibels(Value + correction);
	}
}
