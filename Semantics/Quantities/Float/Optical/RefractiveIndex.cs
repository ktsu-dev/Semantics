// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a refractive index quantity with float precision.
/// </summary>
public sealed record RefractiveIndex : Generic.RefractiveIndex<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RefractiveIndex"/> class.
	/// </summary>
	public RefractiveIndex() : base() { }

	/// <summary>
	/// Creates a new RefractiveIndex from a dimensionless value.
	/// </summary>
	/// <param name="value">The refractive index value.</param>
	/// <returns>A new RefractiveIndex instance.</returns>
	public static new RefractiveIndex FromValue(float value) => new() { Value = value };
}
