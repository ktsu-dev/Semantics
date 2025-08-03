// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a thermal diffusivity quantity with double precision.
/// </summary>
public sealed record ThermalDiffusivity
{
	/// <summary>Gets the underlying generic thermal diffusivity instance.</summary>
	public Generic.ThermalDiffusivity<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ThermalDiffusivity"/> class.
	/// </summary>
	public ThermalDiffusivity() { }

	/// <summary>
	/// Creates a new ThermalDiffusivity from a value in square meters per second.
	/// </summary>
	/// <param name="squareMetersPerSecond">The thermal diffusivity value in m²/s.</param>
	/// <returns>A new ThermalDiffusivity instance.</returns>
	public static ThermalDiffusivity FromSquareMetersPerSecond(double squareMetersPerSecond) => new() { Value = Generic.ThermalDiffusivity<double>.FromSquareMetersPerSecond(squareMetersPerSecond) };

	/// <summary>
	/// Creates a new ThermalDiffusivity from a value in square feet per hour.
	/// </summary>
	/// <param name="squareFeetPerHour">The thermal diffusivity value in ft²/h.</param>
	/// <returns>A new ThermalDiffusivity instance.</returns>
	public static ThermalDiffusivity FromSquareFeetPerHour(double squareFeetPerHour) => new() { Value = Generic.ThermalDiffusivity<double>.FromSquareFeetPerHour(squareFeetPerHour) };

	/// <summary>
	/// Creates a new ThermalDiffusivity from a value in square centimeters per second.
	/// </summary>
	/// <param name="squareCentimetersPerSecond">The thermal diffusivity value in cm²/s.</param>
	/// <returns>A new ThermalDiffusivity instance.</returns>
	public static ThermalDiffusivity FromSquareCentimetersPerSecond(double squareCentimetersPerSecond) => new() { Value = Generic.ThermalDiffusivity<double>.FromSquareCentimetersPerSecond(squareCentimetersPerSecond) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
