// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

using System.Numerics;
using ktsu.Semantics.Generic;

/// <summary>
/// A float-precision 2D vector that adapts System.Numerics.Vector2 to the generic interface.
/// </summary>
public readonly record struct Vector2f : IVector2<Vector2f, float>
{
	private readonly Vector2 _value;

	/// <summary>
	/// Initializes a new instance of the Vector2f struct.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	public Vector2f(float x, float y) => _value = new Vector2(x, y);

	/// <summary>
	/// Initializes a new instance of the Vector2f struct from a System.Numerics.Vector2.
	/// </summary>
	/// <param name="vector">The source vector.</param>
	public Vector2f(Vector2 vector) => _value = vector;

	/// <inheritdoc/>
	public float X => _value.X;

	/// <inheritdoc/>
	public float Y => _value.Y;

	/// <inheritdoc/>
	public static Vector2f Zero => new(Vector2.Zero);

	/// <inheritdoc/>
	public static Vector2f One => new(Vector2.One);

	/// <inheritdoc/>
	public static Vector2f UnitX => new(Vector2.UnitX);

	/// <inheritdoc/>
	public static Vector2f UnitY => new(Vector2.UnitY);

	/// <inheritdoc/>
	public float Length() => _value.Length();

	/// <inheritdoc/>
	public float LengthSquared() => _value.LengthSquared();

	/// <inheritdoc/>
	public float Dot(Vector2f other) => Vector2.Dot(_value, other._value);

	/// <inheritdoc/>
	public float Distance(Vector2f other) => Vector2.Distance(_value, other._value);

	/// <inheritdoc/>
	public float DistanceSquared(Vector2f other) => Vector2.DistanceSquared(_value, other._value);

	/// <inheritdoc/>
	public Vector2f Normalize() => new(Vector2.Normalize(_value));

	// Arithmetic operators
	/// <inheritdoc/>
	public static Vector2f operator +(Vector2f left, Vector2f right) => new(left._value + right._value);

	/// <inheritdoc/>
	public static Vector2f operator -(Vector2f left, Vector2f right) => new(left._value - right._value);

	/// <inheritdoc/>
	public static Vector2f operator *(Vector2f vector, float scalar) => new(vector._value * scalar);

	/// <inheritdoc/>
	public static Vector2f operator *(float scalar, Vector2f vector) => new(scalar * vector._value);

	/// <inheritdoc/>
	public static Vector2f operator /(Vector2f vector, float scalar) => new(vector._value / scalar);

	/// <inheritdoc/>
	public static Vector2f operator -(Vector2f vector) => new(-vector._value);

	// Implicit conversions
	/// <summary>Implicit conversion from System.Numerics.Vector2.</summary>
	public static implicit operator Vector2f(Vector2 vector) => new(vector);

	/// <summary>Implicit conversion to System.Numerics.Vector2.</summary>
	public static implicit operator Vector2(Vector2f vector) => vector._value;

	/// <summary>Returns a string representation of the vector.</summary>
	public override string ToString() => _value.ToString();
}
