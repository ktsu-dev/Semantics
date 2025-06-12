// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a volume quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Volume<T> : PhysicalQuantity<Volume<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Volume;

	/// <summary>
	/// Initializes a new instance of the Volume class.
	/// </summary>
	public Volume() : base() { }

	/// <summary>
	/// Creates a new Volume from a value in cubic meters.
	/// </summary>
	/// <param name="cubicMeters">The value in cubic meters.</param>
	/// <returns>A new Volume instance.</returns>
	public static Volume<T> FromCubicMeters(T cubicMeters) => Create(cubicMeters);
}
