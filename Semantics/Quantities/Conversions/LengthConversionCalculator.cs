// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

/// <summary>
/// Conversion calculator for Length quantities.
/// </summary>
public sealed class LengthConversionCalculator : BaseConversionCalculator<Length>
{
	/// <summary>
	/// Gets the shared instance of the Length conversion calculator.
	/// </summary>
	public static LengthConversionCalculator Instance { get; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="LengthConversionCalculator"/> class.
	/// </summary>
	private LengthConversionCalculator() : base("meters", GetConversions())
	{
	}

	private static IEnumerable<ConversionDefinition> GetConversions()
	{
		return
		[
			// Metric prefixes
			new ConversionDefinition("kilometers", PhysicalConstants.Kilo),
			new ConversionDefinition("km", PhysicalConstants.Kilo),
			new ConversionDefinition("centimeters", PhysicalConstants.Centi),
			new ConversionDefinition("cm", PhysicalConstants.Centi),
			new ConversionDefinition("millimeters", PhysicalConstants.Milli),
			new ConversionDefinition("mm", PhysicalConstants.Milli),
			new ConversionDefinition("micrometers", PhysicalConstants.Micro),
			new ConversionDefinition("μm", PhysicalConstants.Micro),
			new ConversionDefinition("nanometers", PhysicalConstants.Nano),
			new ConversionDefinition("nm", PhysicalConstants.Nano),

			// Imperial and other units
			new ConversionDefinition("feet", PhysicalConstants.FeetToMetersFactor),
			new ConversionDefinition("ft", PhysicalConstants.FeetToMetersFactor),
			new ConversionDefinition("inches", PhysicalConstants.InchesToMetersFactor),
			new ConversionDefinition("in", PhysicalConstants.InchesToMetersFactor),
			new ConversionDefinition("yards", PhysicalConstants.YardsToMetersFactor),
			new ConversionDefinition("yd", PhysicalConstants.YardsToMetersFactor),
			new ConversionDefinition("miles", PhysicalConstants.MilesToMetersFactor),
			new ConversionDefinition("mi", PhysicalConstants.MilesToMetersFactor),
			new ConversionDefinition("nautical_miles", PhysicalConstants.NauticalMilesToMetersFactor),
			new ConversionDefinition("nmi", PhysicalConstants.NauticalMilesToMetersFactor),
			new ConversionDefinition("fathoms", PhysicalConstants.FathomsToMetersFactor),
			new ConversionDefinition("fm", PhysicalConstants.FathomsToMetersFactor),

			// Astronomical units
			new ConversionDefinition("astronomical_units", PhysicalConstants.AstronomicalUnitsToMetersFactor),
			new ConversionDefinition("au", PhysicalConstants.AstronomicalUnitsToMetersFactor),
			new ConversionDefinition("light_years", PhysicalConstants.LightYearsToMetersFactor),
			new ConversionDefinition("ly", PhysicalConstants.LightYearsToMetersFactor),
			new ConversionDefinition("parsecs", PhysicalConstants.ParsecsToMetersFactor),
			new ConversionDefinition("pc", PhysicalConstants.ParsecsToMetersFactor),
		];
	}
}
