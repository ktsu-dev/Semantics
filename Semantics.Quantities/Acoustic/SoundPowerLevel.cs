// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a sound power level quantity with compile-time dimensional safety.
/// Sound power level (PWL) is a logarithmic measure of sound power relative to 10⁻¹² W.
/// </summary>
public sealed record SoundPowerLevel<T> : PhysicalQuantity<SoundPowerLevel<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of sound power level [1].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.SoundPowerLevel;

	/// <summary>
	/// Initializes a new instance of the SoundPowerLevel class.
	/// </summary>
	public SoundPowerLevel() : base() { }

	/// <summary>
	/// Creates a new SoundPowerLevel from a value in decibels PWL.
	/// </summary>
	/// <param name="decibels">The sound power level in dB PWL.</param>
	/// <returns>A new SoundPowerLevel instance.</returns>
	public static SoundPowerLevel<T> FromDecibels(T decibels) => Create(decibels);

	/// <summary>
	/// Creates a SoundPowerLevel from a SoundPower value using PWL formula.
	/// PWL = 10 * log₁₀(W / W₀) where W₀ = 10⁻¹² W
	/// </summary>
	/// <param name="soundPower">The sound power.</param>
	/// <returns>The corresponding sound power level in dB PWL.</returns>
	public static SoundPowerLevel<T> FromSoundPower(SoundPower<T> soundPower)
	{
		Guard.NotNull(soundPower);

		// Reference power: 10⁻¹² W
		T referencePower = T.CreateChecked(1e-12);
		T ratio = soundPower.Value / referencePower;
		T decibels = T.CreateChecked(10.0 * Math.Log10(double.CreateChecked(ratio)));
		return Create(decibels);
	}

	/// <summary>
	/// Converts this SoundPowerLevel back to SoundPower.
	/// W = W₀ * 10^(PWL/10)
	/// </summary>
	/// <returns>The corresponding sound power.</returns>
	public SoundPower<T> ToSoundPower()
	{
		T referencePower = T.CreateChecked(1e-12); // 10⁻¹² W
		T exponent = Value / T.CreateChecked(10.0);
		T power = referencePower * T.CreateChecked(Math.Pow(10.0, double.CreateChecked(exponent)));
		return SoundPower<T>.Create(power);
	}

	/// <summary>
	/// Calculates the combined power level of multiple sound sources.
	/// PWL_total = 10 * log₁₀(10^(PWL₁/10) + 10^(PWL₂/10) + ...)
	/// </summary>
	/// <param name="other">Another sound power level.</param>
	/// <returns>The combined sound power level.</returns>
	public SoundPowerLevel<T> CombineWith(SoundPowerLevel<T> other)
	{
		Guard.NotNull(other);

		T exp1 = T.CreateChecked(Math.Pow(10.0, double.CreateChecked(Value / T.CreateChecked(10.0))));
		T exp2 = T.CreateChecked(Math.Pow(10.0, double.CreateChecked(other.Value / T.CreateChecked(10.0))));
		T combined = T.CreateChecked(10.0 * Math.Log10(double.CreateChecked(exp1 + exp2)));
		return Create(combined);
	}

	/// <summary>
	/// Calculates directivity from sound power level and sound pressure level.
	/// D = PWL - SPL + 10*log₁₀(4πr²)
	/// </summary>
	/// <param name="soundPressureLevel">The measured sound pressure level.</param>
	/// <param name="distance">The measurement distance.</param>
	/// <returns>The directivity factor in dB.</returns>
	public T CalculateDirectivity(SoundPressureLevel<T> soundPressureLevel, Length<T> distance)
	{
		Guard.NotNull(soundPressureLevel);
		Guard.NotNull(distance);

		T sphericalSpreading = T.CreateChecked(10.0 * Math.Log10(4.0 * Math.PI * double.CreateChecked(distance.Value * distance.Value)));
		return Value - soundPressureLevel.Value + sphericalSpreading;
	}
}
