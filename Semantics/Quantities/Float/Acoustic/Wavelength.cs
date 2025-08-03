// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a wavelength quantity with float precision.
/// </summary>
public sealed record Wavelength
{
	/// <summary>Gets the underlying generic wavelength instance.</summary>
	public Generic.Wavelength<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="Wavelength"/> class.
	/// </summary>
	public Wavelength() { }

	/// <summary>
	/// Creates a new Wavelength from a value in meters.
	/// </summary>
	/// <param name="meters">The value in meters.</param>
	/// <returns>A new Wavelength instance.</returns>
	public static Wavelength FromMeters(float meters) => new() { Value = Generic.Wavelength<float>.FromMeters(meters) };

	/// <summary>
	/// Creates a new Wavelength from a value in millimeters.
	/// </summary>
	/// <param name="millimeters">The value in millimeters.</param>
	/// <returns>A new Wavelength instance.</returns>
	public static Wavelength FromMillimeters(float millimeters) => new() { Value = Generic.Wavelength<float>.FromMillimeters(millimeters) };

	/// <summary>
	/// Creates a new Wavelength from a value in micrometers.
	/// </summary>
	/// <param name="micrometers">The value in micrometers.</param>
	/// <returns>A new Wavelength instance.</returns>
	public static Wavelength FromMicrometers(float micrometers) => new() { Value = Generic.Wavelength<float>.FromMicrometers(micrometers) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
