// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a momentum quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Momentum<T> : PhysicalQuantity<Momentum<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Momentum;

	/// <summary>
	/// Initializes a new instance of the Momentum class.
	/// </summary>
	public Momentum() : base() { }

	/// <summary>
	/// Creates a new Momentum from a value in kilogram-meters per second.
	/// </summary>
	/// <param name="kilogramMetersPerSecond">The value in kilogram-meters per second.</param>
	/// <returns>A new Momentum instance.</returns>
	public static Momentum<T> FromKilogramMetersPerSecond(T kilogramMetersPerSecond) => Create(kilogramMetersPerSecond);

	/// <summary>
	/// Multiplies mass by velocity to create momentum.
	/// </summary>
	/// <param name="mass">The mass.</param>
	/// <param name="velocity">The velocity.</param>
	/// <returns>The resulting momentum.</returns>
	public static Momentum<T> Multiply(Mass<T> mass, Velocity<T> velocity)
	{
		ArgumentNullException.ThrowIfNull(mass);
		ArgumentNullException.ThrowIfNull(velocity);
		return Create(mass.Value * velocity.Value);
	}

	/// <summary>
	/// Divides momentum by mass to get velocity.
	/// </summary>
	/// <param name="momentum">The momentum.</param>
	/// <param name="mass">The mass.</param>
	/// <returns>The resulting velocity.</returns>
	public static Velocity<T> Divide(Momentum<T> momentum, Mass<T> mass)
	{
		ArgumentNullException.ThrowIfNull(momentum);
		ArgumentNullException.ThrowIfNull(mass);
		return Velocity<T>.Create(momentum.Value / mass.Value);
	}

	/// <summary>
	/// Divides momentum by velocity to get mass.
	/// </summary>
	/// <param name="momentum">The momentum.</param>
	/// <param name="velocity">The velocity.</param>
	/// <returns>The resulting mass.</returns>
	public static Mass<T> Divide(Momentum<T> momentum, Velocity<T> velocity)
	{
		ArgumentNullException.ThrowIfNull(momentum);
		ArgumentNullException.ThrowIfNull(velocity);
		return Mass<T>.Create(momentum.Value / velocity.Value);
	}
}
