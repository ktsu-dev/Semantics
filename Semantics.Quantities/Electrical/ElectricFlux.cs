// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an electric flux quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record ElectricFlux<T> : PhysicalQuantity<ElectricFlux<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of electricflux [M L³ T⁻³ I⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricFlux;

	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricFlux{T}"/> class.
	/// </summary>
	public ElectricFlux() : base() { }

	/// <summary>
	/// Creates a new ElectricFlux from a value in volt-meters.
	/// </summary>
	/// <param name="voltMeters">The value in volt-meters.</param>
	/// <returns>A new ElectricFlux instance.</returns>
	public static ElectricFlux<T> FromVoltMeters(T voltMeters) => Create(voltMeters);

	/// <summary>
	/// Multiplies electric field by area to create electric flux.
	/// </summary>
	/// <param name="field">The electric field.</param>
	/// <param name="area">The area.</param>
	/// <returns>The resulting electric flux.</returns>
	public static ElectricFlux<T> Multiply(ElectricField<T> field, Area<T> area)
	{
		Ensure.NotNull(field);
		Ensure.NotNull(area);
		return Create(field.Value * area.Value);
	}

	/// <summary>
	/// Divides electric flux by area to get electric field.
	/// </summary>
	/// <param name="flux">The electric flux.</param>
	/// <param name="area">The area.</param>
	/// <returns>The resulting electric field.</returns>
	public static ElectricField<T> Divide(ElectricFlux<T> flux, Area<T> area)
	{
		Ensure.NotNull(flux);
		Ensure.NotNull(area);
		return ElectricField<T>.Create(flux.Value / area.Value);
	}
}
