// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents enzyme activity with a specific unit of measurement.
/// Enzyme activity measures the rate of substrate conversion by an enzyme.
/// </summary>
/// <typeparam name="T">The numeric type for the enzyme activity value.</typeparam>
public record EnzymeActivity<T> : PhysicalQuantity<EnzymeActivity<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of enzyme activity [N T⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.EnzymeActivity;

	/// <summary>Initializes a new instance of the <see cref="EnzymeActivity{T}"/> class.</summary>
	public EnzymeActivity() : base() { }

	/// <summary>Calculates enzyme activity from substrate conversion rate.</summary>
	/// <param name="substrateConverted">Amount of substrate converted.</param>
	/// <param name="timeInterval">Time interval.</param>
	/// <returns>The enzyme activity.</returns>
	public static EnzymeActivity<T> FromSubstrateConversion(AmountOfSubstance<T> substrateConverted, Time<T> timeInterval)
	{
		ArgumentNullException.ThrowIfNull(substrateConverted);
		ArgumentNullException.ThrowIfNull(timeInterval);

		T moles = substrateConverted.In(Units.Mole);
		T seconds = timeInterval.In(Units.Second);
		T activity = moles / seconds;
		return Create(activity);
	}

	/// <summary>Calculates specific activity (activity per unit mass of enzyme).</summary>
	/// <param name="enzymeMass">Mass of enzyme.</param>
	/// <returns>Specific activity in units per gram.</returns>
	public T CalculateSpecificActivity(Mass<T> enzymeMass)
	{
		ArgumentNullException.ThrowIfNull(enzymeMass);

		T activity = In(Units.Katal);
		T massInGrams = enzymeMass.In(Units.Gram);
		return activity / massInGrams;
	}

	/// <summary>Michaelis-Menten kinetics: v = (Vmax * [S]) / (Km + [S]).</summary>
	/// <param name="maxVelocity">Maximum velocity (Vmax).</param>
	/// <param name="substrateConcentration">Substrate concentration.</param>
	/// <param name="michaelisConstant">Michaelis constant (Km).</param>
	/// <returns>The reaction velocity.</returns>
	public static EnzymeActivity<T> MichaelisMenten(EnzymeActivity<T> maxVelocity,
		Concentration<T> substrateConcentration, Concentration<T> michaelisConstant)
	{
		ArgumentNullException.ThrowIfNull(maxVelocity);
		ArgumentNullException.ThrowIfNull(substrateConcentration);
		ArgumentNullException.ThrowIfNull(michaelisConstant);

		T vmax = maxVelocity.In(Units.Katal);
		T s = substrateConcentration.In(Units.Molar);
		T km = michaelisConstant.In(Units.Molar);

		T velocity = vmax * s / (km + s);
		return Create(velocity);
	}

	/// <summary>Calculates turnover number (kcat) from Vmax and enzyme concentration.</summary>
	/// <param name="maxVelocity">Maximum velocity.</param>
	/// <param name="enzymeConcentration">Total enzyme concentration.</param>
	/// <returns>Turnover number in s⁻¹.</returns>
	public static T CalculateTurnoverNumber(EnzymeActivity<T> maxVelocity, Concentration<T> enzymeConcentration)
	{
		ArgumentNullException.ThrowIfNull(maxVelocity);
		ArgumentNullException.ThrowIfNull(enzymeConcentration);

		T vmax = maxVelocity.In(Units.Katal);
		T et = enzymeConcentration.In(Units.Molar);
		return vmax / et;
	}

	/// <summary>Common enzyme activities for reference.</summary>
	public static class CommonValues
	{
		/// <summary>Typical enzyme activity: ~1 μmol/min.</summary>
		public static EnzymeActivity<T> TypicalEnzyme => Create(T.CreateChecked(1e-6 / 60)); // 1 μmol/min in mol/s

		/// <summary>High activity enzyme: ~1000 μmol/min.</summary>
		public static EnzymeActivity<T> HighActivity => Create(T.CreateChecked(1e-3 / 60)); // 1000 μmol/min in mol/s

		/// <summary>Catalase (very fast): ~6 × 10⁶ μmol/min.</summary>
		public static EnzymeActivity<T> Catalase => Create(T.CreateChecked(6e3 / 60)); // 6 × 10⁶ μmol/min in mol/s

		/// <summary>Carbonic anhydrase: ~1 × 10⁶ μmol/min.</summary>
		public static EnzymeActivity<T> CarbonicAnhydrase => Create(T.CreateChecked(1e3 / 60)); // 1 × 10⁶ μmol/min in mol/s
	}
}
