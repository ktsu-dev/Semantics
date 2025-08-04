// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a power quantity with float precision.
/// </summary>
public sealed record Power : Generic.Power<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Power"/> class.
	/// </summary>
	public Power() : base() { }

	/// <summary>
	/// Creates a new Power from a value in watts.
	/// </summary>
	/// <param name="watts">The value in watts.</param>
	/// <returns>A new Power instance.</returns>
	public static new Power FromWatts(float watts) => new() { Quantity = watts };

}
