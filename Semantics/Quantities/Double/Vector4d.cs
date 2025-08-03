// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

using ktsu.Semantics.Generic;

/// <summary>
/// A double-precision 4D vector that implements the generic interface.
/// </summary>
public readonly record struct Vector4d : IVector4<Vector4d, double>
{

	/// <summary>
	/// Initializes a new instance of the Vector4d struct.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <param name="z">The Z component.</param>
	/// <param name="w">The W component.</param>
	public Vector4d(double x, double y, double z, double w) : this()
	{
		X = x;
		Y = y;
		Z = z;
		W = w;
	}

	/// <inheritdoc/>
	public double X { get; }

	/// <inheritdoc/>
	public double Y { get; }

	/// <inheritdoc/>
	public double Z { get; }

	/// <inheritdoc/>
	public double W { get; }

	/// <inheritdoc/>
	public static Vector4d Zero => new(0.0, 0.0, 0.0, 0.0);

	/// <inheritdoc/>
	public static Vector4d One => new(1.0, 1.0, 1.0, 1.0);

	/// <inheritdoc/>
	public static Vector4d UnitX => new(1.0, 0.0, 0.0, 0.0);

	/// <inheritdoc/>
	public static Vector4d UnitY => new(0.0, 1.0, 0.0, 0.0);

	/// <inheritdoc/>
	public static Vector4d UnitZ => new(0.0, 0.0, 1.0, 0.0);

	/// <inheritdoc/>
	public static Vector4d UnitW => new(0.0, 0.0, 0.0, 1.0);

	/// <inheritdoc/>
	public double Length() => Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));

	/// <inheritdoc/>
	public double LengthSquared() => (X * X) + (Y * Y) + (Z * Z) + (W * W);

	/// <inheritdoc/>
	public double Dot(Vector4d other) => (X * other.X) + (Y * other.Y) + (Z * other.Z) + (W * other.W);

	/// <inheritdoc/>
	public double Distance(Vector4d other) => (this - other).Length();

	/// <inheritdoc/>
	public double DistanceSquared(Vector4d other) => (this - other).LengthSquared();

	/// <inheritdoc/>
	public Vector4d Normalize()
	{
		double length = Length();
		return length > 0 ? new Vector4d(X / length, Y / length, Z / length, W / length) : Zero;
	}

	// Arithmetic operators
	/// <inheritdoc/>
	public static Vector4d operator +(Vector4d left, Vector4d right) => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);

	/// <inheritdoc/>
	public static Vector4d operator -(Vector4d left, Vector4d right) => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);

	/// <inheritdoc/>
	public static Vector4d operator *(Vector4d vector, double scalar) => new(vector.X * scalar, vector.Y * scalar, vector.Z * scalar, vector.W * scalar);

	/// <inheritdoc/>
	public static Vector4d operator *(double scalar, Vector4d vector) => new(scalar * vector.X, scalar * vector.Y, scalar * vector.Z, scalar * vector.W);

	/// <inheritdoc/>
	public static Vector4d operator /(Vector4d vector, double scalar) => new(vector.X / scalar, vector.Y / scalar, vector.Z / scalar, vector.W / scalar);

	/// <inheritdoc/>
	public static Vector4d operator -(Vector4d vector) => new(-vector.X, -vector.Y, -vector.Z, -vector.W);

	/// <summary>Returns a string representation of the vector.</summary>
	public override string ToString() => $"<{X}, {Y}, {Z}, {W}>";
}