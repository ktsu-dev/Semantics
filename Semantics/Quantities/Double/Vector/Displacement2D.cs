// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

using System.Numerics;

/// <summary>
/// Represents a 2D displacement vector with double precision.
/// </summary>
public sealed record Displacement2D : Generic.Displacement2D<double>
{

	/// <summary>Gets the 2D vector value stored in this quantity.</summary>
	public Vector2d Value { get; init; } = Vector2d.Zero;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public bool IsPhysicallyValid => !double.IsNaN(Value.X) && !double.IsNaN(Value.Y) &&
									 double.IsFinite(Value.X) && double.IsFinite(Value.Y);

	/// <summary>
	/// Initializes a new instance of the <see cref="Displacement2D"/> class.
	/// </summary>
	public Displacement2D() : base() { }

	/// <summary>
	/// Creates a new instance with the specified vector value.
	/// </summary>
	/// <param name="value">The vector value for the quantity.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Displacement2D Create(Vector2d value) => new() { Value = value };

	/// <summary>
	/// Creates a new instance with the specified X and Y components.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Displacement2D Create(double x, double y) => Create(new Vector2d(x, y));	/// <summary>
	/// Gets the displacement as a Vector2d in meters (the base unit).
	/// </summary>
	/// <returns>The displacement vector in meters.</returns>
	public Vector2d InMeters() => Value;

	// Vector arithmetic operations
	/// <summary>Adds two displacements.</summary>
	public static Displacement2D operator +(Displacement2D left, Displacement2D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value + right.Value);
	}

	/// <summary>Subtracts two displacements.</summary>
	public static Displacement2D operator -(Displacement2D left, Displacement2D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value - right.Value);
	}

	/// <summary>Multiplies a displacement by a scalar.</summary>
	public static Displacement2D operator *(Displacement2D displacement, double scalar)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(displacement.Value * scalar);
	}

	/// <summary>Multiplies a scalar by a displacement.</summary>
	public static Displacement2D operator *(double scalar, Displacement2D displacement)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(scalar * displacement.Value);
	}

	/// <summary>Divides a displacement by a scalar.</summary>
	public static Displacement2D operator /(Displacement2D displacement, double scalar)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(displacement.Value / scalar);
	}

	/// <summary>Negates a displacement.</summary>
	public static Displacement2D operator -(Displacement2D displacement)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(-displacement.Value);
	}

	/// <summary>Gets the zero displacement.</summary>
	public static Displacement2D Zero => Create(0, 0);

	/// <summary>
	/// Calculates the dot product of two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The dot product as an area quantity.</returns>
	public Area Dot(Displacement2D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Area.FromSquareMeters(Value.Dot(other.Value));
	}

	/// <summary>
	/// Calculates the distance between two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The distance as a length quantity.</returns>
	public Length Distance(Displacement2D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Length.FromMeters(Value.Distance(other.Value));
	}

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => $"{Value} {Dimension.BaseUnit.Symbol}";
}
