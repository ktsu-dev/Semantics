// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an absorbed dose quantity with float precision.
/// </summary>
public sealed record AbsorbedDose : Generic.AbsorbedDose<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AbsorbedDose"/> class.
	/// </summary>
	public AbsorbedDose() : base() { }

	/// <summary>
	/// Creates a new AbsorbedDose from a value in grays.
	/// </summary>
	/// <param name="grays">The value in grays.</param>
	/// <returns>A new AbsorbedDose instance.</returns>
	public static new AbsorbedDose FromGrays(float grays) => new() { Value = grays };

}
