// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a velocity quantity with compile-time dimensional safety.
/// </summary>
public sealed record Velocity : PhysicalQuantity<Velocity>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Velocity;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.MetersPerSecond;

	/// <summary>
	/// Initializes a new instance of the Velocity class.
	/// </summary>
	public Velocity() : base() { }

	/// <summary>
	/// Creates a new Velocity from a value in meters per second.
	/// </summary>
	/// <param name="metersPerSecond">The value in meters per second.</param>
	/// <returns>A new Velocity instance.</returns>
	public static Velocity FromMetersPerSecond(double metersPerSecond) => Create(metersPerSecond);

	/// <summary>
	/// Gets the velocity value in meters per second.
	/// </summary>
	/// <returns>The value in meters per second.</returns>
	public double MetersPerSecond() => Value;

	/// <summary>
	/// Gets the velocity value in the specified unit.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in meters per second.</returns>
	public T MetersPerSecond<T>() where T : struct, INumber<T> => T.CreateChecked(Value);

	/// <summary>
	/// Gets the velocity value in kilometers per hour.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in kilometers per hour.</returns>
	public T KilometersPerHour<T>() where T : struct, INumber<T> => T.CreateChecked(Value * 3.6);

	/// <summary>
	/// Gets the velocity value in miles per hour.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in miles per hour.</returns>
	public T MilesPerHour<T>() where T : struct, INumber<T> => T.CreateChecked(Value * 2.237);

	/// <summary>
	/// Gets the velocity value in knots.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in knots.</returns>
	public T Knots<T>() where T : struct, INumber<T> => T.CreateChecked(Value * 1.944);
}

/// <summary>
/// Generic velocity quantity with configurable storage type.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Velocity<T> : PhysicalQuantity<Velocity<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Velocity;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.MetersPerSecond;

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
}
