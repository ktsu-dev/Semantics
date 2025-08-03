// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an equivalent dose quantity with double precision.
/// </summary>
public sealed record EquivalentDose : Generic.EquivalentDose<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="EquivalentDose"/> class.
	/// </summary>
	public EquivalentDose() : base() { }

	/// <summary>
	/// Creates a new EquivalentDose from a value in sieverts.
	/// </summary>
	/// <param name="sieverts">The value in sieverts.</param>
	/// <returns>A new EquivalentDose instance.</returns>
	public static new EquivalentDose FromSieverts(double sieverts) => new() { Value = sieverts };
}
