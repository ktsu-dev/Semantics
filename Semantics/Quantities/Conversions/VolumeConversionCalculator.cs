// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

/// <summary>
/// Conversion calculator for Volume quantities.
/// </summary>
public sealed class VolumeConversionCalculator : BaseConversionCalculator<Volume>
{
	/// <summary>
	/// Gets the shared instance of the Volume conversion calculator.
	/// </summary>
	public static VolumeConversionCalculator Instance { get; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="VolumeConversionCalculator"/> class.
	/// </summary>
	private VolumeConversionCalculator() : base("cubic_meters", GetConversions())
	{
	}

	private static IEnumerable<ConversionDefinition> GetConversions()
	{
		return
		[
			// SI volume units
			new ConversionDefinition("cubic_millimeters", PhysicalConstants.Milli * PhysicalConstants.Milli * PhysicalConstants.Milli),
			new ConversionDefinition("cubic_centimeters", PhysicalConstants.Centi * PhysicalConstants.Centi * PhysicalConstants.Centi),
			new ConversionDefinition("cubic_kilometers", PhysicalConstants.Kilo * PhysicalConstants.Kilo * PhysicalConstants.Kilo),

			// Metric volume units
			new ConversionDefinition("liters", 0.001), // 1 liter = 0.001 m³
			new ConversionDefinition("milliliters", 0.000001), // 1 mL = 0.000001 m³

			// Imperial volume units
			new ConversionDefinition("cubic_feet", PhysicalConstants.FeetToMetersFactor * PhysicalConstants.FeetToMetersFactor * PhysicalConstants.FeetToMetersFactor),
			new ConversionDefinition("cubic_inches", PhysicalConstants.InchesToMetersFactor * PhysicalConstants.InchesToMetersFactor * PhysicalConstants.InchesToMetersFactor),
			new ConversionDefinition("cubic_yards", PhysicalConstants.YardsToMetersFactor * PhysicalConstants.YardsToMetersFactor * PhysicalConstants.YardsToMetersFactor),
			new ConversionDefinition("gallons", 3.785411784 * 0.001), // US gallons to m³
			new ConversionDefinition("fluid_ounces", 29.5735296875 * 0.000001),

			// Alternative names
			new ConversionDefinition("litres", 0.001),
			new ConversionDefinition("cubic_metre", 1.0),
			new ConversionDefinition("cc", 0.000001), // cubic centimeters
		];
	}
}
