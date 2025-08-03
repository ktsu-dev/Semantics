// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a nuclear cross section quantity with double precision.
/// </summary>
public sealed record NuclearCrossSection : Generic.NuclearCrossSection<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="NuclearCrossSection"/> class.
	/// </summary>
	public NuclearCrossSection() : base() { }

	/// <summary>
	/// Creates a new NuclearCrossSection from a value in barns.
	/// </summary>
	/// <param name="barns">The value in barns.</param>
	/// <returns>A new NuclearCrossSection instance.</returns>
	public static new NuclearCrossSection FromBarns(double barns) => new() { Value = barns };
}
