// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a sound speed quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record SoundSpeed<T> : PhysicalQuantity<SoundSpeed<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of soundspeed [L T⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.SoundSpeed;

	/// <summary>
	/// Initializes a new instance of the <see cref="SoundSpeed{T}"/> class.
	/// </summary>
	public SoundSpeed() : base() { }

	/// <summary>
	/// Creates a new SoundSpeed from a value in meters per second.
	/// </summary>
	/// <param name="metersPerSecond">The value in meters per second.</param>
	/// <returns>A new SoundSpeed instance.</returns>
	public static SoundSpeed<T> FromMetersPerSecond(T metersPerSecond) => Create(metersPerSecond);

	/// <summary>
	/// Creates a new SoundSpeed from a value in feet per second.
	/// </summary>
	/// <param name="feetPerSecond">The value in feet per second.</param>
	/// <returns>A new SoundSpeed instance.</returns>
	public static SoundSpeed<T> FromFeetPerSecond(T feetPerSecond) => Create(feetPerSecond * PhysicalConstants.Generic.FeetToMeters<T>());

	/// <summary>
	/// Multiplies wavelength by frequency to create sound speed.
	/// </summary>
	/// <param name="wavelength">The wavelength.</param>
	/// <param name="frequency">The frequency.</param>
	/// <returns>The resulting sound speed.</returns>
	public static SoundSpeed<T> Multiply(Wavelength<T> wavelength, Frequency<T> frequency)
	{
		Ensure.NotNull(wavelength);
		Ensure.NotNull(frequency);
		return Create(wavelength.Value * frequency.Value);
	}

	/// <summary>
	/// Converts SoundSpeed to Velocity.
	/// </summary>
	/// <returns>The equivalent velocity.</returns>
	public Velocity<T> ToVelocity() => Velocity<T>.Create(Value);
}
