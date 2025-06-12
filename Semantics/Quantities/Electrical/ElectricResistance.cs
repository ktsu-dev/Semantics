// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an electric resistance quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record ElectricResistance<T> : PhysicalQuantity<ElectricResistance<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricResistance;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Ohm;

	/// <summary>
	/// Initializes a new instance of the ElectricResistance class.
	/// </summary>
	public ElectricResistance() : base() { }

	/// <summary>
	/// Creates a new ElectricResistance from a value in ohms.
	/// </summary>
	/// <param name="ohms">The value in ohms.</param>
	/// <returns>A new ElectricResistance instance.</returns>
	public static ElectricResistance<T> FromOhms(T ohms) => Create(ohms);
}
