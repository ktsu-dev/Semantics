// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an electric resistance quantity with double precision.
/// </summary>
public sealed record ElectricResistance : Generic.ElectricResistance<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricResistance"/> class.
	/// </summary>
	public ElectricResistance() : base() { }

	/// <summary>
	/// Creates a new ElectricResistance from a value in ohms.
	/// </summary>
	/// <param name="ohms">The value in ohms.</param>
	/// <returns>A new ElectricResistance instance.</returns>
	public static new ElectricResistance FromOhms(double ohms) => new() { Quantity = ohms };
}
