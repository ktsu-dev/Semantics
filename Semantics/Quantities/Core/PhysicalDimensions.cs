// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Collections.Frozen;

/// <inheritdoc/>
public static class PhysicalDimensions
{
	// Bootstrap units for fundamental SI units
	private static readonly BootstrapUnit BootstrapRadian = new("radian", "rad");
	private static readonly BootstrapUnit BootstrapMeter = new("meter", "m");
	private static readonly BootstrapUnit BootstrapKilogram = new("kilogram", "kg");
	private static readonly BootstrapUnit BootstrapSecond = new("second", "s");
	private static readonly BootstrapUnit BootstrapAmpere = new("ampere", "A");
	private static readonly BootstrapUnit BootstrapKelvin = new("kelvin", "K");
	private static readonly BootstrapUnit BootstrapMole = new("mole", "mol");
	private static readonly BootstrapUnit BootstrapCandela = new("candela", "cd");

	// Bootstrap units for dimensionless quantities
	private static readonly BootstrapUnit BootstrapDimensionless = new("dimensionless", "1");
	private static readonly BootstrapUnit BootstrapDecibel = new("decibel", "dB");
	private static readonly BootstrapUnit BootstrapPH = new("pH", "pH");
	private static readonly BootstrapUnit BootstrapCoefficient = new("coefficient", "coeff");

	// Additional bootstrap units needed to break circular dependencies
	private static readonly BootstrapUnit BootstrapGram = new("gram", "g");
	private static readonly BootstrapUnit BootstrapMolar = new("molar", "M");
	private static readonly BootstrapUnit BootstrapLumen = new("lumen", "lm");
	private static readonly BootstrapUnit BootstrapLux = new("lux", "lx");
	private static readonly BootstrapUnit BootstrapBecquerel = new("becquerel", "Bq");
	private static readonly BootstrapUnit BootstrapGray = new("gray", "Gy");
	private static readonly BootstrapUnit BootstrapSievert = new("sievert", "Sv");
	private static readonly BootstrapUnit BootstrapBarn = new("barn", "b");
	private static readonly BootstrapUnit BootstrapDiopter = new("diopter", "D");

	/// <summary>Dimensionless quantity [1].</summary>
	public static readonly PhysicalDimension Dimensionless = new(baseUnit: BootstrapDimensionless);

	/// <summary>Length dimension [L].</summary>
	public static readonly PhysicalDimension Length = new(baseUnit: BootstrapMeter, length: 1);

	/// <summary>Mass dimension [M].</summary>
	public static readonly PhysicalDimension Mass = new(baseUnit: BootstrapKilogram, mass: 1);

	/// <summary>Time dimension [T].</summary>
	public static readonly PhysicalDimension Time = new(baseUnit: BootstrapSecond, time: 1);

	/// <summary>Electric current dimension [I].</summary>
	public static readonly PhysicalDimension ElectricCurrent = new(baseUnit: BootstrapAmpere, electricCurrent: 1);

	/// <summary>Temperature dimension [Θ].</summary>
	public static readonly PhysicalDimension Temperature = new(baseUnit: BootstrapKelvin, temperature: 1);

	/// <summary>Amount of substance dimension [N].</summary>
	public static readonly PhysicalDimension AmountOfSubstance = new(baseUnit: BootstrapMole, amountOfSubstance: 1);

	/// <summary>Luminous intensity dimension [J].</summary>
	public static readonly PhysicalDimension LuminousIntensity = new(baseUnit: BootstrapCandela, luminousIntensity: 1);

	// Additional bootstrap units for derived dimensions
	private static readonly BootstrapUnit BootstrapSquareMeter = new("square meter", "m²");
	private static readonly BootstrapUnit BootstrapCubicMeter = new("cubic meter", "m³");
	private static readonly BootstrapUnit BootstrapMetersPerSecond = new("meters per second", "m/s");
	private static readonly BootstrapUnit BootstrapMetersPerSecondSquared = new("meters per second squared", "m/s²");
	private static readonly BootstrapUnit BootstrapNewton = new("newton", "N");
	private static readonly BootstrapUnit BootstrapPascal = new("pascal", "Pa");
	private static readonly BootstrapUnit BootstrapJoule = new("joule", "J");
	private static readonly BootstrapUnit BootstrapWatt = new("watt", "W");
	private static readonly BootstrapUnit BootstrapVolt = new("volt", "V");
	private static readonly BootstrapUnit BootstrapOhm = new("ohm", "Ω");
	private static readonly BootstrapUnit BootstrapCoulomb = new("coulomb", "C");
	private static readonly BootstrapUnit BootstrapFarad = new("farad", "F");
	private static readonly BootstrapUnit BootstrapHertz = new("hertz", "Hz");

	/// <summary>Area dimension [L²].</summary>
	public static readonly PhysicalDimension Area = new(baseUnit: BootstrapSquareMeter, length: 2);

	/// <summary>Volume dimension [L³].</summary>
	public static readonly PhysicalDimension Volume = new(baseUnit: BootstrapCubicMeter, length: 3);

	/// <summary>Velocity dimension [L T⁻¹].</summary>
	public static readonly PhysicalDimension Velocity = new(baseUnit: BootstrapMetersPerSecond, length: 1, time: -1);

	/// <summary>Acceleration dimension [L T⁻²].</summary>
	public static readonly PhysicalDimension Acceleration = new(baseUnit: BootstrapMetersPerSecondSquared, length: 1, time: -2);

	/// <summary>Force dimension [M L T⁻²].</summary>
	public static readonly PhysicalDimension Force = new(mass: 1, length: 1, time: -2, baseUnit: BootstrapNewton);

	/// <summary>Pressure dimension [M L⁻¹ T⁻²].</summary>
	public static readonly PhysicalDimension Pressure = new(mass: 1, length: -1, time: -2, baseUnit: BootstrapPascal);

	/// <summary>Energy dimension [M L² T⁻²].</summary>
	public static readonly PhysicalDimension Energy = new(mass: 1, length: 2, time: -2, baseUnit: BootstrapJoule);

	/// <summary>Power dimension [M L² T⁻³].</summary>
	public static readonly PhysicalDimension Power = new(mass: 1, length: 2, time: -3, baseUnit: BootstrapWatt);

	/// <summary>Electric potential dimension [M L² T⁻³ I⁻¹].</summary>
	public static readonly PhysicalDimension ElectricPotential = new(mass: 1, length: 2, time: -3, electricCurrent: -1, baseUnit: BootstrapVolt);

	/// <summary>Electric resistance dimension [M L² T⁻³ I⁻²].</summary>
	public static readonly PhysicalDimension ElectricResistance = new(mass: 1, length: 2, time: -3, electricCurrent: -2, baseUnit: BootstrapOhm);

	/// <summary>Electric charge dimension [I T].</summary>
	public static readonly PhysicalDimension ElectricCharge = new(electricCurrent: 1, time: 1, baseUnit: BootstrapCoulomb);

	/// <summary>Electric capacitance dimension [M⁻¹ L⁻² T⁴ I²].</summary>
	public static readonly PhysicalDimension ElectricCapacitance = new(mass: -1, length: -2, time: 4, electricCurrent: 2, baseUnit: BootstrapFarad);

	/// <summary>Frequency dimension [T⁻¹].</summary>
	public static readonly PhysicalDimension Frequency = new(baseUnit: BootstrapHertz, time: -1);

	/// <summary>Momentum dimension [M L T⁻¹].</summary>
	public static readonly PhysicalDimension Momentum = new(mass: 1, length: 1, time: -1, baseUnit: BootstrapNewton);

	/// <summary>Angular velocity dimension [T⁻¹].</summary>
	public static readonly PhysicalDimension AngularVelocity = new(baseUnit: BootstrapRadian, time: -1);

	/// <summary>Angular acceleration dimension [T⁻²].</summary>
	public static readonly PhysicalDimension AngularAcceleration = new(baseUnit: BootstrapRadian, time: -2);

	/// <summary>Torque dimension [M L² T⁻²].</summary>
	public static readonly PhysicalDimension Torque = new(mass: 1, length: 2, time: -2, baseUnit: BootstrapNewton);

	/// <summary>Moment of inertia dimension [M L²].</summary>
	public static readonly PhysicalDimension MomentOfInertia = new(mass: 1, length: 2, baseUnit: BootstrapKilogram);

	/// <summary>Density dimension [M L⁻³].</summary>
	public static readonly PhysicalDimension Density = new(mass: 1, length: -3, baseUnit: BootstrapKilogram);

	/// <summary>Concentration dimension [N L⁻³].</summary>
	public static readonly PhysicalDimension Concentration = new(amountOfSubstance: 1, length: -3, baseUnit: BootstrapMolar);

	/// <summary>Electric field dimension [M L T⁻³ I⁻¹].</summary>
	public static readonly PhysicalDimension ElectricField = new(mass: 1, length: 1, time: -3, electricCurrent: -1, baseUnit: BootstrapVolt);

	/// <summary>Electric flux dimension [M L³ T⁻³ I⁻¹].</summary>
	public static readonly PhysicalDimension ElectricFlux = new(mass: 1, length: 3, time: -3, electricCurrent: -1, baseUnit: BootstrapVolt);

	/// <summary>Permittivity dimension [M⁻¹ L⁻³ T⁴ I²].</summary>
	public static readonly PhysicalDimension Permittivity = new(mass: -1, length: -3, time: 4, electricCurrent: 2, baseUnit: BootstrapFarad);

	/// <summary>Electric conductivity dimension [M⁻¹ L⁻³ T³ I²].</summary>
	public static readonly PhysicalDimension ElectricConductivity = new(mass: -1, length: -3, time: 3, electricCurrent: 2, baseUnit: BootstrapOhm);

	/// <summary>Electric power density dimension [M L⁻¹ T⁻³].</summary>
	public static readonly PhysicalDimension ElectricPowerDensity = new(mass: 1, length: -1, time: -3, baseUnit: BootstrapWatt);

	/// <summary>AC impedance dimension [M L² T⁻³ I⁻²].</summary>
	public static readonly PhysicalDimension ImpedanceAC = new(mass: 1, length: 2, time: -3, electricCurrent: -2, baseUnit: BootstrapOhm);

	/// <summary>Sound pressure dimension [M L⁻¹ T⁻²].</summary>
	public static readonly PhysicalDimension SoundPressure = new(mass: 1, length: -1, time: -2, baseUnit: BootstrapPascal);

	/// <summary>Sound intensity dimension [M T⁻³].</summary>
	public static readonly PhysicalDimension SoundIntensity = new(mass: 1, time: -3, baseUnit: BootstrapWatt);

	/// <summary>Sound power dimension [M L² T⁻³].</summary>
	public static readonly PhysicalDimension SoundPower = new(mass: 1, length: 2, time: -3, baseUnit: BootstrapWatt);

	/// <summary>Acoustic impedance dimension [M L⁻² T⁻¹].</summary>
	public static readonly PhysicalDimension AcousticImpedance = new(mass: 1, length: -2, time: -1, baseUnit: BootstrapPascal);

	/// <summary>Sound speed dimension [L T⁻¹].</summary>
	public static readonly PhysicalDimension SoundSpeed = new(length: 1, time: -1, baseUnit: BootstrapMetersPerSecond);

	/// <summary>Sound absorption coefficient dimension [1] (dimensionless).</summary>
	public static readonly PhysicalDimension SoundAbsorption = new(baseUnit: BootstrapCoefficient);

	/// <summary>Reverberation time dimension [T].</summary>
	public static readonly PhysicalDimension ReverberationTime = new(time: 1, baseUnit: BootstrapSecond);

	/// <summary>Wavelength dimension [L].</summary>
	public static readonly PhysicalDimension Wavelength = new(length: 1, baseUnit: BootstrapMeter);

	/// <summary>Wave number dimension [L⁻¹].</summary>
	public static readonly PhysicalDimension WaveNumber = new(length: -1, baseUnit: BootstrapMeter);

	/// <summary>Sound pressure level dimension [1] (dB SPL).</summary>
	public static readonly PhysicalDimension SoundPressureLevel = new(baseUnit: BootstrapDecibel);

	/// <summary>Sound intensity level dimension [1] (dB IL).</summary>
	public static readonly PhysicalDimension SoundIntensityLevel = new(baseUnit: BootstrapDecibel);

	/// <summary>Sound power level dimension [1] (dB PWL).</summary>
	public static readonly PhysicalDimension SoundPowerLevel = new(baseUnit: BootstrapDecibel);

	/// <summary>Reflection coefficient dimension [1] (dimensionless).</summary>
	public static readonly PhysicalDimension ReflectionCoefficient = new(baseUnit: BootstrapCoefficient);

	/// <summary>Noise reduction coefficient dimension [1] (dimensionless).</summary>
	public static readonly PhysicalDimension NoiseReductionCoefficient = new(baseUnit: BootstrapCoefficient);

	/// <summary>Sound transmission class dimension [1] (dimensionless).</summary>
	public static readonly PhysicalDimension SoundTransmissionClass = new(baseUnit: BootstrapDimensionless);

	/// <summary>Loudness dimension [1] (sones - dimensionless).</summary>
	public static readonly PhysicalDimension Loudness = new(baseUnit: BootstrapDimensionless);

	/// <summary>Pitch dimension [T⁻¹] (frequency-based).</summary>
	public static readonly PhysicalDimension Pitch = new(time: -1, baseUnit: BootstrapHertz);

	/// <summary>Sharpness dimension [1] (acum - dimensionless).</summary>
	public static readonly PhysicalDimension Sharpness = new(baseUnit: BootstrapDimensionless);

	/// <summary>Sensitivity dimension [M⁻¹ L⁻¹ T² I] (V/Pa for microphones).</summary>
	public static readonly PhysicalDimension Sensitivity = new(mass: -1, length: -1, time: 2, electricCurrent: 1, baseUnit: BootstrapVolt);

	/// <summary>Directionality index dimension [1] (dB).</summary>
	public static readonly PhysicalDimension DirectionalityIndex = new(baseUnit: BootstrapDecibel);

	/// <summary>Heat dimension [M L² T⁻²].</summary>
	public static readonly PhysicalDimension Heat = new(mass: 1, length: 2, time: -2, baseUnit: BootstrapJoule);

	/// <summary>Entropy dimension [M L² T⁻² Θ⁻¹].</summary>
	public static readonly PhysicalDimension Entropy = new(mass: 1, length: 2, time: -2, temperature: -1, baseUnit: BootstrapJoule);

	/// <summary>Thermal conductivity dimension [M L T⁻³ Θ⁻¹].</summary>
	public static readonly PhysicalDimension ThermalConductivity = new(mass: 1, length: 1, time: -3, temperature: -1, baseUnit: BootstrapWatt);

	/// <summary>Heat capacity dimension [M L² T⁻² Θ⁻¹].</summary>
	public static readonly PhysicalDimension HeatCapacity = new(mass: 1, length: 2, time: -2, temperature: -1, baseUnit: BootstrapJoule);

	/// <summary>Specific heat dimension [L² T⁻² Θ⁻¹].</summary>
	public static readonly PhysicalDimension SpecificHeat = new(length: 2, time: -2, temperature: -1, baseUnit: BootstrapJoule);

	/// <summary>Heat transfer coefficient dimension [M T⁻³ Θ⁻¹].</summary>
	public static readonly PhysicalDimension HeatTransferCoefficient = new(mass: 1, time: -3, temperature: -1, baseUnit: BootstrapWatt);

	/// <summary>Thermal expansion coefficient dimension [Θ⁻¹].</summary>
	public static readonly PhysicalDimension ThermalExpansion = new(temperature: -1, baseUnit: BootstrapKelvin);

	/// <summary>Thermal diffusivity dimension [L² T⁻¹].</summary>
	public static readonly PhysicalDimension ThermalDiffusivity = new(length: 2, time: -1, baseUnit: BootstrapMeter);

	/// <summary>Thermal resistance dimension [Θ T³ M⁻¹ L⁻²].</summary>
	public static readonly PhysicalDimension ThermalResistance = new(temperature: 1, time: 3, mass: -1, length: -2, baseUnit: BootstrapKelvin);

	/// <summary>Molar mass dimension [M N⁻¹].</summary>
	public static readonly PhysicalDimension MolarMass = new(mass: 1, amountOfSubstance: -1, baseUnit: BootstrapGram);

	/// <summary>pH dimension [1] (logarithmic acidity scale).</summary>
	public static readonly PhysicalDimension pH = new(baseUnit: BootstrapPH);

	/// <summary>Reaction rate dimension [N L⁻³ T⁻¹].</summary>
	public static readonly PhysicalDimension ReactionRate = new(amountOfSubstance: 1, length: -3, time: -1, baseUnit: BootstrapMolar);

	/// <summary>Activation energy dimension [M L² T⁻² N⁻¹].</summary>
	public static readonly PhysicalDimension ActivationEnergy = new(mass: 1, length: 2, time: -2, amountOfSubstance: -1, baseUnit: BootstrapJoule);

	/// <summary>Rate constant dimension [varies by reaction order].</summary>
	public static readonly PhysicalDimension RateConstant = new(baseUnit: BootstrapSecond); // First-order default

	/// <summary>Enzyme activity dimension [N T⁻¹].</summary>
	public static readonly PhysicalDimension EnzymeActivity = new(amountOfSubstance: 1, time: -1, baseUnit: BootstrapMole);

	/// <summary>Solubility dimension [N L⁻³].</summary>
	public static readonly PhysicalDimension Solubility = new(amountOfSubstance: 1, length: -3, baseUnit: BootstrapMolar);

	/// <summary>Surface tension dimension [M T⁻²].</summary>
	public static readonly PhysicalDimension SurfaceTension = new(mass: 1, time: -2, baseUnit: BootstrapNewton);

	/// <summary>Vapor pressure dimension [M L⁻¹ T⁻²].</summary>
	public static readonly PhysicalDimension VaporPressure = new(mass: 1, length: -1, time: -2, baseUnit: BootstrapPascal);

	/// <summary>Dynamic viscosity dimension [M L⁻¹ T⁻¹].</summary>
	public static readonly PhysicalDimension DynamicViscosity = new(mass: 1, length: -1, time: -1, baseUnit: BootstrapPascal);

	/// <summary>Luminous flux dimension [J] (cd⋅sr).</summary>
	public static readonly PhysicalDimension LuminousFlux = new(luminousIntensity: 1, baseUnit: BootstrapLumen);

	/// <summary>Illuminance dimension [J L⁻²] (lm/m²).</summary>
	public static readonly PhysicalDimension Illuminance = new(luminousIntensity: 1, length: -2, baseUnit: BootstrapLux);

	/// <summary>Luminance dimension [J L⁻²] (cd/m²).</summary>
	public static readonly PhysicalDimension Luminance = new(luminousIntensity: 1, length: -2, baseUnit: BootstrapCandela);

	/// <summary>Refractive index dimension [1] (dimensionless optical ratio).</summary>
	public static readonly PhysicalDimension RefractiveIndex = new(baseUnit: BootstrapDimensionless);

	/// <summary>Optical power dimension [L⁻¹] (diopters).</summary>
	public static readonly PhysicalDimension OpticalPower = new(length: -1, baseUnit: BootstrapDiopter);

	/// <summary>Radioactive activity dimension [T⁻¹] (becquerels).</summary>
	public static readonly PhysicalDimension RadioactiveActivity = new(time: -1, baseUnit: BootstrapBecquerel);

	/// <summary>Absorbed dose dimension [L² T⁻²] (grays).</summary>
	public static readonly PhysicalDimension AbsorbedDose = new(length: 2, time: -2, baseUnit: BootstrapGray);

	/// <summary>Equivalent dose dimension [L² T⁻²] (sieverts).</summary>
	public static readonly PhysicalDimension EquivalentDose = new(length: 2, time: -2, baseUnit: BootstrapSievert);

	/// <summary>Exposure dimension [M⁻¹ T I] (coulombs per kilogram).</summary>
	public static readonly PhysicalDimension Exposure = new(mass: -1, time: 1, electricCurrent: 1, baseUnit: BootstrapCoulomb);

	/// <summary>Nuclear cross section dimension [L²] (barns).</summary>
	public static readonly PhysicalDimension NuclearCrossSection = new(length: 2, baseUnit: BootstrapBarn);

	/// <summary>Kinematic viscosity dimension [L² T⁻¹] (m²/s).</summary>
	public static readonly PhysicalDimension KinematicViscosity = new(length: 2, time: -1, baseUnit: BootstrapMeter);

	/// <summary>Bulk modulus dimension [M L⁻¹ T⁻²] (Pa).</summary>
	public static readonly PhysicalDimension BulkModulus = new(mass: 1, length: -1, time: -2, baseUnit: BootstrapPascal);

	/// <summary>Volumetric flow rate dimension [L³ T⁻¹] (m³/s).</summary>
	public static readonly PhysicalDimension VolumetricFlowRate = new(length: 3, time: -1, baseUnit: BootstrapCubicMeter);

	/// <summary>Mass flow rate dimension [M T⁻¹] (kg/s).</summary>
	public static readonly PhysicalDimension MassFlowRate = new(mass: 1, time: -1, baseUnit: BootstrapKilogram);

	/// <summary>Reynolds number dimension [1] (dimensionless fluid dynamics ratio).</summary>
	public static readonly PhysicalDimension ReynoldsNumber = new(baseUnit: BootstrapDimensionless);

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
