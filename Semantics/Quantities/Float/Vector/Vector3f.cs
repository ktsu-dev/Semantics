// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

using System.Numerics;
using ktsu.Semantics.Generic;

/// <summary>
/// A float-precision 3D vector that adapts System.Numerics.Vector3 to the generic interface.
/// </summary>
public readonly record struct Vector3f : IVector3<Vector3f, float>
{
	private readonly Vector3 _value;

	/// <summary>
	/// Initializes a new instance of the Vector3f struct.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <param name="z">The Z component.</param>
	public Vector3f(float x, float y, float z) => _value = new Vector3(x, y, z);

	/// <summary>
	/// Initializes a new instance of the Vector3f struct from a System.Numerics.Vector3.
	/// </summary>
	/// <param name="vector">The source vector.</param>
	public Vector3f(Vector3 vector) => _value = vector;

	/// <inheritdoc/>
	public float X => _value.X;

	/// <inheritdoc/>
	public float Y => _value.Y;

	/// <inheritdoc/>
	public float Z => _value.Z;

	/// <inheritdoc/>
	public static Vector3f Zero => new(Vector3.Zero);

	/// <inheritdoc/>
	public static Vector3f One => new(Vector3.One);

	/// <inheritdoc/>
	public static Vector3f UnitX => new(Vector3.UnitX);

	/// <inheritdoc/>
	public static Vector3f UnitY => new(Vector3.UnitY);

	/// <inheritdoc/>
	public static Vector3f UnitZ => new(Vector3.UnitZ);

	/// <inheritdoc/>
	public float Length() => _value.Length();

	/// <inheritdoc/>
	public float LengthSquared() => _value.LengthSquared();

	/// <inheritdoc/>
	public float Dot(Vector3f other) => Vector3.Dot(_value, other._value);

	/// <inheritdoc/>
	public Vector3f Cross(Vector3f other) => new(Vector3.Cross(_value, other._value));

	/// <inheritdoc/>
	public float Distance(Vector3f other) => Vector3.Distance(_value, other._value);

	/// <inheritdoc/>
	public float DistanceSquared(Vector3f other) => Vector3.DistanceSquared(_value, other._value);

	/// <inheritdoc/>
	public Vector3f Normalize() => new(Vector3.Normalize(_value));

	// Arithmetic operators
	/// <inheritdoc/>
	public static Vector3f operator +(Vector3f left, Vector3f right) => new(left._value + right._value);

	/// <inheritdoc/>
	public static Vector3f operator -(Vector3f left, Vector3f right) => new(left._value - right._value);

	/// <inheritdoc/>
	public static Vector3f operator *(Vector3f vector, float scalar) => new(vector._value * scalar);

	/// <inheritdoc/>
	public static Vector3f operator *(float scalar, Vector3f vector) => new(scalar * vector._value);

	/// <inheritdoc/>
	public static Vector3f operator /(Vector3f vector, float scalar) => new(vector._value / scalar);

	/// <inheritdoc/>
	public static Vector3f operator -(Vector3f vector) => new(-vector._value);

	// Implicit conversions
	/// <summary>Implicit conversion from System.Numerics.Vector3.</summary>
	public static implicit operator Vector3f(Vector3 vector) => new(vector);

	/// <summary>Implicit conversion to System.Numerics.Vector3.</summary>
	public static implicit operator Vector3(Vector3f vector) => vector._value;

	/// <summary>Returns a string representation of the vector.</summary>
	public override string ToString() => _value.ToString();
}
