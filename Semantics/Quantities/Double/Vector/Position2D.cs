// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;
/// <summary>
/// Represents a 2D position vector with double precision.
/// </summary>
public sealed record Position2D : Generic.Position2D<double>
{

	/// <summary>Gets the 2D vector value stored in this quantity.</summary>
	public Vector2d Value { get; init; } = Vector2d.Zero;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public bool IsPhysicallyValid => !double.IsNaN(Value.X) && !double.IsNaN(Value.Y) &&
									 double.IsFinite(Value.X) && double.IsFinite(Value.Y);

	/// <summary>
	/// Initializes a new instance of the <see cref="Position2D"/> class.
	/// </summary>
	public Position2D() : base() { }

	/// <summary>
	/// Creates a new instance with the specified vector value.
	/// </summary>
	/// <param name="value">The vector value for the quantity.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Position2D Create(Vector2d value) => new() { Value = value };

	/// <summary>
	/// Creates a new instance with the specified X and Y components.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Position2D Create(double x, double y) => Create(new Vector2d(x, y));  /// <summary>
																						/// Gets the position as a Vector2d in meters (the base unit).
																						/// </summary>
																						/// <returns>The position vector in meters.</returns>
	public Vector2d InMeters() => Value;

	// Vector arithmetic operations
	/// <summary>Adds two positions (vector addition).</summary>
	public static Position2D operator +(Position2D left, Position2D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value + right.Value);
	}

	/// <summary>Subtracts two positions (displacement vector).</summary>
	public static Displacement2D operator -(Position2D left, Position2D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Displacement2D.Create(left.Value - right.Value);
	}

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

	/// <summary>Multiplies a position by a scalar.</summary>
	public static Position2D operator *(Position2D position, double scalar)
	{
		ArgumentNullException.ThrowIfNull(position);
		return Create(position.Value * scalar);
	}

	/// <summary>Multiplies a scalar by a position.</summary>
	public static Position2D operator *(double scalar, Position2D position)
	{
		ArgumentNullException.ThrowIfNull(position);
		return Create(scalar * position.Value);
	}

	/// <summary>Divides a position by a scalar.</summary>
	public static Position2D operator /(Position2D position, double scalar)
	{
		ArgumentNullException.ThrowIfNull(position);
		return Create(position.Value / scalar);
	}

	/// <summary>Negates a position.</summary>
	public static Position2D operator -(Position2D position)
	{
		ArgumentNullException.ThrowIfNull(position);
		return Create(-position.Value);
	}

	/// <summary>Gets the origin position.</summary>
	public static Position2D Origin => Create(0, 0);

	/// <summary>
	/// Calculates the dot product of two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The dot product as an area quantity.</returns>
	public Area Dot(Position2D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Area.FromSquareMeters(Value.Dot(other.Value));
	}

	/// <summary>
	/// Calculates the distance between two positions.
	/// </summary>
	/// <param name="other">The other position.</param>
	/// <returns>The distance between the positions as a length quantity.</returns>
	public Length Distance(Position2D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Length.FromMeters(Value.Distance(other.Value));
	}

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => $"{Value} {Dimension.BaseUnit.Symbol}";
}
