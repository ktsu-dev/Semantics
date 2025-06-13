// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents refractive index with a specific unit of measurement.
/// Refractive index is a dimensionless number that describes how fast light travels through a material.
/// It is the ratio of the speed of light in vacuum to the speed of light in the material.
/// </summary>
/// <typeparam name="T">The numeric type for the refractive index value.</typeparam>
public sealed record RefractiveIndex<T> : PhysicalQuantity<RefractiveIndex<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of refractive index [1] (dimensionless).</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.RefractiveIndex;

	/// <summary>Initializes a new instance of the <see cref="RefractiveIndex{T}"/> class.</summary>
	public RefractiveIndex() : base() { }

	/// <summary>Creates a new refractive index from a dimensionless value.</summary>
	/// <param name="value">The refractive index value.</param>
	/// <returns>A new RefractiveIndex instance.</returns>
	public static RefractiveIndex<T> FromValue(T value) => Create(value);

	/// <summary>Creates a refractive index for vacuum (n = 1.0 exactly).</summary>
	/// <returns>A new RefractiveIndex instance for vacuum.</returns>
	public static RefractiveIndex<T> Vacuum() => Create(T.One);

	/// <summary>Creates a refractive index for air at standard conditions (n ≈ 1.000293).</summary>
	/// <returns>A new RefractiveIndex instance for air.</returns>
	public static RefractiveIndex<T> Air() => Create(T.CreateChecked(1.000293));

	/// <summary>Creates a refractive index for water at 20°C (n ≈ 1.333).</summary>
	/// <returns>A new RefractiveIndex instance for water.</returns>
	public static RefractiveIndex<T> Water() => Create(T.CreateChecked(1.333));

	/// <summary>Creates a refractive index for crown glass (n ≈ 1.52).</summary>
	/// <returns>A new RefractiveIndex instance for crown glass.</returns>
	public static RefractiveIndex<T> CrownGlass() => Create(T.CreateChecked(1.52));

	/// <summary>Creates a refractive index for diamond (n ≈ 2.42).</summary>
	/// <returns>A new RefractiveIndex instance for diamond.</returns>
	public static RefractiveIndex<T> Diamond() => Create(T.CreateChecked(2.42));

	/// <summary>Calculates the critical angle for total internal reflection.</summary>
	/// <param name="externalMedium">The refractive index of the external medium.</param>
	/// <returns>The critical angle in radians.</returns>
	/// <remarks>
	/// Uses Snell's law: sin(θc) = n₂/n₁
	/// where θc is the critical angle, n₁ is this medium, and n₂ is the external medium.
	/// Only valid when n₁ > n₂.
	/// </remarks>
	public T CalculateCriticalAngle(RefractiveIndex<T> externalMedium)
	{
		ArgumentNullException.ThrowIfNull(externalMedium);

		T n1 = In(Units.Radian);
		T n2 = externalMedium.In(Units.Radian);

		if (n2 >= n1)
		{
			throw new ArgumentException("Critical angle only exists when the internal medium has a higher refractive index than the external medium.");
		}

		T sinCritical = n2 / n1;
		return T.CreateChecked(Math.Asin(double.CreateChecked(sinCritical)));
	}

	/// <summary>Calculates the reflection coefficient at normal incidence (Fresnel reflection).</summary>
	/// <param name="externalMedium">The refractive index of the external medium.</param>
	/// <returns>The reflection coefficient (fraction of light reflected).</returns>
	/// <remarks>
	/// Uses the Fresnel equation for normal incidence: R = ((n₁-n₂)/(n₁+n₂))²
	/// where R is reflectance, n₁ is this medium, and n₂ is the external medium.
	/// </remarks>
	public T CalculateReflectionCoefficient(RefractiveIndex<T> externalMedium)
	{
		ArgumentNullException.ThrowIfNull(externalMedium);

		T n1 = In(Units.Radian);
		T n2 = externalMedium.In(Units.Radian);
		T numerator = n1 - n2;
		T denominator = n1 + n2;
		return (numerator * numerator) / (denominator * denominator);
	}

	/// <summary>Calculates the speed of light in this medium.</summary>
	/// <returns>The speed of light in this medium.</returns>
	/// <remarks>
	/// Uses the relationship: v = c / n
	/// where v is the speed in the medium, c is the speed of light in vacuum, and n is the refractive index.
	/// </remarks>
	public Velocity<T> CalculateSpeedOfLight()
	{
		T n = In(Units.Radian);
		T speedOfLightInVacuum = PhysicalConstants.Generic.SpeedOfLight<T>();
		T speedInMedium = speedOfLightInVacuum / n;
		return Velocity<T>.Create(speedInMedium);
	}
}
