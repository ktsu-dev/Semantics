// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents pH (potential of Hydrogen) with logarithmic scale.
/// pH is a measure of acidity or alkalinity on a scale from 0-14.
/// </summary>
/// <typeparam name="T">The numeric type for the pH value.</typeparam>
public sealed record PH<T> : PhysicalQuantity<PH<T>, T>
	where T : struct, INumber<T>, IFloatingPoint<T>
{
	/// <summary>Gets the physical dimension of pH [1] (dimensionless).</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.pH;

	/// <summary>Initializes a new instance of the <see cref="PH{T}"/> class.</summary>
	public PH() : base() { }

	/// <summary>Calculates pH from hydrogen ion concentration.</summary>
	/// <param name="hydrogenConcentration">Hydrogen ion concentration in mol/L.</param>
	/// <returns>The pH value.</returns>
	public static PH<T> FromHydrogenConcentration(Concentration<T> hydrogenConcentration)
	{
		ArgumentNullException.ThrowIfNull(hydrogenConcentration);

		T concentration = hydrogenConcentration.In(Units.Molar);
		T logValue = T.CreateChecked(Math.Log10(double.CreateChecked(concentration)));
		T pHValue = -logValue;
		return Create(pHValue);
	}

	/// <summary>Calculates hydrogen ion concentration from pH.</summary>
	/// <returns>The hydrogen ion concentration in mol/L.</returns>
	public Concentration<T> ToHydrogenConcentration()
	{
		T pHValue = In(Units.Radian); // dimensionless
		T concentration = T.CreateChecked(Math.Pow(10.0, double.CreateChecked(-pHValue)));
		return Concentration<T>.Create(concentration);
	}

	/// <summary>Calculates pOH from pH: pH + pOH = 14 at 25°C.</summary>
	/// <returns>The pOH value.</returns>
	public PH<T> ToPOH()
	{
		T pHValue = In(Units.Radian);
		T kw = T.CreateChecked(14.0); // Water dissociation constant at 25°C
		T pOHValue = kw - pHValue;
		return Create(pOHValue);
	}

	/// <summary>Common pH values for reference.</summary>
	public static class CommonValues
	{
		/// <summary>Pure water at 25°C: pH 7.0 (neutral).</summary>
		public static PH<T> NeutralWater => Create(T.CreateChecked(7.0));

		/// <summary>Battery acid: pH ~0.0 (extremely acidic).</summary>
		public static PH<T> BatteryAcid => Create(T.CreateChecked(0.0));

		/// <summary>Lemon juice: pH ~2.0 (very acidic).</summary>
		public static PH<T> LemonJuice => Create(T.CreateChecked(2.0));

		/// <summary>Coffee: pH ~5.0 (acidic).</summary>
		public static PH<T> Coffee => Create(T.CreateChecked(5.0));

		/// <summary>Baking soda: pH ~9.0 (basic).</summary>
		public static PH<T> BakingSoda => Create(T.CreateChecked(9.0));

		/// <summary>Household ammonia: pH ~11.0 (very basic).</summary>
		public static PH<T> Ammonia => Create(T.CreateChecked(11.0));

		/// <summary>Household bleach: pH ~12.0 (very basic).</summary>
		public static PH<T> Bleach => Create(T.CreateChecked(12.0));

		/// <summary>Drain cleaner: pH ~14.0 (extremely basic).</summary>
		public static PH<T> DrainCleaner => Create(T.CreateChecked(14.0));
	}

	/// <summary>Determines if the solution is acidic (pH &lt; 7).</summary>
	/// <returns>True if acidic, false otherwise.</returns>
	public bool IsAcidic()
	{
		T pHValue = In(Units.Radian);
		T neutral = T.CreateChecked(7.0);
		return pHValue < neutral;
	}

	/// <summary>Determines if the solution is basic/alkaline (pH &gt; 7).</summary>
	/// <returns>True if basic, false otherwise.</returns>
	public bool IsBasic()
	{
		T pHValue = In(Units.Radian);
		T neutral = T.CreateChecked(7.0);
		return pHValue > neutral;
	}

	/// <summary>Determines if the solution is neutral (pH = 7).</summary>
	/// <returns>True if neutral, false otherwise.</returns>
	public bool IsNeutral()
	{
		T pHValue = In(Units.Radian);
		T neutral = T.CreateChecked(7.0);
		T tolerance = T.CreateChecked(0.01);
		return T.Abs(pHValue - neutral) < tolerance;
	}
}
