// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a thermal diffusivity quantity with float precision.
/// </summary>
public sealed record ThermalDiffusivity : Generic.ThermalDiffusivity<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ThermalDiffusivity"/> class.
	/// </summary>
	public ThermalDiffusivity() : base() { }

	/// <summary>
	/// Creates a new ThermalDiffusivity from a value in square meters per second.
	/// </summary>
	/// <param name="squareMetersPerSecond">The thermal diffusivity value in m²/s.</param>
	/// <returns>A new ThermalDiffusivity instance.</returns>
	public static new ThermalDiffusivity FromSquareMetersPerSecond(float squareMetersPerSecond) => new() { Value = squareMetersPerSecond };

	/// <summary>
	/// Creates a new ThermalDiffusivity from a value in square feet per hour.
	/// </summary>
	/// <param name="squareFeetPerHour">The thermal diffusivity value in ft²/h.</param>
	/// <returns>A new ThermalDiffusivity instance.</returns>
	public static new ThermalDiffusivity FromSquareFeetPerHour(float squareFeetPerHour) => new() { Value = squareFeetPerHour };

	/// <summary>
	/// Creates a new ThermalDiffusivity from a value in square centimeters per second.
	/// </summary>
	/// <param name="squareCentimetersPerSecond">The thermal diffusivity value in cm²/s.</param>
	/// <returns>A new ThermalDiffusivity instance.</returns>
	public static new ThermalDiffusivity FromSquareCentimetersPerSecond(float squareCentimetersPerSecond) => new() { Value = squareCentimetersPerSecond };
}
