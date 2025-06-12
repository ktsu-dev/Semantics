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
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Wavelength;

	/// <summary>
	/// Initializes a new instance of the Wavelength class.
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
		ArgumentNullException.ThrowIfNull(speed);
		ArgumentNullException.ThrowIfNull(frequency);
		return Create(speed.Value / frequency.Value);
	}
}
