// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;
/// <summary>
/// Represents a 3D velocity vector with float precision.
/// </summary>
public sealed record Velocity3D : Generic.Velocity3D<float>
{

	/// <summary>Gets the 3D vector value stored in this quantity.</summary>
	public Vector3f Value { get; init; } = Vector3f.Zero;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public bool IsPhysicallyValid => !float.IsNaN(Value.X) && !float.IsNaN(Value.Y) && !float.IsNaN(Value.Z) &&
									 float.IsFinite(Value.X) && float.IsFinite(Value.Y) && float.IsFinite(Value.Z);

	/// <summary>
	/// Initializes a new instance of the <see cref="Velocity3D"/> class.
	/// </summary>
	public Velocity3D() : base() { }

	/// <summary>
	/// Creates a new instance with the specified vector value.
	/// </summary>
	/// <param name="value">The vector value for the quantity.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Velocity3D Create(Vector3f value) => new() { Value = value };

	/// <summary>
	/// Creates a new instance with the specified X, Y, and Z components.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <param name="z">The Z component.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Velocity3D Create(float x, float y, float z) => Create(new Vector3f(x, y, z));    /// <summary>
																									/// Gets the velocity as a Vector3f in meters per second (the base unit).
																									/// </summary>
																									/// <returns>The velocity vector in m/s.</returns>
	public Vector3f InMetersPerSecond() => Value;

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
	public static Velocity3D operator *(Velocity3D velocity, float scalar)
	{
		ArgumentNullException.ThrowIfNull(velocity);
		return Create(velocity.Value * scalar);
	}

	/// <summary>Scales a velocity by a scalar.</summary>
	public static Velocity3D operator *(float scalar, Velocity3D velocity)
	{
		ArgumentNullException.ThrowIfNull(velocity);
		return Create(scalar * velocity.Value);
	}

	/// <summary>Divides a velocity by a scalar.</summary>
	public static Velocity3D operator /(Velocity3D velocity, float scalar)
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
