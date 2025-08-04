// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a wavelength quantity with float precision.
/// </summary>
public sealed record Wavelength : Generic.Wavelength<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Wavelength"/> class.
	/// </summary>
	public Wavelength() : base() { }

	/// <summary>
	/// Creates a new Wavelength from a value in meters.
	/// </summary>
	/// <param name="meters">The value in meters.</param>
	/// <returns>A new Wavelength instance.</returns>
	public static new Wavelength FromMeters(float meters) => new() { Quantity = meters };

	/// <summary>
	/// Creates a new Wavelength from a value in millimeters.
	/// </summary>
	/// <param name="millimeters">The value in millimeters.</param>
	/// <returns>A new Wavelength instance.</returns>
	public static new Wavelength FromMillimeters(float millimeters) => new() { Quantity = millimeters };

	/// <summary>
	/// Creates a new Wavelength from a value in micrometers.
	/// </summary>
	/// <param name="micrometers">The value in micrometers.</param>
	/// <returns>A new Wavelength instance.</returns>
	public static new Wavelength FromMicrometers(float micrometers) => new() { Quantity = micrometers };
}
