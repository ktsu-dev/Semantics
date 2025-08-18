// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents ionizing radiation exposure with a specific unit of measurement.
/// Exposure is the measure of ionization produced in air by X-rays or gamma rays.
/// It is measured in coulombs per kilogram (C/kg) in the SI system.
/// </summary>
/// <typeparam name="T">The numeric type for the exposure value.</typeparam>
public sealed record Exposure<T> : PhysicalQuantity<Exposure<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of exposure [M⁻¹ T I].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Exposure;

	/// <summary>Initializes a new instance of the <see cref="Exposure{T}"/> class.</summary>
	public Exposure() : base() { }

	/// <summary>Creates a new exposure from a value in coulombs per kilogram.</summary>
	/// <param name="coulombsPerKilogram">The exposure in coulombs per kilogram.</param>
	/// <returns>A new Exposure instance.</returns>
	public static Exposure<T> FromCoulombsPerKilogram(T coulombsPerKilogram) => Create(coulombsPerKilogram);

	/// <summary>Creates a new exposure from a value in roentgens.</summary>
	/// <param name="roentgens">The exposure in roentgens.</param>
	/// <returns>A new Exposure instance.</returns>
	public static Exposure<T> FromRoentgens(T roentgens)
	{
		T coulombsPerKg = roentgens * T.CreateChecked(2.58e-4);
		return Create(coulombsPerKg);
	}

	/// <summary>Creates a new exposure from a value in milliroentgens.</summary>
	/// <param name="milliroentgens">The exposure in milliroentgens.</param>
	/// <returns>A new Exposure instance.</returns>
	public static Exposure<T> FromMilliroentgens(T milliroentgens)
	{
		T coulombsPerKg = milliroentgens * T.CreateChecked(2.58e-7);
		return Create(coulombsPerKg);
	}

	/// <summary>Creates a new exposure from charge and mass.</summary>
	/// <param name="charge">The electric charge produced.</param>
	/// <param name="mass">The mass of air.</param>
	/// <returns>A new Exposure instance.</returns>
	/// <remarks>
	/// Uses the relationship: X = Q / m
	/// where X is exposure, Q is charge, and m is mass.
	/// </remarks>
	public static Exposure<T> FromChargeAndMass(ElectricCharge<T> charge, Mass<T> mass)
	{
		ArgumentNullException.ThrowIfNull(charge);
		ArgumentNullException.ThrowIfNull(mass);

		T chargeCoulombs = charge.In(Units.Coulomb);
		T massKg = mass.In(Units.Kilogram);

		if (massKg == T.Zero)
		{
			throw new ArgumentException("Mass cannot be zero.", nameof(mass));
		}

		T exposure = chargeCoulombs / massKg;
		return Create(exposure);
	}

	/// <summary>Calculates the charge produced from exposure and mass.</summary>
	/// <param name="mass">The mass of air.</param>
	/// <returns>The electric charge produced.</returns>
	/// <remarks>
	/// Uses the relationship: Q = X × m
	/// where Q is charge, X is exposure, and m is mass.
	/// </remarks>
	public ElectricCharge<T> CalculateCharge(Mass<T> mass)
	{
		ArgumentNullException.ThrowIfNull(mass);

		T exposure = In(Units.Coulomb);
		T massKg = mass.In(Units.Kilogram);
		T charge = exposure * massKg;

		return ElectricCharge<T>.Create(charge);
	}

	/// <summary>Calculates the mass of air from exposure and charge.</summary>
	/// <param name="charge">The electric charge produced.</param>
	/// <returns>The mass of air.</returns>
	/// <remarks>
	/// Uses the relationship: m = Q / X
	/// where m is mass, Q is charge, and X is exposure.
	/// </remarks>
	public Mass<T> CalculateMass(ElectricCharge<T> charge)
	{
		ArgumentNullException.ThrowIfNull(charge);

		T exposure = In(Units.Coulomb);
		T chargeCoulombs = charge.In(Units.Coulomb);

		if (exposure == T.Zero)
		{
			throw new InvalidOperationException("Exposure cannot be zero for mass calculation.");
		}

		T mass = chargeCoulombs / exposure;
		return Mass<T>.Create(mass);
	}

	/// <summary>Calculates exposure rate from exposure and time.</summary>
	/// <param name="time">The time period over which the exposure occurred.</param>
	/// <returns>The exposure rate in C/(kg·s).</returns>
	/// <remarks>
	/// Uses the relationship: Exposure Rate = X / t
	/// where X is exposure and t is time.
	/// </remarks>
	public T CalculateExposureRate(Time<T> time)
	{
		ArgumentNullException.ThrowIfNull(time);

		T exposure = In(Units.Coulomb);
		T timeSeconds = time.In(Units.Second);

		return timeSeconds == T.Zero ? throw new ArgumentException("Time cannot be zero.", nameof(time)) : exposure / timeSeconds;
	}

	/// <summary>Converts exposure to approximate absorbed dose in air.</summary>
	/// <returns>The approximate absorbed dose in air.</returns>
	/// <remarks>
	/// Uses the conversion factor: 1 R ≈ 0.00876 Gy in air
	/// This is an approximation and may vary with photon energy.
	/// </remarks>
	public AbsorbedDose<T> ToAbsorbedDoseInAir()
	{
		T exposureCperKg = In(Units.Coulomb);
		T exposureRoentgens = exposureCperKg / T.CreateChecked(2.58e-4);
		T doseGray = exposureRoentgens * PhysicalConstants.Generic.RoentgensToGraysInAir<T>();
		return AbsorbedDose<T>.Create(doseGray);
	}

	/// <summary>Determines if the exposure exceeds occupational safety limits.</summary>
	/// <returns>True if the exposure exceeds 50 mR (typical annual limit for radiation workers).</returns>
	/// <remarks>
	/// Common exposure limits:
	/// - General public: ~2 mR/year background
	/// - Radiation workers: ~50 mR/year (varies by jurisdiction)
	/// - Medical X-ray: ~10 mR typical chest X-ray
	/// </remarks>
	public bool ExceedsOccupationalLimit()
	{
		T exposure = In(Units.Coulomb);
		T limitCperKg = T.CreateChecked(50) * T.CreateChecked(2.58e-7); // 50 mR in C/kg
		return exposure > limitCperKg;
	}

	/// <summary>Determines if the exposure is within natural background levels.</summary>
	/// <returns>True if the exposure is less than 2 mR (typical annual background).</returns>
	public bool IsBackgroundLevel()
	{
		T exposure = In(Units.Coulomb);
		T backgroundCperKg = T.CreateChecked(2) * T.CreateChecked(2.58e-7); // 2 mR in C/kg
		return exposure <= backgroundCperKg;
	}
}
