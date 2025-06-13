// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Collections.Frozen;

/// <inheritdoc/>
public static class PhysicalDimensions
{
	/// <summary>Dimensionless quantity [1].</summary>
	public static readonly PhysicalDimension Dimensionless = new(baseUnit: BootstrapUnits.Dimensionless);

	/// <summary>Length dimension [L].</summary>
	public static readonly PhysicalDimension Length = new(baseUnit: BootstrapUnits.Meter, length: 1);

	/// <summary>Mass dimension [M].</summary>
	public static readonly PhysicalDimension Mass = new(baseUnit: BootstrapUnits.Kilogram, mass: 1);

	/// <summary>Time dimension [T].</summary>
	public static readonly PhysicalDimension Time = new(baseUnit: BootstrapUnits.Second, time: 1);

	/// <summary>Electric current dimension [I].</summary>
	public static readonly PhysicalDimension ElectricCurrent = new(baseUnit: BootstrapUnits.Ampere, electricCurrent: 1);

	/// <summary>Temperature dimension [Θ].</summary>
	public static readonly PhysicalDimension Temperature = new(baseUnit: BootstrapUnits.Kelvin, temperature: 1);

	/// <summary>Amount of substance dimension [N].</summary>
	public static readonly PhysicalDimension AmountOfSubstance = new(baseUnit: BootstrapUnits.Mole, amountOfSubstance: 1);

	/// <summary>Luminous intensity dimension [J].</summary>
	public static readonly PhysicalDimension LuminousIntensity = new(baseUnit: BootstrapUnits.Candela, luminousIntensity: 1);

	/// <summary>Area dimension [L²].</summary>
	public static readonly PhysicalDimension Area = new(baseUnit: BootstrapUnits.SquareMeter, length: 2);

	/// <summary>Volume dimension [L³].</summary>
	public static readonly PhysicalDimension Volume = new(baseUnit: BootstrapUnits.CubicMeter, length: 3);

	/// <summary>Velocity dimension [L T⁻¹].</summary>
	public static readonly PhysicalDimension Velocity = new(baseUnit: BootstrapUnits.MetersPerSecond, length: 1, time: -1);

	/// <summary>Acceleration dimension [L T⁻²].</summary>
	public static readonly PhysicalDimension Acceleration = new(baseUnit: BootstrapUnits.MetersPerSecondSquared, length: 1, time: -2);

	/// <summary>Force dimension [M L T⁻²].</summary>
	public static readonly PhysicalDimension Force = new(mass: 1, length: 1, time: -2, baseUnit: BootstrapUnits.Newton);

	/// <summary>Pressure dimension [M L⁻¹ T⁻²].</summary>
	public static readonly PhysicalDimension Pressure = new(mass: 1, length: -1, time: -2, baseUnit: BootstrapUnits.Pascal);

	/// <summary>Energy dimension [M L² T⁻²].</summary>
	public static readonly PhysicalDimension Energy = new(mass: 1, length: 2, time: -2, baseUnit: BootstrapUnits.Joule);

	/// <summary>Power dimension [M L² T⁻³].</summary>
	public static readonly PhysicalDimension Power = new(mass: 1, length: 2, time: -3, baseUnit: BootstrapUnits.Watt);

	/// <summary>Electric potential dimension [M L² T⁻³ I⁻¹].</summary>
	public static readonly PhysicalDimension ElectricPotential = new(mass: 1, length: 2, time: -3, electricCurrent: -1, baseUnit: BootstrapUnits.Volt);

	/// <summary>Electric resistance dimension [M L² T⁻³ I⁻²].</summary>
	public static readonly PhysicalDimension ElectricResistance = new(mass: 1, length: 2, time: -3, electricCurrent: -2, baseUnit: BootstrapUnits.Ohm);

	/// <summary>Electric charge dimension [I T].</summary>
	public static readonly PhysicalDimension ElectricCharge = new(electricCurrent: 1, time: 1, baseUnit: BootstrapUnits.Coulomb);

	/// <summary>Electric capacitance dimension [M⁻¹ L⁻² T⁴ I²].</summary>
	public static readonly PhysicalDimension ElectricCapacitance = new(mass: -1, length: -2, time: 4, electricCurrent: 2, baseUnit: BootstrapUnits.Farad);

	/// <summary>Frequency dimension [T⁻¹].</summary>
	public static readonly PhysicalDimension Frequency = new(baseUnit: BootstrapUnits.Hertz, time: -1);

	/// <summary>Momentum dimension [M L T⁻¹].</summary>
	public static readonly PhysicalDimension Momentum = new(mass: 1, length: 1, time: -1, baseUnit: BootstrapUnits.Newton);

	/// <summary>Angular velocity dimension [T⁻¹].</summary>
	public static readonly PhysicalDimension AngularVelocity = new(baseUnit: BootstrapUnits.Radian, time: -1);

	/// <summary>Angular acceleration dimension [T⁻²].</summary>
	public static readonly PhysicalDimension AngularAcceleration = new(baseUnit: BootstrapUnits.Radian, time: -2);

	/// <summary>Torque dimension [M L² T⁻²].</summary>
	public static readonly PhysicalDimension Torque = new(mass: 1, length: 2, time: -2, baseUnit: BootstrapUnits.Newton);

	/// <summary>Moment of inertia dimension [M L²].</summary>
	public static readonly PhysicalDimension MomentOfInertia = new(mass: 1, length: 2, baseUnit: BootstrapUnits.Kilogram);

	/// <summary>Density dimension [M L⁻³].</summary>
	public static readonly PhysicalDimension Density = new(mass: 1, length: -3, baseUnit: BootstrapUnits.Kilogram);

	/// <summary>Concentration dimension [N L⁻³].</summary>
	public static readonly PhysicalDimension Concentration = new(amountOfSubstance: 1, length: -3, baseUnit: BootstrapUnits.Molar);

	/// <summary>Electric field dimension [M L T⁻³ I⁻¹].</summary>
	public static readonly PhysicalDimension ElectricField = new(mass: 1, length: 1, time: -3, electricCurrent: -1, baseUnit: BootstrapUnits.Volt);

	/// <summary>Electric flux dimension [M L³ T⁻³ I⁻¹].</summary>
	public static readonly PhysicalDimension ElectricFlux = new(mass: 1, length: 3, time: -3, electricCurrent: -1, baseUnit: BootstrapUnits.Volt);

	/// <summary>Permittivity dimension [M⁻¹ L⁻³ T⁴ I²].</summary>
	public static readonly PhysicalDimension Permittivity = new(mass: -1, length: -3, time: 4, electricCurrent: 2, baseUnit: BootstrapUnits.Farad);

	/// <summary>Electric conductivity dimension [M⁻¹ L⁻³ T³ I²].</summary>
	public static readonly PhysicalDimension ElectricConductivity = new(mass: -1, length: -3, time: 3, electricCurrent: 2, baseUnit: BootstrapUnits.Ohm);

	/// <summary>Electric power density dimension [M L⁻¹ T⁻³].</summary>
	public static readonly PhysicalDimension ElectricPowerDensity = new(mass: 1, length: -1, time: -3, baseUnit: BootstrapUnits.Watt);

	/// <summary>AC impedance dimension [M L² T⁻³ I⁻²].</summary>
	public static readonly PhysicalDimension ImpedanceAC = new(mass: 1, length: 2, time: -3, electricCurrent: -2, baseUnit: BootstrapUnits.Ohm);

	/// <summary>Sound pressure dimension [M L⁻¹ T⁻²].</summary>
	public static readonly PhysicalDimension SoundPressure = new(mass: 1, length: -1, time: -2, baseUnit: BootstrapUnits.Pascal);

	/// <summary>Sound intensity dimension [M T⁻³].</summary>
	public static readonly PhysicalDimension SoundIntensity = new(mass: 1, time: -3, baseUnit: BootstrapUnits.Watt);

	/// <summary>Sound power dimension [M L² T⁻³].</summary>
	public static readonly PhysicalDimension SoundPower = new(mass: 1, length: 2, time: -3, baseUnit: BootstrapUnits.Watt);

	/// <summary>Acoustic impedance dimension [M L⁻² T⁻¹].</summary>
	public static readonly PhysicalDimension AcousticImpedance = new(mass: 1, length: -2, time: -1, baseUnit: BootstrapUnits.Pascal);

	/// <summary>Sound speed dimension [L T⁻¹].</summary>
	public static readonly PhysicalDimension SoundSpeed = new(length: 1, time: -1, baseUnit: BootstrapUnits.MetersPerSecond);

	/// <summary>Sound absorption coefficient dimension [1] (dimensionless).</summary>
	public static readonly PhysicalDimension SoundAbsorption = new(baseUnit: BootstrapUnits.Coefficient);

	/// <summary>Reverberation time dimension [T].</summary>
	public static readonly PhysicalDimension ReverberationTime = new(time: 1, baseUnit: BootstrapUnits.Second);

	/// <summary>Wavelength dimension [L].</summary>
	public static readonly PhysicalDimension Wavelength = new(length: 1, baseUnit: BootstrapUnits.Meter);

	/// <summary>Wave number dimension [L⁻¹].</summary>
	public static readonly PhysicalDimension WaveNumber = new(length: -1, baseUnit: BootstrapUnits.Meter);

	/// <summary>Sound pressure level dimension [1] (dB SPL).</summary>
	public static readonly PhysicalDimension SoundPressureLevel = new(baseUnit: BootstrapUnits.Decibel);

	/// <summary>Sound intensity level dimension [1] (dB IL).</summary>
	public static readonly PhysicalDimension SoundIntensityLevel = new(baseUnit: BootstrapUnits.Decibel);

	/// <summary>Sound power level dimension [1] (dB PWL).</summary>
	public static readonly PhysicalDimension SoundPowerLevel = new(baseUnit: BootstrapUnits.Decibel);

	/// <summary>Reflection coefficient dimension [1] (dimensionless).</summary>
	public static readonly PhysicalDimension ReflectionCoefficient = new(baseUnit: BootstrapUnits.Coefficient);

	/// <summary>Noise reduction coefficient dimension [1] (dimensionless).</summary>
	public static readonly PhysicalDimension NoiseReductionCoefficient = new(baseUnit: BootstrapUnits.Coefficient);

	/// <summary>Sound transmission class dimension [1] (dimensionless).</summary>
	public static readonly PhysicalDimension SoundTransmissionClass = new(baseUnit: BootstrapUnits.Dimensionless);

	/// <summary>Loudness dimension [1] (sones - dimensionless).</summary>
	public static readonly PhysicalDimension Loudness = new(baseUnit: BootstrapUnits.Dimensionless);

	/// <summary>Pitch dimension [T⁻¹] (frequency-based).</summary>
	public static readonly PhysicalDimension Pitch = new(time: -1, baseUnit: BootstrapUnits.Hertz);

	/// <summary>Sharpness dimension [1] (acum - dimensionless).</summary>
	public static readonly PhysicalDimension Sharpness = new(baseUnit: BootstrapUnits.Dimensionless);

	/// <summary>Sensitivity dimension [M⁻¹ L⁻¹ T² I] (V/Pa for microphones).</summary>
	public static readonly PhysicalDimension Sensitivity = new(mass: -1, length: -1, time: 2, electricCurrent: 1, baseUnit: BootstrapUnits.Volt);

	/// <summary>Directionality index dimension [1] (dB).</summary>
	public static readonly PhysicalDimension DirectionalityIndex = new(baseUnit: BootstrapUnits.Decibel);

	/// <summary>Heat dimension [M L² T⁻²].</summary>
	public static readonly PhysicalDimension Heat = new(mass: 1, length: 2, time: -2, baseUnit: BootstrapUnits.Joule);

	/// <summary>Entropy dimension [M L² T⁻² Θ⁻¹].</summary>
	public static readonly PhysicalDimension Entropy = new(mass: 1, length: 2, time: -2, temperature: -1, baseUnit: BootstrapUnits.Joule);

	/// <summary>Thermal conductivity dimension [M L T⁻³ Θ⁻¹].</summary>
	public static readonly PhysicalDimension ThermalConductivity = new(mass: 1, length: 1, time: -3, temperature: -1, baseUnit: BootstrapUnits.Watt);

	/// <summary>Heat capacity dimension [M L² T⁻² Θ⁻¹].</summary>
	public static readonly PhysicalDimension HeatCapacity = new(mass: 1, length: 2, time: -2, temperature: -1, baseUnit: BootstrapUnits.Joule);

	/// <summary>Specific heat dimension [L² T⁻² Θ⁻¹].</summary>
	public static readonly PhysicalDimension SpecificHeat = new(length: 2, time: -2, temperature: -1, baseUnit: BootstrapUnits.Joule);

	/// <summary>Heat transfer coefficient dimension [M T⁻³ Θ⁻¹].</summary>
	public static readonly PhysicalDimension HeatTransferCoefficient = new(mass: 1, time: -3, temperature: -1, baseUnit: BootstrapUnits.Watt);

	/// <summary>Thermal expansion coefficient dimension [Θ⁻¹].</summary>
	public static readonly PhysicalDimension ThermalExpansion = new(temperature: -1, baseUnit: BootstrapUnits.Kelvin);

	/// <summary>Thermal diffusivity dimension [L² T⁻¹].</summary>
	public static readonly PhysicalDimension ThermalDiffusivity = new(length: 2, time: -1, baseUnit: BootstrapUnits.Meter);

	/// <summary>Thermal resistance dimension [Θ T³ M⁻¹ L⁻²].</summary>
	public static readonly PhysicalDimension ThermalResistance = new(temperature: 1, time: 3, mass: -1, length: -2, baseUnit: BootstrapUnits.Kelvin);

	/// <summary>Molar mass dimension [M N⁻¹].</summary>
	public static readonly PhysicalDimension MolarMass = new(mass: 1, amountOfSubstance: -1, baseUnit: BootstrapUnits.Gram);

	/// <summary>pH dimension [1] (logarithmic acidity scale).</summary>
	public static readonly PhysicalDimension pH = new(baseUnit: BootstrapUnits.PH);

	/// <summary>Reaction rate dimension [N L⁻³ T⁻¹].</summary>
	public static readonly PhysicalDimension ReactionRate = new(amountOfSubstance: 1, length: -3, time: -1, baseUnit: BootstrapUnits.Molar);

	/// <summary>Activation energy dimension [M L² T⁻² N⁻¹].</summary>
	public static readonly PhysicalDimension ActivationEnergy = new(mass: 1, length: 2, time: -2, amountOfSubstance: -1, baseUnit: BootstrapUnits.Joule);

	/// <summary>Rate constant dimension [varies by reaction order].</summary>
	public static readonly PhysicalDimension RateConstant = new(baseUnit: BootstrapUnits.Second); // First-order default

	/// <summary>Enzyme activity dimension [N T⁻¹].</summary>
	public static readonly PhysicalDimension EnzymeActivity = new(amountOfSubstance: 1, time: -1, baseUnit: BootstrapUnits.Mole);

	/// <summary>Solubility dimension [N L⁻³].</summary>
	public static readonly PhysicalDimension Solubility = new(amountOfSubstance: 1, length: -3, baseUnit: BootstrapUnits.Molar);

	/// <summary>Surface tension dimension [M T⁻²].</summary>
	public static readonly PhysicalDimension SurfaceTension = new(mass: 1, time: -2, baseUnit: BootstrapUnits.Newton);

	/// <summary>Vapor pressure dimension [M L⁻¹ T⁻²].</summary>
	public static readonly PhysicalDimension VaporPressure = new(mass: 1, length: -1, time: -2, baseUnit: BootstrapUnits.Pascal);

	/// <summary>Dynamic viscosity dimension [M L⁻¹ T⁻¹].</summary>
	public static readonly PhysicalDimension DynamicViscosity = new(mass: 1, length: -1, time: -1, baseUnit: BootstrapUnits.Pascal);

	/// <summary>Luminous flux dimension [J] (cd⋅sr).</summary>
	public static readonly PhysicalDimension LuminousFlux = new(luminousIntensity: 1, baseUnit: BootstrapUnits.Lumen);

	/// <summary>Illuminance dimension [J L⁻²] (lm/m²).</summary>
	public static readonly PhysicalDimension Illuminance = new(luminousIntensity: 1, length: -2, baseUnit: BootstrapUnits.Lux);

	/// <summary>Luminance dimension [J L⁻²] (cd/m²).</summary>
	public static readonly PhysicalDimension Luminance = new(luminousIntensity: 1, length: -2, baseUnit: BootstrapUnits.Candela);

	/// <summary>Refractive index dimension [1] (dimensionless optical ratio).</summary>
	public static readonly PhysicalDimension RefractiveIndex = new(baseUnit: BootstrapUnits.Dimensionless);

	/// <summary>Optical power dimension [L⁻¹] (diopters).</summary>
	public static readonly PhysicalDimension OpticalPower = new(length: -1, baseUnit: BootstrapUnits.Diopter);

	/// <summary>Radioactive activity dimension [T⁻¹] (becquerels).</summary>
	public static readonly PhysicalDimension RadioactiveActivity = new(time: -1, baseUnit: BootstrapUnits.Becquerel);

	/// <summary>Absorbed dose dimension [L² T⁻²] (grays).</summary>
	public static readonly PhysicalDimension AbsorbedDose = new(length: 2, time: -2, baseUnit: BootstrapUnits.Gray);

	/// <summary>Equivalent dose dimension [L² T⁻²] (sieverts).</summary>
	public static readonly PhysicalDimension EquivalentDose = new(length: 2, time: -2, baseUnit: BootstrapUnits.Sievert);

	/// <summary>Exposure dimension [M⁻¹ T I] (coulombs per kilogram).</summary>
	public static readonly PhysicalDimension Exposure = new(mass: -1, time: 1, electricCurrent: 1, baseUnit: BootstrapUnits.Coulomb);

	/// <summary>Nuclear cross section dimension [L²] (barns).</summary>
	public static readonly PhysicalDimension NuclearCrossSection = new(length: 2, baseUnit: BootstrapUnits.Barn);

	/// <summary>Kinematic viscosity dimension [L² T⁻¹] (m²/s).</summary>
	public static readonly PhysicalDimension KinematicViscosity = new(length: 2, time: -1, baseUnit: BootstrapUnits.Meter);

	/// <summary>Bulk modulus dimension [M L⁻¹ T⁻²] (Pa).</summary>
	public static readonly PhysicalDimension BulkModulus = new(mass: 1, length: -1, time: -2, baseUnit: BootstrapUnits.Pascal);

	/// <summary>Volumetric flow rate dimension [L³ T⁻¹] (m³/s).</summary>
	public static readonly PhysicalDimension VolumetricFlowRate = new(length: 3, time: -1, baseUnit: BootstrapUnits.CubicMeter);

	/// <summary>Mass flow rate dimension [M T⁻¹] (kg/s).</summary>
	public static readonly PhysicalDimension MassFlowRate = new(mass: 1, time: -1, baseUnit: BootstrapUnits.Kilogram);

	/// <summary>Reynolds number dimension [1] (dimensionless fluid dynamics ratio).</summary>
	public static readonly PhysicalDimension ReynoldsNumber = new(baseUnit: BootstrapUnits.Dimensionless);

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
			SoundPressureLevel,
			SoundIntensityLevel,
			SoundPowerLevel,
			ReflectionCoefficient,
			NoiseReductionCoefficient,
			SoundTransmissionClass,
			Loudness,
			Pitch,
			Sharpness,
			Sensitivity,
			DirectionalityIndex,
			Heat,
			Entropy,
			ThermalConductivity,
			HeatCapacity,
			SpecificHeat,
			HeatTransferCoefficient,
			ThermalExpansion,
			ThermalDiffusivity,
			ThermalResistance,
			MolarMass,
			pH,
			ReactionRate,
			ActivationEnergy,
			RateConstant,
			EnzymeActivity,
			Solubility,
			SurfaceTension,
			VaporPressure,
			DynamicViscosity,
			LuminousFlux,
			Illuminance,
			Luminance,
			RefractiveIndex,
			OpticalPower,
			RadioactiveActivity,
			AbsorbedDose,
			EquivalentDose,
			Exposure,
			NuclearCrossSection,
			KinematicViscosity,
			BulkModulus,
			VolumetricFlowRate,
			MassFlowRate,
			ReynoldsNumber
		]).ToFrozenSet();
}
