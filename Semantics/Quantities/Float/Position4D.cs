// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

using System.Numerics;

/// <summary>
/// Represents a 4D spacetime position vector with float precision (x, y, z, ct).
/// The fourth component represents time multiplied by the speed of light for dimensional consistency.
/// </summary>
public sealed record Position4D : Generic.Position4D<float>
{

	/// <summary>Gets the 4D vector value stored in this quantity (x, y, z, ct).</summary>
	public Vector4 Value { get; init; } = Vector4.Zero;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public bool IsPhysicallyValid => !float.IsNaN(Value.X) && !float.IsNaN(Value.Y) && !float.IsNaN(Value.Z) && !float.IsNaN(Value.W) &&
									 float.IsFinite(Value.X) && float.IsFinite(Value.Y) && float.IsFinite(Value.Z) && float.IsFinite(Value.W);

	/// <summary>
	/// Initializes a new instance of the <see cref="Position4D"/> class.
	/// </summary>
	public Position4D() : base() { }

	/// <summary>
	/// Creates a new instance with the specified vector value.
	/// </summary>
	/// <param name="value">The 4D vector value (x, y, z, ct).</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Position4D Create(Vector4 value) => new() { Value = value };

	/// <summary>
	/// Creates a new instance with the specified X, Y, Z, and T components.
	/// </summary>
	/// <param name="x">The X spatial component in meters.</param>
	/// <param name="y">The Y spatial component in meters.</param>
	/// <param name="z">The Z spatial component in meters.</param>
	/// <param name="ct">The time component multiplied by speed of light in meters.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Position4D Create(float x, float y, float z, float ct) => Create(new Vector4(x, y, z, ct));

	/// <summary>
	/// Creates a new Position4D from spatial coordinates and time.
	/// </summary>
	/// <param name="x">The X coordinate in meters.</param>
	/// <param name="y">The Y coordinate in meters.</param>
	/// <param name="z">The Z coordinate in meters.</param>
	/// <param name="time">The time coordinate in seconds.</param>
	/// <returns>A new Position4D instance.</returns>
	public static Position4D FromSpacetime(float x, float y, float z, float time)
	{
		// Convert time to ct (time * speed of light) for dimensional consistency
		const float c = 299792458.0f; // Speed of light in m/s
		return Create(x, y, z, time * c);
	}

	/// <summary>
	/// Creates a new Position4D from a 3D position and time.
	/// </summary>
	/// <param name="position3D">The 3D spatial position.</param>
	/// <param name="time">The time coordinate in seconds.</param>
	/// <returns>A new Position4D instance.</returns>
	public static Position4D FromSpacetime(Position3D position3D, float time)
	{
		ArgumentNullException.ThrowIfNull(position3D);
		const float c = 299792458.0f; // Speed of light in m/s
		return Create(position3D.X, position3D.Y, position3D.Z, time * c);
	}

	/// <summary>Gets the X spatial component of this vector quantity.</summary>
	public float X => Value.X;

	/// <summary>Gets the Y spatial component of this vector quantity.</summary>
	public float Y => Value.Y;

	/// <summary>Gets the Z spatial component of this vector quantity.</summary>
	public float Z => Value.Z;

	/// <summary>Gets the time component (ct) of this vector quantity.</summary>
	public float CT => Value.W;

	/// <summary>Gets the time component in seconds.</summary>
	public float TimeInSeconds
	{
		get
		{
			const float c = 299792458.0f; // Speed of light in m/s
			return CT / c;
		}
	}

	/// <summary>Gets the 3D spatial part of this 4D position.</summary>
	public Position3D SpatialPart => Position3D.FromMeters(X, Y, Z);

	/// <summary>Gets the proper time interval (spacetime interval) magnitude.</summary>
	public float ProperTimeInterval
	{
		get
		{
			// For a spacetime interval: s² = c²t² - x² - y² - z²
			// Here we use the Minkowski metric signature (+, -, -, -)
			float spatialSquared = X * X + Y * Y + Z * Z;
			float timeSquared = CT * CT;
			float intervalSquared = timeSquared - spatialSquared;
			
			// Return the square root if positive (timelike), otherwise return 0
			return intervalSquared >= 0 ? MathF.Sqrt(intervalSquared) : 0.0f;
		}
	}

	/// <summary>Gets whether this spacetime interval is timelike (s^2 > 0).</summary>
	public bool IsTimelike
	{
		get
		{
			float spatialSquared = X * X + Y * Y + Z * Z;
			float timeSquared = CT * CT;
			return timeSquared > spatialSquared;
		}
	}

	/// <summary>Gets whether this spacetime interval is spacelike (s^2 &lt; 0).</summary>
	public bool IsSpacelike
	{
		get
		{
			float spatialSquared = X * X + Y * Y + Z * Z;
			float timeSquared = CT * CT;
			return timeSquared < spatialSquared;
		}
	}

	/// <summary>Gets whether this spacetime interval is lightlike (s^2 = 0).</summary>	
	public bool IsLightlike
	{
		get
		{
			float spatialSquared = X * X + Y * Y + Z * Z;
			float timeSquared = CT * CT;
			return MathF.Abs(timeSquared - spatialSquared) < 1e-6f;
		}
	}

	/// <summary>
	/// Calculates displacement from this position to another position.
	/// </summary>
	/// <param name="other">The target position.</param>
	/// <returns>The 4D displacement vector from this position to the other.</returns>
	public Displacement4D DisplacementTo(Position4D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Displacement4D.Create(other.Value - Value);
	}

	/// <summary>
	/// Moves this position by the specified displacement.
	/// </summary>
	/// <param name="displacement">The displacement to apply.</param>
	/// <returns>A new position after applying the displacement.</returns>
	public Position4D Move(Displacement4D displacement)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(Value + displacement.Value);
	}

	// Vector arithmetic operations
	/// <summary>Adds a displacement to a position.</summary>
	public static Position4D operator +(Position4D position, Displacement4D displacement)
	{
		ArgumentNullException.ThrowIfNull(position);
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(position.Value + displacement.Value);
	}

	/// <summary>Subtracts a displacement from a position.</summary>
	public static Position4D operator -(Position4D position, Displacement4D displacement)
	{
		ArgumentNullException.ThrowIfNull(position);
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(position.Value - displacement.Value);
	}

	/// <summary>Calculates displacement between two positions.</summary>
	public static Displacement4D operator -(Position4D position1, Position4D position2)
	{
		ArgumentNullException.ThrowIfNull(position1);
		ArgumentNullException.ThrowIfNull(position2);
		return Displacement4D.Create(position1.Value - position2.Value);
	}

	/// <summary>Gets the origin position (0, 0, 0, 0).</summary>
	public static Position4D Origin => Create(0, 0, 0, 0);

	/// <summary>
	/// Calculates the Minkowski dot product of two 4-positions.
	/// Uses the metric signature (+, -, -, -) where the result represents spacetime interval squared.
	/// </summary>
	/// <param name="other">The other position vector.</param>
	/// <returns>The Minkowski dot product.</returns>
	public float MinkowskiDot(Position4D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		// Minkowski metric: η = diag(+1, -1, -1, -1)
		return (CT * other.CT) - (X * other.X) - (Y * other.Y) - (Z * other.Z);
	}

	/// <summary>
	/// Calculates the spatial distance between two 4D positions (ignoring time).
	/// </summary>
	/// <param name="other">The other position.</param>
	/// <returns>The spatial distance as a length quantity.</returns>
	public Length SpatialDistance(Position4D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		float dx = X - other.X;
		float dy = Y - other.Y;
		float dz = Z - other.Z;
		return Length.FromMeters(MathF.Sqrt(dx * dx + dy * dy + dz * dz));
	}

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => $"({X:F3}, {Y:F3}, {Z:F3}, {CT:F3}) {Dimension.BaseUnit.Symbol}";
}