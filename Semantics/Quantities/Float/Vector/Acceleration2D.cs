// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

using System.Numerics;

/// <summary>
/// Represents a 2D acceleration vector with float precision.
/// </summary>
public sealed record Acceleration2D : Generic.Acceleration2D<float>
{
	/// <summary>Gets the 2D vector value stored in this quantity.</summary>
	public Vector2f Value { get; init; } = Vector2f.Zero;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public bool IsPhysicallyValid => !float.IsNaN(Value.X) && !float.IsNaN(Value.Y) &&
									 float.IsFinite(Value.X) && float.IsFinite(Value.Y);

	/// <summary>
	/// Initializes a new instance of the <see cref="Acceleration2D"/> class.
	/// </summary>
	public Acceleration2D() : base() { }

	/// <summary>
	/// Creates a new instance with the specified vector value.
	/// </summary>
	/// <param name="value">The vector value for the quantity.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Acceleration2D Create(Vector2f value) => new() { Value = value };

	/// <summary>
	/// Creates a new instance with the specified X and Y components.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Acceleration2D Create(float x, float y) => Create(new Vector2f(x, y));

	/// <summary>
	/// Creates a new Acceleration2D from X and Y components in meters per second squared.
	/// </summary>
	/// <param name="x">The X component in meters per second squared.</param>
	/// <param name="y">The Y component in meters per second squared.</param>
	/// <returns>A new Acceleration2D instance.</returns>
	public static Acceleration2D FromMetersPerSecondSquared(float x, float y) => Create(x, y);

	/// <summary>
	/// Creates a new Acceleration2D from a Vector2 in meters per second squared.
	/// </summary>
	/// <param name="metersPerSecondSquared">The acceleration vector in meters per second squared.</param>
	/// <returns>A new Acceleration2D instance.</returns>
	public static Acceleration2D FromMetersPerSecondSquared(Vector2 metersPerSecondSquared) => Create(new Vector2f(metersPerSecondSquared));

	/// <summary>Gets the magnitude of this acceleration vector.</summary>
	public float Magnitude => Value.Length();

	/// <summary>Gets the X component of this vector quantity.</summary>
	public float X => Value.X;

	/// <summary>Gets the Y component of this vector quantity.</summary>
	public float Y => Value.Y;

	/// <summary>
	/// Gets the unit vector (normalized) form of this quantity.
	/// </summary>
	/// <returns>A new instance representing the unit vector, or zero if magnitude is zero.</returns>
	public Acceleration2D Unit()
	{
		float magnitude = Magnitude;
		return magnitude > 0 ? Create(Value.Normalize()) : Create(Vector2f.Zero);
	}

	/// <summary>
	/// Gets the acceleration as a Vector2f in meters per second squared (the base unit).
	/// </summary>
	/// <returns>The acceleration vector in meters per second squared.</returns>
	public Vector2f InMetersPerSecondSquared() => Value;

	// Vector arithmetic operations
	/// <summary>Adds two accelerations.</summary>
	public static Acceleration2D operator +(Acceleration2D left, Acceleration2D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value + right.Value);
	}

	/// <summary>Subtracts two accelerations.</summary>
	public static Acceleration2D operator -(Acceleration2D left, Acceleration2D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value - right.Value);
	}

	/// <summary>Multiplies an acceleration by a scalar.</summary>
	public static Acceleration2D operator *(Acceleration2D acceleration, float scalar)
	{
		ArgumentNullException.ThrowIfNull(acceleration);
		return Create(acceleration.Value * scalar);
	}

	/// <summary>Multiplies a scalar by an acceleration.</summary>
	public static Acceleration2D operator *(float scalar, Acceleration2D acceleration)
	{
		ArgumentNullException.ThrowIfNull(acceleration);
		return Create(scalar * acceleration.Value);
	}

	/// <summary>Divides an acceleration by a scalar.</summary>
	public static Acceleration2D operator /(Acceleration2D acceleration, float scalar)
	{
		ArgumentNullException.ThrowIfNull(acceleration);
		return Create(acceleration.Value / scalar);
	}

	/// <summary>Negates an acceleration.</summary>
	public static Acceleration2D operator -(Acceleration2D acceleration)
	{
		ArgumentNullException.ThrowIfNull(acceleration);
		return Create(-acceleration.Value);
	}

	/// <summary>Gets the zero acceleration.</summary>
	public static Acceleration2D Zero => Create(0, 0);

	/// <summary>
	/// Calculates the dot product of two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The dot product as a power quantity.</returns>
	public Power Dot(Acceleration2D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Power.FromWatts(Value.Dot(other.Value));
	}

	/// <summary>
	/// Calculates the distance between two acceleration vectors in acceleration space.
	/// </summary>
	/// <param name="other">The other acceleration vector.</param>
	/// <returns>The distance in acceleration space as an acceleration quantity.</returns>
	public Acceleration AccelerationDistance(Acceleration2D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Acceleration.FromMetersPerSecondSquared(Value.Distance(other.Value));
	}

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => $"{Value} {Dimension.BaseUnit.Symbol}";
}
