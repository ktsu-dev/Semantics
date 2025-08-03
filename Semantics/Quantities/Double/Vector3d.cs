// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

using ktsu.Semantics.Generic;

/// <summary>
/// A double-precision 3D vector that implements the generic interface.
/// </summary>
public readonly record struct Vector3d : IVector3<Vector3d, double>
{

	/// <summary>
	/// Initializes a new instance of the Vector3d struct.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <param name="z">The Z component.</param>
	public Vector3d(double x, double y, double z) : this()
	{
		X = x;
		Y = y;
		Z = z;
	}

	/// <inheritdoc/>
	public double X { get; }

	/// <inheritdoc/>
	public double Y { get; }

	/// <inheritdoc/>
	public double Z { get; }

	/// <inheritdoc/>
	public static Vector3d Zero => new(0.0, 0.0, 0.0);

	/// <inheritdoc/>
	public static Vector3d One => new(1.0, 1.0, 1.0);

	/// <inheritdoc/>
	public static Vector3d UnitX => new(1.0, 0.0, 0.0);

	/// <inheritdoc/>
	public static Vector3d UnitY => new(0.0, 1.0, 0.0);

	/// <inheritdoc/>
	public static Vector3d UnitZ => new(0.0, 0.0, 1.0);

	/// <inheritdoc/>
	public double Length() => Math.Sqrt((X * X) + (Y * Y) + (Z * Z));

	/// <inheritdoc/>
	public double LengthSquared() => (X * X) + (Y * Y) + (Z * Z);

	/// <inheritdoc/>
	public double Dot(Vector3d other) => (X * other.X) + (Y * other.Y) + (Z * other.Z);

	/// <inheritdoc/>
	public Vector3d Cross(Vector3d other) => new(
		(Y * other.Z) - (Z * other.Y),
		(Z * other.X) - (X * other.Z),
		(X * other.Y) - (Y * other.X)
	);

	/// <inheritdoc/>
	public double Distance(Vector3d other) => (this - other).Length();

	/// <inheritdoc/>
	public double DistanceSquared(Vector3d other) => (this - other).LengthSquared();

	/// <inheritdoc/>
	public Vector3d Normalize()
	{
		double length = Length();
		return length > 0 ? new Vector3d(X / length, Y / length, Z / length) : Zero;
	}

	// Arithmetic operators
	/// <inheritdoc/>
	public static Vector3d operator +(Vector3d left, Vector3d right) => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

	/// <inheritdoc/>
	public static Vector3d operator -(Vector3d left, Vector3d right) => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

	/// <inheritdoc/>
	public static Vector3d operator *(Vector3d vector, double scalar) => new(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);

	/// <inheritdoc/>
	public static Vector3d operator *(double scalar, Vector3d vector) => new(scalar * vector.X, scalar * vector.Y, scalar * vector.Z);

	/// <inheritdoc/>
	public static Vector3d operator /(Vector3d vector, double scalar) => new(vector.X / scalar, vector.Y / scalar, vector.Z / scalar);

	/// <inheritdoc/>
	public static Vector3d operator -(Vector3d vector) => new(-vector.X, -vector.Y, -vector.Z);

	/// <summary>Returns a string representation of the vector.</summary>
	public override string ToString() => $"<{X}, {Y}, {Z}>";
}
