// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an energy quantity with float precision.
/// </summary>
public sealed record Energy : Generic.Energy<float>
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
	public static new Energy FromJoules(float joules) => new() { Value = joules };

}
