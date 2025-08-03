// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents bulk modulus with a specific unit of measurement.
/// Bulk modulus is a measure of a substance's resistance to uniform compression.
/// It is measured in pascals (Pa) in the SI system.
/// </summary>
/// <typeparam name="T">The numeric type for the bulk modulus value.</typeparam>
public record BulkModulus<T> : PhysicalQuantity<BulkModulus<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of bulk modulus [M L⁻¹ T⁻²].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.BulkModulus;

	/// <summary>Initializes a new instance of the <see cref="BulkModulus{T}"/> class.</summary>
	public BulkModulus() : base() { }

	/// <summary>Creates a new bulk modulus from a value in pascals.</summary>
	/// <param name="pascals">The bulk modulus in pascals.</param>
	/// <returns>A new BulkModulus instance.</returns>
	public static BulkModulus<T> FromPascals(T pascals) => Create(pascals);

	/// <summary>Creates a new bulk modulus from a value in gigapascals.</summary>
	/// <param name="gigapascals">The bulk modulus in gigapascals.</param>
	/// <returns>A new BulkModulus instance.</returns>
	public static BulkModulus<T> FromGigapascals(T gigapascals)
	{
		T pascals = gigapascals * T.CreateChecked(1e9);
		return Create(pascals);
	}

	/// <summary>Creates a new bulk modulus from a value in megapascals.</summary>
	/// <param name="megapascals">The bulk modulus in megapascals.</param>
	/// <returns>A new BulkModulus instance.</returns>
	public static BulkModulus<T> FromMegapascals(T megapascals)
	{
		T pascals = megapascals * T.CreateChecked(1e6);
		return Create(pascals);
	}

	/// <summary>Creates a new bulk modulus from a value in kilopascals.</summary>
	/// <param name="kilopascals">The bulk modulus in kilopascals.</param>
	/// <returns>A new BulkModulus instance.</returns>
	public static BulkModulus<T> FromKilopascals(T kilopascals)
	{
		T pascals = kilopascals * T.CreateChecked(1000);
		return Create(pascals);
	}

	/// <summary>Creates a new bulk modulus from pressure change and relative volume change.</summary>
	/// <param name="pressureChange">The change in pressure.</param>
	/// <param name="relativeVolumeChange">The relative change in volume (ΔV/V₀).</param>
	/// <returns>A new BulkModulus instance.</returns>
	/// <remarks>
	/// Uses the relationship: K = -ΔP / (ΔV/V₀)
	/// where K is bulk modulus, ΔP is pressure change, and ΔV/V₀ is relative volume change.
	/// The negative sign accounts for the fact that increased pressure typically decreases volume.
	/// </remarks>
	public static BulkModulus<T> FromPressureAndVolumeChange(Pressure<T> pressureChange, T relativeVolumeChange)
	{
		ArgumentNullException.ThrowIfNull(pressureChange);

		if (relativeVolumeChange == T.Zero)
		{
			throw new ArgumentException("Relative volume change cannot be zero.", nameof(relativeVolumeChange));
		}

		T deltaP = pressureChange.In(Units.Pascal);
		T bulkModulus = -deltaP / relativeVolumeChange;
		return Create(bulkModulus);
	}

	/// <summary>Calculates the relative volume change from bulk modulus and pressure change.</summary>
	/// <param name="pressureChange">The change in pressure.</param>
	/// <returns>The relative volume change (ΔV/V₀).</returns>
	/// <remarks>
	/// Uses the relationship: ΔV/V₀ = -ΔP / K
	/// where ΔV/V₀ is relative volume change, ΔP is pressure change, and K is bulk modulus.
	/// </remarks>
	public T CalculateRelativeVolumeChange(Pressure<T> pressureChange)
	{
		ArgumentNullException.ThrowIfNull(pressureChange);

		T deltaP = pressureChange.In(Units.Pascal);
		T k = In(Units.Pascal);

		return k == T.Zero ? throw new InvalidOperationException("Bulk modulus cannot be zero for volume change calculation.") : -deltaP / k;
	}

	/// <summary>Calculates the compressibility from bulk modulus.</summary>
	/// <returns>The compressibility (1/Pa).</returns>
	/// <remarks>
	/// Uses the relationship: β = 1/K
	/// where β is compressibility and K is bulk modulus.
	/// </remarks>
	public T CalculateCompressibility()
	{
		T k = In(Units.Pascal);

		return k == T.Zero ? throw new InvalidOperationException("Bulk modulus cannot be zero for compressibility calculation.") : T.One / k;
	}

	/// <summary>Calculates the speed of sound in the material from bulk modulus and density.</summary>
	/// <param name="density">The material density.</param>
	/// <returns>The speed of sound.</returns>
	/// <remarks>
	/// Uses the relationship: c = √(K / ρ)
	/// where c is speed of sound, K is bulk modulus, and ρ is density.
	/// This is the Newton-Laplace equation for sound speed in fluids.
	/// </remarks>
	public Velocity<T> CalculateSpeedOfSound(Density<T> density)
	{
		ArgumentNullException.ThrowIfNull(density);

		T k = In(Units.Pascal);
		T rho = density.In(Units.Kilogram);

		if (rho == T.Zero)
		{
			throw new ArgumentException("Density cannot be zero.", nameof(density));
		}

		T speedSquared = k / rho;
		T speed = T.CreateChecked(Math.Sqrt(double.CreateChecked(speedSquared)));
		return Velocity<T>.Create(speed);
	}

	/// <summary>Calculates pressure change from bulk modulus and relative volume change.</summary>
	/// <param name="relativeVolumeChange">The relative change in volume (ΔV/V₀).</param>
	/// <returns>The pressure change.</returns>
	/// <remarks>
	/// Uses the relationship: ΔP = -K × (ΔV/V₀)
	/// where ΔP is pressure change, K is bulk modulus, and ΔV/V₀ is relative volume change.
	/// </remarks>
	public Pressure<T> CalculatePressureChange(T relativeVolumeChange)
	{
		T k = In(Units.Pascal);
		T pressureChange = -k * relativeVolumeChange;
		return Pressure<T>.Create(pressureChange);
	}

	/// <summary>Determines if this bulk modulus is typical for water.</summary>
	/// <returns>True if the value is close to water's bulk modulus (≈ 2.2 GPa at 20°C).</returns>
	public bool IsTypicalForWater()
	{
		T modulus = In(Units.Pascal);
		T waterModulus = T.CreateChecked(2.2e9); // 2.2 GPa
		T tolerance = T.CreateChecked(0.5e9);     // ±0.5 GPa
		T difference = T.Abs(modulus - waterModulus);
		return difference <= tolerance;
	}

	/// <summary>Determines if this bulk modulus is typical for steel.</summary>
	/// <returns>True if the value is close to steel's bulk modulus (≈ 160 GPa).</returns>
	public bool IsTypicalForSteel()
	{
		T modulus = In(Units.Pascal);
		T steelModulus = T.CreateChecked(160e9); // 160 GPa
		T tolerance = T.CreateChecked(20e9);     // ±20 GPa
		T difference = T.Abs(modulus - steelModulus);
		return difference <= tolerance;
	}

	/// <summary>Determines if this material is essentially incompressible.</summary>
	/// <returns>True if the bulk modulus is very high (> 100 GPa), indicating low compressibility.</returns>
	public bool IsEssentiallyIncompressible()
	{
		T modulus = In(Units.Pascal);
		T threshold = T.CreateChecked(100e9); // 100 GPa
		return modulus > threshold;
	}
}
