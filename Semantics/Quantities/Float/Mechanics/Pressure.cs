// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a pressure quantity with float precision.
/// </summary>
public sealed record Pressure : Generic.Pressure<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Pressure"/> class.
	/// </summary>
	public Pressure() : base() { }

	/// <summary>
	/// Creates a new Pressure from a value in pascals.
	/// </summary>
	/// <param name="pascals">The value in pascals.</param>
	/// <returns>A new Pressure instance.</returns>
	public static new Pressure FromPascals(float pascals) => new() { Quantity = pascals };

}
