// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a dynamic viscosity quantity with float precision.
/// </summary>
public sealed record DynamicViscosity : Generic.DynamicViscosity<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DynamicViscosity"/> class.
	/// </summary>
	public DynamicViscosity() : base() { }

	/// <summary>
	/// Creates a new DynamicViscosity from a value in pascal seconds.
	/// </summary>
	/// <param name="pascalSeconds">The value in pascal seconds.</param>
	/// <returns>A new DynamicViscosity instance.</returns>
	public static DynamicViscosity FromPascalSeconds(float pascalSeconds) => new() { Quantity = pascalSeconds };
}
