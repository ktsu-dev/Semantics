// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a density quantity with float precision.
/// </summary>
public sealed record Density : Generic.Density<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Density"/> class.
	/// </summary>
	public Density() : base() { }

	/// <summary>
	/// Creates a new Density from a value in kilograms per cubic meter.
	/// </summary>
	/// <param name="kilogramsPerCubicMeter">The value in kilograms per cubic meter.</param>
	/// <returns>A new Density instance.</returns>
	public static new Density FromKilogramsPerCubicMeter(float kilogramsPerCubicMeter) => new() { Value = kilogramsPerCubicMeter };

}
