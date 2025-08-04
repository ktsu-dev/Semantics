// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;
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
	public static Velocity2D Create(double x, double y) => Create(new Vector2d(x, y));  /// <summary>
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
