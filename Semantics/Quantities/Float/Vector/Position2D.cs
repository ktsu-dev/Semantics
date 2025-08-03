// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

using System.Numerics;

/// <summary>
/// Represents a 2D position vector with float precision.
/// </summary>
public sealed record Position2D : Generic.Position2D<float>
{
	/// <summary>Gets the 2D vector value stored in this quantity.</summary>
	public Vector2f Value { get; init; } = Vector2f.Zero;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public bool IsPhysicallyValid => !float.IsNaN(Value.X) && !float.IsNaN(Value.Y) &&
									 float.IsFinite(Value.X) && float.IsFinite(Value.Y);

	/// <summary>
	/// Initializes a new instance of the <see cref="Position2D"/> class.
	/// </summary>
	public Position2D() : base() { }

	/// <summary>
	/// Creates a new instance with the specified vector value.
	/// </summary>
	/// <param name="value">The vector value for the quantity.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Position2D Create(Vector2f value) => new() { Value = value };

	/// <summary>
	/// Creates a new instance with the specified X and Y components.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Position2D Create(float x, float y) => Create(new Vector2f(x, y));

	/// <summary>
	/// Creates a new Position2D from X and Y coordinates in meters.
	/// </summary>
	/// <param name="x">The X coordinate in meters.</param>
	/// <param name="y">The Y coordinate in meters.</param>
	/// <returns>A new Position2D instance.</returns>
	public static Position2D FromMeters(float x, float y) => Create(x, y);

	/// <summary>
	/// Creates a new Position2D from a Vector2 in meters.
	/// </summary>
	/// <param name="meters">The position vector in meters.</param>
	/// <returns>A new Position2D instance.</returns>
	public static Position2D FromMeters(Vector2 meters) => Create(new Vector2f(meters));

	/// <summary>Gets the magnitude (length) of this vector quantity.</summary>
	public float Magnitude => Value.Length();

	/// <summary>Gets the X component of this vector quantity.</summary>
	public float X => Value.X;

	/// <summary>Gets the Y component of this vector quantity.</summary>
	public float Y => Value.Y;

	/// <summary>
	/// Gets the unit vector (normalized) form of this quantity.
	/// </summary>
	/// <returns>A new instance representing the unit vector, or zero if magnitude is zero.</returns>
	public Position2D Unit()
	{
		float magnitude = Magnitude;
		return magnitude > 0 ? Create(Value.Normalize()) : Create(Vector2f.Zero);
	}

	/// <summary>
	/// Gets the position as a Vector2f in meters (the base unit).
	/// </summary>
	/// <returns>The position vector in meters.</returns>
	public Vector2f InMeters() => Value;

	/// <summary>
	/// Calculates displacement from this position to another position.
	/// </summary>
	/// <param name="other">The target position.</param>
	/// <returns>The displacement vector from this position to the other.</returns>
	public Displacement2D DisplacementTo(Position2D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Displacement2D.Create(other.Value - Value);
	}

	/// <summary>
	/// Moves this position by the specified displacement.
	/// </summary>
	/// <param name="displacement">The displacement to apply.</param>
	/// <returns>A new position after applying the displacement.</returns>
	public Position2D Move(Displacement2D displacement)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(Value + displacement.Value);
	}

	// Vector arithmetic operations
	/// <summary>Adds a displacement to a position.</summary>
	public static Position2D operator +(Position2D position, Displacement2D displacement)
	{
		ArgumentNullException.ThrowIfNull(position);
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(position.Value + displacement.Value);
	}

	/// <summary>Subtracts a displacement from a position.</summary>
	public static Position2D operator -(Position2D position, Displacement2D displacement)
	{
		ArgumentNullException.ThrowIfNull(position);
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(position.Value - displacement.Value);
	}

	/// <summary>Calculates displacement between two positions.</summary>
	public static Displacement2D operator -(Position2D position1, Position2D position2)
	{
		ArgumentNullException.ThrowIfNull(position1);
		ArgumentNullException.ThrowIfNull(position2);
		return Displacement2D.Create(position1.Value - position2.Value);
	}

	/// <summary>Gets the origin position (0, 0).</summary>
	public static Position2D Origin => Create(0, 0);

	/// <summary>
	/// Calculates the dot product of two position vectors, resulting in an area.
	/// </summary>
	/// <param name="other">The other position vector.</param>
	/// <returns>The dot product as an area (L × L = L²).</returns>
	public Area Dot(Position2D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Area.FromSquareMeters(Value.Dot(other.Value));
	}

	/// <summary>
	/// Calculates the distance between two positions.
	/// </summary>
	/// <param name="other">The other position.</param>
	/// <returns>The distance as a length quantity.</returns>
	public Length Distance(Position2D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Length.FromMeters(Value.Distance(other.Value));
	}

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => $"{Value} {Dimension.BaseUnit.Symbol}";
}
