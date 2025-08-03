// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents nuclear cross section with a specific unit of measurement.
/// Nuclear cross section is a measure of the probability of interaction between a particle and a nucleus.
/// It is measured in barns (b) where 1 barn = 10⁻²⁴ cm² = 10⁻²⁸ m².
/// </summary>
/// <typeparam name="T">The numeric type for the nuclear cross section value.</typeparam>
public record NuclearCrossSection<T> : PhysicalQuantity<NuclearCrossSection<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of nuclear cross section [L²].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.NuclearCrossSection;

	/// <summary>Initializes a new instance of the <see cref="NuclearCrossSection{T}"/> class.</summary>
	public NuclearCrossSection() : base() { }

	/// <summary>Creates a new nuclear cross section from a value in barns.</summary>
	/// <param name="barns">The nuclear cross section in barns.</param>
	/// <returns>A new NuclearCrossSection instance.</returns>
	public static NuclearCrossSection<T> FromBarns(T barns) => Create(barns);

	/// <summary>Creates a new nuclear cross section from a value in millibarns.</summary>
	/// <param name="millibarns">The nuclear cross section in millibarns.</param>
	/// <returns>A new NuclearCrossSection instance.</returns>
	public static NuclearCrossSection<T> FromMillibarns(T millibarns)
	{
		T barns = millibarns / T.CreateChecked(1000);
		return Create(barns);
	}

	/// <summary>Creates a new nuclear cross section from a value in microbarns.</summary>
	/// <param name="microbarns">The nuclear cross section in microbarns.</param>
	/// <returns>A new NuclearCrossSection instance.</returns>
	public static NuclearCrossSection<T> FromMicrobarns(T microbarns)
	{
		T barns = microbarns / T.CreateChecked(1e6);
		return Create(barns);
	}

	/// <summary>Creates a new nuclear cross section from a value in femtobarns.</summary>
	/// <param name="femtobarns">The nuclear cross section in femtobarns.</param>
	/// <returns>A new NuclearCrossSection instance.</returns>
	public static NuclearCrossSection<T> FromFemtobarns(T femtobarns)
	{
		T barns = femtobarns / T.CreateChecked(1e15);
		return Create(barns);
	}

	/// <summary>Creates a new nuclear cross section from a value in square meters.</summary>
	/// <param name="squareMeters">The nuclear cross section in square meters.</param>
	/// <returns>A new NuclearCrossSection instance.</returns>
	public static NuclearCrossSection<T> FromSquareMeters(T squareMeters)
	{
		T barns = squareMeters * T.CreateChecked(1e28);
		return Create(barns);
	}

	/// <summary>Creates a new nuclear cross section from a value in square centimeters.</summary>
	/// <param name="squareCentimeters">The nuclear cross section in square centimeters.</param>
	/// <returns>A new NuclearCrossSection instance.</returns>
	public static NuclearCrossSection<T> FromSquareCentimeters(T squareCentimeters)
	{
		T barns = squareCentimeters * T.CreateChecked(1e24);
		return Create(barns);
	}

	/// <summary>Calculates the reaction rate from cross section, flux, and number density.</summary>
	/// <param name="neutronFlux">The neutron flux (particles per cm² per second).</param>
	/// <param name="numberDensity">The number density of target nuclei (nuclei per cm³).</param>
	/// <returns>The reaction rate (reactions per cm³ per second).</returns>
	/// <remarks>
	/// Uses the relationship: R = σ × Φ × N
	/// where R is reaction rate, σ is cross section, Φ is flux, and N is number density.
	/// </remarks>
	public T CalculateReactionRate(T neutronFlux, T numberDensity)
	{
		T crossSection = In(Units.Barn);
		T crossSectionCm2 = crossSection * T.CreateChecked(1e-24); // Convert barns to cm²
		return crossSectionCm2 * neutronFlux * numberDensity;
	}

	/// <summary>Calculates the macroscopic cross section from microscopic cross section and number density.</summary>
	/// <param name="numberDensity">The number density of target nuclei (nuclei per cm³).</param>
	/// <returns>The macroscopic cross section (cm⁻¹).</returns>
	/// <remarks>
	/// Uses the relationship: Σ = σ × N
	/// where Σ is macroscopic cross section, σ is microscopic cross section, and N is number density.
	/// </remarks>
	public T CalculateMacroscopicCrossSection(T numberDensity)
	{
		T crossSection = In(Units.Barn);
		T crossSectionCm2 = crossSection * T.CreateChecked(1e-24); // Convert barns to cm²
		return crossSectionCm2 * numberDensity;
	}

	/// <summary>Calculates the mean free path from macroscopic cross section.</summary>
	/// <param name="numberDensity">The number density of target nuclei (nuclei per cm³).</param>
	/// <returns>The mean free path in cm.</returns>
	/// <remarks>
	/// Uses the relationship: λ = 1 / Σ = 1 / (σ × N)
	/// where λ is mean free path, Σ is macroscopic cross section, σ is microscopic cross section, and N is number density.
	/// </remarks>
	public T CalculateMeanFreePath(T numberDensity)
	{
		T macroscopicCrossSection = CalculateMacroscopicCrossSection(numberDensity);

		return macroscopicCrossSection == T.Zero
			? throw new InvalidOperationException("Macroscopic cross section cannot be zero for mean free path calculation.")
			: T.One / macroscopicCrossSection;
	}

	/// <summary>Calculates the transmission probability through a material.</summary>
	/// <param name="thickness">The thickness of the material in cm.</param>
	/// <param name="numberDensity">The number density of target nuclei (nuclei per cm³).</param>
	/// <returns>The transmission probability (0 to 1).</returns>
	/// <remarks>
	/// Uses the relationship: T = e^(-Σ×x) = e^(-σ×N×x)
	/// where T is transmission, Σ is macroscopic cross section, and x is thickness.
	/// </remarks>
	public T CalculateTransmissionProbability(T thickness, T numberDensity)
	{
		T macroscopicCrossSection = CalculateMacroscopicCrossSection(numberDensity);
		T exponent = -macroscopicCrossSection * thickness;
		return T.CreateChecked(Math.Exp(double.CreateChecked(exponent)));
	}

	/// <summary>Calculates the attenuation coefficient from cross section and number density.</summary>
	/// <param name="numberDensity">The number density of target nuclei (nuclei per cm³).</param>
	/// <returns>The linear attenuation coefficient (cm⁻¹).</returns>
	/// <remarks>
	/// The attenuation coefficient is equivalent to the macroscopic cross section.
	/// Uses the relationship: μ = σ × N
	/// where μ is attenuation coefficient, σ is cross section, and N is number density.
	/// </remarks>
	public T CalculateAttenuationCoefficient(T numberDensity) => CalculateMacroscopicCrossSection(numberDensity);

	/// <summary>Determines if the cross section is typical for nuclear reactions.</summary>
	/// <returns>True if the cross section is between 1 millibarn and 10 barns.</returns>
	/// <remarks>
	/// Typical nuclear cross sections:
	/// - Neutron capture: 1-1000 barns
	/// - Nuclear fission: 1-1000 barns
	/// - Elastic scattering: 1-20 barns
	/// - High-energy reactions: millibarns to microbarns
	/// </remarks>
	public bool IsTypicalNuclearReaction()
	{
		T crossSection = In(Units.Barn);
		T lowerBound = T.CreateChecked(0.001); // 1 millibarn
		T upperBound = T.CreateChecked(10.0);   // 10 barns
		return crossSection >= lowerBound && crossSection <= upperBound;
	}

	/// <summary>Determines if the cross section is typical for high-energy particle physics.</summary>
	/// <returns>True if the cross section is less than 1 millibarn.</returns>
	public bool IsHighEnergyScale()
	{
		T crossSection = In(Units.Barn);
		T threshold = T.CreateChecked(0.001); // 1 millibarn
		return crossSection < threshold;
	}
}
