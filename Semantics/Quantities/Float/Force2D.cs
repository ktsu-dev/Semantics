// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

using System.Numerics;

/// <summary>
/// Represents a 2D force vector with float precision.
/// </summary>
public sealed record Force2D : Generic.Force2D<float>
{

	/// <summary>Gets the 2D vector value stored in this quantity.</summary>
	public Vector2f Value { get; init; } = Vector2f.Zero;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public bool IsPhysicallyValid => !float.IsNaN(Value.X) && !float.IsNaN(Value.Y) &&
									 float.IsFinite(Value.X) && float.IsFinite(Value.Y);

	/// <summary>
	/// Initializes a new instance of the <see cref="Force2D"/> class.
	/// </summary>
	public Force2D() : base() { }

	/// <summary>
	/// Creates a new instance with the specified vector value.
	/// </summary>
	/// <param name="value">The vector value for the quantity.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Force2D Create(Vector2f value) => new() { Value = value };

	/// <summary>
	/// Creates a new instance with the specified X and Y components.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Force2D Create(float x, float y) => Create(new Vector2f(x, y));

	/// <summary>
	/// Creates a new Force2D from X and Y components in newtons.
	/// </summary>
	/// <param name="x">The X component in newtons.</param>
	/// <param name="y">The Y component in newtons.</param>
	/// <returns>A new Force2D instance.</returns>
	public static Force2D FromNewtons(float x, float y) => Create(x, y);

	/// <summary>
	/// Creates a new Force2D from a Vector2 in newtons.
	/// </summary>
	/// <param name="newtons">The force vector in newtons.</param>
	/// <returns>A new Force2D instance.</returns>
	public static Force2D FromNewtons(Vector2 newtons) => Create(new Vector2f(newtons));

	/// <summary>Gets the magnitude of this force vector.</summary>
	public float Magnitude => Value.Length();

	/// <summary>Gets the X component of this vector quantity.</summary>
	public float X => Value.X;

	/// <summary>Gets the Y component of this vector quantity.</summary>
	public float Y => Value.Y;

	/// <summary>
	/// Gets the unit vector (normalized) form of this quantity.
	/// </summary>
	/// <returns>A new instance representing the unit vector, or zero if magnitude is zero.</returns>
	public Force2D Unit()
	{
		float magnitude = Magnitude;
		return magnitude > 0 ? Create(Value.Normalize()) : Create(Vector2f.Zero);
	}

	/// <summary>
	/// Gets the force as a Vector2f in newtons (the base unit).
	/// </summary>
	/// <returns>The force vector in newtons.</returns>
	public Vector2f InNewtons() => Value;

	// Vector arithmetic operations
	/// <summary>Adds two forces.</summary>
	public static Force2D operator +(Force2D left, Force2D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value + right.Value);
	}

	/// <summary>Subtracts two forces.</summary>
	public static Force2D operator -(Force2D left, Force2D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value - right.Value);
	}

	/// <summary>Multiplies a force by a scalar.</summary>
	public static Force2D operator *(Force2D force, float scalar)
	{
		ArgumentNullException.ThrowIfNull(force);
		return Create(force.Value * scalar);
	}

	/// <summary>Multiplies a scalar by a force.</summary>
	public static Force2D operator *(float scalar, Force2D force)
	{
		ArgumentNullException.ThrowIfNull(force);
		return Create(scalar * force.Value);
	}

	/// <summary>Divides a force by a scalar.</summary>
	public static Force2D operator /(Force2D force, float scalar)
	{
		ArgumentNullException.ThrowIfNull(force);
		return Create(force.Value / scalar);
	}

	/// <summary>Negates a force.</summary>
	public static Force2D operator -(Force2D force)
	{
		ArgumentNullException.ThrowIfNull(force);
		return Create(-force.Value);
	}

	/// <summary>Gets the zero force.</summary>
	public static Force2D Zero => Create(0, 0);

	/// <summary>
	/// Calculates the dot product of two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The dot product as an energy quantity.</returns>
	public Energy Dot(Force2D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Energy.FromJoules(Value.Dot(other.Value));
	}

	/// <summary>
	/// Calculates the distance between two force vectors in force space.
	/// </summary>
	/// <param name="other">The other force vector.</param>
	/// <returns>The distance in force space as a force quantity.</returns>
	public Force ForceDistance(Force2D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Force.FromNewtons(Value.Distance(other.Value));
	}

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => $"{Value} {Dimension.BaseUnit.Symbol}";
}
