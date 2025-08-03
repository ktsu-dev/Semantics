// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a length quantity with double precision.
/// </summary>
public sealed record Length : Generic.Length<double>
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
	public static new Length FromMeters(double meters) => new() { Value = meters };
}
