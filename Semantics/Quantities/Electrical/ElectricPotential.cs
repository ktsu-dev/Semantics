// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an electric potential quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record ElectricPotential<T> : PhysicalQuantity<ElectricPotential<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricPotential;

	/// <summary>
	/// Initializes a new instance of the ElectricPotential class.
	/// </summary>
	public ElectricPotential() : base() { }

	/// <summary>
	/// Creates a new ElectricPotential from a value in volts.
	/// </summary>
	/// <param name="volts">The value in volts.</param>
	/// <returns>A new ElectricPotential instance.</returns>
	public static ElectricPotential<T> FromVolts(T volts) => Create(volts);
}
