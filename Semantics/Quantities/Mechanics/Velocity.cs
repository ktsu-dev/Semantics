// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a velocity quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Velocity<T> : PhysicalQuantity<Velocity<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Velocity;

	/// <summary>
	/// Initializes a new instance of the Velocity class.
	/// </summary>
	public Velocity() : base() { }

	/// <summary>
	/// Creates a new Velocity from a value in meters per second.
	/// </summary>
	/// <param name="metersPerSecond">The value in meters per second.</param>
	/// <returns>A new Velocity instance.</returns>
	public static Velocity<T> FromMetersPerSecond(T metersPerSecond) => Create(metersPerSecond);

	/// <summary>
	/// Divides this velocity by time to create an acceleration.
	/// </summary>
	/// <param name="left">The velocity.</param>
	/// <param name="right">The time.</param>
	/// <returns>The resulting acceleration.</returns>
	public static Acceleration<T> operator /(Velocity<T> left, Time<T> right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Acceleration<T>.Create(left.Value / right.Value);
	}

	/// <summary>
	/// Divides this velocity by time to create an acceleration.
	/// </summary>
	/// <param name="left">The velocity.</param>
	/// <param name="right">The time.</param>
	/// <returns>The resulting acceleration.</returns>
	public static Acceleration<T> Divide(Velocity<T> left, Time<T> right) => left / right;
}
