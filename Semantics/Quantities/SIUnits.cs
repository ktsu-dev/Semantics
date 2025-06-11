// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Provides standard SI units and common derived units.
/// </summary>
public static class SIUnits
{
	// Base SI Units
	/// <summary>Meter - SI base unit of length.</summary>
	public static readonly SIUnit Meter = new("m", "meter", "meters");

	/// <summary>Kilogram - SI base unit of mass.</summary>
	public static readonly SIUnit Kilogram = new("kg", "kilogram", "kilograms");

	/// <summary>Second - SI base unit of time.</summary>
	public static readonly SIUnit Second = new("s", "second", "seconds");

	/// <summary>Ampere - SI base unit of electric current.</summary>
	public static readonly SIUnit Ampere = new("A", "ampere", "amperes");

	/// <summary>Kelvin - SI base unit of thermodynamic temperature.</summary>
	public static readonly SIUnit Kelvin = new("K", "kelvin", "kelvins");

	/// <summary>Mole - SI base unit of amount of substance.</summary>
	public static readonly SIUnit Mole = new("mol", "mole", "moles");

	/// <summary>Candela - SI base unit of luminous intensity.</summary>
	public static readonly SIUnit Candela = new("cd", "candela", "candelas");

	// Derived SI Units
	/// <summary>Newton - SI derived unit of force.</summary>
	public static readonly SIUnit Newton = new("N", "newton", "newtons");

	/// <summary>Joule - SI derived unit of energy.</summary>
	public static readonly SIUnit Joule = new("J", "joule", "joules");

	/// <summary>Watt - SI derived unit of power.</summary>
	public static readonly SIUnit Watt = new("W", "watt", "watts");

	/// <summary>Pascal - SI derived unit of pressure.</summary>
	public static readonly SIUnit Pascal = new("Pa", "pascal", "pascals");

	/// <summary>Hertz - SI derived unit of frequency.</summary>
	public static readonly SIUnit Hertz = new("Hz", "hertz", "hertz");

	/// <summary>Volt - SI derived unit of electric potential.</summary>
	public static readonly SIUnit Volt = new("V", "volt", "volts");

	/// <summary>Coulomb - SI derived unit of electric charge.</summary>
	public static readonly SIUnit Coulomb = new("C", "coulomb", "coulombs");

	/// <summary>Radian - SI derived unit of angle.</summary>
	public static readonly SIUnit Radian = new("rad", "radian", "radians");

	/// <summary>Steradian - SI derived unit of solid angle.</summary>
	public static readonly SIUnit Steradian = new("sr", "steradian", "steradians");

	/// <summary>Lumen - SI derived unit of luminous flux.</summary>
	public static readonly SIUnit Lumen = new("lm", "lumen", "lumens");

	/// <summary>Lux - SI derived unit of illuminance.</summary>
	public static readonly SIUnit Lux = new("lx", "lux", "lux");

	// Composite units
	/// <summary>Meter per second - SI derived unit of velocity.</summary>
	public static readonly SIUnit MeterPerSecond = new("m/s", "meter per second", "meters per second");

	/// <summary>Meter per second squared - SI derived unit of acceleration.</summary>
	public static readonly SIUnit MeterPerSecondSquared = new("m/s²", "meter per second squared", "meters per second squared");

	/// <summary>Meter per second cubed - SI derived unit of jerk.</summary>
	public static readonly SIUnit MeterPerSecondCubed = new("m/s³", "meter per second cubed", "meters per second cubed");

	/// <summary>Square meter - SI derived unit of area.</summary>
	public static readonly SIUnit SquareMeter = new("m²", "square meter", "square meters");

	/// <summary>Cubic meter - SI derived unit of volume.</summary>
	public static readonly SIUnit CubicMeter = new("m³", "cubic meter", "cubic meters");

	/// <summary>Kilogram per cubic meter - SI derived unit of density.</summary>
	public static readonly SIUnit KilogramPerCubicMeter = new("kg/m³", "kilogram per cubic meter", "kilograms per cubic meter");

	/// <summary>Kilogram meter per second - SI derived unit of momentum.</summary>
	public static readonly SIUnit KilogramMeterPerSecond = new("kg⋅m/s", "kilogram meter per second", "kilogram meters per second");

	/// <summary>Newton meter - SI derived unit of torque.</summary>
	public static readonly SIUnit NewtonMeter = new("N⋅m", "newton meter", "newton meters");

	/// <summary>Kilogram square meter - SI derived unit of moment of inertia.</summary>
	public static readonly SIUnit KilogramSquareMeter = new("kg⋅m²", "kilogram square meter", "kilogram square meters");

	/// <summary>Radian per second - SI derived unit of angular velocity.</summary>
	public static readonly SIUnit RadianPerSecond = new("rad/s", "radian per second", "radians per second");

	/// <summary>Radian per second squared - SI derived unit of angular acceleration.</summary>
	public static readonly SIUnit RadianPerSecondSquared = new("rad/s²", "radian per second squared", "radians per second squared");

	/// <summary>Kilogram per mole - SI derived unit of molar mass.</summary>
	public static readonly SIUnit KilogramPerMole = new("kg/mol", "kilogram per mole", "kilograms per mole");

	/// <summary>Ohm - SI derived unit of electrical resistance.</summary>
	public static readonly SIUnit Ohm = new("Ω", "ohm", "ohms");

	/// <summary>Kilogram square meter per second - SI derived unit of angular momentum.</summary>
	public static readonly SIUnit KilogramSquareMeterPerSecond = new("kg⋅m²/s", "kilogram square meter per second", "kilogram square meters per second");

	// Length units
	/// <summary>Millimeter - derived unit of length.</summary>
	public static readonly DerivedSIUnit Millimeter = new("mm", "millimeter", "millimeters", Meter, PhysicalConstants.Milli);

	/// <summary>Centimeter - derived unit of length.</summary>
	public static readonly DerivedSIUnit Centimeter = new("cm", "centimeter", "centimeters", Meter, PhysicalConstants.Centi);

	/// <summary>Kilometer - derived unit of length.</summary>
	public static readonly DerivedSIUnit Kilometer = new("km", "kilometer", "kilometers", Meter, PhysicalConstants.Kilo);

	// Mass units
	/// <summary>Gram - derived unit of mass.</summary>
	public static readonly DerivedSIUnit Gram = new("g", "gram", "grams", Kilogram, PhysicalConstants.Milli);

	/// <summary>Milligram - derived unit of mass.</summary>
	public static readonly DerivedSIUnit Milligram = new("mg", "milligram", "milligrams", Kilogram, PhysicalConstants.Micro);

	// Force units
	/// <summary>Kilonewton - derived unit of force.</summary>
	public static readonly DerivedSIUnit Kilonewton = new("kN", "kilonewton", "kilonewtons", Newton, PhysicalConstants.Kilo);

	/// <summary>Millinewton - derived unit of force.</summary>
	public static readonly DerivedSIUnit Millinewton = new("mN", "millinewton", "millinewtons", Newton, PhysicalConstants.Milli);

	/// <summary>Micronewton - derived unit of force.</summary>
	public static readonly DerivedSIUnit Micronewton = new("μN", "micronewton", "micronewtons", Newton, PhysicalConstants.Micro);

	// Energy units
	/// <summary>Kilojoule - derived unit of energy.</summary>
	public static readonly DerivedSIUnit Kilojoule = new("kJ", "kilojoule", "kilojoules", Joule, PhysicalConstants.Kilo);

	/// <summary>Millijoule - derived unit of energy.</summary>
	public static readonly DerivedSIUnit Millijoule = new("mJ", "millijoule", "millijoules", Joule, PhysicalConstants.Milli);

	/// <summary>Microjoule - derived unit of energy.</summary>
	public static readonly DerivedSIUnit Microjoule = new("μJ", "microjoule", "microjoules", Joule, PhysicalConstants.Micro);

	/// <summary>Nanojoule - derived unit of energy.</summary>
	public static readonly DerivedSIUnit Nanojoule = new("nJ", "nanojoule", "nanojoules", Joule, PhysicalConstants.Nano);

	/// <summary>Megajoule - derived unit of energy.</summary>
	public static readonly DerivedSIUnit Megajoule = new("MJ", "megajoule", "megajoules", Joule, PhysicalConstants.Mega);

	/// <summary>Gigajoule - derived unit of energy.</summary>
	public static readonly DerivedSIUnit Gigajoule = new("GJ", "gigajoule", "gigajoules", Joule, PhysicalConstants.Giga);
}
