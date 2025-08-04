// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an energy quantity with double precision.
/// </summary>
public sealed record Energy : Generic.Energy<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Energy"/> class.
	/// </summary>
	public Energy() : base() { }

	/// <summary>
	/// Creates a new Energy from a value in joules.
	/// </summary>
	/// <param name="joules">The value in joules.</param>
	/// <returns>A new Energy instance.</returns>
	public static new Energy FromJoules(double joules) => new() { Quantity = joules };
}
