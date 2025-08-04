// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a length quantity with float precision.
/// </summary>
public sealed record Length : Generic.Length<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Length"/> class.
	/// </summary>
	public Length() : base() { }

	/// <summary>
	/// Creates a new Length from a value in meters.
	/// </summary>
	/// <param name="meters">The value in meters.</param>
	/// <returns>A new Length instance.</returns>
	public static new Length FromMeters(float meters) => (Length)Create(meters);
}
