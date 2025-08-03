// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an electric field quantity with float precision.
/// </summary>
public sealed record ElectricField : Generic.ElectricField<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricField"/> class.
	/// </summary>
	public ElectricField() : base() { }

	/// <summary>
	/// Creates a new ElectricField from a value in volts per meter.
	/// </summary>
	/// <param name="voltsPerMeter">The value in volts per meter.</param>
	/// <returns>A new ElectricField instance.</returns>
	public static new ElectricField FromVoltsPerMeter(float voltsPerMeter) => new() { Value = voltsPerMeter };
}
