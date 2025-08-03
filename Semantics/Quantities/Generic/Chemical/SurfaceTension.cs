// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents surface tension with a specific unit of measurement.
/// Surface tension is the force per unit length acting on the surface of a liquid.
/// </summary>
/// <typeparam name="T">The numeric type for the surface tension value.</typeparam>
public record SurfaceTension<T> : PhysicalQuantity<SurfaceTension<T>, T>
	where T : struct, INumber<T>, IFloatingPoint<T>
{
	/// <summary>Gets the physical dimension of surface tension [M T⁻²].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.SurfaceTension;

	/// <summary>Initializes a new instance of the <see cref="SurfaceTension{T}"/> class.</summary>
	public SurfaceTension() : base() { }

	/// <summary>Calculates surface tension from force and length.</summary>
	/// <param name="force">Force acting on the surface.</param>
	/// <param name="length">Length over which the force acts.</param>
	/// <returns>The surface tension.</returns>
	public static SurfaceTension<T> FromForceAndLength(Force<T> force, Length<T> length)
	{
		ArgumentNullException.ThrowIfNull(force);
		ArgumentNullException.ThrowIfNull(length);

		T forceValue = force.In(Units.Newton);
		T lengthValue = length.In(Units.Meter);
		T tension = forceValue / lengthValue;
		return Create(tension);
	}

	/// <summary>Calculates surface energy from surface tension and area change.</summary>
	/// <param name="areaChange">Change in surface area.</param>
	/// <returns>The surface energy.</returns>
	public Energy<T> CalculateSurfaceEnergy(Area<T> areaChange)
	{
		ArgumentNullException.ThrowIfNull(areaChange);

		T tensionValue = In(Units.NewtonPerMeter);
		T areaValue = areaChange.In(Units.SquareMeter);
		T energy = tensionValue * areaValue;
		return Energy<T>.Create(energy);
	}

	/// <summary>Common surface tension values for various liquids.</summary>
	public static class CommonValues
	{
		/// <summary>Water at 20°C: 0.0728 N/m.</summary>
		public static SurfaceTension<T> Water => Create(T.CreateChecked(0.0728));

		/// <summary>Mercury at 20°C: 0.486 N/m.</summary>
		public static SurfaceTension<T> Mercury => Create(T.CreateChecked(0.486));

		/// <summary>Ethanol at 20°C: 0.0223 N/m.</summary>
		public static SurfaceTension<T> Ethanol => Create(T.CreateChecked(0.0223));

		/// <summary>Benzene at 20°C: 0.0289 N/m.</summary>
		public static SurfaceTension<T> Benzene => Create(T.CreateChecked(0.0289));

		/// <summary>Olive oil at 20°C: 0.032 N/m.</summary>
		public static SurfaceTension<T> OliveOil => Create(T.CreateChecked(0.032));

		/// <summary>Soap solution: ~0.025 N/m (reduced from water).</summary>
		public static SurfaceTension<T> SoapSolution => Create(T.CreateChecked(0.025));
	}

	/// <summary>Calculates capillary rise height using Young-Laplace equation.</summary>
	/// <param name="contactAngle">Contact angle with the surface.</param>
	/// <param name="tubeRadius">Radius of the capillary tube.</param>
	/// <param name="liquidDensity">Density of the liquid.</param>
	/// <param name="gravity">Gravitational acceleration.</param>
	/// <returns>Height of capillary rise.</returns>
	public Length<T> CalculateCapillaryRise(T contactAngle, Length<T> tubeRadius,
		Density<T> liquidDensity, Acceleration<T> gravity)
	{
		ArgumentNullException.ThrowIfNull(tubeRadius);
		ArgumentNullException.ThrowIfNull(liquidDensity);
		ArgumentNullException.ThrowIfNull(gravity);

		T gamma = In(Units.NewtonPerMeter);
		T cosTheta = T.CreateChecked(Math.Cos(double.CreateChecked(contactAngle)));
		T r = tubeRadius.In(Units.Meter);
		T rho = liquidDensity.In(Units.Gram); // kg/m³
		T g = gravity.In(Units.MetersPerSecondSquared);

		T height = T.CreateChecked(2) * gamma * cosTheta / (rho * g * r);
		return Length<T>.Create(height);
	}
}
