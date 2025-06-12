// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an electric current quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record ElectricCurrent<T> : PhysicalQuantity<ElectricCurrent<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricCurrent;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Ampere;

	/// <summary>
	/// Initializes a new instance of the ElectricCurrent class.
	/// </summary>
	public ElectricCurrent() : base() { }

	/// <summary>
	/// Creates a new ElectricCurrent from a value in amperes.
	/// </summary>
	/// <param name="amperes">The value in amperes.</param>
	/// <returns>A new ElectricCurrent instance.</returns>
	public static ElectricCurrent<T> FromAmperes(T amperes) => Create(amperes);
}
