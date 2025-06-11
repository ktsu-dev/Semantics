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
