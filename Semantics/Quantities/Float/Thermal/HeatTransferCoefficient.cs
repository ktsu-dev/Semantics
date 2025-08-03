// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a heat transfer coefficient quantity with float precision.
/// </summary>
public sealed record HeatTransferCoefficient : Generic.HeatTransferCoefficient<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="HeatTransferCoefficient"/> class.
	/// </summary>
	public HeatTransferCoefficient() : base() { }

	/// <summary>
	/// Creates a new HeatTransferCoefficient from a value in watts per square meter-kelvin.
	/// </summary>
	/// <param name="wattsPerSquareMeterKelvin">The value in watts per square meter-kelvin.</param>
	/// <returns>A new HeatTransferCoefficient instance.</returns>
	public static new HeatTransferCoefficient FromWattsPerSquareMeterKelvin(float wattsPerSquareMeterKelvin) => new() { Value = wattsPerSquareMeterKelvin };
}
