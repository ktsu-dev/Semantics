// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Double;

using System.Numerics;

/// <summary>
/// Represents a 3D velocity vector with double precision.
/// </summary>
public sealed record Velocity3D : Generic.Velocity3D<double>
{

	/// <summary>Gets the 3D vector value stored in this quantity.</summary>
	public Vector3d Value { get; init; } = Vector3d.Zero;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public bool IsPhysicallyValid => !double.IsNaN(Value.X) && !double.IsNaN(Value.Y) && !double.IsNaN(Value.Z) &&
									 double.IsFinite(Value.X) && double.IsFinite(Value.Y) && double.IsFinite(Value.Z);

	/// <summary>
	/// Initializes a new instance of the <see cref="Velocity3D"/> class.
	/// </summary>
	public Velocity3D() : base() { }

	/// <summary>
	/// Creates a new instance with the specified vector value.
	/// </summary>
	/// <param name="value">The vector value for the quantity.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Velocity3D Create(Vector3d value) => new() { Value = value };

	/// <summary>
	/// Creates a new instance with the specified X, Y, and Z components.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <param name="z">The Z component.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Velocity3D Create(double x, double y, double z) => Create(new Vector3d(x, y, z));

	/// <summary>
	/// Creates a new Velocity3D from X, Y, and Z components in meters per second.
	/// </summary>
	/// <param name="x">The X component in m/s.</param>
	/// <param name="y">The Y component in m/s.</param>
	/// <param name="z">The Z component in m/s.</param>
	/// <returns>A new Velocity3D instance.</returns>
	public static Velocity3D FromMetersPerSecond(double x, double y, double z) => Create(x, y, z);

	/// <summary>
	/// Creates a new Velocity3D from a Vector3 in meters per second.
	/// </summary>
	/// <param name="metersPerSecond">The velocity vector in m/s.</param>
	/// <returns>A new Velocity3D instance.</returns>
	public static Velocity3D FromMetersPerSecond(Vector3 metersPerSecond) => Create(new Vector3d(metersPerSecond.X, metersPerSecond.Y, metersPerSecond.Z));

	/// <summary>Gets the magnitude (speed) of this velocity vector.</summary>
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
	public Velocity3D Unit()
	{
		double magnitude = Magnitude;
		return magnitude > 0 ? Create(Value.Normalize()) : Create(Vector3d.Zero);
	}

	/// <summary>
	/// Gets the velocity as a Vector3d in meters per second (the base unit).
	/// </summary>
	/// <returns>The velocity vector in m/s.</returns>
	public Vector3d InMetersPerSecond() => Value;

	// Vector arithmetic operations
	/// <summary>Adds two velocities.</summary>
	public static Velocity3D operator +(Velocity3D left, Velocity3D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value + right.Value);
	}

	/// <summary>Subtracts two velocities.</summary>
	public static Velocity3D operator -(Velocity3D left, Velocity3D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value - right.Value);
	}

	/// <summary>Negates a velocity.</summary>
	public static Velocity3D operator -(Velocity3D velocity)
	{
		ArgumentNullException.ThrowIfNull(velocity);
		return Create(-velocity.Value);
	}

	/// <summary>Scales a velocity by a scalar.</summary>
	public static Velocity3D operator *(Velocity3D velocity, double scalar)
	{
		ArgumentNullException.ThrowIfNull(velocity);
		return Create(velocity.Value * scalar);
	}

	/// <summary>Scales a velocity by a scalar.</summary>
	public static Velocity3D operator *(double scalar, Velocity3D velocity)
	{
		ArgumentNullException.ThrowIfNull(velocity);
		return Create(scalar * velocity.Value);
	}

	/// <summary>Divides a velocity by a scalar.</summary>
	public static Velocity3D operator /(Velocity3D velocity, double scalar)
	{
		ArgumentNullException.ThrowIfNull(velocity);
		return Create(velocity.Value / scalar);
	}

	/// <summary>Gets the zero velocity (0, 0, 0).</summary>
	public static Velocity3D Zero => Create(0, 0, 0);

	/// <summary>
	/// Calculates the dot product of two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The dot product as an energy quantity.</returns>
	public Energy Dot(Velocity3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Energy.FromJoules(Value.Dot(other.Value));
	}

	/// <summary>
	/// Calculates the cross product of two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The cross product.</returns>
	public Velocity3D Cross(Velocity3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Create(Value.Cross(other.Value));
	}

	/// <summary>
	/// Calculates the distance between two velocity vectors in velocity space.
	/// </summary>
	/// <param name="other">The other velocity vector.</param>
	/// <returns>The distance in velocity space as a velocity quantity.</returns>
	public Velocity VelocityDistance(Velocity3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Velocity.FromMetersPerSecond(Value.Distance(other.Value));
	}

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => $"{Value} {Dimension.BaseUnit.Symbol}";
}