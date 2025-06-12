// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an acoustic impedance quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record AcousticImpedance<T> : PhysicalQuantity<AcousticImpedance<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.AcousticImpedance;

	/// <summary>
	/// Initializes a new instance of the AcousticImpedance class.
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
	/// <param name="rayls">The value in rayls.</param>
	/// <returns>A new AcousticImpedance instance.</returns>
	public static AcousticImpedance<T> FromRayls(T rayls) => Create(rayls);

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
