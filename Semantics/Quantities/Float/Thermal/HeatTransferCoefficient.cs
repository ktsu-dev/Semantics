// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a heat transfer coefficient quantity with float precision.
/// </summary>
public sealed record HeatTransferCoefficient
{
	/// <summary>Gets the underlying generic heat transfer coefficient instance.</summary>
	public Generic.HeatTransferCoefficient<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="HeatTransferCoefficient"/> class.
	/// </summary>
	public HeatTransferCoefficient() { }

	/// <summary>
	/// Creates a new HeatTransferCoefficient from a value in watts per square meter-kelvin.
	/// </summary>
	/// <param name="wattsPerSquareMeterKelvin">The value in watts per square meter-kelvin.</param>
	/// <returns>A new HeatTransferCoefficient instance.</returns>
	public static HeatTransferCoefficient FromWattsPerSquareMeterKelvin(float wattsPerSquareMeterKelvin) => new() { Value = Generic.HeatTransferCoefficient<float>.FromWattsPerSquareMeterKelvin(wattsPerSquareMeterKelvin) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
