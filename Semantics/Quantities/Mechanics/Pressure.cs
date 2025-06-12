// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a pressure quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Pressure<T> : PhysicalQuantity<Pressure<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Pressure;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Pascal;

	/// <summary>
	/// Initializes a new instance of the Pressure class.
	/// </summary>
	public Pressure() : base() { }

	/// <summary>
	/// Creates a new Pressure from a value in pascals.
	/// </summary>
	/// <param name="pascals">The value in pascals.</param>
	/// <returns>A new Pressure instance.</returns>
	public static Pressure<T> FromPascals(T pascals) => Create(pascals);
}
