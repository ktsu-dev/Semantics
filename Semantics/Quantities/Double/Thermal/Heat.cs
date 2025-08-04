// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a heat quantity with double precision.
/// </summary>
public sealed record Heat : Generic.Heat<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Heat"/> class.
	/// </summary>
	public Heat() : base() { }

	/// <summary>
	/// Creates a new Heat from a value in joules.
	/// </summary>
	/// <param name="joules">The value in joules.</param>
	/// <returns>A new Heat instance.</returns>
	public static new Heat FromJoules(double joules) => new() { Quantity = joules };
}
