// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents absorbed dose with a specific unit of measurement.
/// Absorbed dose is the amount of energy deposited per unit mass by ionizing radiation.
/// It is measured in grays (Gy) in the SI system.
/// </summary>
/// <typeparam name="T">The numeric type for the absorbed dose value.</typeparam>
public sealed record AbsorbedDose<T> : PhysicalQuantity<AbsorbedDose<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of absorbed dose [L² T⁻²].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.AbsorbedDose;

	/// <summary>Initializes a new instance of the <see cref="AbsorbedDose{T}"/> class.</summary>
	public AbsorbedDose() : base() { }

	/// <summary>Creates a new absorbed dose from a value in grays.</summary>
	/// <param name="grays">The absorbed dose in grays.</param>
	/// <returns>A new AbsorbedDose instance.</returns>
	public static AbsorbedDose<T> FromGrays(T grays) => Create(grays);

	/// <summary>Creates a new absorbed dose from a value in rads.</summary>
	/// <param name="rads">The absorbed dose in rads.</param>
	/// <returns>A new AbsorbedDose instance.</returns>
	public static AbsorbedDose<T> FromRads(T rads)
	{
		T grays = rads * PhysicalConstants.Generic.RadsToGrays<T>();
		return Create(grays);
	}

	/// <summary>Creates a new absorbed dose from a value in milligrays.</summary>
	/// <param name="milligrays">The absorbed dose in milligrays.</param>
	/// <returns>A new AbsorbedDose instance.</returns>
	public static AbsorbedDose<T> FromMilligrays(T milligrays)
	{
		T grays = milligrays / T.CreateChecked(1000);
		return Create(grays);
	}

	/// <summary>Creates a new absorbed dose from a value in micrograys.</summary>
	/// <param name="micrograys">The absorbed dose in micrograys.</param>
	/// <returns>A new AbsorbedDose instance.</returns>
	public static AbsorbedDose<T> FromMicrograys(T micrograys)
	{
		T grays = micrograys / T.CreateChecked(1e6);
		return Create(grays);
	}

	/// <summary>Creates a new absorbed dose from energy and mass.</summary>
	/// <param name="energy">The energy absorbed.</param>
	/// <param name="mass">The mass of the absorbing material.</param>
	/// <returns>A new AbsorbedDose instance.</returns>
	/// <remarks>
	/// Uses the relationship: D = E / m
	/// where D is absorbed dose, E is energy, and m is mass.
	/// </remarks>
	public static AbsorbedDose<T> FromEnergyAndMass(Energy<T> energy, Mass<T> mass)
	{
		ArgumentNullException.ThrowIfNull(energy);
		ArgumentNullException.ThrowIfNull(mass);

		T energyJoules = energy.In(Units.Joule);
		T massKg = mass.In(Units.Kilogram);

		if (massKg == T.Zero)
		{
			throw new ArgumentException("Mass cannot be zero.", nameof(mass));
		}

		T dose = energyJoules / massKg;
		return Create(dose);
	}

	/// <summary>Calculates the energy absorbed from dose and mass.</summary>
	/// <param name="mass">The mass of the absorbing material.</param>
	/// <returns>The energy absorbed.</returns>
	/// <remarks>
	/// Uses the relationship: E = D × m
	/// where E is energy, D is absorbed dose, and m is mass.
	/// </remarks>
	public Energy<T> CalculateEnergyAbsorbed(Mass<T> mass)
	{
		ArgumentNullException.ThrowIfNull(mass);

		T dose = In(Units.Gray);
		T massKg = mass.In(Units.Kilogram);
		T energy = dose * massKg;

		return Energy<T>.Create(energy);
	}

	/// <summary>Calculates equivalent dose using radiation weighting factor.</summary>
	/// <param name="radiationWeightingFactor">The radiation weighting factor (dimensionless).</param>
	/// <returns>The equivalent dose.</returns>
	/// <remarks>
	/// Uses the relationship: H = D × wR
	/// where H is equivalent dose, D is absorbed dose, and wR is radiation weighting factor.
	/// </remarks>
	public EquivalentDose<T> CalculateEquivalentDose(T radiationWeightingFactor)
	{
		T dose = In(Units.Gray);
		T equivalentDose = dose * radiationWeightingFactor;
		return EquivalentDose<T>.Create(equivalentDose);
	}

	/// <summary>Calculates dose rate from absorbed dose and time.</summary>
	/// <param name="time">The time period over which the dose was absorbed.</param>
	/// <returns>The dose rate in Gy/s.</returns>
	/// <remarks>
	/// Uses the relationship: Dose Rate = D / t
	/// where D is absorbed dose and t is time.
	/// </remarks>
	public T CalculateDoseRate(Time<T> time)
	{
		ArgumentNullException.ThrowIfNull(time);

		T dose = In(Units.Gray);
		T timeSeconds = time.In(Units.Second);

		return timeSeconds == T.Zero ? throw new ArgumentException("Time cannot be zero.", nameof(time)) : dose / timeSeconds;
	}

	/// <summary>Calculates the mass of material from absorbed dose and energy.</summary>
	/// <param name="energy">The energy absorbed.</param>
	/// <returns>The mass of the absorbing material.</returns>
	/// <remarks>
	/// Uses the relationship: m = E / D
	/// where m is mass, E is energy, and D is absorbed dose.
	/// </remarks>
	public Mass<T> CalculateMass(Energy<T> energy)
	{
		ArgumentNullException.ThrowIfNull(energy);

		T dose = In(Units.Gray);
		T energyJoules = energy.In(Units.Joule);

		return dose == T.Zero
			? throw new InvalidOperationException("Absorbed dose cannot be zero for mass calculation.")
			: Mass<T>.Create(energyJoules / dose);
	}

	/// <summary>Determines if the dose is potentially lethal.</summary>
	/// <returns>True if the dose exceeds 4 Gy (approximate LD50 for humans).</returns>
	/// <remarks>
	/// Reference doses for humans:
	/// - 0.25 Gy: Mild radiation sickness
	/// - 1 Gy: Severe radiation sickness
	/// - 4 Gy: LD50 (50% lethality in 30 days without treatment)
	/// - 10+ Gy: Lethal within days
	/// </remarks>
	public bool IsPotentiallyLethal()
	{
		T dose = In(Units.Gray);
		T lethalThreshold = T.CreateChecked(4.0); // 4 Gy LD50
		return dose > lethalThreshold;
	}

	/// <summary>Determines if the dose causes radiation sickness symptoms.</summary>
	/// <returns>True if the dose exceeds 0.25 Gy.</returns>
	public bool CausesRadiationSickness()
	{
		T dose = In(Units.Gray);
		T threshold = T.CreateChecked(0.25); // 0.25 Gy threshold for symptoms
		return dose > threshold;
	}
}
