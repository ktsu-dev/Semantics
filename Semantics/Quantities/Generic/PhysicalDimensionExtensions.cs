// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Extension methods for creating physical quantities from numeric values using the unit system.
/// </summary>
public static class PhysicalDimensionExtensions
{
	#region Length Extensions
	/// <summary>
	/// Creates a Length from a value in meters.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in meters.</param>
	/// <returns>A new Length instance.</returns>
	public static Length<T> Meters<T>(this T value) where T : struct, INumber<T>
		=> Length<T>.Create(ConvertToBaseUnit(value, Units.Meter));

	/// <summary>
	/// Creates a Length from a value in kilometers.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in kilometers.</param>
	/// <returns>A new Length instance.</returns>
	public static Length<T> Kilometers<T>(this T value) where T : struct, INumber<T>
		=> Length<T>.Create(ConvertToBaseUnit(value, Units.Kilometer));

	/// <summary>
	/// Creates a Length from a value in centimeters.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in centimeters.</param>
	/// <returns>A new Length instance.</returns>
	public static Length<T> Centimeters<T>(this T value) where T : struct, INumber<T>
		=> Length<T>.Create(ConvertToBaseUnit(value, Units.Centimeter));

	/// <summary>
	/// Creates a Length from a value in millimeters.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in millimeters.</param>
	/// <returns>A new Length instance.</returns>
	public static Length<T> Millimeters<T>(this T value) where T : struct, INumber<T>
		=> Length<T>.Create(ConvertToBaseUnit(value, Units.Millimeter));

	/// <summary>
	/// Creates a Length from a value in feet.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in feet.</param>
	/// <returns>A new Length instance.</returns>
	public static Length<T> Feet<T>(this T value) where T : struct, INumber<T>
		=> Length<T>.Create(ConvertToBaseUnit(value, Units.Foot));

	/// <summary>
	/// Creates a Length from a value in inches.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in inches.</param>
	/// <returns>A new Length instance.</returns>
	public static Length<T> Inches<T>(this T value) where T : struct, INumber<T>
		=> Length<T>.Create(ConvertToBaseUnit(value, Units.Inch));

	/// <summary>
	/// Creates a Length from a value in yards.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in yards.</param>
	/// <returns>A new Length instance.</returns>
	public static Length<T> Yards<T>(this T value) where T : struct, INumber<T>
		=> Length<T>.Create(ConvertToBaseUnit(value, Units.Yard));

	/// <summary>
	/// Creates a Length from a value in miles.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in miles.</param>
	/// <returns>A new Length instance.</returns>
	public static Length<T> Miles<T>(this T value) where T : struct, INumber<T>
		=> Length<T>.Create(ConvertToBaseUnit(value, Units.Mile));
	#endregion

	#region Mass Extensions
	/// <summary>
	/// Creates a Mass from a value in kilograms.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in kilograms.</param>
	/// <returns>A new Mass instance.</returns>
	public static Mass<T> Kilograms<T>(this T value) where T : struct, INumber<T>
		=> Mass<T>.Create(ConvertToBaseUnit(value, Units.Kilogram));

	/// <summary>
	/// Creates a Mass from a value in grams.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in grams.</param>
	/// <returns>A new Mass instance.</returns>
	public static Mass<T> Grams<T>(this T value) where T : struct, INumber<T>
		=> Mass<T>.Create(ConvertToBaseUnit(value, Units.Gram));

	/// <summary>
	/// Creates a Mass from a value in pounds.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in pounds.</param>
	/// <returns>A new Mass instance.</returns>
	public static Mass<T> Pounds<T>(this T value) where T : struct, INumber<T>
		=> Mass<T>.Create(ConvertToBaseUnit(value, Units.Pound));

	/// <summary>
	/// Creates a Mass from a value in ounces.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in ounces.</param>
	/// <returns>A new Mass instance.</returns>
	public static Mass<T> Ounces<T>(this T value) where T : struct, INumber<T>
		=> Mass<T>.Create(ConvertToBaseUnit(value, Units.Ounce));

	/// <summary>
	/// Creates a Mass from a value in stones.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in stones.</param>
	/// <returns>A new Mass instance.</returns>
	public static Mass<T> Stones<T>(this T value) where T : struct, INumber<T>
		=> Mass<T>.Create(ConvertToBaseUnit(value, Units.Stone));
	#endregion

	#region Time Extensions
	/// <summary>
	/// Creates a Time from a value in seconds.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in seconds.</param>
	/// <returns>A new Time instance.</returns>
	public static Time<T> Seconds<T>(this T value) where T : struct, INumber<T>
		=> Time<T>.Create(ConvertToBaseUnit(value, Units.Second));

	/// <summary>
	/// Creates a Time from a value in minutes.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in minutes.</param>
	/// <returns>A new Time instance.</returns>
	public static Time<T> Minutes<T>(this T value) where T : struct, INumber<T>
		=> Time<T>.Create(ConvertToBaseUnit(value, Units.Minute));

	/// <summary>
	/// Creates a Time from a value in hours.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in hours.</param>
	/// <returns>A new Time instance.</returns>
	public static Time<T> Hours<T>(this T value) where T : struct, INumber<T>
		=> Time<T>.Create(ConvertToBaseUnit(value, Units.Hour));

	/// <summary>
	/// Creates a Time from a value in days.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in days.</param>
	/// <returns>A new Time instance.</returns>
	public static Time<T> Days<T>(this T value) where T : struct, INumber<T>
		=> Time<T>.Create(ConvertToBaseUnit(value, Units.Day));

	/// <summary>
	/// Creates a Time from a value in milliseconds.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in milliseconds.</param>
	/// <returns>A new Time instance.</returns>
	public static Time<T> Milliseconds<T>(this T value) where T : struct, INumber<T>
		=> Time<T>.Create(ConvertToBaseUnit(value, Units.Millisecond));
	#endregion

	#region Area Extensions
	/// <summary>
	/// Creates an Area from a value in square meters.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in square meters.</param>
	/// <returns>A new Area instance.</returns>
	public static Area<T> SquareMeters<T>(this T value) where T : struct, INumber<T>
		=> Area<T>.Create(ConvertToBaseUnit(value, Units.SquareMeter));

	/// <summary>
	/// Creates an Area from a value in square kilometers.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in square kilometers.</param>
	/// <returns>A new Area instance.</returns>
	public static Area<T> SquareKilometers<T>(this T value) where T : struct, INumber<T>
		=> Area<T>.Create(ConvertToBaseUnit(value, Units.SquareKilometer));

	/// <summary>
	/// Creates an Area from a value in square feet.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in square feet.</param>
	/// <returns>A new Area instance.</returns>
	public static Area<T> SquareFeet<T>(this T value) where T : struct, INumber<T>
		=> Area<T>.Create(ConvertToBaseUnit(value, Units.SquareFoot));

	/// <summary>
	/// Creates an Area from a value in acres.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in acres.</param>
	/// <returns>A new Area instance.</returns>
	public static Area<T> Acres<T>(this T value) where T : struct, INumber<T>
		=> Area<T>.Create(ConvertToBaseUnit(value, Units.Acre));
	#endregion

	#region Volume Extensions
	/// <summary>
	/// Creates a Volume from a value in cubic meters.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in cubic meters.</param>
	/// <returns>A new Volume instance.</returns>
	public static Volume<T> CubicMeters<T>(this T value) where T : struct, INumber<T>
		=> Volume<T>.Create(ConvertToBaseUnit(value, Units.CubicMeter));

	/// <summary>
	/// Creates a Volume from a value in liters.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in liters.</param>
	/// <returns>A new Volume instance.</returns>
	public static Volume<T> Liters<T>(this T value) where T : struct, INumber<T>
		=> Volume<T>.Create(ConvertToBaseUnit(value, Units.Liter));

	/// <summary>
	/// Creates a Volume from a value in milliliters.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in milliliters.</param>
	/// <returns>A new Volume instance.</returns>
	public static Volume<T> Milliliters<T>(this T value) where T : struct, INumber<T>
		=> Volume<T>.Create(ConvertToBaseUnit(value, Units.Milliliter));

	/// <summary>
	/// Creates a Volume from a value in cubic feet.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in cubic feet.</param>
	/// <returns>A new Volume instance.</returns>
	public static Volume<T> CubicFeet<T>(this T value) where T : struct, INumber<T>
		=> Volume<T>.Create(ConvertToBaseUnit(value, Units.CubicFoot));

	/// <summary>
	/// Creates a Volume from a value in US gallons.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in US gallons.</param>
	/// <returns>A new Volume instance.</returns>
	public static Volume<T> USGallons<T>(this T value) where T : struct, INumber<T>
		=> Volume<T>.Create(ConvertToBaseUnit(value, Units.USGallon));
	#endregion

	#region Energy Extensions
	/// <summary>
	/// Creates an Energy from a value in joules.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in joules.</param>
	/// <returns>A new Energy instance.</returns>
	public static Energy<T> Joules<T>(this T value) where T : struct, INumber<T>
		=> Energy<T>.Create(ConvertToBaseUnit(value, Units.Joule));

	/// <summary>
	/// Creates an Energy from a value in kilojoules.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in kilojoules.</param>
	/// <returns>A new Energy instance.</returns>
	public static Energy<T> Kilojoules<T>(this T value) where T : struct, INumber<T>
		=> Energy<T>.Create(ConvertToBaseUnit(value, Units.Kilojoule));

	/// <summary>
	/// Creates an Energy from a value in calories.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in calories.</param>
	/// <returns>A new Energy instance.</returns>
	public static Energy<T> Calories<T>(this T value) where T : struct, INumber<T>
		=> Energy<T>.Create(ConvertToBaseUnit(value, Units.Calorie));

	/// <summary>
	/// Creates an Energy from a value in kilowatt-hours.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in kilowatt-hours.</param>
	/// <returns>A new Energy instance.</returns>
	public static Energy<T> KilowattHours<T>(this T value) where T : struct, INumber<T>
		=> Energy<T>.Create(ConvertToBaseUnit(value, Units.KilowattHour));
	#endregion

	#region Power Extensions
	/// <summary>
	/// Creates a Power from a value in watts.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in watts.</param>
	/// <returns>A new Power instance.</returns>
	public static Power<T> Watts<T>(this T value) where T : struct, INumber<T>
		=> Power<T>.Create(ConvertToBaseUnit(value, Units.Watt));

	/// <summary>
	/// Creates a Power from a value in kilowatts.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in kilowatts.</param>
	/// <returns>A new Power instance.</returns>
	public static Power<T> Kilowatts<T>(this T value) where T : struct, INumber<T>
		=> Power<T>.Create(ConvertToBaseUnit(value, Units.Kilowatt));

	/// <summary>
	/// Creates a Power from a value in horsepower.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in horsepower.</param>
	/// <returns>A new Power instance.</returns>
	public static Power<T> Horsepower<T>(this T value) where T : struct, INumber<T>
		=> Power<T>.Create(ConvertToBaseUnit(value, Units.Horsepower));
	#endregion

	#region Force Extensions
	/// <summary>
	/// Creates a Force from a value in newtons.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in newtons.</param>
	/// <returns>A new Force instance.</returns>
	public static Force<T> Newtons<T>(this T value) where T : struct, INumber<T>
		=> Force<T>.Create(ConvertToBaseUnit(value, Units.Newton));

	/// <summary>
	/// Creates a Force from a value in pounds-force.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in pounds-force.</param>
	/// <returns>A new Force instance.</returns>
	public static Force<T> PoundsForce<T>(this T value) where T : struct, INumber<T>
		=> Force<T>.Create(ConvertToBaseUnit(value, Units.PoundForce));
	#endregion

	#region Pressure Extensions
	/// <summary>
	/// Creates a Pressure from a value in pascals.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in pascals.</param>
	/// <returns>A new Pressure instance.</returns>
	public static Pressure<T> Pascals<T>(this T value) where T : struct, INumber<T>
		=> Pressure<T>.Create(ConvertToBaseUnit(value, Units.Pascal));

	/// <summary>
	/// Creates a Pressure from a value in atmospheres.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in atmospheres.</param>
	/// <returns>A new Pressure instance.</returns>
	public static Pressure<T> Atmospheres<T>(this T value) where T : struct, INumber<T>
		=> Pressure<T>.Create(ConvertToBaseUnit(value, Units.Atmosphere));

	/// <summary>
	/// Creates a Pressure from a value in PSI (pounds per square inch).
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in PSI.</param>
	/// <returns>A new Pressure instance.</returns>
	public static Pressure<T> PSI<T>(this T value) where T : struct, INumber<T>
		=> Pressure<T>.Create(ConvertToBaseUnit(value, Units.PSI));
	#endregion

	#region Temperature Extensions
	/// <summary>
	/// Creates a Temperature from a value in Kelvin.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in Kelvin.</param>
	/// <returns>A new Temperature instance.</returns>
	public static Temperature<T> Kelvin<T>(this T value) where T : struct, INumber<T>
		=> Temperature<T>.Create(ConvertToBaseUnit(value, Units.Kelvin));

	/// <summary>
	/// Creates a Temperature from a value in Celsius.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in Celsius.</param>
	/// <returns>A new Temperature instance.</returns>
	public static Temperature<T> Celsius<T>(this T value) where T : struct, INumber<T>
		=> Temperature<T>.Create(ConvertToBaseUnit(value, Units.Celsius));

	/// <summary>
	/// Creates a Temperature from a value in Fahrenheit.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in Fahrenheit.</param>
	/// <returns>A new Temperature instance.</returns>
	public static Temperature<T> Fahrenheit<T>(this T value) where T : struct, INumber<T>
		=> Temperature<T>.Create(ConvertToBaseUnit(value, Units.Fahrenheit));
	#endregion

	#region Velocity Extensions
	/// <summary>
	/// Creates a Velocity from a value in meters per second.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in meters per second.</param>
	/// <returns>A new Velocity instance.</returns>
	public static Velocity<T> MetersPerSecond<T>(this T value) where T : struct, INumber<T>
		=> Velocity<T>.Create(ConvertToBaseUnit(value, Units.MeterPerSecond));

	/// <summary>
	/// Creates a Velocity from a value in kilometers per hour.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in kilometers per hour.</param>
	/// <returns>A new Velocity instance.</returns>
	public static Velocity<T> KilometersPerHour<T>(this T value) where T : struct, INumber<T>
		=> Velocity<T>.Create(ConvertToBaseUnit(value, Units.KilometerPerHour));

	/// <summary>
	/// Creates a Velocity from a value in miles per hour.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in miles per hour.</param>
	/// <returns>A new Velocity instance.</returns>
	public static Velocity<T> MilesPerHour<T>(this T value) where T : struct, INumber<T>
		=> Velocity<T>.Create(ConvertToBaseUnit(value, Units.MilePerHour));
	#endregion

	#region Frequency Extensions
	/// <summary>
	/// Creates a Frequency from a value in hertz.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in hertz.</param>
	/// <returns>A new Frequency instance.</returns>
	public static Frequency<T> Hertz<T>(this T value) where T : struct, INumber<T>
		=> Frequency<T>.Create(ConvertToBaseUnit(value, Units.Hertz));

	/// <summary>
	/// Creates a Frequency from a value in kilohertz.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in kilohertz.</param>
	/// <returns>A new Frequency instance.</returns>
	public static Frequency<T> Kilohertz<T>(this T value) where T : struct, INumber<T>
		=> Frequency<T>.Create(ConvertToBaseUnit(value, Units.Kilohertz));

	/// <summary>
	/// Creates a Frequency from a value in megahertz.
	/// </summary>
	/// <typeparam name="T">The numeric type of the input value.</typeparam>
	/// <param name="value">The value in megahertz.</param>
	/// <returns>A new Frequency instance.</returns>
	public static Frequency<T> Megahertz<T>(this T value) where T : struct, INumber<T>
		=> Frequency<T>.Create(ConvertToBaseUnit(value, Units.Megahertz));
	#endregion

	#region Helper Methods
	/// <summary>
	/// Converts a value from a specific unit to the base unit of that dimension.
	/// </summary>
	/// <typeparam name="T">The numeric type of the value.</typeparam>
	/// <param name="value">The value in the source unit.</param>
	/// <param name="sourceUnit">The source unit to convert from.</param>
	/// <returns>The value converted to the base unit.</returns>
	private static T ConvertToBaseUnit<T>(T value, IUnit sourceUnit) where T : struct, INumber<T>
	{
		ArgumentNullException.ThrowIfNull(sourceUnit);

		// Handle offset units (like temperature conversions)
		if (Math.Abs(sourceUnit.ToBaseOffset) > 1e-10)
		{
			T factor = T.CreateChecked(sourceUnit.ToBaseFactor);
			T offset = T.CreateChecked(sourceUnit.ToBaseOffset);
			return (value * factor) + offset;
		}

		// Handle linear units (most common case)
		T conversionFactor = T.CreateChecked(sourceUnit.ToBaseFactor);
		return value * conversionFactor;
	}
	#endregion

	//TODO: #region Thermal Extensions
	// Temperature: .Celsius(), .Fahrenheit(), .Rankine()
	// Heat: .Joules(), .Calories(), .BTU()
	// Thermal Conductivity: .WattsPerMeterKelvin()
	// Heat Capacity: .JoulesPerKelvin()
	// Specific Heat: .JoulesPerKilogramKelvin()
	// #endregion

	//TODO: #region Fluid Dynamics Extensions
	// Flow Rate: .CubicMetersPerSecond(), .LitersPerMinute(), .GallonsPerMinute()
	// Viscosity: .PascalSeconds(), .Poise(), .Centipoise()
	// Density: .KilogramsPerCubicMeter(), .GramsPerCubicCentimeter()
	// Surface Tension: .NewtonsPerMeter()
	// #endregion

	//TODO: #region Electromagnetic Extensions
	// Magnetic Field: .Tesla(), .Gauss(), .Webers()
	// Inductance: .Henries(), .Millihenries(), .Microhenries()
	// Electric Field: .VoltsPerMeter(), .NewtonsPerCoulomb()
	// Magnetic Flux: .Webers(), .Maxwells()
	// #endregion

	//TODO: #region Materials Science Extensions
	// Stress/Pressure: .Pascals(), .Megapascals(), .Gigapascals(), .PSI()
	// Strain: .Percent(), .Dimensionless()
	// Elastic Modulus: .Gigapascals(), .MegapascalsPerStrain()
	// Hardness: .Brinell(), .Rockwell(), .Vickers()
	// #endregion

	//TODO: #region Nuclear & Radiation Extensions
	// Radioactivity: .Becquerels(), .Curies()
	// Absorbed Dose: .Grays(), .Rads()
	// Dose Equivalent: .Sieverts(), .Rems()
	// Cross Section: .Barns(), .SquareMeters()
	// #endregion

	//TODO: #region Optics Extensions
	// Wavelength: .Nanometers(), .Angstroms(), .Micrometers()
	// Frequency: .Terahertz(), .PetaHertz()
	// Illuminance: .Lux(), .FootCandles()
	// Luminance: .CandelasPerSquareMeter(), .Nits()
	// Optical Power: .Diopters()
	// #endregion

	//TODO: #region Astronomy Extensions
	// Distance: .AstronomicalUnits(), .LightYears(), .Parsecs()
	// Mass: .SolarMasses(), .EarthMasses(), .JupiterMasses()
	// Angular Resolution: .Arcseconds(), .Milliarcseconds()
	// Flux Density: .Janskys()
	// #endregion

	//TODO: #region Geophysics Extensions
	// Seismic Velocity: .KilometersPerSecond()
	// Gravity: .Gals(), .MilliGals()
	// Atmospheric Pressure: .Millibars(), .InchesOfMercury()
	// Magnetic Field: .Nanoteslas(), .Gammas()
	// #endregion

	//TODO: #region Acoustics Extensions
	// Sound Pressure: .Micropascals(), .Decibels()
	// Sound Intensity: .WattsPerSquareMeter()
	// Frequency: .Hertz(), .Kilohertz(), .Megahertz()
	// Acoustic Power: .AcousticWatts()
	// #endregion

	//TODO: #region Biochemistry Extensions
	// Concentration: .Molar(), .Millimolar(), .Micromolar()
	// Enzyme Activity: .Katals(), .EnzymeUnits()
	// pH: .pHUnits()
	// Molecular Weight: .Daltons(), .KiloDaltons()
	// #endregion
}
