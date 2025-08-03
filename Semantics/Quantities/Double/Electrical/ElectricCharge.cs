// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an electric charge quantity with double precision.
/// </summary>
public sealed record ElectricCharge
{
	/// <summary>Gets the underlying generic electric charge instance.</summary>
	public Generic.ElectricCharge<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricCharge"/> class.
	/// </summary>
	public ElectricCharge() { }

	/// <summary>
	/// Creates a new ElectricCharge from a value in coulombs.
	/// </summary>
	/// <param name="coulombs">The value in coulombs.</param>
	/// <returns>A new ElectricCharge instance.</returns>
	public static ElectricCharge FromCoulombs(double coulombs) => new() { Value = Generic.ElectricCharge<double>.FromCoulombs(coulombs) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
