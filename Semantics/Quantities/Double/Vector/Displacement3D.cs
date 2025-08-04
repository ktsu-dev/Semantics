// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Double;
/// <summary>
/// Represents a 3D displacement vector with double precision.
/// </summary>
public sealed record Displacement3D : Generic.Displacement3D<double>
{

	/// <summary>Gets the 3D vector value stored in this quantity.</summary>
	public Vector3d Value { get; init; } = Vector3d.Zero;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public bool IsPhysicallyValid => !double.IsNaN(Value.X) && !double.IsNaN(Value.Y) && !double.IsNaN(Value.Z) &&
									 double.IsFinite(Value.X) && double.IsFinite(Value.Y) && double.IsFinite(Value.Z);

	/// <summary>
	/// Initializes a new instance of the <see cref="Displacement3D"/> class.
	/// </summary>
	public Displacement3D() : base() { }

	/// <summary>
	/// Creates a new instance with the specified vector value.
	/// </summary>
	/// <param name="value">The vector value for the quantity.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Displacement3D Create(Vector3d value) => new() { Value = value };

	/// <summary>
	/// Creates a new instance with the specified X, Y, and Z components.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <param name="z">The Z component.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Displacement3D Create(double x, double y, double z) => Create(new Vector3d(x, y, z)); /// <summary>
																										/// Gets the displacement as a Vector3d in meters (the base unit).
																										/// </summary>
																										/// <returns>The displacement vector in meters.</returns>
	public Vector3d InMeters() => Value;

	// Vector arithmetic operations
	/// <summary>Adds two displacements.</summary>
	public static Displacement3D operator +(Displacement3D left, Displacement3D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value + right.Value);
	}

	/// <summary>Subtracts two displacements.</summary>
	public static Displacement3D operator -(Displacement3D left, Displacement3D right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Create(left.Value - right.Value);
	}

	/// <summary>Negates a displacement.</summary>
	public static Displacement3D operator -(Displacement3D displacement)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(-displacement.Value);
	}

	/// <summary>Scales a displacement by a scalar.</summary>
	public static Displacement3D operator *(Displacement3D displacement, double scalar)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(displacement.Value * scalar);
	}

	/// <summary>Scales a displacement by a scalar.</summary>
	public static Displacement3D operator *(double scalar, Displacement3D displacement)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(scalar * displacement.Value);
	}

	/// <summary>Divides a displacement by a scalar.</summary>
	public static Displacement3D operator /(Displacement3D displacement, double scalar)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(displacement.Value / scalar);
	}

	/// <summary>Gets the zero displacement (0, 0, 0).</summary>
	public static Displacement3D Zero => Create(0, 0, 0);

	/// <summary>
	/// Calculates the dot product of two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The dot product as an area quantity.</returns>
	public Area Dot(Displacement3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Area.FromSquareMeters(Value.Dot(other.Value));
	}

	/// <summary>
	/// Calculates the cross product of two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The cross product.</returns>
	public Displacement3D Cross(Displacement3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Create(Value.Cross(other.Value));
	}

	/// <summary>
	/// Calculates the distance between two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The distance as a length quantity.</returns>
	public Length Distance(Displacement3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Length.FromMeters(Value.Distance(other.Value));
	}

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => $"{Value} {Dimension.BaseUnit.Symbol}";
}
