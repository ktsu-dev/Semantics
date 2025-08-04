// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an absorbed dose quantity with double precision.
/// </summary>
public sealed record AbsorbedDose : Generic.AbsorbedDose<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AbsorbedDose"/> class.
	/// </summary>
	public AbsorbedDose() : base() { }

	/// <summary>
	/// Creates a new AbsorbedDose from a value in grays.
	/// </summary>
	/// <param name="grays">The value in grays.</param>
	/// <returns>A new AbsorbedDose instance.</returns>
	public static new AbsorbedDose FromGrays(double grays) => new() { Quantity = grays };
}
