// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

using ktsu.Semantics.Generic;

/// <summary>
/// A double-precision 2D vector that implements the generic interface.
/// </summary>
public readonly record struct Vector2d : IVector2<Vector2d, double>
{

	/// <summary>
	/// Initializes a new instance of the Vector2d struct.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	public Vector2d(double x, double y) : this()
	{
		X = x;
		Y = y;
	}

	/// <inheritdoc/>
	public double X { get; }

	/// <inheritdoc/>
	public double Y { get; }

	/// <inheritdoc/>
	public static Vector2d Zero => new(0.0, 0.0);

	/// <inheritdoc/>
	public static Vector2d One => new(1.0, 1.0);

	/// <inheritdoc/>
	public static Vector2d UnitX => new(1.0, 0.0);

	/// <inheritdoc/>
	public static Vector2d UnitY => new(0.0, 1.0);

	/// <inheritdoc/>
	public double Length() => Math.Sqrt((X * X) + (Y * Y));

	/// <inheritdoc/>
	public double LengthSquared() => (X * X) + (Y * Y);

	/// <inheritdoc/>
	public double Dot(Vector2d other) => (X * other.X) + (Y * other.Y);

	/// <inheritdoc/>
	public double Distance(Vector2d other) => (this - other).Length();

	/// <inheritdoc/>
	public double DistanceSquared(Vector2d other) => (this - other).LengthSquared();

	/// <inheritdoc/>
	public Vector2d Normalize()
	{
		double length = Length();
		return length > 0 ? new Vector2d(X / length, Y / length) : Zero;
	}

	// Arithmetic operators
	/// <inheritdoc/>
	public static Vector2d operator +(Vector2d left, Vector2d right) => new(left.X + right.X, left.Y + right.Y);

	/// <inheritdoc/>
	public static Vector2d operator -(Vector2d left, Vector2d right) => new(left.X - right.X, left.Y - right.Y);

	/// <inheritdoc/>
	public static Vector2d operator *(Vector2d vector, double scalar) => new(vector.X * scalar, vector.Y * scalar);

	/// <inheritdoc/>
	public static Vector2d operator *(double scalar, Vector2d vector) => new(scalar * vector.X, scalar * vector.Y);

	/// <inheritdoc/>
	public static Vector2d operator /(Vector2d vector, double scalar) => new(vector.X / scalar, vector.Y / scalar);

	/// <inheritdoc/>
	public static Vector2d operator -(Vector2d vector) => new(-vector.X, -vector.Y);

	/// <summary>Returns a string representation of the vector.</summary>
	public override string ToString() => $"<{X}, {Y}>";
}
