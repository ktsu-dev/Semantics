// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an amount of substance quantity with float precision.
/// </summary>
public sealed record AmountOfSubstance : Generic.AmountOfSubstance<float>
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
	public static new AmountOfSubstance FromMoles(float moles) => new() { Quantity = moles };
}
