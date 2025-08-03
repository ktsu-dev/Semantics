// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a permittivity quantity with double precision.
/// </summary>
public sealed record Permittivity
{
	/// <summary>Gets the underlying generic permittivity instance.</summary>
	public Generic.Permittivity<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="Permittivity"/> class.
	/// </summary>
	public Permittivity() { }

	/// <summary>
	/// Creates a new Permittivity from a value in farads per meter.
	/// </summary>
	/// <param name="faradsPerMeter">The value in farads per meter.</param>
	/// <returns>A new Permittivity instance.</returns>
	public static Permittivity FromFaradsPerMeter(double faradsPerMeter) => new() { Value = Generic.Permittivity<double>.FromFaradsPerMeter(faradsPerMeter) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
