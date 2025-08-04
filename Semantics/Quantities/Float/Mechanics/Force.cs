// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a force quantity with float precision.
/// </summary>
public sealed record Force : Generic.Force<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Force"/> class.
	/// </summary>
	public Force() : base() { }

	/// <summary>
	/// Creates a new Force from a value in newtons.
	/// </summary>
	/// <param name="newtons">The value in newtons.</param>
	/// <returns>A new Force instance.</returns>
	public static new Force FromNewtons(float newtons) => new() { Quantity = newtons };

}
