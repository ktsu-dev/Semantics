// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a nuclear cross section quantity with double precision.
/// </summary>
public sealed record NuclearCrossSection
{
	/// <summary>Gets the underlying generic nuclear cross section instance.</summary>
	public Generic.NuclearCrossSection<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="NuclearCrossSection"/> class.
	/// </summary>
	public NuclearCrossSection() { }

	/// <summary>
	/// Creates a new NuclearCrossSection from a value in barns.
	/// </summary>
	/// <param name="barns">The value in barns.</param>
	/// <returns>A new NuclearCrossSection instance.</returns>
	public static NuclearCrossSection FromBarns(double barns) => new() { Value = Generic.NuclearCrossSection<double>.FromBarns(barns) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
