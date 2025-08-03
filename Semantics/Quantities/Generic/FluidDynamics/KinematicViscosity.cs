// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents kinematic viscosity with a specific unit of measurement.
/// Kinematic viscosity is the ratio of dynamic viscosity to fluid density.
/// It is measured in square meters per second (m²/s) in the SI system.
/// </summary>
/// <typeparam name="T">The numeric type for the kinematic viscosity value.</typeparam>
public record KinematicViscosity<T> : PhysicalQuantity<KinematicViscosity<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of kinematic viscosity [L² T⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.KinematicViscosity;

	/// <summary>Initializes a new instance of the <see cref="KinematicViscosity{T}"/> class.</summary>
	public KinematicViscosity() : base() { }

	/// <summary>Creates a new kinematic viscosity from a value in square meters per second.</summary>
	/// <param name="squareMetersPerSecond">The kinematic viscosity in m²/s.</param>
	/// <returns>A new KinematicViscosity instance.</returns>
	public static KinematicViscosity<T> FromSquareMetersPerSecond(T squareMetersPerSecond) => Create(squareMetersPerSecond);

	/// <summary>Creates a new kinematic viscosity from a value in stokes.</summary>
	/// <param name="stokes">The kinematic viscosity in stokes.</param>
	/// <returns>A new KinematicViscosity instance.</returns>
	public static KinematicViscosity<T> FromStokes(T stokes)
	{
		T squareMetersPerSecond = stokes * T.CreateChecked(1e-4);
		return Create(squareMetersPerSecond);
	}

	/// <summary>Creates a new kinematic viscosity from a value in centistokes.</summary>
	/// <param name="centistokes">The kinematic viscosity in centistokes.</param>
	/// <returns>A new KinematicViscosity instance.</returns>
	public static KinematicViscosity<T> FromCentistokes(T centistokes)
	{
		T squareMetersPerSecond = centistokes * T.CreateChecked(1e-6);
		return Create(squareMetersPerSecond);
	}

	/// <summary>Creates a new kinematic viscosity from dynamic viscosity and density.</summary>
	/// <param name="dynamicViscosity">The dynamic viscosity.</param>
	/// <param name="density">The fluid density.</param>
	/// <returns>A new KinematicViscosity instance.</returns>
	/// <remarks>
	/// Uses the relationship: ν = μ / ρ
	/// where ν is kinematic viscosity, μ is dynamic viscosity, and ρ is density.
	/// </remarks>
	public static KinematicViscosity<T> FromDynamicViscosityAndDensity(DynamicViscosity<T> dynamicViscosity, Density<T> density)
	{
		ArgumentNullException.ThrowIfNull(dynamicViscosity);
		ArgumentNullException.ThrowIfNull(density);

		T mu = dynamicViscosity.In(Units.Pascal);
		T rho = density.In(Units.Kilogram);

		if (rho == T.Zero)
		{
			throw new ArgumentException("Density cannot be zero.", nameof(density));
		}

		T kinematicViscosity = mu / rho;
		return Create(kinematicViscosity);
	}

	/// <summary>Calculates dynamic viscosity from kinematic viscosity and density.</summary>
	/// <param name="density">The fluid density.</param>
	/// <returns>The dynamic viscosity.</returns>
	/// <remarks>
	/// Uses the relationship: μ = ν × ρ
	/// where μ is dynamic viscosity, ν is kinematic viscosity, and ρ is density.
	/// </remarks>
	public DynamicViscosity<T> CalculateDynamicViscosity(Density<T> density)
	{
		ArgumentNullException.ThrowIfNull(density);

		T nu = In(Units.Meter);
		T rho = density.In(Units.Kilogram);
		T dynamicViscosity = nu * rho;

		return DynamicViscosity<T>.Create(dynamicViscosity);
	}

	/// <summary>Calculates fluid density from kinematic viscosity and dynamic viscosity.</summary>
	/// <param name="dynamicViscosity">The dynamic viscosity.</param>
	/// <returns>The fluid density.</returns>
	/// <remarks>
	/// Uses the relationship: ρ = μ / ν
	/// where ρ is density, μ is dynamic viscosity, and ν is kinematic viscosity.
	/// </remarks>
	public Density<T> CalculateDensity(DynamicViscosity<T> dynamicViscosity)
	{
		ArgumentNullException.ThrowIfNull(dynamicViscosity);

		T nu = In(Units.Meter);
		T mu = dynamicViscosity.In(Units.Pascal);

		if (nu == T.Zero)
		{
			throw new InvalidOperationException("Kinematic viscosity cannot be zero for density calculation.");
		}

		T density = mu / nu;
		return Density<T>.Create(density);
	}

	/// <summary>Calculates Reynolds number from velocity, characteristic length, and kinematic viscosity.</summary>
	/// <param name="velocity">The flow velocity.</param>
	/// <param name="characteristicLength">The characteristic length.</param>
	/// <returns>The Reynolds number (dimensionless).</returns>
	/// <remarks>
	/// Uses the relationship: Re = (v × L) / ν
	/// where Re is Reynolds number, v is velocity, L is characteristic length, and ν is kinematic viscosity.
	/// </remarks>
	public T CalculateReynoldsNumber(Velocity<T> velocity, Length<T> characteristicLength)
	{
		ArgumentNullException.ThrowIfNull(velocity);
		ArgumentNullException.ThrowIfNull(characteristicLength);

		T v = velocity.In(Units.MetersPerSecond);
		T length = characteristicLength.In(Units.Meter);
		T nu = In(Units.Meter);

		return nu == T.Zero
			? throw new InvalidOperationException("Kinematic viscosity cannot be zero for Reynolds number calculation.")
			: v * length / nu;
	}

	/// <summary>Determines if the flow is laminar based on Reynolds number criteria.</summary>
	/// <param name="velocity">The flow velocity.</param>
	/// <param name="characteristicLength">The characteristic length.</param>
	/// <param name="laminarThreshold">The Reynolds number threshold for laminar flow (default: 2300 for pipe flow).</param>
	/// <returns>True if the flow is laminar (Re &lt; threshold).</returns>
	public bool IsLaminarFlow(Velocity<T> velocity, Length<T> characteristicLength, T? laminarThreshold = null)
	{
		T threshold = laminarThreshold ?? T.CreateChecked(2300); // Default for pipe flow
		T reynoldsNumber = CalculateReynoldsNumber(velocity, characteristicLength);
		return reynoldsNumber < threshold;
	}

	/// <summary>Determines if the flow is turbulent based on Reynolds number criteria.</summary>
	/// <param name="velocity">The flow velocity.</param>
	/// <param name="characteristicLength">The characteristic length.</param>
	/// <param name="turbulentThreshold">The Reynolds number threshold for turbulent flow (default: 4000 for pipe flow).</param>
	/// <returns>True if the flow is turbulent (Re &gt; threshold).</returns>
	public bool IsTurbulentFlow(Velocity<T> velocity, Length<T> characteristicLength, T? turbulentThreshold = null)
	{
		T threshold = turbulentThreshold ?? T.CreateChecked(4000); // Default for pipe flow
		T reynoldsNumber = CalculateReynoldsNumber(velocity, characteristicLength);
		return reynoldsNumber > threshold;
	}

	/// <summary>Determines if this kinematic viscosity is typical for water at room temperature.</summary>
	/// <returns>True if the value is close to water's kinematic viscosity (≈ 1.0 × 10⁻⁶ m²/s at 20°C).</returns>
	public bool IsTypicalForWater()
	{
		T viscosity = In(Units.Meter);
		T waterViscosity = T.CreateChecked(1.0e-6); // m²/s at 20°C
		T tolerance = T.CreateChecked(0.5e-6);
		T difference = T.Abs(viscosity - waterViscosity);
		return difference <= tolerance;
	}

	/// <summary>Determines if this kinematic viscosity is typical for air at standard conditions.</summary>
	/// <returns>True if the value is close to air's kinematic viscosity (≈ 1.5 × 10⁻⁵ m²/s at 20°C).</returns>
	public bool IsTypicalForAir()
	{
		T viscosity = In(Units.Meter);
		T airViscosity = T.CreateChecked(1.5e-5); // m²/s at 20°C
		T tolerance = T.CreateChecked(0.5e-5);
		T difference = T.Abs(viscosity - airViscosity);
		return difference <= tolerance;
	}
}
