// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Static registry of commonly used units organized by physical domain.
/// </summary>
public static class Units
{
	/// <summary>Meter - SI base unit of length.</summary>
	public static readonly IUnit Meter = new Unit("meter", "m", PhysicalDimensions.Length, UnitSystem.SIBase);

	/// <summary>Kilogram - SI base unit of mass.</summary>
	public static readonly IUnit Kilogram = new Unit("kilogram", "kg", PhysicalDimensions.Mass, UnitSystem.SIBase);

	/// <summary>Second - SI base unit of time.</summary>
	public static readonly IUnit Second = new Unit("second", "s", PhysicalDimensions.Time, UnitSystem.SIBase);

	/// <summary>Ampere - SI base unit of electric current.</summary>
	public static readonly IUnit Ampere = new Unit("ampere", "A", PhysicalDimensions.ElectricCurrent, UnitSystem.SIBase);

	/// <summary>Kelvin - SI base unit of thermodynamic temperature.</summary>
	public static readonly IUnit Kelvin = new Unit("kelvin", "K", PhysicalDimensions.Temperature, UnitSystem.SIBase);

	/// <summary>Mole - SI base unit of amount of substance.</summary>
	public static readonly IUnit Mole = new Unit("mole", "mol", PhysicalDimensions.AmountOfSubstance, UnitSystem.SIBase);

	/// <summary>Candela - SI base unit of luminous intensity.</summary>
	public static readonly IUnit Candela = new Unit("candela", "cd", PhysicalDimensions.LuminousIntensity, UnitSystem.SIBase);

	/// <summary>Kilometer - 1000 meters.</summary>
	public static readonly IUnit Kilometer = new Unit("kilometer", "km", PhysicalDimensions.Length, UnitSystem.SIDerived, MetricMagnitudes.Kilo);

	/// <summary>Centimeter - 0.01 meters.</summary>
	public static readonly IUnit Centimeter = new Unit("centimeter", "cm", PhysicalDimensions.Length, UnitSystem.SIDerived, MetricMagnitudes.Centi);

	/// <summary>Millimeter - 0.001 meters.</summary>
	public static readonly IUnit Millimeter = new Unit("millimeter", "mm", PhysicalDimensions.Length, UnitSystem.SIDerived, MetricMagnitudes.Milli);

	/// <summary>Micrometer - 0.000001 meters.</summary>
	public static readonly IUnit Micrometer = new Unit("micrometer", "μm", PhysicalDimensions.Length, UnitSystem.SIDerived, MetricMagnitudes.Micro);

	/// <summary>Nanometer - 0.000000001 meters.</summary>
	public static readonly IUnit Nanometer = new Unit("nanometer", "nm", PhysicalDimensions.Length, UnitSystem.SIDerived, MetricMagnitudes.Nano);

	/// <summary>Foot - Imperial unit of length.</summary>
	public static readonly IUnit Foot = new Unit("foot", "ft", PhysicalDimensions.Length, UnitSystem.Imperial, PhysicalConstants.Conversion.FeetToMeters);

	/// <summary>Inch - Imperial unit of length.</summary>
	public static readonly IUnit Inch = new Unit("inch", "in", PhysicalDimensions.Length, UnitSystem.Imperial, 0.0254);

	/// <summary>Yard - Imperial unit of length.</summary>
	public static readonly IUnit Yard = new Unit("yard", "yd", PhysicalDimensions.Length, UnitSystem.Imperial, 0.9144);

	/// <summary>Mile - Imperial unit of length.</summary>
	public static readonly IUnit Mile = new Unit("mile", "mi", PhysicalDimensions.Length, UnitSystem.Imperial, 1609.344);

	/// <summary>Nautical mile - International nautical mile.</summary>
	public static readonly IUnit NauticalMile = new Unit("nautical mile", "nmi", PhysicalDimensions.Length, UnitSystem.Other, 1852.0);

	/// <summary>Ängström - Common unit in atomic physics.</summary>
	public static readonly IUnit Angstrom = new Unit("ångström", "Å", PhysicalDimensions.Length, UnitSystem.Other, 1e-10);

	/// <summary>Square meter - SI derived unit of area.</summary>
	public static readonly IUnit SquareMeter = new Unit("square meter", "m²", PhysicalDimensions.Area, UnitSystem.SIDerived);

	/// <summary>Square kilometer - 1,000,000 square meters.</summary>
	public static readonly IUnit SquareKilometer = new Unit("square kilometer", "km²", PhysicalDimensions.Area, UnitSystem.SIDerived, MetricMagnitudes.Kilo * MetricMagnitudes.Kilo);

	/// <summary>Square centimeter - 0.0001 square meters.</summary>
	public static readonly IUnit SquareCentimeter = new Unit("square centimeter", "cm²", PhysicalDimensions.Area, UnitSystem.SIDerived, MetricMagnitudes.Centi * MetricMagnitudes.Centi);

	/// <summary>Hectare - 10,000 square meters.</summary>
	public static readonly IUnit Hectare = new Unit("hectare", "ha", PhysicalDimensions.Area, UnitSystem.Metric, 10000.0);

	/// <summary>Square foot - Imperial unit of area.</summary>
	public static readonly IUnit SquareFoot = new Unit("square foot", "ft²", PhysicalDimensions.Area, UnitSystem.Imperial, 0.092903);

	/// <summary>Square inch - Imperial unit of area.</summary>
	public static readonly IUnit SquareInch = new Unit("square inch", "in²", PhysicalDimensions.Area, UnitSystem.Imperial, 0.00064516);

	/// <summary>Acre - Imperial unit of area.</summary>
	public static readonly IUnit Acre = new Unit("acre", "ac", PhysicalDimensions.Area, UnitSystem.Imperial, 4046.86);

	/// <summary>Square mile - Imperial unit of area.</summary>
	public static readonly IUnit SquareMile = new Unit("square mile", "mi²", PhysicalDimensions.Area, UnitSystem.Imperial, 2589988.11);

	/// <summary>Cubic meter - SI derived unit of volume.</summary>
	public static readonly IUnit CubicMeter = new Unit("cubic meter", "m³", PhysicalDimensions.Volume, UnitSystem.SIDerived);

	/// <summary>Liter - Common metric unit of volume.</summary>
	public static readonly IUnit Liter = new Unit("liter", "L", PhysicalDimensions.Volume, UnitSystem.Metric, MetricMagnitudes.Milli);

	/// <summary>Milliliter - 0.001 liters.</summary>
	public static readonly IUnit Milliliter = new Unit("milliliter", "mL", PhysicalDimensions.Volume, UnitSystem.Metric, MetricMagnitudes.Milli * MetricMagnitudes.Milli);

	/// <summary>Cubic centimeter - 0.000001 cubic meters.</summary>
	public static readonly IUnit CubicCentimeter = new Unit("cubic centimeter", "cm³", PhysicalDimensions.Volume, UnitSystem.SIDerived, MetricMagnitudes.Centi * MetricMagnitudes.Centi * MetricMagnitudes.Centi);

	/// <summary>Cubic foot - Imperial unit of volume.</summary>
	public static readonly IUnit CubicFoot = new Unit("cubic foot", "ft³", PhysicalDimensions.Volume, UnitSystem.Imperial, 0.0283168);

	/// <summary>Cubic inch - Imperial unit of volume.</summary>
	public static readonly IUnit CubicInch = new Unit("cubic inch", "in³", PhysicalDimensions.Volume, UnitSystem.Imperial, 0.0000163871);

	/// <summary>US gallon - US customary unit of volume.</summary>
	public static readonly IUnit USGallon = new Unit("US gallon", "gal", PhysicalDimensions.Volume, UnitSystem.USCustomary, 0.00378541);

	/// <summary>Imperial gallon - Imperial unit of volume.</summary>
	public static readonly IUnit ImperialGallon = new Unit("imperial gallon", "imp gal", PhysicalDimensions.Volume, UnitSystem.Imperial, 0.00454609);

	/// <summary>US quart - US customary unit of volume.</summary>
	public static readonly IUnit USQuart = new Unit("US quart", "qt", PhysicalDimensions.Volume, UnitSystem.USCustomary, 0.000946353);

	/// <summary>US pint - US customary unit of volume.</summary>
	public static readonly IUnit USPint = new Unit("US pint", "pt", PhysicalDimensions.Volume, UnitSystem.USCustomary, 0.000473176);

	/// <summary>US fluid ounce - US customary unit of volume.</summary>
	public static readonly IUnit USFluidOunce = new Unit("US fluid ounce", "fl oz", PhysicalDimensions.Volume, UnitSystem.USCustomary, 0.0000295735);

	/// <summary>Gram - 0.001 kilograms.</summary>
	public static readonly IUnit Gram = new Unit("gram", "g", PhysicalDimensions.Mass, UnitSystem.SIDerived, MetricMagnitudes.Milli);

	/// <summary>Metric ton - 1000 kilograms.</summary>
	public static readonly IUnit MetricTon = new Unit("metric ton", "t", PhysicalDimensions.Mass, UnitSystem.SIDerived, MetricMagnitudes.Kilo);

	/// <summary>Pound - Imperial unit of mass.</summary>
	public static readonly IUnit Pound = new Unit("pound", "lb", PhysicalDimensions.Mass, UnitSystem.Imperial, 0.453592);

	/// <summary>Ounce - Imperial unit of mass.</summary>
	public static readonly IUnit Ounce = new Unit("ounce", "oz", PhysicalDimensions.Mass, UnitSystem.Imperial, 0.0283495);

	/// <summary>Stone - Imperial unit of mass.</summary>
	public static readonly IUnit Stone = new Unit("stone", "st", PhysicalDimensions.Mass, UnitSystem.Imperial, 6.35029);

	/// <summary>Short ton - US customary unit of mass.</summary>
	public static readonly IUnit ShortTon = new Unit("short ton", "ton", PhysicalDimensions.Mass, UnitSystem.USCustomary, 907.185);

	/// <summary>Atomic mass unit - Used in atomic physics.</summary>
	public static readonly IUnit AtomicMassUnit = new Unit("atomic mass unit", "u", PhysicalDimensions.Mass, UnitSystem.Atomic, 1.66054e-27);

	/// <summary>Minute - 60 seconds.</summary>
	public static readonly IUnit Minute = new Unit("minute", "min", PhysicalDimensions.Time, UnitSystem.SIDerived, 60.0);

	/// <summary>Hour - 3600 seconds.</summary>
	public static readonly IUnit Hour = new Unit("hour", "h", PhysicalDimensions.Time, UnitSystem.SIDerived, 3600.0);

	/// <summary>Day - 86400 seconds.</summary>
	public static readonly IUnit Day = new Unit("day", "d", PhysicalDimensions.Time, UnitSystem.SIDerived, 86400.0);

	/// <summary>Week - 604800 seconds.</summary>
	public static readonly IUnit Week = new Unit("week", "wk", PhysicalDimensions.Time, UnitSystem.SIDerived, 604800.0);

	/// <summary>Year - 365.25 days (Julian year).</summary>
	public static readonly IUnit Year = new Unit("year", "yr", PhysicalDimensions.Time, UnitSystem.SIDerived, 31557600.0);

	/// <summary>Millisecond - 0.001 seconds.</summary>
	public static readonly IUnit Millisecond = new Unit("millisecond", "ms", PhysicalDimensions.Time, UnitSystem.SIDerived, MetricMagnitudes.Milli);

	/// <summary>Microsecond - 0.000001 seconds.</summary>
	public static readonly IUnit Microsecond = new Unit("microsecond", "μs", PhysicalDimensions.Time, UnitSystem.SIDerived, MetricMagnitudes.Micro);

	/// <summary>Nanosecond - 0.000000001 seconds.</summary>
	public static readonly IUnit Nanosecond = new Unit("nanosecond", "ns", PhysicalDimensions.Time, UnitSystem.SIDerived, MetricMagnitudes.Nano);

	/// <summary>Radian - SI derived unit of plane angle.</summary>
	public static readonly IUnit Radian = new Unit("radian", "rad", PhysicalDimensions.Dimensionless, UnitSystem.SIDerived);

	/// <summary>Degree - Common angle unit.</summary>
	public static readonly IUnit Degree = new Unit("degree", "°", PhysicalDimensions.Dimensionless, UnitSystem.Other, Math.PI / 180.0);

	/// <summary>Gradian - Alternative angle unit.</summary>
	public static readonly IUnit Gradian = new Unit("gradian", "grad", PhysicalDimensions.Dimensionless, UnitSystem.Other, Math.PI / 200.0);

	/// <summary>Revolution - Full rotation angle unit.</summary>
	public static readonly IUnit Revolution = new Unit("revolution", "rev", PhysicalDimensions.Dimensionless, UnitSystem.Other, 2.0 * Math.PI);

	/// <summary>Milliradian - 0.001 radians.</summary>
	public static readonly IUnit Milliradian = new Unit("milliradian", "mrad", PhysicalDimensions.Dimensionless, UnitSystem.SIDerived, 0.001);

	/// <summary>Meters per second - SI derived unit of velocity.</summary>
	public static readonly IUnit MetersPerSecond = new Unit("meters per second", "m/s", PhysicalDimensions.Velocity, UnitSystem.SIDerived);

	/// <summary>Kilometers per hour - Common velocity unit.</summary>
	public static readonly IUnit KilometersPerHour = new Unit("kilometers per hour", "km/h", PhysicalDimensions.Velocity, UnitSystem.SIDerived, 1000.0 / 3600.0);

	/// <summary>Miles per hour - Imperial velocity unit.</summary>
	public static readonly IUnit MilesPerHour = new Unit("miles per hour", "mph", PhysicalDimensions.Velocity, UnitSystem.Imperial, 1609.344 / 3600.0);

	/// <summary>Knot - Nautical velocity unit.</summary>
	public static readonly IUnit Knot = new Unit("knot", "kn", PhysicalDimensions.Velocity, UnitSystem.Other, 1852.0 / 3600.0);

	/// <summary>Feet per second - Imperial velocity unit.</summary>
	public static readonly IUnit FeetPerSecond = new Unit("feet per second", "ft/s", PhysicalDimensions.Velocity, UnitSystem.Imperial, PhysicalConstants.Conversion.FeetToMeters);

	/// <summary>Meters per second squared - SI derived unit of acceleration.</summary>
	public static readonly IUnit MetersPerSecondSquared = new Unit("meters per second squared", "m/s²", PhysicalDimensions.Acceleration, UnitSystem.SIDerived);

	/// <summary>Standard gravity - Earth's gravitational acceleration.</summary>
	public static readonly IUnit StandardGravity = new Unit("standard gravity", "g", PhysicalDimensions.Acceleration, UnitSystem.Other, PhysicalConstants.Mechanical.StandardGravity);

	/// <summary>Newton - SI derived unit of force.</summary>
	public static readonly IUnit Newton = new Unit("newton", "N", PhysicalDimensions.Force, UnitSystem.SIDerived);

	/// <summary>Kilonewton - 1000 newtons.</summary>
	public static readonly IUnit Kilonewton = new Unit("kilonewton", "kN", PhysicalDimensions.Force, UnitSystem.SIDerived, 1000.0);

	/// <summary>Pound-force - Imperial unit of force.</summary>
	public static readonly IUnit PoundForce = new Unit("pound-force", "lbf", PhysicalDimensions.Force, UnitSystem.Imperial, PhysicalConstants.Mechanical.PoundMassToKilogram * PhysicalConstants.Mechanical.StandardGravity);

	/// <summary>Dyne - CGS unit of force.</summary>
	public static readonly IUnit Dyne = new Unit("dyne", "dyn", PhysicalDimensions.Force, UnitSystem.CGS, 1e-5);

	/// <summary>Pascal - SI derived unit of pressure.</summary>
	public static readonly IUnit Pascal = new Unit("pascal", "Pa", PhysicalDimensions.Pressure, UnitSystem.SIDerived);

	/// <summary>Kilopascal - 1000 pascals.</summary>
	public static readonly IUnit Kilopascal = new Unit("kilopascal", "kPa", PhysicalDimensions.Pressure, UnitSystem.SIDerived, 1000.0);

	/// <summary>Bar - Metric unit of pressure.</summary>
	public static readonly IUnit Bar = new Unit("bar", "bar", PhysicalDimensions.Pressure, UnitSystem.Metric, 100000.0);

	/// <summary>Atmosphere - Standard atmospheric pressure.</summary>
	public static readonly IUnit Atmosphere = new Unit("atmosphere", "atm", PhysicalDimensions.Pressure, UnitSystem.Other, PhysicalConstants.Mechanical.StandardAtmosphericPressure);

	/// <summary>Pounds per square inch - Imperial pressure unit.</summary>
	public static readonly IUnit PSI = new Unit("pounds per square inch", "psi", PhysicalDimensions.Pressure, UnitSystem.Imperial, 6894.76);

	/// <summary>Torr - Pressure unit based on mercury column.</summary>
	public static readonly IUnit Torr = new Unit("torr", "Torr", PhysicalDimensions.Pressure, UnitSystem.Other, 133.322);

	/// <summary>Joule - SI derived unit of energy.</summary>
	public static readonly IUnit Joule = new Unit("joule", "J", PhysicalDimensions.Energy, UnitSystem.SIDerived);

	/// <summary>Kilojoule - 1000 joules.</summary>
	public static readonly IUnit Kilojoule = new Unit("kilojoule", "kJ", PhysicalDimensions.Energy, UnitSystem.SIDerived, MetricMagnitudes.Kilo);

	/// <summary>Calorie - Common energy unit (thermochemical calorie).</summary>
	public static readonly IUnit Calorie = new Unit("calorie", "cal", PhysicalDimensions.Energy, UnitSystem.Other, 4.184);

	/// <summary>Kilocalorie - 1000 calories (dietary calorie).</summary>
	public static readonly IUnit Kilocalorie = new Unit("kilocalorie", "kcal", PhysicalDimensions.Energy, UnitSystem.Other, 4184.0);

	/// <summary>Watt-hour - Energy unit.</summary>
	public static readonly IUnit WattHour = new Unit("watt-hour", "Wh", PhysicalDimensions.Energy, UnitSystem.SIDerived, 3600.0);

	/// <summary>Kilowatt-hour - 1000 watt-hours.</summary>
	public static readonly IUnit KilowattHour = new Unit("kilowatt-hour", "kWh", PhysicalDimensions.Energy, UnitSystem.SIDerived, 3_600_000.0);

	/// <summary>Erg - CGS unit of energy.</summary>
	public static readonly IUnit Erg = new Unit("erg", "erg", PhysicalDimensions.Energy, UnitSystem.CGS, 1e-7);

	/// <summary>Electron volt - Atomic unit of energy.</summary>
	public static readonly IUnit ElectronVolt = new Unit("electron volt", "eV", PhysicalDimensions.Energy, UnitSystem.Atomic, 1.602176634e-19);

	/// <summary>British thermal unit - Imperial energy unit.</summary>
	public static readonly IUnit BTU = new Unit("British thermal unit", "BTU", PhysicalDimensions.Energy, UnitSystem.Imperial, 1055.06);

	/// <summary>Watt - SI derived unit of power.</summary>
	public static readonly IUnit Watt = new Unit("watt", "W", PhysicalDimensions.Power, UnitSystem.SIDerived);

	/// <summary>Kilowatt - 1000 watts.</summary>
	public static readonly IUnit Kilowatt = new Unit("kilowatt", "kW", PhysicalDimensions.Power, UnitSystem.SIDerived, MetricMagnitudes.Kilo);

	/// <summary>Megawatt - 1,000,000 watts.</summary>
	public static readonly IUnit Megawatt = new Unit("megawatt", "MW", PhysicalDimensions.Power, UnitSystem.SIDerived, MetricMagnitudes.Mega);

	/// <summary>Horsepower - Imperial unit of power (mechanical).</summary>
	public static readonly IUnit Horsepower = new Unit("horsepower", "hp", PhysicalDimensions.Power, UnitSystem.Imperial, 745.699872);

	/// <summary>Celsius - Common temperature scale.</summary>
	public static readonly IUnit Celsius = new Unit("celsius", "°C", PhysicalDimensions.Temperature, UnitSystem.SIDerived, 1.0, 273.15);

	/// <summary>Fahrenheit - Imperial temperature scale.</summary>
	public static readonly IUnit Fahrenheit = new Unit("fahrenheit", "°F", PhysicalDimensions.Temperature, UnitSystem.Imperial, 5.0 / 9.0, 273.15 - (32.0 * 5.0 / 9.0));

	/// <summary>Rankine - Absolute temperature scale based on Fahrenheit.</summary>
	public static readonly IUnit Rankine = new Unit("rankine", "°R", PhysicalDimensions.Temperature, UnitSystem.Imperial, 5.0 / 9.0);

	/// <summary>Milliampere - 0.001 amperes.</summary>
	public static readonly IUnit Milliampere = new Unit("milliampere", "mA", PhysicalDimensions.ElectricCurrent, UnitSystem.SIDerived, MetricMagnitudes.Milli);

	/// <summary>Kiloampere - 1000 amperes.</summary>
	public static readonly IUnit Kiloampere = new Unit("kiloampere", "kA", PhysicalDimensions.ElectricCurrent, UnitSystem.SIDerived, MetricMagnitudes.Kilo);

	/// <summary>Coulomb - SI derived unit of electric charge.</summary>
	public static readonly IUnit Coulomb = new Unit("coulomb", "C", PhysicalDimensions.ElectricCharge, UnitSystem.SIDerived);

	/// <summary>Ampere-hour - Common unit for battery capacity.</summary>
	public static readonly IUnit AmpereHour = new Unit("ampere-hour", "Ah", PhysicalDimensions.ElectricCharge, UnitSystem.SIDerived, 3600.0);

	/// <summary>Volt - SI derived unit of electric potential.</summary>
	public static readonly IUnit Volt = new Unit("volt", "V", PhysicalDimensions.ElectricPotential, UnitSystem.SIDerived);

	/// <summary>Kilovolt - 1000 volts.</summary>
	public static readonly IUnit Kilovolt = new Unit("kilovolt", "kV", PhysicalDimensions.ElectricPotential, UnitSystem.SIDerived, MetricMagnitudes.Kilo);

	/// <summary>Ohm - SI derived unit of electric resistance.</summary>
	public static readonly IUnit Ohm = new Unit("ohm", "Ω", PhysicalDimensions.ElectricResistance, UnitSystem.SIDerived);

	/// <summary>Kilohm - 1000 ohms.</summary>
	public static readonly IUnit Kilohm = new Unit("kilohm", "kΩ", PhysicalDimensions.ElectricResistance, UnitSystem.SIDerived, MetricMagnitudes.Kilo);

	/// <summary>Megohm - 1,000,000 ohms.</summary>
	public static readonly IUnit Megohm = new Unit("megohm", "MΩ", PhysicalDimensions.ElectricResistance, UnitSystem.SIDerived, MetricMagnitudes.Mega);

	/// <summary>Farad - SI derived unit of electric capacitance.</summary>
	public static readonly IUnit Farad = new Unit("farad", "F", PhysicalDimensions.ElectricCapacitance, UnitSystem.SIDerived);

	/// <summary>Microfarad - 0.000001 farads.</summary>
	public static readonly IUnit Microfarad = new Unit("microfarad", "μF", PhysicalDimensions.ElectricCapacitance, UnitSystem.SIDerived, MetricMagnitudes.Micro);

	/// <summary>Nanofarad - 0.000000001 farads.</summary>
	public static readonly IUnit Nanofarad = new Unit("nanofarad", "nF", PhysicalDimensions.ElectricCapacitance, UnitSystem.SIDerived, MetricMagnitudes.Nano);

	/// <summary>Picofarad - 0.000000000001 farads.</summary>
	public static readonly IUnit Picofarad = new Unit("picofarad", "pF", PhysicalDimensions.ElectricCapacitance, UnitSystem.SIDerived, MetricMagnitudes.Pico);

	/// <summary>Kilomole - 1000 moles.</summary>
	public static readonly IUnit Kilomole = new Unit("kilomole", "kmol", PhysicalDimensions.AmountOfSubstance, UnitSystem.SIDerived, MetricMagnitudes.Kilo);

	/// <summary>Millimole - 0.001 moles.</summary>
	public static readonly IUnit Millimole = new Unit("millimole", "mmol", PhysicalDimensions.AmountOfSubstance, UnitSystem.SIDerived, MetricMagnitudes.Milli);

	/// <summary>Molar - Moles per liter.</summary>
	public static readonly IUnit Molar = new Unit("molar", "M", PhysicalDimensions.Concentration, UnitSystem.SIDerived, 1000.0);

	/// <summary>Parts per million - Dimensionless concentration.</summary>
	public static readonly IUnit PartsPerMillion = new Unit("parts per million", "ppm", PhysicalDimensions.Dimensionless, UnitSystem.Other, 1e-6);

	/// <summary>Millicandela - 0.001 candelas.</summary>
	public static readonly IUnit Millicandela = new Unit("millicandela", "mcd", PhysicalDimensions.LuminousIntensity, UnitSystem.SIDerived, MetricMagnitudes.Milli);

	/// <summary>Lumen - SI derived unit of luminous flux.</summary>
	public static readonly IUnit Lumen = new Unit("lumen", "lm", PhysicalDimensions.LuminousFlux, UnitSystem.SIDerived);

	/// <summary>Lux - SI derived unit of illuminance.</summary>
	public static readonly IUnit Lux = new Unit("lux", "lx", PhysicalDimensions.Illuminance, UnitSystem.SIDerived);

	/// <summary>Foot-candle - Imperial unit of illuminance.</summary>
	public static readonly IUnit FootCandle = new Unit("foot-candle", "fc", PhysicalDimensions.Illuminance, UnitSystem.Imperial, 10.764);

	/// <summary>Candela per square meter - SI unit of luminance.</summary>
	public static readonly IUnit CandelaPerSquareMeter = new Unit("candela per square meter", "cd/m²", PhysicalDimensions.Luminance, UnitSystem.SIDerived);

	/// <summary>Nit - Common unit of luminance (same as cd/m²).</summary>
	public static readonly IUnit Nit = new Unit("nit", "nt", PhysicalDimensions.Luminance, UnitSystem.SIDerived);

	/// <summary>Foot-lambert - Imperial unit of luminance.</summary>
	public static readonly IUnit FootLambert = new Unit("foot-lambert", "fL", PhysicalDimensions.Luminance, UnitSystem.Imperial, 3.426);

	/// <summary>Diopter - SI unit of optical power.</summary>
	public static readonly IUnit Diopter = new Unit("diopter", "D", PhysicalDimensions.OpticalPower, UnitSystem.SIDerived);

	/// <summary>Becquerel - SI derived unit of radioactive activity.</summary>
	public static readonly IUnit Becquerel = new Unit("becquerel", "Bq", PhysicalDimensions.RadioactiveActivity, UnitSystem.SIDerived);

	/// <summary>Curie - Traditional unit of radioactive activity.</summary>
	public static readonly IUnit Curie = new Unit("curie", "Ci", PhysicalDimensions.RadioactiveActivity, UnitSystem.Other, 3.7e10);

	/// <summary>Gray - SI derived unit of absorbed dose.</summary>
	public static readonly IUnit Gray = new Unit("gray", "Gy", PhysicalDimensions.AbsorbedDose, UnitSystem.SIDerived);

	/// <summary>Rad - Traditional unit of absorbed dose.</summary>
	public static readonly IUnit Rad = new Unit("rad", "rad", PhysicalDimensions.AbsorbedDose, UnitSystem.Other, 0.01);

	/// <summary>Sievert - SI derived unit of equivalent dose.</summary>
	public static readonly IUnit Sievert = new Unit("sievert", "Sv", PhysicalDimensions.EquivalentDose, UnitSystem.SIDerived);

	/// <summary>Rem - Traditional unit of equivalent dose.</summary>
	public static readonly IUnit Rem = new Unit("rem", "rem", PhysicalDimensions.EquivalentDose, UnitSystem.Other, 0.01);

	/// <summary>Coulomb per kilogram - SI unit of exposure.</summary>
	public static readonly IUnit CoulombPerKilogram = new Unit("coulomb per kilogram", "C/kg", PhysicalDimensions.Exposure, UnitSystem.SIDerived);

	/// <summary>Roentgen - Traditional unit of exposure.</summary>
	public static readonly IUnit Roentgen = new Unit("roentgen", "R", PhysicalDimensions.Exposure, UnitSystem.Other, 2.58e-4);

	/// <summary>Barn - Unit of nuclear cross section.</summary>
	public static readonly IUnit Barn = new Unit("barn", "b", PhysicalDimensions.NuclearCrossSection, UnitSystem.Other, 1e-28);

	/// <summary>Hertz - SI derived unit of frequency.</summary>
	public static readonly IUnit Hertz = new Unit("hertz", "Hz", PhysicalDimensions.Frequency, UnitSystem.SIDerived);

	/// <summary>Kilohertz - 1000 hertz.</summary>
	public static readonly IUnit Kilohertz = new Unit("kilohertz", "kHz", PhysicalDimensions.Frequency, UnitSystem.SIDerived, MetricMagnitudes.Kilo);

	/// <summary>Megahertz - 1,000,000 hertz.</summary>
	public static readonly IUnit Megahertz = new Unit("megahertz", "MHz", PhysicalDimensions.Frequency, UnitSystem.SIDerived, MetricMagnitudes.Mega);

	// === CHEMICAL DOMAIN ===

	/// <summary>Gram per mole - Common unit for molar mass.</summary>
	public static readonly IUnit GramPerMole = new Unit("gram per mole", "g/mol", PhysicalDimensions.MolarMass, UnitSystem.SIDerived, 0.001);

	/// <summary>Kilogram per mole - SI base unit for molar mass.</summary>
	public static readonly IUnit KilogramPerMole = new Unit("kilogram per mole", "kg/mol", PhysicalDimensions.MolarMass, UnitSystem.SIDerived);

	/// <summary>Dalton - Atomic mass unit for molecular masses.</summary>
	public static readonly IUnit Dalton = new Unit("dalton", "Da", PhysicalDimensions.MolarMass, UnitSystem.Atomic, 1.66054e-27);

	/// <summary>Millimolar - 0.001 molar concentration.</summary>
	public static readonly IUnit Millimolar = new Unit("millimolar", "mM", PhysicalDimensions.Concentration, UnitSystem.SIDerived, 1.0);

	/// <summary>Micromolar - 0.000001 molar concentration.</summary>
	public static readonly IUnit Micromolar = new Unit("micromolar", "μM", PhysicalDimensions.Concentration, UnitSystem.SIDerived, 0.001);

	/// <summary>Parts per billion - Dimensionless concentration.</summary>
	public static readonly IUnit PartsPerBillion = new Unit("parts per billion", "ppb", PhysicalDimensions.Dimensionless, UnitSystem.Other, 1e-9);

	/// <summary>Percent by weight - Dimensionless concentration.</summary>
	public static readonly IUnit PercentByWeight = new Unit("percent by weight", "% w/w", PhysicalDimensions.Dimensionless, UnitSystem.Other, 0.01);

	/// <summary>Moles per second - Reaction rate unit.</summary>
	public static readonly IUnit MolesPerSecond = new Unit("moles per second", "mol/s", PhysicalDimensions.ReactionRate, UnitSystem.SIDerived, 1000.0);

	/// <summary>Joules per mole - Activation energy unit.</summary>
	public static readonly IUnit JoulesPerMole = new Unit("joules per mole", "J/mol", PhysicalDimensions.ActivationEnergy, UnitSystem.SIDerived);

	/// <summary>Kilojoules per mole - Common activation energy unit.</summary>
	public static readonly IUnit KilojoulesPerMole = new Unit("kilojoules per mole", "kJ/mol", PhysicalDimensions.ActivationEnergy, UnitSystem.SIDerived, 1000.0);

	/// <summary>Calories per mole - Traditional activation energy unit.</summary>
	public static readonly IUnit CaloriesPerMole = new Unit("calories per mole", "cal/mol", PhysicalDimensions.ActivationEnergy, UnitSystem.Other, 4.184);

	/// <summary>Per second - First-order rate constant.</summary>
	public static readonly IUnit PerSecond = new Unit("per second", "s⁻¹", PhysicalDimensions.RateConstant, UnitSystem.SIDerived);

	/// <summary>Katal - SI unit of enzyme activity.</summary>
	public static readonly IUnit Katal = new Unit("katal", "kat", PhysicalDimensions.EnzymeActivity, UnitSystem.SIDerived);

	/// <summary>Enzyme unit - Traditional enzyme activity unit.</summary>
	public static readonly IUnit EnzymeUnit = new Unit("enzyme unit", "U", PhysicalDimensions.EnzymeActivity, UnitSystem.Other, 1.0 / 60.0e6); // μmol/min

	/// <summary>Newton per meter - Surface tension unit.</summary>
	public static readonly IUnit NewtonPerMeter = new Unit("newton per meter", "N/m", PhysicalDimensions.SurfaceTension, UnitSystem.SIDerived);

	/// <summary>Dyne per centimeter - CGS surface tension unit.</summary>
	public static readonly IUnit DynePerCentimeter = new Unit("dyne per centimeter", "dyn/cm", PhysicalDimensions.SurfaceTension, UnitSystem.CGS, 1e-3);

	/// <summary>Pascal second - Dynamic viscosity unit.</summary>
	public static readonly IUnit PascalSecond = new Unit("pascal second", "Pa·s", PhysicalDimensions.DynamicViscosity, UnitSystem.SIDerived);

	/// <summary>Poise - CGS dynamic viscosity unit.</summary>
	public static readonly IUnit Poise = new Unit("poise", "P", PhysicalDimensions.DynamicViscosity, UnitSystem.CGS, 0.1);

	/// <summary>Centipoise - Common dynamic viscosity unit.</summary>
	public static readonly IUnit Centipoise = new Unit("centipoise", "cP", PhysicalDimensions.DynamicViscosity, UnitSystem.CGS, 0.001);

	// === FLUID DYNAMICS DOMAIN ===

	/// <summary>Kilogram per cubic meter - SI base unit for density.</summary>
	public static readonly IUnit KilogramPerCubicMeter = new Unit("kilogram per cubic meter", "kg/m³", PhysicalDimensions.Density, UnitSystem.SIDerived);

	/// <summary>Gram per cubic centimeter - Common density unit.</summary>
	public static readonly IUnit GramPerCubicCentimeter = new Unit("gram per cubic centimeter", "g/cm³", PhysicalDimensions.Density, UnitSystem.SIDerived, 1000.0);

	/// <summary>Gram per liter - Common density unit for liquids.</summary>
	public static readonly IUnit GramPerLiter = new Unit("gram per liter", "g/L", PhysicalDimensions.Density, UnitSystem.SIDerived, 1.0);

	//TODO: === REMAINING FLUID DYNAMICS DOMAIN ===
	// Flow Rate Units: cubic meters per second (m³/s), liters per minute (L/min), gallons per minute (GPM)
	// Viscosity Units: pascal-second (Pa·s), poise (P), centipoise (cP)
	// Surface Tension Units: newtons per meter (N/m), dynes per centimeter (dyn/cm)
	// Reynolds Number: dimensionless [1]

	//TODO: === THERMODYNAMICS DOMAIN ===
	// Heat Units: joule (J), calorie (cal), British thermal unit (BTU)
	// Heat Capacity Units: joules per kelvin (J/K), calories per kelvin (cal/K)
	// Specific Heat Units: joules per kilogram-kelvin (J/(kg·K))
	// Thermal Conductivity Units: watts per meter-kelvin (W/(m·K))
	// Entropy Units: joules per kelvin (J/K)
	// Heat Transfer Coefficient Units: watts per square meter-kelvin (W/(m²·K))
	// Thermal Expansion Coefficient Units: per kelvin (K⁻¹)

	//TODO: === ELECTROMAGNETICS DOMAIN ===
	// Magnetic Field Units: tesla (T), gauss (G)
	// Magnetic Flux Units: weber (Wb), maxwell (Mx)
	// Inductance Units: henry (H), millihenry (mH), microhenry (μH)
	// Magnetic Permeability Units: henries per meter (H/m)
	// Electric Field Units: volts per meter (V/m), newtons per coulomb (N/C)
	// Electric Flux Units: volt-meters (V·m)
	// Permittivity Units: farads per meter (F/m)

	//TODO: === MATERIALS SCIENCE DOMAIN ===
	// Stress Units: pascal (Pa), pounds per square inch (psi), bars
	// Strain Units: dimensionless [1], percent (%)
	// Elastic Modulus Units: pascal (Pa), gigapascals (GPa)
	// Yield Strength Units: pascal (Pa), megapascals (MPa)
	// Hardness Units: Brinell (HB), Rockwell (HRC), Vickers (HV)
	// Fracture Toughness Units: pascal-square root meter (Pa·√m)

	//TODO: === NUCLEAR & RADIATION DOMAIN ===
	// Radioactivity Units: becquerel (Bq), curie (Ci)
	// Absorbed Dose Units: gray (Gy), rad
	// Dose Equivalent Units: sievert (Sv), rem
	// Exposure Units: coulombs per kilogram (C/kg), roentgen (R)
	// Cross Section Units: barn (b), square meters (m²)
	// Half-Life Units: seconds (s), years (yr)

	//TODO: === OPTICS & PHOTONICS DOMAIN ===
	// Luminous Efficacy Units: lumens per watt (lm/W)
	// Illuminance Units: lux (lx), foot-candles (fc)
	// Luminance Units: candelas per square meter (cd/m²), nits
	// Wavelength Units: meters (m), nanometers (nm), angstroms (Å)
	// Frequency Units: hertz (Hz), terahertz (THz)
	// Refractive Index Units: dimensionless [1]
	// Optical Power Units: diopters (D)

	//TODO: === ASTRONOMY & COSMOLOGY DOMAIN ===
	// Distance Units: astronomical unit (AU), light-year (ly), parsec (pc)
	// Stellar Magnitude Units: dimensionless [1]
	// Solar Mass Units: kilograms (kg), solar masses (M☉)
	// Redshift Units: dimensionless [1]
	// Angular Resolution Units: arcseconds ("), milliarcseconds (mas)
	// Flux Density Units: janskys (Jy), watts per square meter per hertz (W/(m²·Hz))

	//TODO: === GEOPHYSICS DOMAIN ===
	// Seismic Velocity Units: meters per second (m/s), kilometers per second (km/s)
	// Earthquake Magnitude Units: Richter scale [1], moment magnitude [1]
	// Gravity Units: meters per second squared (m/s²), gals (Gal)
	// Magnetic Declination Units: degrees (°), radians (rad)
	// Atmospheric Pressure Units: pascal (Pa), millibars (mbar), inches of mercury (inHg)

	//TODO: === QUANTUM MECHANICS DOMAIN ===
	// Action Units: joule-seconds (J·s), reduced Planck constant (ℏ)
	// Wave Number Units: per meter (m⁻¹), per centimeter (cm⁻¹)
	// Angular Momentum Units: joule-seconds (J·s), reduced Planck constant (ℏ)
	// Magnetic Moment Units: joules per tesla (J/T), Bohr magnetons (μB)
	// Cross Section Units: square meters (m²), barns (b)

	//TODO: === BIOCHEMISTRY DOMAIN ===
	// Molarity Units: moles per liter (mol/L), millimolar (mM)
	// Enzyme Activity Units: katal (kat), enzyme units (U)
	// pH Units: dimensionless [1]
	// Buffer Capacity Units: moles per liter per pH unit (mol/(L·pH))
	// Dissociation Constant Units: molar (M)

	//TODO: === ACOUSTICS DOMAIN ===
	// Sound Pressure Units: pascal (Pa), micropascals (μPa)
	// Sound Intensity Units: watts per square meter (W/m²)
	// Sound Power Units: watts (W), acoustic watts
	// Decibel Units: dimensionless [1] (logarithmic scale)
	// Acoustic Impedance Units: pascal-seconds per meter (Pa·s/m)
	// Reverberation Time Units: seconds (s)
}
