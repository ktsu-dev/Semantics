// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a refractive index quantity with double precision.
/// </summary>
public sealed record RefractiveIndex : Generic.RefractiveIndex<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RefractiveIndex"/> class.
	/// </summary>
	public RefractiveIndex() : base() { }

	/// <summary>
	/// Creates a new RefractiveIndex from a dimensionless value.
	/// </summary>
	/// <param name="value">The refractive index value.</param>
	/// <returns>A new RefractiveIndex instance.</returns>
	public static new RefractiveIndex FromValue(double value) => new() { Value = value };
}
