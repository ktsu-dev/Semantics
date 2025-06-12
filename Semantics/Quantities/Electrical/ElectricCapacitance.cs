// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an electric capacitance quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record ElectricCapacitance<T> : PhysicalQuantity<ElectricCapacitance<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricCapacitance;

	/// <summary>
	/// Initializes a new instance of the ElectricCapacitance class.
	/// </summary>
	public ElectricCapacitance() : base() { }

	/// <summary>
	/// Creates a new ElectricCapacitance from a value in farads.
	/// </summary>
	/// <param name="farads">The value in farads.</param>
	/// <returns>A new ElectricCapacitance instance.</returns>
	public static ElectricCapacitance<T> FromFarads(T farads) => Create(farads);
}
