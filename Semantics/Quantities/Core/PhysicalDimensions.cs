// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Collections.Frozen;

/// <inheritdoc/>
public static class PhysicalDimensions
{
	/// <summary>Dimensionless quantity [1].</summary>
	public static readonly PhysicalDimension Dimensionless;

	/// <summary>Length dimension [L].</summary>
	public static readonly PhysicalDimension Length = new(length: 1);

	/// <summary>Mass dimension [M].</summary>
	public static readonly PhysicalDimension Mass = new(mass: 1);

	/// <summary>Time dimension [T].</summary>
	public static readonly PhysicalDimension Time = new(time: 1);

	/// <summary>Electric current dimension [I].</summary>
	public static readonly PhysicalDimension ElectricCurrent = new(electricCurrent: 1);

	/// <summary>Temperature dimension [Θ].</summary>
	public static readonly PhysicalDimension Temperature = new(temperature: 1);

	/// <summary>Amount of substance dimension [N].</summary>
	public static readonly PhysicalDimension AmountOfSubstance = new(amountOfSubstance: 1);

	/// <summary>Luminous intensity dimension [J].</summary>
	public static readonly PhysicalDimension LuminousIntensity = new(luminousIntensity: 1);

	/// <summary>Area dimension [L²].</summary>
	public static readonly PhysicalDimension Area = new(length: 2);

	/// <summary>Volume dimension [L³].</summary>
	public static readonly PhysicalDimension Volume = new(length: 3);

	/// <summary>Velocity dimension [L T⁻¹].</summary>
	public static readonly PhysicalDimension Velocity = new(length: 1, time: -1);

	/// <summary>Acceleration dimension [L T⁻²].</summary>
	public static readonly PhysicalDimension Acceleration = new(length: 1, time: -2);

	/// <summary>Force dimension [M L T⁻²].</summary>
	public static readonly PhysicalDimension Force = new(mass: 1, length: 1, time: -2);

	/// <summary>Pressure dimension [M L⁻¹ T⁻²].</summary>
	public static readonly PhysicalDimension Pressure = new(mass: 1, length: -1, time: -2);

	/// <summary>Energy dimension [M L² T⁻²].</summary>
	public static readonly PhysicalDimension Energy = new(mass: 1, length: 2, time: -2);

	/// <summary>Power dimension [M L² T⁻³].</summary>
	public static readonly PhysicalDimension Power = new(mass: 1, length: 2, time: -3);

	/// <summary>Electric potential dimension [M L² T⁻³ I⁻¹].</summary>
	public static readonly PhysicalDimension ElectricPotential = new(mass: 1, length: 2, time: -3, electricCurrent: -1);

	/// <summary>Electric resistance dimension [M L² T⁻³ I⁻²].</summary>
	public static readonly PhysicalDimension ElectricResistance = new(mass: 1, length: 2, time: -3, electricCurrent: -2);

	/// <summary>Electric charge dimension [I T].</summary>
	public static readonly PhysicalDimension ElectricCharge = new(electricCurrent: 1, time: 1);

	/// <summary>Electric capacitance dimension [M⁻¹ L⁻² T⁴ I²].</summary>
	public static readonly PhysicalDimension ElectricCapacitance = new(mass: -1, length: -2, time: 4, electricCurrent: 2);

	/// <summary>Frequency dimension [T⁻¹].</summary>
	public static readonly PhysicalDimension Frequency = new(time: -1);

	/// <summary>Momentum dimension [M L T⁻¹].</summary>
	public static readonly PhysicalDimension Momentum = new(mass: 1, length: 1, time: -1);

	/// <summary>Angular velocity dimension [T⁻¹].</summary>
	public static readonly PhysicalDimension AngularVelocity = new(time: -1);

	/// <summary>Angular acceleration dimension [T⁻²].</summary>
	public static readonly PhysicalDimension AngularAcceleration = new(time: -2);

	/// <summary>Torque dimension [M L² T⁻²].</summary>
	public static readonly PhysicalDimension Torque = new(mass: 1, length: 2, time: -2);

	/// <summary>Moment of inertia dimension [M L²].</summary>
	public static readonly PhysicalDimension MomentOfInertia = new(mass: 1, length: 2);

	/// <summary>Density dimension [M L⁻³].</summary>
	public static readonly PhysicalDimension Density = new(mass: 1, length: -3);

	/// <summary>Concentration dimension [N L⁻³].</summary>
	public static readonly PhysicalDimension Concentration = new(amountOfSubstance: 1, length: -3);

	/// <summary>Gets a frozen collection of all standard physical dimensions.</summary>
	public static FrozenSet<PhysicalDimension> All { get; } = new HashSet<PhysicalDimension>(
		[
			Dimensionless,
			Length,
			Mass,
			Time,
			ElectricCurrent,
			Temperature,
			AmountOfSubstance,
			LuminousIntensity,
			Area,
			Volume,
			Velocity,
			Acceleration,
			Force,
			Pressure,
			Energy,
			Power,
			ElectricPotential,
			ElectricResistance,
			ElectricCharge,
			ElectricCapacitance,
			Frequency,
			Momentum,
			AngularVelocity,
			AngularAcceleration,
			Torque,
			MomentOfInertia,
			Density,
			Concentration
		]).ToFrozenSet();
}
