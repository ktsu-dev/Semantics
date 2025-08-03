// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an electric field quantity with double precision.
/// </summary>
public sealed record ElectricField
{
	/// <summary>Gets the underlying generic electric field instance.</summary>
	public Generic.ElectricField<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricField"/> class.
	/// </summary>
	public ElectricField() { }

	/// <summary>
	/// Creates a new ElectricField from a value in volts per meter.
	/// </summary>
	/// <param name="voltsPerMeter">The value in volts per meter.</param>
	/// <returns>A new ElectricField instance.</returns>
	public static ElectricField FromVoltsPerMeter(double voltsPerMeter) => new() { Value = Generic.ElectricField<double>.FromVoltsPerMeter(voltsPerMeter) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
