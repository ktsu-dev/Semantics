// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Double;

using System.Numerics;

/// <summary>
/// Represents a 3D force vector with double precision.
/// </summary>
public sealed record Force3D : Generic.Force3D<double>
{

	/// <summary>Gets the 3D vector value stored in this quantity.</summary>
	public Vector3d Value { get; init; } = Vector3d.Zero;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public bool IsPhysicallyValid => !double.IsNaN(Value.X) && !double.IsNaN(Value.Y) && !double.IsNaN(Value.Z) &&
									 double.IsFinite(Value.X) && double.IsFinite(Value.Y) && double.IsFinite(Value.Z);

	/// <summary>
	/// Initializes a new instance of the <see cref="Force3D"/> class.
	/// </summary>
	public Force3D() : base() { }

	/// <summary>
	/// Creates a new instance with the specified vector value.
	/// </summary>
	/// <param name="value">The vector value for the quantity.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Force3D Create(Vector3d value) => new() { Value = value };

	/// <summary>
	/// Creates a new instance with the specified X, Y, and Z components.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <param name="z">The Z component.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Force3D Create(double x, double y, double z) => Create(new Vector3d(x, y, z));

	/// <summary>
	/// Creates a new Force3D from X, Y, and Z components in newtons.
	/// </summary>
	/// <param name="x">The X component in N.</param>
	/// <param name="y">The Y component in N.</param>
	/// <param name="z">The Z component in N.</param>
	/// <returns>A new Force3D instance.</returns>
	public static Force3D FromNewtons(double x, double y, double z) => Create(x, y, z);

	/// <summary>
	/// Creates a new Force3D from a Vector3 in newtons.
	/// </summary>
	/// <param name="newtons">The force vector in N.</param>
	/// <returns>A new Force3D instance.</returns>
	public static Force3D FromNewtons(Vector3 newtons) => Create(new Vector3d(newtons.X, newtons.Y, newtons.Z));

	/// <summary>Gets the magnitude of this force vector.</summary>
	public double Magnitude => Value.Length();

	/// <summary>Gets the X component of this vector quantity.</summary>
	public double X => Value.X;

	/// <summary>Gets the Y component of this vector quantity.</summary>
	public double Y => Value.Y;

	/// <summary>Gets the Z component of this vector quantity.</summary>
	public double Z => Value.Z;

	/// <summary>
	/// Gets the unit vector (normalized) form of this quantity.
	/// </summary>
	/// <returns>A new instance representing the unit vector, or zero if magnitude is zero.</returns>
	public Force3D Unit()
	{
		double magnitude = Magnitude;
		return magnitude > 0 ? Create(Value.Normalize()) : Create(Vector3d.Zero);
	}

	/// <summary>
	/// Gets the force as a Vector3d in newtons (the base unit).
	/// </summary>
	/// <returns>The force vector in N.</returns>
	public Vector3d InNewtons() => Value;

	// Vector arithmetic operations
	/// <summary>Adds two forces.</summary>
	public static Force3D operator +(Force3D left, Force3D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value + right.Value);
	}

	/// <summary>Subtracts two forces.</summary>
	public static Force3D operator -(Force3D left, Force3D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value - right.Value);
	}

	/// <summary>Negates a force.</summary>
	public static Force3D operator -(Force3D force)
	{
		ArgumentNullException.ThrowIfNull(force);
		return Create(-force.Value);
	}

	/// <summary>Scales a force by a scalar.</summary>
	public static Force3D operator *(Force3D force, double scalar)
	{
		ArgumentNullException.ThrowIfNull(force);
		return Create(force.Value * scalar);
	}

	/// <summary>Scales a force by a scalar.</summary>
	public static Force3D operator *(double scalar, Force3D force)
	{
		ArgumentNullException.ThrowIfNull(force);
		return Create(scalar * force.Value);
	}

	/// <summary>Divides a force by a scalar.</summary>
	public static Force3D operator /(Force3D force, double scalar)
	{
		ArgumentNullException.ThrowIfNull(force);
		return Create(force.Value / scalar);
	}

	/// <summary>Gets the zero force (0, 0, 0).</summary>
	public static Force3D Zero => Create(0, 0, 0);

	/// <summary>
	/// Calculates the dot product of two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The dot product as an energy quantity.</returns>
	public Energy Dot(Force3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Energy.FromJoules(Value.Dot(other.Value));
	}

	/// <summary>
	/// Calculates the cross product of two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The cross product.</returns>
	public Force3D Cross(Force3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Create(Value.Cross(other.Value));
	}

	/// <summary>
	/// Calculates the distance between two force vectors in force space.
	/// </summary>
	/// <param name="other">The other force vector.</param>
	/// <returns>The distance in force space as a force quantity.</returns>
	public Force ForceDistance(Force3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Force.FromNewtons(Value.Distance(other.Value));
	}

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => $"{Value} {Dimension.BaseUnit.Symbol}";
}