// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a pressure quantity with double precision.
/// </summary>
public sealed record Pressure
{
	/// <summary>Gets the underlying generic pressure instance.</summary>
	public Generic.Pressure<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="Pressure"/> class.
	/// </summary>
	public Pressure() { }

	/// <summary>
	/// Creates a new Pressure from a value in pascals.
	/// </summary>
	/// <param name="pascals">The value in pascals.</param>
	/// <returns>A new Pressure instance.</returns>
	public static Pressure FromPascals(double pascals) => new() { Value = Generic.Pressure<double>.FromPascals(pascals) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
