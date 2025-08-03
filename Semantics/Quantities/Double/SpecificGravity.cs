// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a specific gravity quantity with double precision.
/// </summary>
public sealed record SpecificGravity
{
	/// <summary>Gets the underlying generic specific gravity instance.</summary>
	public Generic.SpecificGravity<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="SpecificGravity"/> class.
	/// </summary>
	public SpecificGravity() { }

	/// <summary>
	/// Creates a new SpecificGravity from a dimensionless ratio.
	/// </summary>
	/// <param name="ratio">The dimensionless ratio value.</param>
	/// <returns>A new SpecificGravity instance.</returns>
	public static SpecificGravity FromRatio(double ratio) => new() { Value = Generic.SpecificGravity<double>.FromRatio(ratio) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
