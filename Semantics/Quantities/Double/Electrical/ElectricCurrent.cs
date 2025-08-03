// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an electric current quantity with double precision.
/// </summary>
public sealed record ElectricCurrent : Generic.ElectricCurrent<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricCurrent"/> class.
	/// </summary>
	public ElectricCurrent() : base() { }

	/// <summary>
	/// Creates a new ElectricCurrent from a value in amperes.
	/// </summary>
	/// <param name="amperes">The value in amperes.</param>
	/// <returns>A new ElectricCurrent instance.</returns>
	public static new ElectricCurrent FromAmperes(double amperes) => new() { Value = amperes };
}
