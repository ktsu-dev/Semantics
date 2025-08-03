// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable IDE0040 // Accessibility modifiers required
#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Interface for 2D vector types with generic numeric component support.
/// </summary>
/// <typeparam name="TVector">The implementing vector type.</typeparam>
/// <typeparam name="T">The numeric component type.</typeparam>
public interface IVector2<TVector, T>
	where TVector : IVector2<TVector, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the X component.</summary>
	T X { get; }

	/// <summary>Gets the Y component.</summary>
	T Y { get; }

	/// <summary>Gets a vector with all components set to zero.</summary>
	public static abstract TVector Zero { get; }

	/// <summary>Gets a vector with all components set to one.</summary>
	public static abstract TVector One { get; }

	/// <summary>Gets the unit vector for the X-axis.</summary>
	public static abstract TVector UnitX { get; }

	/// <summary>Gets the unit vector for the Y-axis.</summary>
	public static abstract TVector UnitY { get; }

	/// <summary>
	/// Calculates the length of the vector.
	/// </summary>
	/// <returns>The length of the vector.</returns>
	public T Length();

	/// <summary>
	/// Calculates the squared length of the vector.
	/// </summary>
	/// <returns>The squared length of the vector.</returns>
	public T LengthSquared();

	/// <summary>
	/// Calculates the dot product of two vectors.
	/// </summary>
	/// <param name="other">The other vector.</param>
	/// <returns>The dot product.</returns>
	public T Dot(TVector other);

	/// <summary>
	/// Calculates the distance between two vectors.
	/// </summary>
	/// <param name="other">The other vector.</param>
	/// <returns>The distance between the vectors.</returns>
	public T Distance(TVector other);

	/// <summary>
	/// Calculates the squared distance between two vectors.
	/// </summary>
	/// <param name="other">The other vector.</param>
	/// <returns>The squared distance between the vectors.</returns>
	public T DistanceSquared(TVector other);

	/// <summary>
	/// Returns a normalized version of the vector.
	/// </summary>
	/// <returns>The normalized vector.</returns>
	public TVector Normalize();

	// Arithmetic operators
	/// <summary>Adds two vectors.</summary>
	public static abstract TVector operator +(TVector left, TVector right);

	/// <summary>Subtracts two vectors.</summary>
	public static abstract TVector operator -(TVector left, TVector right);

	/// <summary>Multiplies a vector by a scalar.</summary>
	public static abstract TVector operator *(TVector vector, T scalar);

	/// <summary>Multiplies a scalar by a vector.</summary>
	public static abstract TVector operator *(T scalar, TVector vector);

	/// <summary>Divides a vector by a scalar.</summary>
	public static abstract TVector operator /(TVector vector, T scalar);

	/// <summary>Negates a vector.</summary>
	public static abstract TVector operator -(TVector vector);
}
