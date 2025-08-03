// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

using System.Numerics;

/// <summary>
/// Represents a 3D acceleration vector with float precision.
/// </summary>
public sealed record Acceleration3D : Generic.Acceleration3D<float>
{

	/// <summary>Gets the 3D vector value stored in this quantity.</summary>
	public Vector3f Value { get; init; } = Vector3f.Zero;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public bool IsPhysicallyValid => !float.IsNaN(Value.X) && !float.IsNaN(Value.Y) && !float.IsNaN(Value.Z) &&
									 float.IsFinite(Value.X) && float.IsFinite(Value.Y) && float.IsFinite(Value.Z);

	/// <summary>
	/// Initializes a new instance of the <see cref="Acceleration3D"/> class.
	/// </summary>
	public Acceleration3D() : base() { }

	/// <summary>
	/// Creates a new instance with the specified vector value.
	/// </summary>
	/// <param name="value">The vector value for the quantity.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Acceleration3D Create(Vector3f value) => new() { Value = value };

	/// <summary>
	/// Creates a new instance with the specified X, Y, and Z components.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <param name="z">The Z component.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Acceleration3D Create(float x, float y, float z) => Create(new Vector3f(x, y, z));

	/// <summary>
	/// Creates a new Acceleration3D from X, Y, and Z components in meters per second squared.
	/// </summary>
	/// <param name="x">The X component in m/s².</param>
	/// <param name="y">The Y component in m/s².</param>
	/// <param name="z">The Z component in m/s².</param>
	/// <returns>A new Acceleration3D instance.</returns>
	public static Acceleration3D FromMetersPerSecondSquared(float x, float y, float z) => Create(x, y, z);

	/// <summary>
	/// Creates a new Acceleration3D from a Vector3 in meters per second squared.
	/// </summary>
	/// <param name="metersPerSecondSquared">The acceleration vector in m/s².</param>
	/// <returns>A new Acceleration3D instance.</returns>
	public static Acceleration3D FromMetersPerSecondSquared(Vector3 metersPerSecondSquared) => Create(new Vector3f(metersPerSecondSquared));

	/// <summary>Gets the magnitude of this acceleration vector.</summary>
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
	public Acceleration3D Unit()
	{
		float magnitude = Magnitude;
		return magnitude > 0 ? Create(Value.Normalize()) : Create(Vector3f.Zero);
	}

	/// <summary>
	/// Gets the acceleration as a Vector3f in meters per second squared (the base unit).
	/// </summary>
	/// <returns>The acceleration vector in m/s².</returns>
	public Vector3f InMetersPerSecondSquared() => Value;

	// Vector arithmetic operations
	/// <summary>Adds two accelerations.</summary>
	public static Acceleration3D operator +(Acceleration3D left, Acceleration3D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value + right.Value);
	}

	/// <summary>Subtracts two accelerations.</summary>
	public static Acceleration3D operator -(Acceleration3D left, Acceleration3D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value - right.Value);
	}

	/// <summary>Negates an acceleration.</summary>
	public static Acceleration3D operator -(Acceleration3D acceleration)
	{
		ArgumentNullException.ThrowIfNull(acceleration);
		return Create(-acceleration.Value);
	}

	/// <summary>Scales an acceleration by a scalar.</summary>
	public static Acceleration3D operator *(Acceleration3D acceleration, float scalar)
	{
		ArgumentNullException.ThrowIfNull(acceleration);
		return Create(acceleration.Value * scalar);
	}

	/// <summary>Scales an acceleration by a scalar.</summary>
	public static Acceleration3D operator *(float scalar, Acceleration3D acceleration)
	{
		ArgumentNullException.ThrowIfNull(acceleration);
		return Create(scalar * acceleration.Value);
	}

	/// <summary>Divides an acceleration by a scalar.</summary>
	public static Acceleration3D operator /(Acceleration3D acceleration, float scalar)
	{
		ArgumentNullException.ThrowIfNull(acceleration);
		return Create(acceleration.Value / scalar);
	}

	/// <summary>Gets the zero acceleration (0, 0, 0).</summary>
	public static Acceleration3D Zero => Create(0, 0, 0);

	/// <summary>Gets the standard gravity acceleration (0, 0, -9.80665).</summary>
	public static Acceleration3D StandardGravity => Create(0, 0, -9.80665f);

	/// <summary>
	/// Calculates the dot product of two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The dot product as a power quantity.</returns>
	public Power Dot(Acceleration3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Power.FromWatts(Value.Dot(other.Value));
	}

	/// <summary>
	/// Calculates the cross product of two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The cross product.</returns>
	public Acceleration3D Cross(Acceleration3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Create(Value.Cross(other.Value));
	}

	/// <summary>
	/// Calculates the distance between two acceleration vectors in acceleration space.
	/// </summary>
	/// <param name="other">The other acceleration vector.</param>
	/// <returns>The distance in acceleration space as an acceleration quantity.</returns>
	public Acceleration AccelerationDistance(Acceleration3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Acceleration.FromMetersPerSecondSquared(Value.Distance(other.Value));
	}

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => $"{Value} {Dimension.BaseUnit.Symbol}";
}
