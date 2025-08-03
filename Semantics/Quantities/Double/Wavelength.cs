// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a wavelength quantity with double precision.
/// </summary>
public sealed record Wavelength
{
	/// <summary>Gets the underlying generic wavelength instance.</summary>
	public Generic.Wavelength<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="Wavelength"/> class.
	/// </summary>
	public Wavelength() { }

	/// <summary>
	/// Creates a new Wavelength from a value in meters.
	/// </summary>
	/// <param name="meters">The value in meters.</param>
	/// <returns>A new Wavelength instance.</returns>
	public static Wavelength FromMeters(double meters) => new() { Value = Generic.Wavelength<double>.FromMeters(meters) };

	/// <summary>
	/// Creates a new Wavelength from a value in millimeters.
	/// </summary>
	/// <param name="millimeters">The value in millimeters.</param>
	/// <returns>A new Wavelength instance.</returns>
	public static Wavelength FromMillimeters(double millimeters) => new() { Value = Generic.Wavelength<double>.FromMillimeters(millimeters) };

	/// <summary>
	/// Creates a new Wavelength from a value in micrometers.
	/// </summary>
	/// <param name="micrometers">The value in micrometers.</param>
	/// <returns>A new Wavelength instance.</returns>
	public static Wavelength FromMicrometers(double micrometers) => new() { Value = Generic.Wavelength<double>.FromMicrometers(micrometers) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
