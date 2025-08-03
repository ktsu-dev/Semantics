// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

using System.Numerics;

/// <summary>
/// Represents a 2D velocity vector with double precision.
/// </summary>
public sealed record Velocity2D : Generic.Velocity2D<double>
{

	/// <summary>Gets the 2D vector value stored in this quantity.</summary>
	public Vector2d Value { get; init; } = Vector2d.Zero;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public bool IsPhysicallyValid => !double.IsNaN(Value.X) && !double.IsNaN(Value.Y) &&
									 double.IsFinite(Value.X) && double.IsFinite(Value.Y);

	/// <summary>
	/// Initializes a new instance of the <see cref="Velocity2D"/> class.
	/// </summary>
	public Velocity2D() : base() { }

	/// <summary>
	/// Creates a new instance with the specified vector value.
	/// </summary>
	/// <param name="value">The vector value for the quantity.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Velocity2D Create(Vector2d value) => new() { Value = value };

	/// <summary>
	/// Creates a new instance with the specified X and Y components.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Velocity2D Create(double x, double y) => Create(new Vector2d(x, y));

	/// <summary>
	/// Creates a new Velocity2D from X and Y components in meters per second.
	/// </summary>
	/// <param name="x">The X component in meters per second.</param>
	/// <param name="y">The Y component in meters per second.</param>
	/// <returns>A new Velocity2D instance.</returns>
	public static Velocity2D FromMetersPerSecond(double x, double y) => Create(x, y);

	/// <summary>
	/// Creates a new Velocity2D from a Vector2 in meters per second.
	/// </summary>
	/// <param name="metersPerSecond">The velocity vector in meters per second.</param>
	/// <returns>A new Velocity2D instance.</returns>
	public static Velocity2D FromMetersPerSecond(Vector2 metersPerSecond) => Create(new Vector2d(metersPerSecond.X, metersPerSecond.Y));

	/// <summary>Gets the magnitude of this velocity vector (speed).</summary>
	public double Magnitude => Value.Length();

	/// <summary>Gets the speed (magnitude of velocity vector).</summary>
	public double Speed => Magnitude;

	/// <summary>Gets the X component of this vector quantity.</summary>
	public double X => Value.X;

	/// <summary>Gets the Y component of this vector quantity.</summary>
	public double Y => Value.Y;

	/// <summary>
	/// Gets the unit vector (normalized) form of this quantity.
	/// </summary>
	/// <returns>A new instance representing the unit vector, or zero if magnitude is zero.</returns>
	public Velocity2D Unit()
	{
		double magnitude = Magnitude;
		return magnitude > 0 ? Create(Value.Normalize()) : Create(Vector2d.Zero);
	}

	/// <summary>
	/// Gets the velocity as a Vector2d in meters per second (the base unit).
	/// </summary>
	/// <returns>The velocity vector in meters per second.</returns>
	public Vector2d InMetersPerSecond() => Value;

	// Vector arithmetic operations
	/// <summary>Adds two velocities.</summary>
	public static Velocity2D operator +(Velocity2D left, Velocity2D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value + right.Value);
	}

	/// <summary>Subtracts two velocities.</summary>
	public static Velocity2D operator -(Velocity2D left, Velocity2D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value - right.Value);
	}

	/// <summary>Multiplies a velocity by a scalar.</summary>
	public static Velocity2D operator *(Velocity2D velocity, double scalar)
	{
		ArgumentNullException.ThrowIfNull(velocity);
		return Create(velocity.Value * scalar);
	}

	/// <summary>Multiplies a scalar by a velocity.</summary>
	public static Velocity2D operator *(double scalar, Velocity2D velocity)
	{
		ArgumentNullException.ThrowIfNull(velocity);
		return Create(scalar * velocity.Value);
	}

	/// <summary>Divides a velocity by a scalar.</summary>
	public static Velocity2D operator /(Velocity2D velocity, double scalar)
	{
		ArgumentNullException.ThrowIfNull(velocity);
		return Create(velocity.Value / scalar);
	}

	/// <summary>Negates a velocity.</summary>
	public static Velocity2D operator -(Velocity2D velocity)
	{
		ArgumentNullException.ThrowIfNull(velocity);
		return Create(-velocity.Value);
	}

	/// <summary>Gets the zero velocity.</summary>
	public static Velocity2D Zero => Create(0, 0);

	/// <summary>
	/// Calculates the dot product of two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The dot product as an energy quantity.</returns>
	public Energy Dot(Velocity2D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Energy.FromJoules(Value.Dot(other.Value));
	}

	/// <summary>
	/// Calculates the distance between two velocity vectors in velocity space.
	/// </summary>
	/// <param name="other">The other velocity vector.</param>
	/// <returns>The distance in velocity space as a velocity quantity.</returns>
	public Velocity VelocityDistance(Velocity2D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Velocity.FromMetersPerSecond(Value.Distance(other.Value));
	}

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => $"{Value} {Dimension.BaseUnit.Symbol}";
}
