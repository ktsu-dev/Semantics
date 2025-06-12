// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an electric charge quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record ElectricCharge<T> : PhysicalQuantity<ElectricCharge<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricCharge;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Coulomb;

	/// <summary>
	/// Initializes a new instance of the ElectricCharge class.
	/// </summary>
	public ElectricCharge() : base() { }

	/// <summary>
	/// Creates a new ElectricCharge from a value in coulombs.
	/// </summary>
	/// <param name="coulombs">The value in coulombs.</param>
	/// <returns>A new ElectricCharge instance.</returns>
	public static ElectricCharge<T> FromCoulombs(T coulombs) => Create(coulombs);
}
