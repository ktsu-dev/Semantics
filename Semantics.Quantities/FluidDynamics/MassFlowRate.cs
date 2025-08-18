// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents mass flow rate with a specific unit of measurement.
/// Mass flow rate is the mass of substance that passes per unit time.
/// It is measured in kilograms per second (kg/s) in the SI system.
/// </summary>
/// <typeparam name="T">The numeric type for the mass flow rate value.</typeparam>
public sealed record MassFlowRate<T> : PhysicalQuantity<MassFlowRate<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of mass flow rate [M T⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.MassFlowRate;

	/// <summary>Initializes a new instance of the <see cref="MassFlowRate{T}"/> class.</summary>
	public MassFlowRate() : base() { }

	/// <summary>Creates a new mass flow rate from a value in kilograms per second.</summary>
	/// <param name="kilogramsPerSecond">The mass flow rate in kg/s.</param>
	/// <returns>A new MassFlowRate instance.</returns>
	public static MassFlowRate<T> FromKilogramsPerSecond(T kilogramsPerSecond) => Create(kilogramsPerSecond);

	/// <summary>Creates a new mass flow rate from a value in grams per second.</summary>
	/// <param name="gramsPerSecond">The mass flow rate in g/s.</param>
	/// <returns>A new MassFlowRate instance.</returns>
	public static MassFlowRate<T> FromGramsPerSecond(T gramsPerSecond)
	{
		T kilogramsPerSecond = gramsPerSecond / T.CreateChecked(1000);
		return Create(kilogramsPerSecond);
	}

	/// <summary>Creates a new mass flow rate from a value in kilograms per hour.</summary>
	/// <param name="kilogramsPerHour">The mass flow rate in kg/h.</param>
	/// <returns>A new MassFlowRate instance.</returns>
	public static MassFlowRate<T> FromKilogramsPerHour(T kilogramsPerHour)
	{
		T kilogramsPerSecond = kilogramsPerHour / T.CreateChecked(3600);
		return Create(kilogramsPerSecond);
	}

	/// <summary>Creates a new mass flow rate from a value in pounds per second.</summary>
	/// <param name="poundsPerSecond">The mass flow rate in lb/s.</param>
	/// <returns>A new MassFlowRate instance.</returns>
	public static MassFlowRate<T> FromPoundsPerSecond(T poundsPerSecond)
	{
		T kilogramsPerSecond = poundsPerSecond * PhysicalConstants.Generic.PoundMassToKilogram<T>();
		return Create(kilogramsPerSecond);
	}

	/// <summary>Creates a new mass flow rate from volumetric flow rate and density.</summary>
	/// <param name="volumetricFlowRate">The volumetric flow rate.</param>
	/// <param name="density">The fluid density.</param>
	/// <returns>A new MassFlowRate instance.</returns>
	/// <remarks>
	/// Uses the relationship: ṁ = ρ × Q
	/// where ṁ is mass flow rate, ρ is density, and Q is volumetric flow rate.
	/// </remarks>
	public static MassFlowRate<T> FromVolumetricFlowRateAndDensity(VolumetricFlowRate<T> volumetricFlowRate, Density<T> density)
	{
		ArgumentNullException.ThrowIfNull(volumetricFlowRate);
		ArgumentNullException.ThrowIfNull(density);

		T q = volumetricFlowRate.In(Units.CubicMeter);
		T rho = density.In(Units.Kilogram);
		T massFlowRate = rho * q;
		return Create(massFlowRate);
	}

	/// <summary>Calculates volumetric flow rate from mass flow rate and density.</summary>
	/// <param name="density">The fluid density.</param>
	/// <returns>The volumetric flow rate.</returns>
	/// <remarks>
	/// Uses the relationship: Q = ṁ / ρ
	/// where Q is volumetric flow rate, ṁ is mass flow rate, and ρ is density.
	/// </remarks>
	public VolumetricFlowRate<T> CalculateVolumetricFlowRate(Density<T> density)
	{
		ArgumentNullException.ThrowIfNull(density);

		T massFlow = In(Units.Kilogram);
		T rho = density.In(Units.Kilogram);

		if (rho == T.Zero)
		{
			throw new ArgumentException("Density cannot be zero.", nameof(density));
		}

		T volumetricFlow = massFlow / rho;
		return VolumetricFlowRate<T>.Create(volumetricFlow);
	}

	/// <summary>Calculates fluid density from mass flow rate and volumetric flow rate.</summary>
	/// <param name="volumetricFlowRate">The volumetric flow rate.</param>
	/// <returns>The fluid density.</returns>
	/// <remarks>
	/// Uses the relationship: ρ = ṁ / Q
	/// where ρ is density, ṁ is mass flow rate, and Q is volumetric flow rate.
	/// </remarks>
	public Density<T> CalculateDensity(VolumetricFlowRate<T> volumetricFlowRate)
	{
		ArgumentNullException.ThrowIfNull(volumetricFlowRate);

		T massFlow = In(Units.Kilogram);
		T volumeFlow = volumetricFlowRate.In(Units.CubicMeter);

		if (volumeFlow == T.Zero)
		{
			throw new ArgumentException("Volumetric flow rate cannot be zero.", nameof(volumetricFlowRate));
		}

		T density = massFlow / volumeFlow;
		return Density<T>.Create(density);
	}

	/// <summary>Calculates the mass transferred over a given time period.</summary>
	/// <param name="time">The time period.</param>
	/// <returns>The total mass transferred.</returns>
	/// <remarks>
	/// Uses the relationship: m = ṁ × t
	/// where m is mass, ṁ is mass flow rate, and t is time.
	/// </remarks>
	public Mass<T> CalculateMassTransferred(Time<T> time)
	{
		ArgumentNullException.ThrowIfNull(time);

		T massFlow = In(Units.Kilogram);
		T t = time.In(Units.Second);
		T mass = massFlow * t;

		return Mass<T>.Create(mass);
	}

	/// <summary>Calculates the time required to transfer a given mass.</summary>
	/// <param name="mass">The mass to transfer.</param>
	/// <returns>The time required.</returns>
	/// <remarks>
	/// Uses the relationship: t = m / ṁ
	/// where t is time, m is mass, and ṁ is mass flow rate.
	/// </remarks>
	public Time<T> CalculateTransferTime(Mass<T> mass)
	{
		ArgumentNullException.ThrowIfNull(mass);

		T m = mass.In(Units.Kilogram);
		T massFlow = In(Units.Kilogram);

		if (massFlow == T.Zero)
		{
			throw new InvalidOperationException("Mass flow rate cannot be zero for time calculation.");
		}

		T time = m / massFlow;
		return Time<T>.Create(time);
	}

	/// <summary>Calculates the momentum flow rate from mass flow rate and velocity.</summary>
	/// <param name="velocity">The flow velocity.</param>
	/// <returns>The momentum flow rate (kg⋅m/s²).</returns>
	/// <remarks>
	/// Uses the relationship: Momentum Flow = ṁ × v
	/// where ṁ is mass flow rate and v is velocity.
	/// This represents the rate of momentum transfer.
	/// </remarks>
	public T CalculateMomentumFlowRate(Velocity<T> velocity)
	{
		ArgumentNullException.ThrowIfNull(velocity);

		T massFlow = In(Units.Kilogram);
		T v = velocity.In(Units.MetersPerSecond);
		return massFlow * v;
	}

	/// <summary>Determines if this mass flow rate is typical for industrial pipelines.</summary>
	/// <returns>True if the mass flow rate is greater than 1 kg/s.</returns>
	public bool IsTypicalIndustrialFlow()
	{
		T massFlow = In(Units.Kilogram);
		T threshold = T.CreateChecked(1.0); // 1 kg/s
		return massFlow > threshold;
	}

	/// <summary>Determines if this mass flow rate is typical for laboratory scale.</summary>
	/// <returns>True if the mass flow rate is less than 0.1 kg/s.</returns>
	public bool IsTypicalLaboratoryScale()
	{
		T massFlow = In(Units.Kilogram);
		T threshold = T.CreateChecked(0.1); // 0.1 kg/s
		return massFlow < threshold;
	}

	/// <summary>Determines if this mass flow rate is typical for household applications.</summary>
	/// <returns>True if the mass flow rate is between 0.001 and 0.1 kg/s.</returns>
	public bool IsTypicalHouseholdScale()
	{
		T massFlow = In(Units.Kilogram);
		T lowerBound = T.CreateChecked(0.001); // 0.001 kg/s
		T upperBound = T.CreateChecked(0.1);   // 0.1 kg/s
		return massFlow >= lowerBound && massFlow <= upperBound;
	}
}
