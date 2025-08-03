// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents an area quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public record Area<T> : PhysicalQuantity<Area<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of area [L²].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Area;

	/// <summary>
	/// Initializes a new instance of the <see cref="Area{T}"/> class.
	/// </summary>
	public Area() : base() { }

	/// <summary>
	/// Creates a new Area from a value in square meters.
	/// </summary>
	/// <param name="squareMeters">The value in square meters.</param>
	/// <returns>A new Area instance.</returns>
	public static Area<T> FromSquareMeters(T squareMeters) => Create(squareMeters);

	/// <summary>
	/// Multiplies this area by a length to create a volume.
	/// </summary>
	/// <param name="left">The area.</param>
	/// <param name="right">The length.</param>
	/// <returns>The resulting volume.</returns>
	public static Volume<T> operator *(Area<T> left, Length<T> right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Volume<T>.Create(left.Value * right.Value);
	}

	/// <summary>
	/// Multiplies this area by a length to create a volume.
	/// </summary>
	/// <param name="left">The area.</param>
	/// <param name="right">The length.</param>
	/// <returns>The resulting volume.</returns>
	public static Volume<T> Multiply(Area<T> left, Length<T> right) => left * right;
}
