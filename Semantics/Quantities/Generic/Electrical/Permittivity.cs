// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents a permittivity quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public record Permittivity<T> : PhysicalQuantity<Permittivity<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of permittivity [M⁻¹ L⁻³ T⁴ I²].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Permittivity;

	/// <summary>
	/// Initializes a new instance of the <see cref="Permittivity{T}"/> class.
	/// </summary>
	public Permittivity() : base() { }

	/// <summary>
	/// Creates a new Permittivity from a value in farads per meter.
	/// </summary>
	/// <param name="faradsPerMeter">The value in farads per meter.</param>
	/// <returns>A new Permittivity instance.</returns>
	public static Permittivity<T> FromFaradsPerMeter(T faradsPerMeter) => Create(faradsPerMeter);
}
