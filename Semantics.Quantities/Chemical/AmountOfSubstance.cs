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
		Guard.NotNull(molarMass);
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
		Guard.NotNull(amount);
		Guard.NotNull(volume);

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
		Guard.NotNull(amount);
		Guard.NotNull(molarMass);

		T massValue = amount.Value * molarMass.Value;

		return Mass<T>.Create(massValue);
	}

	/// <summary>
	/// Creates a new AmountOfSubstance from a value in moles.
	/// </summary>
	/// <param name="moles">The value in moles.</param>
	/// <returns>A new AmountOfSubstance instance.</returns>
	public static AmountOfSubstance<T> FromMoles(T moles) => Create(moles);

	/// <summary>
	/// Calculates amount of substance using ideal gas law (n = PV/RT).
	/// </summary>
	/// <param name="pressure">The pressure.</param>
	/// <param name="volume">The volume of the gas.</param>
	/// <param name="temperature">The absolute temperature.</param>
	/// <returns>The resulting amount of substance in moles.</returns>
	public static AmountOfSubstance<T> FromIdealGasLaw(Pressure<T> pressure, Volume<T> volume, Temperature<T> temperature)
	{
		Guard.NotNull(pressure);
		Guard.NotNull(volume);
		Guard.NotNull(temperature);

		T gasConstant = PhysicalConstants.Generic.GasConstant<T>();
		T amountValue = pressure.Value * volume.Value / (gasConstant * temperature.Value);

		return Create(amountValue);
	}

	/// <summary>
	/// Calculates pressure using ideal gas law (P = nRT/V).
	/// </summary>
	/// <param name="amount">The amount of substance.</param>
	/// <param name="temperature">The absolute temperature.</param>
	/// <param name="volume">The volume of the gas.</param>
	/// <returns>The resulting pressure.</returns>
	public static Pressure<T> CalculatePressureFromIdealGas(AmountOfSubstance<T> amount, Temperature<T> temperature, Volume<T> volume)
	{
		Guard.NotNull(amount);
		Guard.NotNull(temperature);
		Guard.NotNull(volume);

		T gasConstant = PhysicalConstants.Generic.GasConstant<T>();
		T pressureValue = amount.Value * gasConstant * temperature.Value / volume.Value;

		return Pressure<T>.Create(pressureValue);
	}

	/// <summary>
	/// Calculates volume using ideal gas law (V = nRT/P).
	/// </summary>
	/// <param name="amount">The amount of substance.</param>
	/// <param name="temperature">The absolute temperature.</param>
	/// <param name="pressure">The pressure.</param>
	/// <returns>The resulting volume.</returns>
	public static Volume<T> CalculateVolumeFromIdealGas(AmountOfSubstance<T> amount, Temperature<T> temperature, Pressure<T> pressure)
	{
		Guard.NotNull(amount);
		Guard.NotNull(temperature);
		Guard.NotNull(pressure);

		T gasConstant = PhysicalConstants.Generic.GasConstant<T>();
		T volumeValue = amount.Value * gasConstant * temperature.Value / pressure.Value;

		return Volume<T>.Create(volumeValue);
	}

	/// <summary>
	/// Calculates temperature using ideal gas law (T = PV/nR).
	/// </summary>
	/// <param name="pressure">The pressure.</param>
	/// <param name="volume">The volume of the gas.</param>
	/// <param name="amount">The amount of substance.</param>
	/// <returns>The resulting absolute temperature.</returns>
	public static Temperature<T> CalculateTemperatureFromIdealGas(Pressure<T> pressure, Volume<T> volume, AmountOfSubstance<T> amount)
	{
		Guard.NotNull(pressure);
		Guard.NotNull(volume);
		Guard.NotNull(amount);

		T gasConstant = PhysicalConstants.Generic.GasConstant<T>();
		T temperatureValue = pressure.Value * volume.Value / (amount.Value * gasConstant);

		return Temperature<T>.Create(temperatureValue);
	}
}
