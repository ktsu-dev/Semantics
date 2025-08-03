// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents a specific gravity (relative density) quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public record SpecificGravity<T> : PhysicalQuantity<SpecificGravity<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of specificgravity [dimensionless].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Dimensionless;

	/// <summary>
	/// Initializes a new instance of the <see cref="SpecificGravity{T}"/> class.
	/// </summary>
	public SpecificGravity() : base() { }

	/// <summary>
	/// Creates a new SpecificGravity from a dimensionless ratio value.
	/// </summary>
	/// <param name="ratio">The dimensionless ratio relative to reference density (typically water).</param>
	/// <returns>A new SpecificGravity instance.</returns>
	public static SpecificGravity<T> FromRatio(T ratio) => Create(ratio);

	/// <summary>
	/// Divides density by reference density to create specific gravity.
	/// </summary>
	/// <param name="density">The density.</param>
	/// <param name="referenceDensity">The reference density (typically water at standard conditions).</param>
	/// <returns>The resulting specific gravity.</returns>
	public static SpecificGravity<T> Divide(Density<T> density, Density<T> referenceDensity)
	{
		ArgumentNullException.ThrowIfNull(density);
		ArgumentNullException.ThrowIfNull(referenceDensity);
		return Create(density.Value / referenceDensity.Value);
	}

	/// <summary>
	/// Multiplies specific gravity by reference density to get density.
	/// </summary>
	/// <param name="specificGravity">The specific gravity.</param>
	/// <param name="referenceDensity">The reference density.</param>
	/// <returns>The resulting density.</returns>
	public static Density<T> Multiply(SpecificGravity<T> specificGravity, Density<T> referenceDensity)
	{
		ArgumentNullException.ThrowIfNull(specificGravity);
		ArgumentNullException.ThrowIfNull(referenceDensity);
		return Density<T>.Create(specificGravity.Value * referenceDensity.Value);
	}
}
