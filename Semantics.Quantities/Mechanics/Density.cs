// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a density quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Density<T> : PhysicalQuantity<Density<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of density [M L⁻³].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Density;

	/// <summary>
	/// Initializes a new instance of the <see cref="Density{T}"/> class.
	/// </summary>
	public Density() : base() { }

	/// <summary>
	/// Creates a new Density from a value in kilograms per cubic meter.
	/// </summary>
	/// <param name="kilogramsPerCubicMeter">The value in kilograms per cubic meter.</param>
	/// <returns>A new Density instance.</returns>
	public static Density<T> FromKilogramsPerCubicMeter(T kilogramsPerCubicMeter) => Create(kilogramsPerCubicMeter);

	/// <summary>
	/// Divides mass by volume to create density.
	/// </summary>
	/// <param name="mass">The mass.</param>
	/// <param name="volume">The volume.</param>
	/// <returns>The resulting density.</returns>
	public static Density<T> Divide(Mass<T> mass, Volume<T> volume)
	{
		ArgumentNullException.ThrowIfNull(mass);
		ArgumentNullException.ThrowIfNull(volume);
		return Create(mass.Value / volume.Value);
	}

	/// <summary>
	/// Multiplies density by volume to get mass.
	/// </summary>
	/// <param name="density">The density.</param>
	/// <param name="volume">The volume.</param>
	/// <returns>The resulting mass.</returns>
	public static Mass<T> Multiply(Density<T> density, Volume<T> volume)
	{
		ArgumentNullException.ThrowIfNull(density);
		ArgumentNullException.ThrowIfNull(volume);
		return Mass<T>.Create(density.Value * volume.Value);
	}

	/// <summary>
	/// Divides mass by density to get volume.
	/// </summary>
	/// <param name="mass">The mass.</param>
	/// <param name="density">The density.</param>
	/// <returns>The resulting volume.</returns>
	public static Volume<T> Divide(Mass<T> mass, Density<T> density)
	{
		ArgumentNullException.ThrowIfNull(mass);
		ArgumentNullException.ThrowIfNull(density);
		return Volume<T>.Create(mass.Value / density.Value);
	}
}
