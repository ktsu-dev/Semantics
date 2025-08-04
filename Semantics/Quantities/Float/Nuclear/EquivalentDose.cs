// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an equivalent dose quantity with float precision.
/// </summary>
public sealed record EquivalentDose : Generic.EquivalentDose<float>
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
	public static new EquivalentDose FromSieverts(float sieverts) => new() { Quantity = sieverts };
}
