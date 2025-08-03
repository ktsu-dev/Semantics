// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents equivalent dose with a specific unit of measurement.
/// Equivalent dose is the absorbed dose weighted by the biological effectiveness of the radiation.
/// It is measured in sieverts (Sv) in the SI system.
/// </summary>
/// <typeparam name="T">The numeric type for the equivalent dose value.</typeparam>
public record EquivalentDose<T> : PhysicalQuantity<EquivalentDose<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of equivalent dose [L² T⁻²].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.EquivalentDose;

	/// <summary>Initializes a new instance of the <see cref="EquivalentDose{T}"/> class.</summary>
	public EquivalentDose() : base() { }

	/// <summary>Creates a new equivalent dose from a value in sieverts.</summary>
	/// <param name="sieverts">The equivalent dose in sieverts.</param>
	/// <returns>A new EquivalentDose instance.</returns>
	public static EquivalentDose<T> FromSieverts(T sieverts) => Create(sieverts);

	/// <summary>Creates a new equivalent dose from a value in rems.</summary>
	/// <param name="rems">The equivalent dose in rems.</param>
	/// <returns>A new EquivalentDose instance.</returns>
	public static EquivalentDose<T> FromRems(T rems)
	{
		T sieverts = rems * PhysicalConstants.Generic.RemsToSieverts<T>();
		return Create(sieverts);
	}

	/// <summary>Creates a new equivalent dose from a value in millisieverts.</summary>
	/// <param name="millisieverts">The equivalent dose in millisieverts.</param>
	/// <returns>A new EquivalentDose instance.</returns>
	public static EquivalentDose<T> FromMillisieverts(T millisieverts)
	{
		T sieverts = millisieverts / T.CreateChecked(1000);
		return Create(sieverts);
	}

	/// <summary>Creates a new equivalent dose from a value in microsieverts.</summary>
	/// <param name="microsieverts">The equivalent dose in microsieverts.</param>
	/// <returns>A new EquivalentDose instance.</returns>
	public static EquivalentDose<T> FromMicrosieverts(T microsieverts)
	{
		T sieverts = microsieverts / T.CreateChecked(1e6);
		return Create(sieverts);
	}

	/// <summary>Creates a new equivalent dose from absorbed dose and radiation weighting factor.</summary>
	/// <param name="absorbedDose">The absorbed dose.</param>
	/// <param name="radiationWeightingFactor">The radiation weighting factor (dimensionless).</param>
	/// <returns>A new EquivalentDose instance.</returns>
	/// <remarks>
	/// Uses the relationship: H = D × wR
	/// where H is equivalent dose, D is absorbed dose, and wR is radiation weighting factor.
	/// </remarks>
	public static EquivalentDose<T> FromAbsorbedDoseAndWeightingFactor(AbsorbedDose<T> absorbedDose, T radiationWeightingFactor)
	{
		ArgumentNullException.ThrowIfNull(absorbedDose);

		T dose = absorbedDose.In(Units.Gray);
		T equivalentDose = dose * radiationWeightingFactor;
		return Create(equivalentDose);
	}

	/// <summary>Calculates effective dose using tissue weighting factors.</summary>
	/// <param name="tissueWeightingFactor">The tissue weighting factor (dimensionless).</param>
	/// <returns>The effective dose contribution from this tissue.</returns>
	/// <remarks>
	/// Uses the relationship: E = Σ(HT × wT)
	/// where E is effective dose, HT is equivalent dose to tissue T, and wT is tissue weighting factor.
	/// This method calculates the contribution from one tissue type.
	/// </remarks>
	public EquivalentDose<T> CalculateEffectiveDoseContribution(T tissueWeightingFactor)
	{
		T equivalentDose = In(Units.Sievert);
		T effectiveDoseContribution = equivalentDose * tissueWeightingFactor;
		return Create(effectiveDoseContribution);
	}

	/// <summary>Calculates the absorbed dose from equivalent dose and radiation weighting factor.</summary>
	/// <param name="radiationWeightingFactor">The radiation weighting factor (dimensionless).</param>
	/// <returns>The absorbed dose.</returns>
	/// <remarks>
	/// Uses the relationship: D = H / wR
	/// where D is absorbed dose, H is equivalent dose, and wR is radiation weighting factor.
	/// </remarks>
	public AbsorbedDose<T> CalculateAbsorbedDose(T radiationWeightingFactor)
	{
		if (radiationWeightingFactor == T.Zero)
		{
			throw new ArgumentException("Radiation weighting factor cannot be zero.", nameof(radiationWeightingFactor));
		}

		T equivalentDose = In(Units.Sievert);
		return AbsorbedDose<T>.Create(equivalentDose / radiationWeightingFactor);
	}

	/// <summary>Calculates dose rate from equivalent dose and time.</summary>
	/// <param name="time">The time period over which the dose was received.</param>
	/// <returns>The dose rate in Sv/s.</returns>
	/// <remarks>
	/// Uses the relationship: Dose Rate = H / t
	/// where H is equivalent dose and t is time.
	/// </remarks>
	public T CalculateDoseRate(Time<T> time)
	{
		ArgumentNullException.ThrowIfNull(time);

		T dose = In(Units.Sievert);
		T timeSeconds = time.In(Units.Second);

		return timeSeconds == T.Zero ? throw new ArgumentException("Time cannot be zero.", nameof(time)) : dose / timeSeconds;
	}

	/// <summary>Determines if the dose exceeds common safety limits.</summary>
	/// <returns>True if the dose exceeds 20 mSv (annual limit for radiation workers).</returns>
	/// <remarks>
	/// Common limits:
	/// - General public: 1 mSv/year
	/// - Radiation workers: 20 mSv/year
	/// - Emergency workers: 100 mSv (acute)
	/// </remarks>
	public bool ExceedsAnnualWorkerLimit()
	{
		T dose = In(Units.Sievert);
		T limit = T.CreateChecked(0.02); // 20 mSv in Sv
		return dose > limit;
	}

	/// <summary>Determines if the dose exceeds the annual limit for the general public.</summary>
	/// <returns>True if the dose exceeds 1 mSv.</returns>
	public bool ExceedsPublicLimit()
	{
		T dose = In(Units.Sievert);
		T limit = T.CreateChecked(0.001); // 1 mSv in Sv
		return dose > limit;
	}
}
