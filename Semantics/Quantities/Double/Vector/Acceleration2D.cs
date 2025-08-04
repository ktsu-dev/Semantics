// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;
/// <summary>
/// Represents a 2D acceleration vector with double precision.
/// </summary>
public sealed record Acceleration2D : Generic.Acceleration2D<double>
{

	/// <summary>Gets the 2D vector value stored in this quantity.</summary>
	public Vector2d Value { get; init; } = Vector2d.Zero;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public bool IsPhysicallyValid => !double.IsNaN(Value.X) && !double.IsNaN(Value.Y) &&
									 double.IsFinite(Value.X) && double.IsFinite(Value.Y);

	/// <summary>
	/// Initializes a new instance of the <see cref="Acceleration2D"/> class.
	/// </summary>
	public Acceleration2D() : base() { }

	/// <summary>
	/// Creates a new instance with the specified vector value.
	/// </summary>
	/// <param name="value">The vector value for the quantity.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Acceleration2D Create(Vector2d value) => new() { Value = value };

	/// <summary>
	/// Creates a new instance with the specified X and Y components.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Acceleration2D Create(double x, double y) => Create(new Vector2d(x, y));  /// <summary>
																							/// Gets the acceleration as a Vector2d in meters per second squared (the base unit).
																							/// </summary>
																							/// <returns>The acceleration vector in meters per second squared.</returns>
	public Vector2d InMetersPerSecondSquared() => Value;

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
	public static Acceleration2D operator *(Acceleration2D acceleration, double scalar)
	{
		ArgumentNullException.ThrowIfNull(acceleration);
		return Create(acceleration.Value * scalar);
	}

	/// <summary>Multiplies a scalar by an acceleration.</summary>
	public static Acceleration2D operator *(double scalar, Acceleration2D acceleration)
	{
		ArgumentNullException.ThrowIfNull(acceleration);
		return Create(scalar * acceleration.Value);
	}

	/// <summary>Divides an acceleration by a scalar.</summary>
	public static Acceleration2D operator /(Acceleration2D acceleration, double scalar)
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
