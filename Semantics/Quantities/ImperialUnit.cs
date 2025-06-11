// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents an imperial or other non-SI unit with conversion to SI base units.
/// </summary>
public record ImperialUnit : SIUnit
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ImperialUnit"/> class.
	/// </summary>
	/// <param name="symbol">The unit symbol.</param>
	/// <param name="singular">The singular name.</param>
	/// <param name="plural">The plural name.</param>
	/// <param name="siBaseUnit">The SI base unit this converts to.</param>
	/// <param name="conversionFactor">The factor to convert from this unit to the SI base unit.</param>
	/// <param name="conversionOffset">The offset to apply during conversion (default is 0).</param>
	public ImperialUnit(string symbol, string singular, string plural, SIUnit siBaseUnit, double conversionFactor, double conversionOffset = 0)
		: base(symbol, singular, plural)
	{
		SIBaseUnit = siBaseUnit;
		ConversionFactor = conversionFactor;
		ConversionOffset = conversionOffset;
	}

	/// <summary>
	/// Gets the SI base unit this unit converts to.
	/// </summary>
	public SIUnit SIBaseUnit { get; }

	/// <summary>
	/// Gets the conversion factor to convert from this unit to the SI base unit.
	/// </summary>
	public double ConversionFactor { get; }

	/// <summary>
	/// Gets the conversion offset to apply during conversion.
	/// </summary>
	public double ConversionOffset { get; }

	/// <summary>
	/// Converts a value from this unit to the SI base unit.
	/// </summary>
	/// <param name="value">The value in this unit.</param>
	/// <returns>The value in the SI base unit.</returns>
	public double ToSI(double value) => (value * ConversionFactor) + ConversionOffset;

	/// <summary>
	/// Converts a value from the SI base unit to this unit.
	/// </summary>
	/// <param name="siValue">The value in the SI base unit.</param>
	/// <returns>The value in this unit.</returns>
	public double FromSI(double siValue) => (siValue - ConversionOffset) / ConversionFactor;
}

/// <summary>
/// Provides common imperial and other non-SI units.
/// </summary>
public static class ImperialUnits
{
	// Length units
	/// <summary>Inch - imperial unit of length.</summary>
	public static readonly ImperialUnit Inch = new("in", "inch", "inches", SIUnits.Meter, 0.0254);

	/// <summary>Foot - imperial unit of length.</summary>
	public static readonly ImperialUnit Foot = new("ft", "foot", "feet", SIUnits.Meter, 0.3048);

	/// <summary>Yard - imperial unit of length.</summary>
	public static readonly ImperialUnit Yard = new("yd", "yard", "yards", SIUnits.Meter, 0.9144);

	/// <summary>Mile - imperial unit of length.</summary>
	public static readonly ImperialUnit Mile = new("mi", "mile", "miles", SIUnits.Meter, 1609.344);

	// Mass units
	/// <summary>Ounce - imperial unit of mass.</summary>
	public static readonly ImperialUnit Ounce = new("oz", "ounce", "ounces", SIUnits.Kilogram, 0.0283495);

	/// <summary>Pound - imperial unit of mass.</summary>
	public static readonly ImperialUnit Pound = new("lb", "pound", "pounds", SIUnits.Kilogram, 0.453592);

	/// <summary>Stone - imperial unit of mass.</summary>
	public static readonly ImperialUnit Stone = new("st", "stone", "stones", SIUnits.Kilogram, 6.35029);

	// Force units
	/// <summary>Pound-force - imperial unit of force.</summary>
	public static readonly ImperialUnit PoundForce = new("lbf", "pound-force", "pounds-force", SIUnits.Newton, 4.44822);

	// Energy units
	/// <summary>Calorie - unit of energy.</summary>
	public static readonly ImperialUnit Calorie = new("cal", "calorie", "calories", SIUnits.Joule, PhysicalConstants.CaloriesToJoulesFactor);

	/// <summary>Kilocalorie - unit of energy.</summary>
	public static readonly ImperialUnit Kilocalorie = new("kcal", "kilocalorie", "kilocalories", SIUnits.Joule, PhysicalConstants.CaloriesToJoulesFactor * PhysicalConstants.Kilo);

	/// <summary>British Thermal Unit - unit of energy.</summary>
	public static readonly ImperialUnit BTU = new("BTU", "british thermal unit", "british thermal units", SIUnits.Joule, PhysicalConstants.BTUsToJoulesFactor);

	/// <summary>Watt-hour - unit of energy.</summary>
	public static readonly ImperialUnit WattHour = new("Wh", "watt-hour", "watt-hours", SIUnits.Joule, 3600.0);

	/// <summary>Kilowatt-hour - unit of energy.</summary>
	public static readonly ImperialUnit KilowattHour = new("kWh", "kilowatt-hour", "kilowatt-hours", SIUnits.Joule, 3600.0 * PhysicalConstants.Kilo);

	// Temperature units
	/// <summary>Degree Fahrenheit - imperial unit of temperature.</summary>
	public static readonly ImperialUnit Fahrenheit = new("°F", "degree fahrenheit", "degrees fahrenheit", SIUnits.Kelvin, 5.0 / 9.0, -459.67 * 5.0 / 9.0);

	/// <summary>Degree Celsius - metric unit of temperature.</summary>
	public static readonly ImperialUnit Celsius = new("°C", "degree celsius", "degrees celsius", SIUnits.Kelvin, 1.0, -273.15);
}
