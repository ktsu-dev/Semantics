// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a dynamic viscosity quantity with double precision.
/// </summary>
public sealed record DynamicViscosity : Generic.DynamicViscosity<double>
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
	public static new DynamicViscosity FromPascalSeconds(double pascalSeconds) => new() { Value = pascalSeconds };
}
