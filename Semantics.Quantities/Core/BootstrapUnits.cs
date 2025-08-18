// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Internal registry of bootstrap units used during initialization to break circular dependencies.
/// These units are replaced with actual units after the Units class is initialized.
/// </summary>
internal static class BootstrapUnits
{
	#region Fundamental SI Base Units
	/// <summary>Bootstrap unit for radian angle measurement.</summary>
	internal static readonly BootstrapUnit Radian = new("radian", "rad");

	/// <summary>Bootstrap unit for the SI base unit of length.</summary>
	internal static readonly BootstrapUnit Meter = new("meter", "m");

	/// <summary>Bootstrap unit for the SI base unit of mass.</summary>
	internal static readonly BootstrapUnit Kilogram = new("kilogram", "kg");

	/// <summary>Bootstrap unit for the SI base unit of time.</summary>
	internal static readonly BootstrapUnit Second = new("second", "s");

	/// <summary>Bootstrap unit for the SI base unit of electric current.</summary>
	internal static readonly BootstrapUnit Ampere = new("ampere", "A");

	/// <summary>Bootstrap unit for the SI base unit of temperature.</summary>
	internal static readonly BootstrapUnit Kelvin = new("kelvin", "K");

	/// <summary>Bootstrap unit for the SI base unit of amount of substance.</summary>
	internal static readonly BootstrapUnit Mole = new("mole", "mol");

	/// <summary>Bootstrap unit for the SI base unit of luminous intensity.</summary>
	internal static readonly BootstrapUnit Candela = new("candela", "cd");
	#endregion

	#region Dimensionless and Special Units
	/// <summary>Bootstrap unit for dimensionless quantities.</summary>
	internal static readonly BootstrapUnit Dimensionless = new("dimensionless", "1");

	/// <summary>Bootstrap unit for decibel measurements.</summary>
	internal static readonly BootstrapUnit Decibel = new("decibel", "dB");

	/// <summary>Bootstrap unit for pH scale measurements.</summary>
	internal static readonly BootstrapUnit PH = new("pH", "pH");

	/// <summary>Bootstrap unit for coefficient measurements.</summary>
	internal static readonly BootstrapUnit Coefficient = new("coefficient", "coeff");
	#endregion

	#region Additional Bootstrap Units for Circular Dependencies
	/// <summary>Bootstrap unit for mass in grams.</summary>
	internal static readonly BootstrapUnit Gram = new("gram", "g");

	/// <summary>Bootstrap unit for molar concentration.</summary>
	internal static readonly BootstrapUnit Molar = new("molar", "M");

	/// <summary>Bootstrap unit for luminous flux.</summary>
	internal static readonly BootstrapUnit Lumen = new("lumen", "lm");

	/// <summary>Bootstrap unit for illuminance.</summary>
	internal static readonly BootstrapUnit Lux = new("lux", "lx");

	/// <summary>Bootstrap unit for radioactive activity.</summary>
	internal static readonly BootstrapUnit Becquerel = new("becquerel", "Bq");

	/// <summary>Bootstrap unit for absorbed dose.</summary>
	internal static readonly BootstrapUnit Gray = new("gray", "Gy");

	/// <summary>Bootstrap unit for equivalent dose.</summary>
	internal static readonly BootstrapUnit Sievert = new("sievert", "Sv");

	/// <summary>Bootstrap unit for nuclear cross section.</summary>
	internal static readonly BootstrapUnit Barn = new("barn", "b");

	/// <summary>Bootstrap unit for optical power.</summary>
	internal static readonly BootstrapUnit Diopter = new("diopter", "D");
	#endregion

	#region Derived SI Units Bootstrap
	/// <summary>Bootstrap unit for area.</summary>
	internal static readonly BootstrapUnit SquareMeter = new("square meter", "m²");

	/// <summary>Bootstrap unit for volume.</summary>
	internal static readonly BootstrapUnit CubicMeter = new("cubic meter", "m³");

	/// <summary>Bootstrap unit for velocity.</summary>
	internal static readonly BootstrapUnit MetersPerSecond = new("meters per second", "m/s");

	/// <summary>Bootstrap unit for acceleration.</summary>
	internal static readonly BootstrapUnit MetersPerSecondSquared = new("meters per second squared", "m/s²");

	/// <summary>Bootstrap unit for force.</summary>
	internal static readonly BootstrapUnit Newton = new("newton", "N");

	/// <summary>Bootstrap unit for pressure.</summary>
	internal static readonly BootstrapUnit Pascal = new("pascal", "Pa");

	/// <summary>Bootstrap unit for energy.</summary>
	internal static readonly BootstrapUnit Joule = new("joule", "J");

	/// <summary>Bootstrap unit for power.</summary>
	internal static readonly BootstrapUnit Watt = new("watt", "W");

	/// <summary>Bootstrap unit for electric potential.</summary>
	internal static readonly BootstrapUnit Volt = new("volt", "V");

	/// <summary>Bootstrap unit for electric resistance.</summary>
	internal static readonly BootstrapUnit Ohm = new("ohm", "Ω");

	/// <summary>Bootstrap unit for electric charge.</summary>
	internal static readonly BootstrapUnit Coulomb = new("coulomb", "C");

	/// <summary>Bootstrap unit for electric capacitance.</summary>
	internal static readonly BootstrapUnit Farad = new("farad", "F");

	/// <summary>Bootstrap unit for frequency.</summary>
	internal static readonly BootstrapUnit Hertz = new("hertz", "Hz");
	#endregion
}
