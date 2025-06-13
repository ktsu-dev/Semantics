// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an amount of substance with a specific unit of measurement.
/// Amount of substance is a fundamental SI quantity measured in moles.
/// </summary>
/// <typeparam name="T">The numeric type for the amount value.</typeparam>
public sealed record AmountOfSubstance<T> : PhysicalQuantity<AmountOfSubstance<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of amount of substance [N].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.AmountOfSubstance;

	/// <summary>Initializes a new instance of the <see cref="AmountOfSubstance{T}"/> class.</summary>
	public AmountOfSubstance() : base() { }

	/// <summary>Creates an amount from the number of entities and Avogadro's number.</summary>
	/// <param name="numberOfEntities">Number of entities (atoms, molecules, ions).</param>
	/// <returns>The amount of substance in moles.</returns>
	public static AmountOfSubstance<T> FromEntities(T numberOfEntities)
	{
		T avogadroNumber = PhysicalConstants.Generic.AvogadroNumber<T>();
		T moles = numberOfEntities / avogadroNumber;
		return Create(moles);
	}

	/// <summary>Calculates the number of entities from amount of substance.</summary>
	/// <returns>The number of entities (atoms, molecules, ions).</returns>
	public T GetNumberOfEntities()
	{
		T avogadroNumber = PhysicalConstants.Generic.AvogadroNumber<T>();
		T moles = In(Units.Mole);
		return moles * avogadroNumber;
	}

	/// <summary>Calculates mass from amount of substance and molar mass.</summary>
	/// <param name="molarMass">The molar mass of the substance.</param>
	/// <returns>The mass of the substance.</returns>
	public Mass<T> CalculateMass(MolarMass<T> molarMass)
	{
		ArgumentNullException.ThrowIfNull(molarMass);
		T molesValue = In(Units.Mole);
		T molarMassValue = molarMass.In(Units.GramPerMole);
		T massInGrams = molesValue * molarMassValue;
		return Mass<T>.Create(massInGrams);
	}

	/// <summary>Calculates volume from amount of substance at STP.</summary>
	/// <returns>The molar volume at STP (22.414 L/mol).</returns>
	public Volume<T> GetMolarVolumeAtSTP()
	{
		T molarVolumeSTP = PhysicalConstants.Generic.MolarVolumeSTP<T>();
		T moles = In(Units.Mole);
		T volumeInLiters = moles * molarVolumeSTP;
		return Volume<T>.Create(volumeInLiters);
	}

	/// <summary>
	/// Calculates the concentration from amount of substance and volume (C = n/V).
	/// </summary>
	/// <param name="amount">The amount of substance.</param>
	/// <param name="volume">The volume.</param>
	/// <returns>The resulting concentration.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Concentration<T> operator /(AmountOfSubstance<T> amount, Volume<T> volume)
	{
		ArgumentNullException.ThrowIfNull(amount);
		ArgumentNullException.ThrowIfNull(volume);

		T concentrationValue = amount.Value / volume.Value;

		return Concentration<T>.Create(concentrationValue);
	}

	/// <summary>
	/// Calculates the mass from amount of substance and molar mass (m = n × M).
	/// </summary>
	/// <param name="amount">The amount of substance.</param>
	/// <param name="molarMass">The molar mass.</param>
	/// <returns>The resulting mass.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Mass<T> operator *(AmountOfSubstance<T> amount, MolarMass<T> molarMass)
	{
		ArgumentNullException.ThrowIfNull(amount);
		ArgumentNullException.ThrowIfNull(molarMass);

		T massValue = amount.Value * molarMass.Value;

		return Mass<T>.Create(massValue);
	}
}
