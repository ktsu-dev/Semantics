// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a thermal diffusivity quantity with float precision.
/// </summary>
public sealed record ThermalDiffusivity
{
	/// <summary>Gets the underlying generic thermal diffusivity instance.</summary>
	public Generic.ThermalDiffusivity<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ThermalDiffusivity"/> class.
	/// </summary>
	public ThermalDiffusivity() { }

	/// <summary>
	/// Creates a new ThermalDiffusivity from a value in square meters per second.
	/// </summary>
	/// <param name="squareMetersPerSecond">The thermal diffusivity value in m²/s.</param>
	/// <returns>A new ThermalDiffusivity instance.</returns>
	public static ThermalDiffusivity FromSquareMetersPerSecond(float squareMetersPerSecond) => new() { Value = Generic.ThermalDiffusivity<float>.FromSquareMetersPerSecond(squareMetersPerSecond) };

	/// <summary>
	/// Creates a new ThermalDiffusivity from a value in square feet per hour.
	/// </summary>
	/// <param name="squareFeetPerHour">The thermal diffusivity value in ft²/h.</param>
	/// <returns>A new ThermalDiffusivity instance.</returns>
	public static ThermalDiffusivity FromSquareFeetPerHour(float squareFeetPerHour) => new() { Value = Generic.ThermalDiffusivity<float>.FromSquareFeetPerHour(squareFeetPerHour) };

	/// <summary>
	/// Creates a new ThermalDiffusivity from a value in square centimeters per second.
	/// </summary>
	/// <param name="squareCentimetersPerSecond">The thermal diffusivity value in cm²/s.</param>
	/// <returns>A new ThermalDiffusivity instance.</returns>
	public static ThermalDiffusivity FromSquareCentimetersPerSecond(float squareCentimetersPerSecond) => new() { Value = Generic.ThermalDiffusivity<float>.FromSquareCentimetersPerSecond(squareCentimetersPerSecond) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
