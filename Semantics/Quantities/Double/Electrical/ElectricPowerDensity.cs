// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an electric power density quantity with double precision.
/// </summary>
public sealed record ElectricPowerDensity
{
	/// <summary>Gets the underlying generic electric power density instance.</summary>
	public Generic.ElectricPowerDensity<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricPowerDensity"/> class.
	/// </summary>
	public ElectricPowerDensity() { }

	/// <summary>
	/// Creates a new ElectricPowerDensity from a value in watts per cubic meter.
	/// </summary>
	/// <param name="wattsPerCubicMeter">The value in watts per cubic meter.</param>
	/// <returns>A new ElectricPowerDensity instance.</returns>
	public static ElectricPowerDensity FromWattsPerCubicMeter(double wattsPerCubicMeter) => new() { Value = Generic.ElectricPowerDensity<double>.FromWattsPerCubicMeter(wattsPerCubicMeter) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
