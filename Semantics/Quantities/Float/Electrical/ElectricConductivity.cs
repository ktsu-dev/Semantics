// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an electric conductivity quantity with float precision.
/// </summary>
public sealed record ElectricConductivity : Generic.ElectricConductivity<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricConductivity"/> class.
	/// </summary>
	public ElectricConductivity() : base() { }

	/// <summary>
	/// Creates a new ElectricConductivity from a value in siemens per meter.
	/// </summary>
	/// <param name="siemensPerMeter">The value in siemens per meter.</param>
	/// <returns>A new ElectricConductivity instance.</returns>
	public static new ElectricConductivity FromSiemensPerMeter(float siemensPerMeter) => new() { Quantity = siemensPerMeter };
}
