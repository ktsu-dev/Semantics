// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents a frequency quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public record Frequency<T> : PhysicalQuantity<Frequency<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of frequency [T⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Frequency;

	/// <summary>
	/// Initializes a new instance of the <see cref="Frequency{T}"/> class.
	/// </summary>
	public Frequency() : base() { }

	/// <summary>
	/// Creates a new instance with the specified value.
	/// </summary>
	/// <param name="value">The value for the quantity.</param>
	/// <returns>A new instance of the quantity.</returns>
	/// <exception cref="ArgumentException">Thrown when the frequency is negative.</exception>
	public static new Frequency<T> Create(T value)
	{
		if (T.IsNegative(value))
		{
			throw new ArgumentException("Frequency cannot be negative.", nameof(value));
		}

		return new Frequency<T>() with { Quantity = value };
	}

	/// <summary>
	/// Creates a new Frequency from a value in hertz.
	/// </summary>
	/// <param name="hertz">The value in hertz.</param>
	/// <returns>A new Frequency instance.</returns>
	public static Frequency<T> FromHertz(T hertz) => Create(hertz);

	/// <summary>
	/// Creates a new Frequency from a value in kilohertz.
	/// </summary>
	/// <param name="kilohertz">The value in kilohertz.</param>
	/// <returns>A new Frequency instance.</returns>
	public static Frequency<T> FromKilohertz(T kilohertz) => Create(kilohertz * T.CreateChecked(1000));

	/// <summary>
	/// Creates a new Frequency from a value in megahertz.
	/// </summary>
	/// <param name="megahertz">The value in megahertz.</param>
	/// <returns>A new Frequency instance.</returns>
	public static Frequency<T> FromMegahertz(T megahertz) => Create(megahertz * T.CreateChecked(1_000_000));

	/// <summary>
	/// Calculates electromagnetic wavelength from frequency using speed of light (λ = c/f).
	/// </summary>
	/// <param name="frequency">The electromagnetic frequency.</param>
	/// <returns>The resulting wavelength in vacuum.</returns>
	public static Wavelength<T> GetElectromagneticWavelength(Frequency<T> frequency)
	{
		ArgumentNullException.ThrowIfNull(frequency);

		T speedOfLight = PhysicalConstants.Generic.SpeedOfLight<T>();
		T wavelengthValue = speedOfLight / frequency.Value;

		return Wavelength<T>.Create(wavelengthValue);
	}

	/// <summary>
	/// Calculates photon energy from frequency using Planck's constant (E = hf).
	/// </summary>
	/// <param name="frequency">The photon frequency.</param>
	/// <returns>The resulting photon energy.</returns>
	public static Energy<T> GetPhotonEnergy(Frequency<T> frequency)
	{
		ArgumentNullException.ThrowIfNull(frequency);

		T planckConstant = T.CreateChecked(PhysicalConstants.Fundamental.PlanckConstant);
		T energyValue = planckConstant * frequency.Value;

		return Energy<T>.Create(energyValue);
	}

	/// <summary>
	/// Calculates frequency from photon energy using Planck's constant (f = E/h).
	/// </summary>
	/// <param name="photonEnergy">The photon energy.</param>
	/// <returns>The resulting frequency.</returns>
	public static Frequency<T> FromPhotonEnergy(Energy<T> photonEnergy)
	{
		ArgumentNullException.ThrowIfNull(photonEnergy);

		T planckConstant = T.CreateChecked(PhysicalConstants.Fundamental.PlanckConstant);
		T frequencyValue = photonEnergy.Value / planckConstant;

		return Create(frequencyValue);
	}

	/// <summary>
	/// Divides one by time to create frequency.
	/// </summary>
	/// <param name="one">The value one.</param>
	/// <param name="time">The time period.</param>
	/// <returns>The resulting frequency.</returns>
	public static Frequency<T> Divide(T one, Time<T> time)
	{
		ArgumentNullException.ThrowIfNull(time);
		return Create(one / time.Value);
	}

	/// <summary>
	/// Multiplies frequency by wavelength to get speed.
	/// </summary>
	/// <param name="frequency">The frequency.</param>
	/// <param name="wavelength">The wavelength.</param>
	/// <returns>The resulting speed.</returns>
	public static Velocity<T> Multiply(Frequency<T> frequency, Wavelength<T> wavelength)
	{
		ArgumentNullException.ThrowIfNull(frequency);
		ArgumentNullException.ThrowIfNull(wavelength);
		return Velocity<T>.Create(frequency.Value * wavelength.Value);
	}

	/// <summary>
	/// Calculates the wave speed from frequency and wavelength (v = f × λ).
	/// </summary>
	/// <param name="frequency">The frequency.</param>
	/// <param name="wavelength">The wavelength.</param>
	/// <returns>The resulting wave speed.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static SoundSpeed<T> operator *(Frequency<T> frequency, Wavelength<T> wavelength)
	{
		ArgumentNullException.ThrowIfNull(frequency);
		ArgumentNullException.ThrowIfNull(wavelength);

		T speedValue = frequency.Value * wavelength.Value;

		return SoundSpeed<T>.Create(speedValue);
	}

	/// <summary>
	/// Calculates the period from frequency (T = 1/f).
	/// </summary>
	/// <param name="one">The value 1 (dimensionless).</param>
	/// <param name="frequency">The frequency.</param>
	/// <returns>The resulting period (time).</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Time<T> operator /(T one, Frequency<T> frequency)
	{
		ArgumentNullException.ThrowIfNull(frequency);

		T periodValue = one / frequency.Value;

		return Time<T>.Create(periodValue);
	}

	/// <summary>
	/// Calculates the wavelength from frequency and wave speed (λ = v/f).
	/// </summary>
	/// <param name="speed">The wave speed.</param>
	/// <param name="frequency">The frequency.</param>
	/// <returns>The resulting wavelength.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Wavelength<T> operator /(SoundSpeed<T> speed, Frequency<T> frequency)
	{
		ArgumentNullException.ThrowIfNull(speed);
		ArgumentNullException.ThrowIfNull(frequency);

		T wavelengthValue = speed.Value / frequency.Value;

		return Wavelength<T>.Create(wavelengthValue);
	}
}
