// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents a length quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public record Length<T> : PhysicalQuantity<Length<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of length [L].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Length;

	/// <summary>
	/// Initializes a new instance of the <see cref="Length{T}"/> class.
	/// </summary>
	public Length() : base() { }

	/// <summary>
	/// Creates a new Length from a value in meters.
	/// </summary>
	/// <param name="meters">The value in meters.</param>
	/// <returns>A new Length instance.</returns>
	public static Length<T> FromMeters(T meters) => Create(meters);

	/// <summary>
	/// Multiplies this length by another length to create an area.
	/// </summary>
	/// <param name="left">The first length.</param>
	/// <param name="right">The second length.</param>
	/// <returns>The resulting area.</returns>
	public static Area<T> operator *(Length<T> left, Length<T> right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Area<T>.Create(left.Value * right.Value);
	}

	/// <summary>
	/// Multiplies this length by another length to create an area.
	/// </summary>
	/// <param name="left">The first length.</param>
	/// <param name="right">The second length.</param>
	/// <returns>The resulting area.</returns>
	public static Area<T> Multiply(Length<T> left, Length<T> right) => left * right;

	/// <summary>
	/// Divides this length by time to create a velocity.
	/// </summary>
	/// <param name="left">The length.</param>
	/// <param name="right">The time.</param>
	/// <returns>The resulting velocity.</returns>
	public static Velocity<T> operator /(Length<T> left, Time<T> right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Velocity<T>.Create(left.Value / right.Value);
	}

	/// <summary>
	/// Divides this length by time to create a velocity.
	/// </summary>
	/// <param name="left">The length.</param>
	/// <param name="right">The time.</param>
	/// <returns>The resulting velocity.</returns>
	public static Velocity<T> Divide(Length<T> left, Time<T> right) => left / right;
}

// Note: Operator interfaces removed due to C# constraints on binary operators.
// Dimensional operations are defined directly on each quantity type.
