// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a mass quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Mass<T> : PhysicalQuantity<Mass<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Mass;

	/// <summary>
	/// Initializes a new instance of the Mass class.
	/// </summary>
	public Mass() : base() { }

	/// <summary>
	/// Creates a new Mass from a value in kilograms.
	/// </summary>
	/// <param name="kilograms">The value in kilograms.</param>
	/// <returns>A new Mass instance.</returns>
	public static Mass<T> FromKilograms(T kilograms) => Create(kilograms);
}
