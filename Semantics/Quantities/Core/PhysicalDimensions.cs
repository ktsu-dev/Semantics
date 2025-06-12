// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Collections.Frozen;

/// <inheritdoc/>
public static class PhysicalDimensions
{
	/// <summary>Dimensionless quantity [1].</summary>
	public static readonly PhysicalDimension Dimensionless = new(baseUnit: Units.Radian);

	/// <summary>Length dimension [L].</summary>
	public static readonly PhysicalDimension Length = new(baseUnit: Units.Meter, length: 1);

	/// <summary>Mass dimension [M].</summary>
	public static readonly PhysicalDimension Mass = new(baseUnit: Units.Kilogram, mass: 1);

	/// <summary>Time dimension [T].</summary>
	public static readonly PhysicalDimension Time = new(baseUnit: Units.Second, time: 1);

	/// <summary>Electric current dimension [I].</summary>
	public static readonly PhysicalDimension ElectricCurrent = new(baseUnit: Units.Ampere, electricCurrent: 1);

	/// <summary>Temperature dimension [Θ].</summary>
	public static readonly PhysicalDimension Temperature = new(baseUnit: Units.Kelvin, temperature: 1);

	/// <summary>Amount of substance dimension [N].</summary>
	public static readonly PhysicalDimension AmountOfSubstance = new(baseUnit: Units.Mole, amountOfSubstance: 1);

	/// <summary>Luminous intensity dimension [J].</summary>
	public static readonly PhysicalDimension LuminousIntensity = new(baseUnit: Units.Candela, luminousIntensity: 1);

	/// <summary>Area dimension [L²].</summary>
	public static readonly PhysicalDimension Area = new(baseUnit: Units.SquareMeter, length: 2);

	/// <summary>Volume dimension [L³].</summary>
	public static readonly PhysicalDimension Volume = new(baseUnit: Units.CubicMeter, length: 3);

	/// <summary>Velocity dimension [L T⁻¹].</summary>
	public static readonly PhysicalDimension Velocity = new(baseUnit: Units.MetersPerSecond, length: 1, time: -1);

	/// <summary>Acceleration dimension [L T⁻²].</summary>
	public static readonly PhysicalDimension Acceleration = new(baseUnit: Units.MetersPerSecondSquared, length: 1, time: -2);

	/// <summary>Force dimension [M L T⁻²].</summary>
	public static readonly PhysicalDimension Force = new(mass: 1, length: 1, time: -2, baseUnit: Units.Newton);

	/// <summary>Pressure dimension [M L⁻¹ T⁻²].</summary>
	public static readonly PhysicalDimension Pressure = new(mass: 1, length: -1, time: -2, baseUnit: Units.Pascal);

	/// <summary>Energy dimension [M L² T⁻²].</summary>
	public static readonly PhysicalDimension Energy = new(mass: 1, length: 2, time: -2, baseUnit: Units.Joule);

	/// <summary>Power dimension [M L² T⁻³].</summary>
	public static readonly PhysicalDimension Power = new(mass: 1, length: 2, time: -3, baseUnit: Units.Watt);

	/// <summary>Electric potential dimension [M L² T⁻³ I⁻¹].</summary>
	public static readonly PhysicalDimension ElectricPotential = new(mass: 1, length: 2, time: -3, electricCurrent: -1, baseUnit: Units.Volt);

	/// <summary>Electric resistance dimension [M L² T⁻³ I⁻²].</summary>
	public static readonly PhysicalDimension ElectricResistance = new(mass: 1, length: 2, time: -3, electricCurrent: -2, baseUnit: Units.Ohm);

	/// <summary>Electric charge dimension [I T].</summary>
	public static readonly PhysicalDimension ElectricCharge = new(electricCurrent: 1, time: 1, baseUnit: Units.Coulomb);

	/// <summary>Electric capacitance dimension [M⁻¹ L⁻² T⁴ I²].</summary>
	public static readonly PhysicalDimension ElectricCapacitance = new(mass: -1, length: -2, time: 4, electricCurrent: 2, baseUnit: Units.Farad);

	/// <summary>Frequency dimension [T⁻¹].</summary>
	public static readonly PhysicalDimension Frequency = new(baseUnit: Units.Hertz, time: -1);

	/// <summary>Momentum dimension [M L T⁻¹].</summary>
	public static readonly PhysicalDimension Momentum = new(mass: 1, length: 1, time: -1, baseUnit: Units.Newton);

	/// <summary>Angular velocity dimension [T⁻¹].</summary>
	public static readonly PhysicalDimension AngularVelocity = new(baseUnit: Units.Radian, time: -1);

	/// <summary>Angular acceleration dimension [T⁻²].</summary>
	public static readonly PhysicalDimension AngularAcceleration = new(baseUnit: Units.Radian, time: -2);

	/// <summary>Torque dimension [M L² T⁻²].</summary>
	public static readonly PhysicalDimension Torque = new(mass: 1, length: 2, time: -2, baseUnit: Units.Newton);

	/// <summary>Moment of inertia dimension [M L²].</summary>
	public static readonly PhysicalDimension MomentOfInertia = new(mass: 1, length: 2, baseUnit: Units.Kilogram);

	/// <summary>Density dimension [M L⁻³].</summary>
	public static readonly PhysicalDimension Density = new(mass: 1, length: -3, baseUnit: Units.Kilogram);

	/// <summary>Concentration dimension [N L⁻³].</summary>
	public static readonly PhysicalDimension Concentration = new(amountOfSubstance: 1, length: -3, baseUnit: Units.Molar);

	/// <summary>Electric field dimension [M L T⁻³ I⁻¹].</summary>
	public static readonly PhysicalDimension ElectricField = new(mass: 1, length: 1, time: -3, electricCurrent: -1, baseUnit: Units.Volt);

	/// <summary>Electric flux dimension [M L³ T⁻³ I⁻¹].</summary>
	public static readonly PhysicalDimension ElectricFlux = new(mass: 1, length: 3, time: -3, electricCurrent: -1, baseUnit: Units.Volt);

	/// <summary>Permittivity dimension [M⁻¹ L⁻³ T⁴ I²].</summary>
	public static readonly PhysicalDimension Permittivity = new(mass: -1, length: -3, time: 4, electricCurrent: 2, baseUnit: Units.Farad);

	/// <summary>Electric conductivity dimension [M⁻¹ L⁻³ T³ I²].</summary>
	public static readonly PhysicalDimension ElectricConductivity = new(mass: -1, length: -3, time: 3, electricCurrent: 2, baseUnit: Units.Ohm);

	/// <summary>Electric power density dimension [M L⁻¹ T⁻³].</summary>
	public static readonly PhysicalDimension ElectricPowerDensity = new(mass: 1, length: -1, time: -3, baseUnit: Units.Watt);

	/// <summary>AC impedance dimension [M L² T⁻³ I⁻²].</summary>
	public static readonly PhysicalDimension ImpedanceAC = new(mass: 1, length: 2, time: -3, electricCurrent: -2, baseUnit: Units.Ohm);

	/// <summary>Sound pressure dimension [M L⁻¹ T⁻²].</summary>
	public static readonly PhysicalDimension SoundPressure = new(mass: 1, length: -1, time: -2, baseUnit: Units.Pascal);

	/// <summary>Sound intensity dimension [M T⁻³].</summary>
	public static readonly PhysicalDimension SoundIntensity = new(mass: 1, time: -3, baseUnit: Units.Watt);

	/// <summary>Sound power dimension [M L² T⁻³].</summary>
	public static readonly PhysicalDimension SoundPower = new(mass: 1, length: 2, time: -3, baseUnit: Units.Watt);

	/// <summary>Acoustic impedance dimension [M L⁻² T⁻¹].</summary>
	public static readonly PhysicalDimension AcousticImpedance = new(mass: 1, length: -2, time: -1, baseUnit: Units.Pascal);

	/// <summary>Sound speed dimension [L T⁻¹].</summary>
	public static readonly PhysicalDimension SoundSpeed = new(length: 1, time: -1, baseUnit: Units.MetersPerSecond);

	/// <summary>Sound absorption coefficient dimension [1].</summary>
	public static readonly PhysicalDimension SoundAbsorption = new(baseUnit: Units.Radian);

	/// <summary>Reverberation time dimension [T].</summary>
	public static readonly PhysicalDimension ReverberationTime = new(time: 1, baseUnit: Units.Second);

	/// <summary>Wavelength dimension [L].</summary>
	public static readonly PhysicalDimension Wavelength = new(length: 1, baseUnit: Units.Meter);

	/// <summary>Wave number dimension [L⁻¹].</summary>
	public static readonly PhysicalDimension WaveNumber = new(length: -1, baseUnit: Units.Meter);

	/// <summary>Heat dimension [M L² T⁻²].</summary>
	public static readonly PhysicalDimension Heat = new(mass: 1, length: 2, time: -2, baseUnit: Units.Joule);

	/// <summary>Entropy dimension [M L² T⁻² Θ⁻¹].</summary>
	public static readonly PhysicalDimension Entropy = new(mass: 1, length: 2, time: -2, temperature: -1, baseUnit: Units.Joule);

	/// <summary>Thermal conductivity dimension [M L T⁻³ Θ⁻¹].</summary>
	public static readonly PhysicalDimension ThermalConductivity = new(mass: 1, length: 1, time: -3, temperature: -1, baseUnit: Units.Watt);

	/// <summary>Heat capacity dimension [M L² T⁻² Θ⁻¹].</summary>
	public static readonly PhysicalDimension HeatCapacity = new(mass: 1, length: 2, time: -2, temperature: -1, baseUnit: Units.Joule);

	/// <summary>Specific heat dimension [L² T⁻² Θ⁻¹].</summary>
	public static readonly PhysicalDimension SpecificHeat = new(length: 2, time: -2, temperature: -1, baseUnit: Units.Joule);

	/// <summary>Heat transfer coefficient dimension [M T⁻³ Θ⁻¹].</summary>
	public static readonly PhysicalDimension HeatTransferCoefficient = new(mass: 1, time: -3, temperature: -1, baseUnit: Units.Watt);

	/// <summary>Thermal expansion coefficient dimension [Θ⁻¹].</summary>
	public static readonly PhysicalDimension ThermalExpansion = new(temperature: -1, baseUnit: Units.Kelvin);

	/// <summary>Thermal diffusivity dimension [L² T⁻¹].</summary>
	public static readonly PhysicalDimension ThermalDiffusivity = new(length: 2, time: -1, baseUnit: Units.Meter);

	/// <summary>Thermal resistance dimension [Θ T³ M⁻¹ L⁻²].</summary>
	public static readonly PhysicalDimension ThermalResistance = new(temperature: 1, time: 3, mass: -1, length: -2, baseUnit: Units.Kelvin);

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
			Concentration,
			ElectricField,
			ElectricFlux,
			Permittivity,
			ElectricConductivity,
			ElectricPowerDensity,
			ImpedanceAC,
			SoundPressure,
			SoundIntensity,
			SoundPower,
			AcousticImpedance,
			SoundSpeed,
			SoundAbsorption,
			ReverberationTime,
			Wavelength,
			WaveNumber,
			Heat,
			Entropy,
			ThermalConductivity,
			HeatCapacity,
			SpecificHeat,
			HeatTransferCoefficient,
			ThermalExpansion,
			ThermalDiffusivity,
			ThermalResistance
		]).ToFrozenSet();
}
