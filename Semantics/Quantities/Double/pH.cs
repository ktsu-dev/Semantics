// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a pH quantity with double precision.
/// </summary>
public sealed record PH
{
	/// <summary>Gets the underlying generic pH instance.</summary>
	public Generic.PH<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="PH"/> class.
	/// </summary>
	public PH() { }

	/// <summary>
	/// Creates a new pH from a dimensionless value.
	/// </summary>
	/// <param name="value">The pH value.</param>
	/// <returns>A new PH instance.</returns>
	public static PH FromValue(double value) => new() { Value = Generic.PH<double>.Create(value) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
