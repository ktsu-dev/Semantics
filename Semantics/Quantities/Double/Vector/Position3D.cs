// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Double;
/// <summary>
/// Represents a 3D position vector with double precision.
/// </summary>
public sealed record Position3D : Generic.Position3D<double>
{

	/// <summary>Gets the 3D vector value stored in this quantity.</summary>
	public Vector3d Value { get; init; } = Vector3d.Zero;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public bool IsPhysicallyValid => !double.IsNaN(Value.X) && !double.IsNaN(Value.Y) && !double.IsNaN(Value.Z) &&
									 double.IsFinite(Value.X) && double.IsFinite(Value.Y) && double.IsFinite(Value.Z);

	/// <summary>
	/// Initializes a new instance of the <see cref="Position3D"/> class.
	/// </summary>
	public Position3D() : base() { }

	/// <summary>
	/// Creates a new instance with the specified vector value.
	/// </summary>
	/// <param name="value">The vector value for the quantity.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Position3D Create(Vector3d value) => new() { Value = value };

	/// <summary>
	/// Creates a new instance with the specified X, Y, and Z components.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <param name="z">The Z component.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static Position3D Create(double x, double y, double z) => Create(new Vector3d(x, y, z)); /// <summary>
																									/// Gets the position as a Vector3d in meters (the base unit).
																									/// </summary>
																									/// <returns>The position vector in meters.</returns>
	public Vector3d InMeters() => Value;

	/// <summary>
	/// Calculates displacement from this position to another position.
	/// </summary>
	/// <param name="other">The target position.</param>
	/// <returns>The displacement vector from this position to the other.</returns>
	public Displacement3D DisplacementTo(Position3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Displacement3D.Create(other.Value - Value);
	}

	/// <summary>
	/// Moves this position by the specified displacement.
	/// </summary>
	/// <param name="displacement">The displacement to apply.</param>
	/// <returns>A new position after applying the displacement.</returns>
	public Position3D Move(Displacement3D displacement)
	{
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(Value + displacement.Value);
	}

	// Vector arithmetic operations
	/// <summary>Adds a displacement to a position.</summary>
	public static Position3D operator +(Position3D position, Displacement3D displacement)
	{
		ArgumentNullException.ThrowIfNull(position);
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(position.Value + displacement.Value);
	}

	/// <summary>Subtracts a displacement from a position.</summary>
	public static Position3D operator -(Position3D position, Displacement3D displacement)
	{
		ArgumentNullException.ThrowIfNull(position);
		ArgumentNullException.ThrowIfNull(displacement);
		return Create(position.Value - displacement.Value);
	}

	/// <summary>Calculates displacement between two positions.</summary>
	public static Displacement3D operator -(Position3D position1, Position3D position2)
	{
		ArgumentNullException.ThrowIfNull(position1);
		ArgumentNullException.ThrowIfNull(position2);
		return Displacement3D.Create(position1.Value - position2.Value);
	}

	/// <summary>Gets the origin position (0, 0, 0).</summary>
	public static Position3D Origin => Create(0, 0, 0);

	/// <summary>
	/// Calculates the dot product of two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The dot product as an area quantity.</returns>
	public Area Dot(Position3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Area.FromSquareMeters(Value.Dot(other.Value));
	}

	/// <summary>
	/// Calculates the cross product of two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The cross product.</returns>
	public Position3D Cross(Position3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Create(Value.Cross(other.Value));
	}

	/// <summary>
	/// Calculates the distance between two vector quantities.
	/// </summary>
	/// <param name="other">The other vector quantity.</param>
	/// <returns>The distance as a length quantity.</returns>
	public Length Distance(Position3D other)
	{
		ArgumentNullException.ThrowIfNull(other);
		return Length.FromMeters(Value.Distance(other.Value));
	}

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => $"{Value} {Dimension.BaseUnit.Symbol}";
}
