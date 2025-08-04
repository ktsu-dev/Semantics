// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a specific gravity quantity with float precision.
/// </summary>
public sealed record SpecificGravity : Generic.SpecificGravity<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SpecificGravity"/> class.
	/// </summary>
	public SpecificGravity() : base() { }

	/// <summary>
	/// Creates a new SpecificGravity from a dimensionless ratio.
	/// </summary>
	/// <param name="ratio">The dimensionless ratio value.</param>
	/// <returns>A new SpecificGravity instance.</returns>
	public static new SpecificGravity FromRatio(float ratio) => new() { Quantity = ratio };
}
