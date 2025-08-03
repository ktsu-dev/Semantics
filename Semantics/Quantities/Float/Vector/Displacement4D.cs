// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

using System.Numerics;

/// <summary>
/// Represents a 4D spacetime displacement vector with float precision (Δx, Δy, Δz, cΔt).
/// The fourth component represents time interval multiplied by the speed of light for dimensional consistency.
/// </summary>
public sealed record Displacement4D : Generic.Displacement4D<float>
{

	/// <summary>Gets the 4D vector value stored in this quantity (Δx, Δy, Δz, cΔt).</summary>
	public Vector4 Value { get; init; } = Vector4.Zero;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public bool IsPhysicallyValid => !float.IsNaN(Value.X) && !float.IsNaN(Value.Y) && !float.IsNaN(Value.Z) && !float.IsNaN(Value.W) &&
									 float.IsFinite(Value.X) && float.IsFinite(Value.Y) && float.IsFinite(Value.Z) && float.IsFinite(Value.W);

	/// <summary>
	/// Initializes a new instance of the <see cref="Displacement4D"/> class.
	/// </summary>
	public Displacement4D() : base() { }

	/// <summary>
	/// Creates a new instance with the specified vector value.
	/// </summary>
	/// <param name="value">The 4D vector value (Δx, Δy, Δz, cΔt).</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Displacement4D Create(Vector4 value) => new() { Value = value };

	/// <summary>
	/// Creates a new instance with the specified X, Y, Z, and T components.
	/// </summary>
	/// <param name="dx">The X spatial displacement in meters.</param>
	/// <param name="dy">The Y spatial displacement in meters.</param>
	/// <param name="dz">The Z spatial displacement in meters.</param>
	/// <param name="cdt">The time displacement multiplied by speed of light in meters.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Displacement4D Create(float dx, float dy, float dz, float cdt) => Create(new Vector4(dx, dy, dz, cdt));

	/// <summary>
	/// Creates a new Displacement4D from spatial displacement and time interval.
	/// </summary>
	/// <param name="dx">The X displacement in meters.</param>
	/// <param name="dy">The Y displacement in meters.</param>
	/// <param name="dz">The Z displacement in meters.</param>
	/// <param name="dt">The time interval in seconds.</param>
	/// <returns>A new Displacement4D instance.</returns>
	public static Displacement4D FromSpacetime(float dx, float dy, float dz, float dt)
	{
		// Convert time to cdt (time * speed of light) for dimensional consistency
		const float c = 299792458.0f; // Speed of light in m/s
		return Create(dx, dy, dz, dt * c);
	}

	/// <summary>
	/// Creates a new Displacement4D from a 3D displacement and time interval.
	/// </summary>
	/// <param name="displacement3D">The 3D spatial displacement.</param>
	/// <param name="dt">The time interval in seconds.</param>
	/// <returns>A new Displacement4D instance.</returns>
	public static Displacement4D FromSpacetime(Displacement3D displacement3D, float dt)
	{
		ArgumentNullException.ThrowIfNull(displacement3D);
		const float c = 299792458.0f; // Speed of light in m/s
		return Create(displacement3D.X, displacement3D.Y, displacement3D.Z, dt * c);
	}

	/// <summary>Gets the X spatial component of this vector quantity.</summary>
	public float X => Value.X;

	/// <summary>Gets the Y spatial component of this vector quantity.</summary>
	public float Y => Value.Y;

	/// <summary>Gets the Z spatial component of this vector quantity.</summary>
	public float Z => Value.Z;

	/// <summary>Gets the time component (cΔt) of this vector quantity.</summary>
	public float CDT => Value.W;

	/// <summary>Gets the time interval in seconds.</summary>
	public float TimeIntervalInSeconds
	{
		get
		{
			const float c = 299792458.0f; // Speed of light in m/s
			return CDT / c;
		}
	}

	/// <summary>Gets the 3D spatial part of this 4D displacement.</summary>
	public Displacement3D SpatialPart => Displacement3D.FromMeters(X, Y, Z);

	/// <summary>Gets the proper time interval magnitude (invariant interval).</summary>
	public float ProperTimeInterval
	{
		get
		{
			// For a spacetime interval: s² = c²Δt² - Δx² - Δy² - Δz²
			// Here we use the Minkowski metric signature (+, -, -, -)
			float spatialSquared = (X * X) + (Y * Y) + (Z * Z);
			float timeSquared = CDT * CDT;
			float intervalSquared = timeSquared - spatialSquared;

			// Return the square root if positive (timelike), otherwise return 0
			return intervalSquared >= 0 ? MathF.Sqrt(intervalSquared) : 0.0f;
		}
	}

	/// <summary>Gets the spatial magnitude of this displacement (ignoring time).</summary>
	public float SpatialMagnitude => MathF.Sqrt((X * X) + (Y * Y) + (Z * Z));

	/// <summary>Gets whether this spacetime interval is timelike (s^2 > 0).</summary>
	public bool IsTimelike
	{
		get
		{
			float spatialSquared = (X * X) + (Y * Y) + (Z * Z);
			float timeSquared = CDT * CDT;
			return timeSquared > spatialSquared;
		}
	}

	/// <summary>Gets whether this spacetime interval is spacelike (s^2 &lt; 0).</summary>
	public bool IsSpacelike
	{
		get
		{
			float spatialSquared = (X * X) + (Y * Y) + (Z * Z);
			float timeSquared = CDT * CDT;
			return timeSquared < spatialSquared;
		}
	}

	/// <summary>Gets whether this spacetime interval is lightlike (s^2 = 0).</summary>	
	public bool IsLightlike
	{
		get
		{
			float spatialSquared = (X * X) + (Y * Y) + (Z * Z);
			float timeSquared = CDT * CDT;
			return MathF.Abs(timeSquared - spatialSquared) < 1e-6f;
		}
	}

	/// <summary>
	/// Gets the unit vector (normalized) form of this quantity.
	/// For 4D spacetime, this normalizes using the Minkowski metric.
	/// </summary>
	/// <returns>A new instance representing the unit vector, or zero if magnitude is zero.</returns>
	public Displacement4D Unit()
	{
		float magnitude = ProperTimeInterval;
		return magnitude > 0 ? Create(Value / magnitude) : Create(Vector4.Zero);
	}

	// Vector arithmetic operations
	/// <summary>Adds two displacements.</summary>
	public static Displacement4D operator +(Displacement4D left, Displacement4D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value + right.Value);
	}

	/// <summary>Subtracts two displacements.</summary>
	public static Displacement4D operator -(Displacement4D left, Displacement4D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value - right.Value);
	}

	/// <summary>Negates a displacement.</summary>
	public static Displacement4D operator -(Displacement4D displacement)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(-displacement.Value);
	}

	/// <summary>Scales a displacement by a scalar.</summary>
	public static Displacement4D operator *(Displacement4D displacement, float scalar)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(displacement.Value * scalar);
	}

	/// <summary>Scales a displacement by a scalar.</summary>
	public static Displacement4D operator *(float scalar, Displacement4D displacement)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(scalar * displacement.Value);
	}

	/// <summary>Divides a displacement by a scalar.</summary>
	public static Displacement4D operator /(Displacement4D displacement, float scalar)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(displacement.Value / scalar);
	}

	/// <summary>Gets the zero displacement (0, 0, 0, 0).</summary>
	public static Displacement4D Zero => Create(0, 0, 0, 0);

	/// <summary>
	/// Calculates the Minkowski dot product of two 4D displacements.
	/// Uses the metric signature (+, -, -, -) where the result represents spacetime interval squared.
	/// </summary>
	/// <param name="other">The other displacement vector.</param>
	/// <returns>The Minkowski dot product.</returns>
	public float MinkowskiDot(Displacement4D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		// Minkowski metric: η = diag(+1, -1, -1, -1)
		return (CDT * other.CDT) - (X * other.X) - (Y * other.Y) - (Z * other.Z);
	}

	/// <summary>
	/// Calculates the spatial distance between two displacement vectors (ignoring time).
	/// </summary>
	/// <param name="other">The other displacement vector.</param>
	/// <returns>The spatial distance as a length quantity.</returns>
	public Length SpatialDistance(Displacement4D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		float dx = X - other.X;
		float dy = Y - other.Y;
		float dz = Z - other.Z;
		return Length.FromMeters(MathF.Sqrt((dx * dx) + (dy * dy) + (dz * dz)));
	}

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => $"Δ({X:F3}, {Y:F3}, {Z:F3}, {CDT:F3}) {Dimension.BaseUnit.Symbol}";
}
