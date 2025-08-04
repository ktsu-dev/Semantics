// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;
/// <summary>
/// Represents a 2D displacement vector with float precision.
/// </summary>
public sealed record Displacement2D : Generic.Displacement2D<float>
{

	/// <summary>Gets the 2D vector value stored in this quantity.</summary>
	public Vector2f Value { get; init; } = Vector2f.Zero;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public bool IsPhysicallyValid => !float.IsNaN(Value.X) && !float.IsNaN(Value.Y) &&
									 float.IsFinite(Value.X) && float.IsFinite(Value.Y);

	/// <summary>
	/// Initializes a new instance of the <see cref="Displacement2D"/> class.
	/// </summary>
	public Displacement2D() { }

	/// <summary>
	/// Creates a new instance with the specified vector value.
	/// </summary>
	/// <param name="value">The vector value for the quantity.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Displacement2D Create(Vector2f value) => new() { Value = value };

	/// <summary>
	/// Creates a new instance with the specified X and Y components.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Displacement2D Create(float x, float y) => Create(new Vector2f(x, y));

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
	public Displacement2D Unit()
	{
		float magnitude = Magnitude;
		return magnitude > 0 ? Create(Value.Normalize()) : Create(Vector2f.Zero);
	}

	/// <summary>
	/// Gets the displacement as a Vector2f in meters (the base unit).
	/// </summary>
	/// <returns>The displacement vector in meters.</returns>
	public Vector2f InMeters() => Value;

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
	public static Displacement2D operator *(Displacement2D displacement, float scalar)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(displacement.Value * scalar);
	}

	/// <summary>Multiplies a scalar by a displacement.</summary>
	public static Displacement2D operator *(float scalar, Displacement2D displacement)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(scalar * displacement.Value);
	}

	/// <summary>Divides a displacement by a scalar.</summary>
	public static Displacement2D operator /(Displacement2D displacement, float scalar)
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
	/// Calculates the dot product of two displacement vectors, resulting in an area.
	/// </summary>
	/// <param name="other">The other displacement vector.</param>
	/// <returns>The dot product as an area (L × L = L²).</returns>
	public Area Dot(Displacement2D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Area.FromSquareMeters(Value.Dot(other.Value));
	}

	/// <summary>
	/// Calculates the distance between two displacement vectors.
	/// </summary>
	/// <param name="other">The other displacement vector.</param>
	/// <returns>The distance as a length quantity.</returns>
	public Length Distance(Displacement2D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Length.FromMeters(Value.Distance(other.Value));
	}

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => $"{Value} {Dimension.BaseUnit.Symbol}";
}
