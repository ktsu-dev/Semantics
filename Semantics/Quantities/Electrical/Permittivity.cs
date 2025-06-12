// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a permittivity quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Permittivity<T> : PhysicalQuantity<Permittivity<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Permittivity;

	/// <summary>
	/// Initializes a new instance of the Permittivity class.
	/// </summary>
	public Permittivity() : base() { }

	/// <summary>
	/// Creates a new Permittivity from a value in farads per meter.
	/// </summary>
	/// <param name="faradsPerMeter">The value in farads per meter.</param>
	/// <returns>A new Permittivity instance.</returns>
	public static Permittivity<T> FromFaradsPerMeter(T faradsPerMeter) => Create(faradsPerMeter);
}
