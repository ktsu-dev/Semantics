// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

/// <summary>
/// Conversion calculator for Area quantities.
/// </summary>
public sealed class AreaConversionCalculator : BaseConversionCalculator<Area>
{
	/// <summary>
	/// Gets the shared instance of the Area conversion calculator.
	/// </summary>
	public static AreaConversionCalculator Instance { get; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="AreaConversionCalculator"/> class.
	/// </summary>
	private AreaConversionCalculator() : base("square_meters", GetConversions())
	{
	}

	private static IEnumerable<ConversionDefinition> GetConversions()
	{
		return
		[
			// SI area units
			new ConversionDefinition("square_millimeters", PhysicalConstants.Milli * PhysicalConstants.Milli),
			new ConversionDefinition("square_centimeters", PhysicalConstants.Centi * PhysicalConstants.Centi),
			new ConversionDefinition("square_kilometers", PhysicalConstants.Kilo * PhysicalConstants.Kilo),

			// Imperial area units
			new ConversionDefinition("square_feet", PhysicalConstants.FeetToMetersFactor * PhysicalConstants.FeetToMetersFactor),
			new ConversionDefinition("square_inches", PhysicalConstants.InchesToMetersFactor * PhysicalConstants.InchesToMetersFactor),
			new ConversionDefinition("square_yards", PhysicalConstants.YardsToMetersFactor * PhysicalConstants.YardsToMetersFactor),
			new ConversionDefinition("square_miles", PhysicalConstants.MilesToMetersFactor * PhysicalConstants.MilesToMetersFactor),
			new ConversionDefinition("acres", PhysicalConstants.AcresToSquareMetersFactor),

			// Alternative names
			new ConversionDefinition("square_metre", 1.0),
			new ConversionDefinition("hectares", 10000.0), // 1 hectare = 10,000 m²
		];
	}
}
