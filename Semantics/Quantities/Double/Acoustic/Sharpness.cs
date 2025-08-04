// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a sharpness quantity with double precision.
/// </summary>
public sealed record Sharpness : Generic.Sharpness<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Sharpness"/> class.
	/// </summary>
	public Sharpness() : base() { }

	/// <summary>
	/// Creates a new Sharpness from a value in acums.
	/// </summary>
	/// <param name="acums">The sharpness in acums.</param>
	/// <returns>A new Sharpness instance.</returns>
	public static new Sharpness FromAcums(double acums) => new() { Quantity = acums };
}
