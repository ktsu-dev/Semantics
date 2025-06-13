// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents volumetric flow rate with a specific unit of measurement.
/// Volumetric flow rate is the volume of fluid that passes per unit time.
/// It is measured in cubic meters per second (m³/s) in the SI system.
/// </summary>
/// <typeparam name="T">The numeric type for the volumetric flow rate value.</typeparam>
public sealed record VolumetricFlowRate<T> : PhysicalQuantity<VolumetricFlowRate<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of volumetric flow rate [L³ T⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.VolumetricFlowRate;

	/// <summary>Initializes a new instance of the <see cref="VolumetricFlowRate{T}"/> class.</summary>
	public VolumetricFlowRate() : base() { }

	/// <summary>Creates a new volumetric flow rate from a value in cubic meters per second.</summary>
	/// <param name="cubicMetersPerSecond">The volumetric flow rate in m³/s.</param>
	/// <returns>A new VolumetricFlowRate instance.</returns>
	public static VolumetricFlowRate<T> FromCubicMetersPerSecond(T cubicMetersPerSecond) => Create(cubicMetersPerSecond);

	/// <summary>Creates a new volumetric flow rate from a value in liters per second.</summary>
	/// <param name="litersPerSecond">The volumetric flow rate in L/s.</param>
	/// <returns>A new VolumetricFlowRate instance.</returns>
	public static VolumetricFlowRate<T> FromLitersPerSecond(T litersPerSecond)
	{
		T cubicMetersPerSecond = litersPerSecond * T.CreateChecked(0.001);
		return Create(cubicMetersPerSecond);
	}

	/// <summary>Creates a new volumetric flow rate from a value in liters per minute.</summary>
	/// <param name="litersPerMinute">The volumetric flow rate in L/min.</param>
	/// <returns>A new VolumetricFlowRate instance.</returns>
	public static VolumetricFlowRate<T> FromLitersPerMinute(T litersPerMinute)
	{
		T cubicMetersPerSecond = litersPerMinute * T.CreateChecked(0.001 / 60);
		return Create(cubicMetersPerSecond);
	}

	/// <summary>Creates a new volumetric flow rate from a value in gallons per minute.</summary>
	/// <param name="gallonsPerMinute">The volumetric flow rate in GPM (US gallons).</param>
	/// <returns>A new VolumetricFlowRate instance.</returns>
	public static VolumetricFlowRate<T> FromGallonsPerMinute(T gallonsPerMinute)
	{
		T cubicMetersPerSecond = gallonsPerMinute * T.CreateChecked(3.78541e-3 / 60);
		return Create(cubicMetersPerSecond);
	}

	/// <summary>Creates a new volumetric flow rate from velocity and cross-sectional area.</summary>
	/// <param name="velocity">The average flow velocity.</param>
	/// <param name="area">The cross-sectional area.</param>
	/// <returns>A new VolumetricFlowRate instance.</returns>
	/// <remarks>
	/// Uses the relationship: Q = v × A
	/// where Q is volumetric flow rate, v is velocity, and A is area.
	/// </remarks>
	public static VolumetricFlowRate<T> FromVelocityAndArea(Velocity<T> velocity, Area<T> area)
	{
		ArgumentNullException.ThrowIfNull(velocity);
		ArgumentNullException.ThrowIfNull(area);

		T v = velocity.In(Units.MetersPerSecond);
		T a = area.In(Units.SquareMeter);
		T flowRate = v * a;
		return Create(flowRate);
	}

	/// <summary>Calculates the average velocity from flow rate and cross-sectional area.</summary>
	/// <param name="area">The cross-sectional area.</param>
	/// <returns>The average flow velocity.</returns>
	/// <remarks>
	/// Uses the relationship: v = Q / A
	/// where v is velocity, Q is volumetric flow rate, and A is area.
	/// </remarks>
	public Velocity<T> CalculateVelocity(Area<T> area)
	{
		ArgumentNullException.ThrowIfNull(area);

		T q = In(Units.CubicMeter);
		T a = area.In(Units.SquareMeter);

		if (a == T.Zero)
		{
			throw new ArgumentException("Area cannot be zero.", nameof(area));
		}

		T velocity = q / a;
		return Velocity<T>.Create(velocity);
	}

	/// <summary>Calculates the cross-sectional area from flow rate and velocity.</summary>
	/// <param name="velocity">The average flow velocity.</param>
	/// <returns>The cross-sectional area.</returns>
	/// <remarks>
	/// Uses the relationship: A = Q / v
	/// where A is area, Q is volumetric flow rate, and v is velocity.
	/// </remarks>
	public Area<T> CalculateArea(Velocity<T> velocity)
	{
		ArgumentNullException.ThrowIfNull(velocity);

		T q = In(Units.CubicMeter);
		T v = velocity.In(Units.MetersPerSecond);

		if (v == T.Zero)
		{
			throw new ArgumentException("Velocity cannot be zero.", nameof(velocity));
		}

		T area = q / v;
		return Area<T>.Create(area);
	}

	/// <summary>Calculates mass flow rate from volumetric flow rate and density.</summary>
	/// <param name="density">The fluid density.</param>
	/// <returns>The mass flow rate.</returns>
	/// <remarks>
	/// Uses the relationship: ṁ = ρ × Q
	/// where ṁ is mass flow rate, ρ is density, and Q is volumetric flow rate.
	/// </remarks>
	public MassFlowRate<T> CalculateMassFlowRate(Density<T> density)
	{
		ArgumentNullException.ThrowIfNull(density);

		T q = In(Units.CubicMeter);
		T rho = density.In(Units.Kilogram);
		T massFlowRate = rho * q;

		return MassFlowRate<T>.Create(massFlowRate);
	}

	/// <summary>Calculates the time to fill a given volume.</summary>
	/// <param name="volume">The volume to fill.</param>
	/// <returns>The time required to fill the volume.</returns>
	/// <remarks>
	/// Uses the relationship: t = V / Q
	/// where t is time, V is volume, and Q is volumetric flow rate.
	/// </remarks>
	public Time<T> CalculateFillTime(Volume<T> volume)
	{
		ArgumentNullException.ThrowIfNull(volume);

		T v = volume.In(Units.CubicMeter);
		T q = In(Units.CubicMeter);

		if (q == T.Zero)
		{
			throw new InvalidOperationException("Flow rate cannot be zero for time calculation.");
		}

		T time = v / q;
		return Time<T>.Create(time);
	}

	/// <summary>Calculates the volume delivered over a given time period.</summary>
	/// <param name="time">The time period.</param>
	/// <returns>The volume delivered.</returns>
	/// <remarks>
	/// Uses the relationship: V = Q × t
	/// where V is volume, Q is volumetric flow rate, and t is time.
	/// </remarks>
	public Volume<T> CalculateVolumeDelivered(Time<T> time)
	{
		ArgumentNullException.ThrowIfNull(time);

		T q = In(Units.CubicMeter);
		T t = time.In(Units.Second);
		T volume = q * t;

		return Volume<T>.Create(volume);
	}

	/// <summary>Determines if this flow rate is typical for household plumbing.</summary>
	/// <returns>True if the flow rate is between 0.1 and 0.5 L/s (typical for taps and showers).</returns>
	public bool IsTypicalHouseholdFlow()
	{
		T flowRate = In(Units.CubicMeter);
		T lowerBound = T.CreateChecked(0.0001); // 0.1 L/s in m³/s
		T upperBound = T.CreateChecked(0.0005); // 0.5 L/s in m³/s
		return flowRate >= lowerBound && flowRate <= upperBound;
	}

	/// <summary>Determines if this flow rate is typical for industrial processes.</summary>
	/// <returns>True if the flow rate is greater than 1 L/s.</returns>
	public bool IsTypicalIndustrialFlow()
	{
		T flowRate = In(Units.CubicMeter);
		T threshold = T.CreateChecked(0.001); // 1 L/s in m³/s
		return flowRate > threshold;
	}
}
