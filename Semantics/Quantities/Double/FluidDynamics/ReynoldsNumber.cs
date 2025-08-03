// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a Reynolds number quantity with double precision.
/// </summary>
public sealed record ReynoldsNumber
{
	/// <summary>Gets the underlying generic Reynolds number instance.</summary>
	public Generic.ReynoldsNumber<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ReynoldsNumber"/> class.
	/// </summary>
	public ReynoldsNumber() { }

	/// <summary>
	/// Creates a new ReynoldsNumber from a dimensionless value.
	/// </summary>
	/// <param name="value">The dimensionless Reynolds number value.</param>
	/// <returns>A new ReynoldsNumber instance.</returns>
	public static ReynoldsNumber FromValue(double value) => new() { Value = Generic.ReynoldsNumber<double>.FromValue(value) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
