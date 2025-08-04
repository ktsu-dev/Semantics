// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a bulk modulus quantity with float precision.
/// </summary>
public sealed record BulkModulus : Generic.BulkModulus<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="BulkModulus"/> class.
	/// </summary>
	public BulkModulus() : base() { }

	/// <summary>
	/// Creates a new BulkModulus from a value in pascals.
	/// </summary>
	/// <param name="pascals">The value in pascals.</param>
	/// <returns>A new BulkModulus instance.</returns>
	public static new BulkModulus FromPascals(float pascals) => new() { Quantity = pascals };
}
