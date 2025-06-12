// Copyright (c) KTSU. All rights reserved.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents molar mass with a specific unit of measurement.
/// Molar mass is the mass of one mole of a substance.
/// </summary>
/// <typeparam name="T">The numeric type for the molar mass value.</typeparam>
public sealed record MolarMass<T> : PhysicalQuantity<MolarMass<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of molar mass [M N⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.MolarMass;

	/// <summary>Initializes a new instance of the <see cref="MolarMass{T}"/> class.</summary>
	public MolarMass() : base() { }

	/// <summary>Calculates molar mass from atomic masses and composition.</summary>
	/// <param name="totalMass">Total mass of the compound.</param>
	/// <param name="amountOfSubstance">Amount of substance.</param>
	/// <returns>The molar mass.</returns>
	public static MolarMass<T> FromMassAndAmount(Mass<T> totalMass, AmountOfSubstance<T> amountOfSubstance)
	{
		ArgumentNullException.ThrowIfNull(totalMass);
		ArgumentNullException.ThrowIfNull(amountOfSubstance);

		T massInGrams = totalMass.In(Units.Gram);
		T moles = amountOfSubstance.In(Units.Mole);
		T molarMass = massInGrams / moles;
		return Create(molarMass);
	}

	/// <summary>Common molecular weights of chemical compounds.</summary>
	public static class CommonValues
	{
		/// <summary>Water (H₂O) molar mass: 18.015 g/mol.</summary>
		public static MolarMass<T> Water => Create(T.CreateChecked(18.015));

		/// <summary>Carbon dioxide (CO₂) molar mass: 44.010 g/mol.</summary>
		public static MolarMass<T> CarbonDioxide => Create(T.CreateChecked(44.010));

		/// <summary>Methane (CH₄) molar mass: 16.043 g/mol.</summary>
		public static MolarMass<T> Methane => Create(T.CreateChecked(16.043));

		/// <summary>Glucose (C₆H₁₂O₆) molar mass: 180.156 g/mol.</summary>
		public static MolarMass<T> Glucose => Create(T.CreateChecked(180.156));

		/// <summary>Sodium chloride (NaCl) molar mass: 58.440 g/mol.</summary>
		public static MolarMass<T> SodiumChloride => Create(T.CreateChecked(58.440));

		/// <summary>Ethanol (C₂H₆O) molar mass: 46.069 g/mol.</summary>
		public static MolarMass<T> Ethanol => Create(T.CreateChecked(46.069));
	}

	/// <summary>Calculates the mass of a given amount of this substance.</summary>
	/// <param name="amount">The amount of substance.</param>
	/// <returns>The total mass.</returns>
	public Mass<T> CalculateMass(AmountOfSubstance<T> amount)
	{
		ArgumentNullException.ThrowIfNull(amount);

		T molarMassValue = In(Units.GramPerMole);
		T moles = amount.In(Units.Mole);
		T massInGrams = molarMassValue * moles;
		return Mass<T>.Create(massInGrams);
	}
}
