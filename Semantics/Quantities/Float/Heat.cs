// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a heat quantity with float precision.
/// </summary>
public sealed record Heat : Generic.Heat<float>
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
	public static new Heat FromJoules(float joules) => new() { Value = joules };


}
