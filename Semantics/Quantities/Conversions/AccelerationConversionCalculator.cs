// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

/// <summary>
/// Conversion calculator for Acceleration quantities.
/// </summary>
public sealed class AccelerationConversionCalculator : BaseConversionCalculator<Acceleration>
{
	/// <summary>
	/// Gets the shared instance of the Acceleration conversion calculator.
	/// </summary>
	public static AccelerationConversionCalculator Instance { get; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="AccelerationConversionCalculator"/> class.
	/// </summary>
	private AccelerationConversionCalculator() : base("meters_per_second_squared", GetConversions())
	{
	}

	private static IEnumerable<ConversionDefinition> GetConversions()
	{
		return
		[
			// SI derived units
			new ConversionDefinition("millimeters_per_second_squared", PhysicalConstants.Milli),
			new ConversionDefinition("centimeters_per_second_squared", PhysicalConstants.Centi),
			new ConversionDefinition("kilometers_per_second_squared", PhysicalConstants.Kilo),

			// Imperial units
			new ConversionDefinition("feet_per_second_squared", PhysicalConstants.FeetToMetersFactor),
			new ConversionDefinition("inches_per_second_squared", PhysicalConstants.InchesToMetersFactor),
			new ConversionDefinition("yards_per_second_squared", PhysicalConstants.YardsToMetersFactor),
			new ConversionDefinition("miles_per_second_squared", PhysicalConstants.MilesToMetersFactor),
			new ConversionDefinition("nautical_miles_per_second_squared", PhysicalConstants.NauticalMilesToMetersFactor),
			new ConversionDefinition("fathoms_per_second_squared", PhysicalConstants.FathomsToMetersFactor),

			// Standard gravity (Earth's surface)
			new ConversionDefinition("g", PhysicalConstants.StandardGravity),

			// Alternative names
			new ConversionDefinition("meters_per_second2", 1.0),
			new ConversionDefinition("feet_per_second2", PhysicalConstants.FeetToMetersFactor),
		];
	}
}
