// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents an acoustic impedance quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public record AcousticImpedance<T> : PhysicalQuantity<AcousticImpedance<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of acousticimpedance [M L⁻² T⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.AcousticImpedance;

	/// <summary>
	/// Initializes a new instance of the <see cref="AcousticImpedance{T}"/> class.
	/// </summary>
	public AcousticImpedance() : base() { }

	/// <summary>
	/// Creates a new AcousticImpedance from a value in pascal-seconds per meter.
	/// </summary>
	/// <param name="pascalSecondsPerMeter">The value in pascal-seconds per meter.</param>
	/// <returns>A new AcousticImpedance instance.</returns>
	public static AcousticImpedance<T> FromPascalSecondsPerMeter(T pascalSecondsPerMeter) => Create(pascalSecondsPerMeter);

	/// <summary>
	/// Creates a new AcousticImpedance from a value in rayls.
	/// </summary>
	/// <param name="rayls">The value in rayls (Pa·s/m).</param>
	/// <returns>A new AcousticImpedance instance.</returns>
	public static AcousticImpedance<T> FromRayls(T rayls) => Create(rayls);

	/// <summary>
	/// Calculates acoustic impedance from material density and sound speed (Z = ρc).
	/// </summary>
	/// <param name="density">The material density.</param>
	/// <param name="soundSpeed">The sound speed in the material.</param>
	/// <returns>The resulting acoustic impedance.</returns>
	public static AcousticImpedance<T> FromDensityAndSoundSpeed(Density<T> density, SoundSpeed<T> soundSpeed)
	{
		ArgumentNullException.ThrowIfNull(density);
		ArgumentNullException.ThrowIfNull(soundSpeed);

		T impedanceValue = density.Value * soundSpeed.Value;

		return Create(impedanceValue);
	}

	/// <summary>
	/// Calculates sound speed from acoustic impedance and density (c = Z/ρ).
	/// </summary>
	/// <param name="impedance">The acoustic impedance.</param>
	/// <param name="density">The material density.</param>
	/// <returns>The resulting sound speed.</returns>
	public static SoundSpeed<T> CalculateSoundSpeed(AcousticImpedance<T> impedance, Density<T> density)
	{
		ArgumentNullException.ThrowIfNull(impedance);
		ArgumentNullException.ThrowIfNull(density);

		T soundSpeedValue = impedance.Value / density.Value;

		return SoundSpeed<T>.Create(soundSpeedValue);
	}

	/// <summary>
	/// Calculates material density from acoustic impedance and sound speed (ρ = Z/c).
	/// </summary>
	/// <param name="impedance">The acoustic impedance.</param>
	/// <param name="soundSpeed">The sound speed in the material.</param>
	/// <returns>The resulting material density.</returns>
	public static Density<T> CalculateDensity(AcousticImpedance<T> impedance, SoundSpeed<T> soundSpeed)
	{
		ArgumentNullException.ThrowIfNull(impedance);
		ArgumentNullException.ThrowIfNull(soundSpeed);

		T densityValue = impedance.Value / soundSpeed.Value;

		return Density<T>.Create(densityValue);
	}

	/// <summary>
	/// Calculates acoustic impedance using standard air properties at 20°C.
	/// </summary>
	/// <returns>The acoustic impedance of air at standard conditions.</returns>
	public static AcousticImpedance<T> ForStandardAir()
	{
		T airDensity = PhysicalConstants.Generic.StandardAirDensity<T>();
		T airSoundSpeed = T.CreateChecked(343.0); // m/s at 20°C
		T impedanceValue = airDensity * airSoundSpeed;

		return Create(impedanceValue);
	}

	/// <summary>
	/// Multiplies density by sound speed to create acoustic impedance.
	/// </summary>
	/// <param name="density">The density.</param>
	/// <param name="soundSpeed">The sound speed.</param>
	/// <returns>The resulting acoustic impedance.</returns>
	public static AcousticImpedance<T> Multiply(Density<T> density, SoundSpeed<T> soundSpeed)
	{
		ArgumentNullException.ThrowIfNull(density);
		ArgumentNullException.ThrowIfNull(soundSpeed);
		return Create(density.Value * soundSpeed.Value);
	}

	/// <summary>
	/// Divides sound pressure by velocity to create acoustic impedance.
	/// </summary>
	/// <param name="soundPressure">The sound pressure.</param>
	/// <param name="velocity">The particle velocity.</param>
	/// <returns>The resulting acoustic impedance.</returns>
	public static AcousticImpedance<T> Divide(SoundPressure<T> soundPressure, Velocity<T> velocity)
	{
		ArgumentNullException.ThrowIfNull(soundPressure);
		ArgumentNullException.ThrowIfNull(velocity);
		return Create(soundPressure.Value / velocity.Value);
	}
}
