// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an amount of substance quantity with double precision.
/// </summary>
public sealed record AmountOfSubstance : Generic.AmountOfSubstance<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AmountOfSubstance"/> class.
	/// </summary>
	public AmountOfSubstance() : base() { }

	/// <summary>
	/// Creates a new AmountOfSubstance from a value in moles.
	/// </summary>
	/// <param name="moles">The value in moles.</param>
	/// <returns>A new AmountOfSubstance instance.</returns>
	public static new AmountOfSubstance FromMoles(double moles) => new() { Quantity = moles };
}
