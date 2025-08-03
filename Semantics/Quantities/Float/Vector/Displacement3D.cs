// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

using System.Numerics;

/// <summary>
/// Represents a 3D displacement vector with float precision.
/// </summary>
public sealed record Displacement3D : Generic.Displacement3D<float>
{

	/// <summary>Gets the 3D vector value stored in this quantity.</summary>
	public Vector3f Value { get; init; } = Vector3f.Zero;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public bool IsPhysicallyValid => !float.IsNaN(Value.X) && !float.IsNaN(Value.Y) && !float.IsNaN(Value.Z) &&
									 float.IsFinite(Value.X) && float.IsFinite(Value.Y) && float.IsFinite(Value.Z);

	/// <summary>
	/// Initializes a new instance of the <see cref="Displacement3D"/> class.
	/// </summary>
	public Displacement3D() : base() { }

	/// <summary>
	/// Creates a new instance with the specified vector value.
	/// </summary>
	/// <param name="value">The vector value for the quantity.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Displacement3D Create(Vector3f value) => new() { Value = value };

	/// <summary>
	/// Creates a new instance with the specified X, Y, and Z components.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <param name="z">The Z component.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Displacement3D Create(float x, float y, float z) => Create(new Vector3f(x, y, z));

	/// <summary>
	/// Creates a new Displacement3D from X, Y, and Z components in meters.
	/// </summary>
	/// <param name="x">The X component in meters.</param>
	/// <param name="y">The Y component in meters.</param>
	/// <param name="z">The Z component in meters.</param>
	/// <returns>A new Displacement3D instance.</returns>
	public static Displacement3D FromMeters(float x, float y, float z) => Create(x, y, z);

	/// <summary>
	/// Creates a new Displacement3D from a Vector3 in meters.
	/// </summary>
	/// <param name="meters">The displacement vector in meters.</param>
	/// <returns>A new Displacement3D instance.</returns>
	public static Displacement3D FromMeters(Vector3 meters) => Create(new Vector3f(meters));

	/// <summary>Gets the magnitude (length) of this vector quantity.</summary>
	public float Magnitude => Value.Length();

	/// <summary>Gets the X component of this vector quantity.</summary>
	public float X => Value.X;

	/// <summary>Gets the Y component of this vector quantity.</summary>
	public float Y => Value.Y;

	/// <summary>Gets the Z component of this vector quantity.</summary>
	public float Z => Value.Z;

	/// <summary>
	/// Gets the unit vector (normalized) form of this quantity.
	/// </summary>
	/// <returns>A new instance representing the unit vector, or zero if magnitude is zero.</returns>
	public Displacement3D Unit()
	{
		float magnitude = Magnitude;
		return magnitude > 0 ? Create(Value.Normalize()) : Create(Vector3f.Zero);
	}

	/// <summary>
	/// Gets the displacement as a Vector3f in meters (the base unit).
	/// </summary>
	/// <returns>The displacement vector in meters.</returns>
	public Vector3f InMeters() => Value;

	// Vector arithmetic operations
	/// <summary>Adds two displacements.</summary>
	public static Displacement3D operator +(Displacement3D left, Displacement3D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value + right.Value);
	}

	/// <summary>Subtracts two displacements.</summary>
	public static Displacement3D operator -(Displacement3D left, Displacement3D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value - right.Value);
	}

	/// <summary>Negates a displacement.</summary>
	public static Displacement3D operator -(Displacement3D displacement)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(-displacement.Value);
	}

	/// <summary>Scales a displacement by a scalar.</summary>
	public static Displacement3D operator *(Displacement3D displacement, float scalar)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(displacement.Value * scalar);
	}

	/// <summary>Scales a displacement by a scalar.</summary>
	public static Displacement3D operator *(float scalar, Displacement3D displacement)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(scalar * displacement.Value);
	}

	/// <summary>Divides a displacement by a scalar.</summary>
	public static Displacement3D operator /(Displacement3D displacement, float scalar)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(displacement.Value / scalar);
	}

	/// <summary>Gets the zero displacement (0, 0, 0).</summary>
	public static Displacement3D Zero => Create(0, 0, 0);

	/// <summary>
	/// Calculates the dot product of two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The dot product as an area quantity.</returns>
	public Area Dot(Displacement3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Area.FromSquareMeters(Value.Dot(other.Value));
	}

	/// <summary>
	/// Calculates the cross product of two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The cross product.</returns>
	public Displacement3D Cross(Displacement3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Create(Value.Cross(other.Value));
	}

	/// <summary>
	/// Calculates the distance between two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The distance as a length quantity.</returns>
	public Length Distance(Displacement3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Length.FromMeters(Value.Distance(other.Value));
	}

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => $"{Value} {Dimension.BaseUnit.Symbol}";
}
