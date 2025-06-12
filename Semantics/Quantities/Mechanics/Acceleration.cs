// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an acceleration quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Acceleration<T> : PhysicalQuantity<Acceleration<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Acceleration;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.MetersPerSecondSquared;

	/// <summary>
	/// Initializes a new instance of the Acceleration class.
	/// </summary>
	public Acceleration() : base() { }

	/// <summary>
	/// Creates a new Acceleration from a value in meters per second squared.
	/// </summary>
	/// <param name="metersPerSecondSquared">The value in meters per second squared.</param>
	/// <returns>A new Acceleration instance.</returns>
	public static Acceleration<T> FromMetersPerSecondSquared(T metersPerSecondSquared) => Create(metersPerSecondSquared);
}
