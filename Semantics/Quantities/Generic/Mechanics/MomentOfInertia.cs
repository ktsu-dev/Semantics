// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents a moment of inertia quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public record MomentOfInertia<T> : PhysicalQuantity<MomentOfInertia<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of momentofinertia [M L²].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.MomentOfInertia;

	/// <summary>
	/// Initializes a new instance of the <see cref="MomentOfInertia{T}"/> class.
	/// </summary>
	public MomentOfInertia() : base() { }

	/// <summary>
	/// Creates a new MomentOfInertia from a value in kilogram-square meters.
	/// </summary>
	/// <param name="kilogramSquareMeters">The value in kilogram-square meters.</param>
	/// <returns>A new MomentOfInertia instance.</returns>
	public static MomentOfInertia<T> FromKilogramSquareMeters(T kilogramSquareMeters) => Create(kilogramSquareMeters);

	/// <summary>
	/// Multiplies mass by area to create moment of inertia.
	/// </summary>
	/// <param name="mass">The mass.</param>
	/// <param name="area">The area.</param>
	/// <returns>The resulting moment of inertia.</returns>
	public static MomentOfInertia<T> Multiply(Mass<T> mass, Area<T> area)
	{
		ArgumentNullException.ThrowIfNull(mass);
		ArgumentNullException.ThrowIfNull(area);
		return Create(mass.Value * area.Value);
	}

	/// <summary>
	/// Divides moment of inertia by mass to get area.
	/// </summary>
	/// <param name="momentOfInertia">The moment of inertia.</param>
	/// <param name="mass">The mass.</param>
	/// <returns>The resulting area.</returns>
	public static Area<T> Divide(MomentOfInertia<T> momentOfInertia, Mass<T> mass)
	{
		ArgumentNullException.ThrowIfNull(momentOfInertia);
		ArgumentNullException.ThrowIfNull(mass);
		return Area<T>.Create(momentOfInertia.Value / mass.Value);
	}

	/// <summary>
	/// Divides moment of inertia by area to get mass.
	/// </summary>
	/// <param name="momentOfInertia">The moment of inertia.</param>
	/// <param name="area">The area.</param>
	/// <returns>The resulting mass.</returns>
	public static Mass<T> Divide(MomentOfInertia<T> momentOfInertia, Area<T> area)
	{
		ArgumentNullException.ThrowIfNull(momentOfInertia);
		ArgumentNullException.ThrowIfNull(area);
		return Mass<T>.Create(momentOfInertia.Value / area.Value);
	}
}
