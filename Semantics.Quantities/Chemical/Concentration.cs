// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents concentration with a specific unit of measurement.
/// Concentration describes the amount of substance per unit volume.
/// </summary>
/// <typeparam name="T">The numeric type for the concentration value.</typeparam>
public sealed record Concentration<T> : PhysicalQuantity<Concentration<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of concentration [N L⁻³].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Concentration;

	/// <summary>Initializes a new instance of the <see cref="Concentration{T}"/> class.</summary>
	public Concentration() : base() { }

	/// <summary>Calculates molarity from amount of substance and volume.</summary>
	/// <param name="amountOfSubstance">The amount of solute.</param>
	/// <param name="volume">The volume of solution.</param>
	/// <returns>The molarity (mol/L).</returns>
	public static Concentration<T> FromMolarity(AmountOfSubstance<T> amountOfSubstance, Volume<T> volume)
	{
		Ensure.NotNull(amountOfSubstance);
		Ensure.NotNull(volume);
		T moles = amountOfSubstance.In(Units.Mole);
		T liters = volume.In(Units.Liter);
		T molarity = moles / liters;
		return Create(molarity);
	}

	/// <summary>Calculates parts per million concentration.</summary>
	/// <param name="soluteMass">Mass of solute.</param>
	/// <param name="solutionMass">Total mass of solution.</param>
	/// <returns>The concentration in ppm.</returns>
	public static Concentration<T> FromPartsPerMillion(Mass<T> soluteMass, Mass<T> solutionMass)
	{
		Ensure.NotNull(soluteMass);
		Ensure.NotNull(solutionMass);
		T soluteGrams = soluteMass.In(Units.Gram);
		T solutionGrams = solutionMass.In(Units.Gram);
		T ratio = soluteGrams / solutionGrams;
		T ppm = ratio * T.CreateChecked(1e6);
		return Create(ppm);
	}

	/// <summary>Calculates weight/volume percent concentration.</summary>
	/// <param name="soluteMass">Mass of solute.</param>
	/// <param name="solutionVolume">Volume of solution.</param>
	/// <returns>The concentration in % w/v.</returns>
	public static Concentration<T> FromWeightVolumePercent(Mass<T> soluteMass, Volume<T> solutionVolume)
	{
		Ensure.NotNull(soluteMass);
		Ensure.NotNull(solutionVolume);
		T massGrams = soluteMass.In(Units.Gram);
		T volumeML = solutionVolume.In(Units.Milliliter);
		T ratio = massGrams / volumeML;
		T percentWV = ratio * T.CreateChecked(100);
		return Create(percentWV);
	}

	/// <summary>Dilution calculation: C1V1 = C2V2.</summary>
	/// <param name="initialVolume">Initial volume.</param>
	/// <param name="finalVolume">Final volume after dilution.</param>
	/// <returns>The final concentration after dilution.</returns>
	public Concentration<T> Dilute(Volume<T> initialVolume, Volume<T> finalVolume)
	{
		Ensure.NotNull(initialVolume);
		Ensure.NotNull(finalVolume);
		T c1 = In(Units.Molar);
		T v1 = initialVolume.In(Units.Liter);
		T v2 = finalVolume.In(Units.Liter);
		T c2 = c1 * v1 / v2;
		return Create(c2);
	}

	/// <summary>
	/// Calculates the amount of substance from concentration and volume (n = C × V).
	/// </summary>
	/// <param name="concentration">The concentration.</param>
	/// <param name="volume">The volume.</param>
	/// <returns>The resulting amount of substance.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static AmountOfSubstance<T> operator *(Concentration<T> concentration, Volume<T> volume)
	{
		Ensure.NotNull(concentration);
		Ensure.NotNull(volume);

		T amountValue = concentration.Value * volume.Value;

		return AmountOfSubstance<T>.Create(amountValue);
	}

	/// <summary>
	/// Calculates the volume from concentration and amount of substance (V = n/C).
	/// </summary>
	/// <param name="amount">The amount of substance.</param>
	/// <param name="concentration">The concentration.</param>
	/// <returns>The resulting volume.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Volume<T> operator /(AmountOfSubstance<T> amount, Concentration<T> concentration)
	{
		Ensure.NotNull(amount);
		Ensure.NotNull(concentration);

		T volumeValue = amount.Value / concentration.Value;

		return Volume<T>.Create(volumeValue);
	}
}
