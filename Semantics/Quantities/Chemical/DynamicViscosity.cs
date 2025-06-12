// Copyright (c) KTSU. All rights reserved.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents dynamic viscosity with a specific unit of measurement.
/// Dynamic viscosity measures a fluid's resistance to flow.
/// </summary>
/// <typeparam name="T">The numeric type for the dynamic viscosity value.</typeparam>
public sealed record DynamicViscosity<T> : PhysicalQuantity<DynamicViscosity<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of dynamic viscosity [M L⁻¹ T⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.DynamicViscosity;

	/// <summary>Initializes a new instance of the <see cref="DynamicViscosity{T}"/> class.</summary>
	public DynamicViscosity() : base() { }

	/// <summary>Calculates dynamic viscosity from shear stress and shear rate.</summary>
	/// <param name="shearStress">Shear stress in the fluid.</param>
	/// <param name="shearRate">Shear rate (velocity gradient).</param>
	/// <returns>The dynamic viscosity.</returns>
	public static DynamicViscosity<T> FromShearStressAndRate(Pressure<T> shearStress, T shearRate)
	{
		ArgumentNullException.ThrowIfNull(shearStress);

		T stress = shearStress.In(Units.Pascal);
		T viscosity = stress / shearRate;
		return Create(viscosity);
	}

	/// <summary>Calculates kinematic viscosity from dynamic viscosity and density.</summary>
	/// <param name="density">Fluid density.</param>
	/// <returns>The kinematic viscosity.</returns>
	public T CalculateKinematicViscosity(Density<T> density)
	{
		ArgumentNullException.ThrowIfNull(density);

		T dynamicVisc = In(Units.PascalSecond);
		T rho = density.In(Units.Gram); // kg/m³
		return dynamicVisc / rho;
	}

	/// <summary>Common dynamic viscosity values for various fluids.</summary>
	public static class CommonValues
	{
		/// <summary>Water at 20°C: 1.002 × 10⁻³ Pa·s.</summary>
		public static DynamicViscosity<T> Water => Create(T.CreateChecked(1.002e-3));

		/// <summary>Air at 20°C: 1.82 × 10⁻⁵ Pa·s.</summary>
		public static DynamicViscosity<T> Air => Create(T.CreateChecked(1.82e-5));

		/// <summary>Honey: ~10 Pa·s.</summary>
		public static DynamicViscosity<T> Honey => Create(T.CreateChecked(10));

		/// <summary>Motor oil SAE 30: ~0.2 Pa·s.</summary>
		public static DynamicViscosity<T> MotorOil => Create(T.CreateChecked(0.2));

		/// <summary>Glycerol at 20°C: 1.41 Pa·s.</summary>
		public static DynamicViscosity<T> Glycerol => Create(T.CreateChecked(1.41));

		/// <summary>Blood at 37°C: ~4 × 10⁻³ Pa·s.</summary>
		public static DynamicViscosity<T> Blood => Create(T.CreateChecked(4e-3));

		/// <summary>Mercury at 20°C: 1.55 × 10⁻³ Pa·s.</summary>
		public static DynamicViscosity<T> Mercury => Create(T.CreateChecked(1.55e-3));
	}

	/// <summary>Calculates Reynolds number for flow characterization.</summary>
	/// <param name="density">Fluid density.</param>
	/// <param name="velocity">Flow velocity.</param>
	/// <param name="characteristicLength">Characteristic length.</param>
	/// <returns>Reynolds number (dimensionless).</returns>
	public T CalculateReynoldsNumber(Density<T> density, Velocity<T> velocity, Length<T> characteristicLength)
	{
		ArgumentNullException.ThrowIfNull(density);
		ArgumentNullException.ThrowIfNull(velocity);
		ArgumentNullException.ThrowIfNull(characteristicLength);

		T rho = density.In(Units.Gram); // kg/m³
		T v = velocity.In(Units.MetersPerSecond);
		T l = characteristicLength.In(Units.Meter);
		T mu = In(Units.PascalSecond);

		return rho * v * l / mu;
	}
}
