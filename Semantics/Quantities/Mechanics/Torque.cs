// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a torque quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Torque<T> : PhysicalQuantity<Torque<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Torque;

	/// <summary>
	/// Initializes a new instance of the Torque class.
	/// </summary>
	public Torque() : base() { }

	/// <summary>
	/// Creates a new Torque from a value in newton-meters.
	/// </summary>
	/// <param name="newtonMeters">The value in newton-meters.</param>
	/// <returns>A new Torque instance.</returns>
	public static Torque<T> FromNewtonMeters(T newtonMeters) => Create(newtonMeters);

	/// <summary>
	/// Multiplies force by length to create torque.
	/// </summary>
	/// <param name="force">The force.</param>
	/// <param name="length">The length (lever arm).</param>
	/// <returns>The resulting torque.</returns>
	public static Torque<T> Multiply(Force<T> force, Length<T> length)
	{
		ArgumentNullException.ThrowIfNull(force);
		ArgumentNullException.ThrowIfNull(length);
		return Create(force.Value * length.Value);
	}

	/// <summary>
	/// Divides torque by force to get length.
	/// </summary>
	/// <param name="torque">The torque.</param>
	/// <param name="force">The force.</param>
	/// <returns>The resulting length.</returns>
	public static Length<T> Divide(Torque<T> torque, Force<T> force)
	{
		ArgumentNullException.ThrowIfNull(torque);
		ArgumentNullException.ThrowIfNull(force);
		return Length<T>.Create(torque.Value / force.Value);
	}

	/// <summary>
	/// Divides torque by length to get force.
	/// </summary>
	/// <param name="torque">The torque.</param>
	/// <param name="length">The length.</param>
	/// <returns>The resulting force.</returns>
	public static Force<T> Divide(Torque<T> torque, Length<T> length)
	{
		ArgumentNullException.ThrowIfNull(torque);
		ArgumentNullException.ThrowIfNull(length);
		return Force<T>.Create(torque.Value / length.Value);
	}
}
