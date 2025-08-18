// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a noise reduction coefficient quantity with compile-time dimensional safety.
/// NRC is the average absorption coefficient at 250, 500, 1000, and 2000 Hz.
/// </summary>
public sealed record NoiseReductionCoefficient<T> : PhysicalQuantity<NoiseReductionCoefficient<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of noise reduction coefficient [1].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.NoiseReductionCoefficient;

	/// <summary>
	/// Initializes a new instance of the NoiseReductionCoefficient class.
	/// </summary>
	public NoiseReductionCoefficient() : base() { }

	/// <summary>
	/// Creates a new NoiseReductionCoefficient from a value (0 to 1.25).
	/// </summary>
	/// <param name="coefficient">The NRC value (0-1.25, typically 0-1).</param>
	/// <returns>A new NoiseReductionCoefficient instance.</returns>
	public static NoiseReductionCoefficient<T> FromCoefficient(T coefficient) => Create(coefficient);

	/// <summary>
	/// Calculates NRC from absorption coefficients at standard frequencies.
	/// NRC = (α₂₅₀ + α₅₀₀ + α₁₀₀₀ + α₂₀₀₀) / 4
	/// </summary>
	/// <param name="alpha250Hz">Absorption coefficient at 250 Hz.</param>
	/// <param name="alpha500Hz">Absorption coefficient at 500 Hz.</param>
	/// <param name="alpha1000Hz">Absorption coefficient at 1000 Hz.</param>
	/// <param name="alpha2000Hz">Absorption coefficient at 2000 Hz.</param>
	/// <returns>The calculated NRC.</returns>
	public static NoiseReductionCoefficient<T> FromAbsorptionCoefficients(
		SoundAbsorption<T> alpha250Hz,
		SoundAbsorption<T> alpha500Hz,
		SoundAbsorption<T> alpha1000Hz,
		SoundAbsorption<T> alpha2000Hz)
	{
		ArgumentNullException.ThrowIfNull(alpha250Hz);
		ArgumentNullException.ThrowIfNull(alpha500Hz);
		ArgumentNullException.ThrowIfNull(alpha1000Hz);
		ArgumentNullException.ThrowIfNull(alpha2000Hz);

		T sum = alpha250Hz.Value + alpha500Hz.Value + alpha1000Hz.Value + alpha2000Hz.Value;
		T average = sum / T.CreateChecked(4);
		return FromCoefficient(average);
	}

	/// <summary>
	/// Rounds to the nearest 0.05 as per ASTM standards.
	/// </summary>
	/// <returns>The rounded NRC value.</returns>
	public NoiseReductionCoefficient<T> RoundToStandard()
	{
		T roundedValue = T.CreateChecked(Math.Round(double.CreateChecked(Value) * 20.0) / 20.0);
		return FromCoefficient(roundedValue);
	}

	/// <summary>
	/// Converts to percentage.
	/// </summary>
	/// <returns>The NRC as a percentage.</returns>
	public T ToPercentage() => Value * T.CreateChecked(100);

	/// <summary>
	/// Gets the acoustic class rating based on NRC value.
	/// </summary>
	/// <returns>A string describing the acoustic performance class.</returns>
	public string GetAcousticClass() => double.CreateChecked(Value) switch
	{
		< 0.15 => "Class E (Poor)",
		< 0.25 => "Class D (Fair)",
		< 0.35 => "Class C (Good)",
		< 0.50 => "Class B (Very Good)",
		< 0.75 => "Class A (Excellent)",
		_ => "Class A+ (Superior)"
	};
}
