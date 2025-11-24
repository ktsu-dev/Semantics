// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an angular velocity quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record AngularVelocity<T> : PhysicalQuantity<AngularVelocity<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of angularvelocity [T⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.AngularVelocity;

	/// <summary>
	/// Initializes a new instance of the <see cref="AngularVelocity{T}"/> class.
	/// </summary>
	public AngularVelocity() : base() { }

	/// <summary>
	/// Creates a new AngularVelocity from a value in radians per second.
	/// </summary>
	/// <param name="radiansPerSecond">The value in radians per second.</param>
	/// <returns>A new AngularVelocity instance.</returns>
	public static AngularVelocity<T> FromRadiansPerSecond(T radiansPerSecond) => Create(radiansPerSecond);

	/// <summary>
	/// Divides angular velocity by time to create angular acceleration.
	/// </summary>
	/// <param name="angularVelocity">The angular velocity.</param>
	/// <param name="time">The time.</param>
	/// <returns>The resulting angular acceleration.</returns>
	public static AngularAcceleration<T> Divide(AngularVelocity<T> angularVelocity, Time<T> time)
	{
		Guard.NotNull(angularVelocity);
		Guard.NotNull(time);
		return AngularAcceleration<T>.Create(angularVelocity.Value / time.Value);
	}
}
