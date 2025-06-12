// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a power quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Power<T> : PhysicalQuantity<Power<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Power;

	/// <summary>
	/// Initializes a new instance of the Power class.
	/// </summary>
	public Power() : base() { }

	/// <summary>
	/// Creates a new Power from a value in watts.
	/// </summary>
	/// <param name="watts">The value in watts.</param>
	/// <returns>A new Power instance.</returns>
	public static Power<T> FromWatts(T watts) => Create(watts);
}
