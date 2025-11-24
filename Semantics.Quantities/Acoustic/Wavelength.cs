// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a wavelength quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Wavelength<T> : PhysicalQuantity<Wavelength<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of wavelength [L].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Wavelength;

	/// <summary>
	/// Initializes a new instance of the <see cref="Wavelength{T}"/> class.
	/// </summary>
	public Wavelength() : base() { }

	/// <summary>
	/// Creates a new Wavelength from a value in meters.
	/// </summary>
	/// <param name="meters">The value in meters.</param>
	/// <returns>A new Wavelength instance.</returns>
	public static Wavelength<T> FromMeters(T meters) => Create(meters);

	/// <summary>
	/// Creates a new Wavelength from a value in millimeters.
	/// </summary>
	/// <param name="millimeters">The value in millimeters.</param>
	/// <returns>A new Wavelength instance.</returns>
	public static Wavelength<T> FromMillimeters(T millimeters) => Create(millimeters / T.CreateChecked(1000));

	/// <summary>
	/// Creates a new Wavelength from a value in micrometers.
	/// </summary>
	/// <param name="micrometers">The value in micrometers.</param>
	/// <returns>A new Wavelength instance.</returns>
	public static Wavelength<T> FromMicrometers(T micrometers) => Create(micrometers / T.CreateChecked(1_000_000));

	/// <summary>
	/// Divides speed by frequency to create wavelength.
	/// </summary>
	/// <param name="speed">The wave speed.</param>
	/// <param name="frequency">The frequency.</param>
	/// <returns>The resulting wavelength.</returns>
	public static Wavelength<T> Divide(Velocity<T> speed, Frequency<T> frequency)
	{
		Guard.NotNull(speed);
		Guard.NotNull(frequency);
		return Create(speed.Value / frequency.Value);
	}

	/// <summary>
	/// Calculates the wave speed from wavelength and frequency (v = f × λ).
	/// </summary>
	/// <param name="wavelength">The wavelength.</param>
	/// <param name="frequency">The frequency.</param>
	/// <returns>The resulting wave speed.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static SoundSpeed<T> operator *(Wavelength<T> wavelength, Frequency<T> frequency)
	{
		Guard.NotNull(wavelength);
		Guard.NotNull(frequency);

		T speedValue = wavelength.Value * frequency.Value;

		return SoundSpeed<T>.Create(speedValue);
	}

	/// <summary>
	/// Calculates the frequency from wavelength and wave speed (f = v/λ).
	/// </summary>
	/// <param name="speed">The wave speed.</param>
	/// <param name="wavelength">The wavelength.</param>
	/// <returns>The resulting frequency.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Frequency<T> operator /(SoundSpeed<T> speed, Wavelength<T> wavelength)
	{
		Guard.NotNull(speed);
		Guard.NotNull(wavelength);

		T frequencyValue = speed.Value / wavelength.Value;

		return Frequency<T>.Create(frequencyValue);
	}
}
