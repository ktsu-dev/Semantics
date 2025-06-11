// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

/// <summary>
/// Conversion calculator for Velocity quantities.
/// </summary>
public sealed class VelocityConversionCalculator : BaseConversionCalculator<Velocity>
{
	/// <summary>
	/// Gets the shared instance of the Velocity conversion calculator.
	/// </summary>
	public static VelocityConversionCalculator Instance { get; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="VelocityConversionCalculator"/> class.
	/// </summary>
	private VelocityConversionCalculator() : base("meters_per_second", GetConversions())
	{
	}

	private static IEnumerable<ConversionDefinition> GetConversions()
	{
		return
		[
			// Base unit variations
			new ConversionDefinition("m/s", 1.0),
			new ConversionDefinition("mps", 1.0),

			// Imperial velocity units
			new ConversionDefinition("feet_per_second", PhysicalConstants.FeetToMetersFactor),
			new ConversionDefinition("ft/s", PhysicalConstants.FeetToMetersFactor),
			new ConversionDefinition("fps", PhysicalConstants.FeetToMetersFactor),
			new ConversionDefinition("inches_per_second", PhysicalConstants.InchesToMetersFactor),
			new ConversionDefinition("in/s", PhysicalConstants.InchesToMetersFactor),
			new ConversionDefinition("ips", PhysicalConstants.InchesToMetersFactor),

			// Common velocity units
			new ConversionDefinition("miles_per_hour", PhysicalConstants.MilesToMetersFactor / PhysicalConstants.HoursToSecondsFactor),
			new ConversionDefinition("mph", PhysicalConstants.MilesToMetersFactor / PhysicalConstants.HoursToSecondsFactor),
			new ConversionDefinition("kilometers_per_hour", PhysicalConstants.Kilo / PhysicalConstants.HoursToSecondsFactor),
			new ConversionDefinition("km/h", PhysicalConstants.Kilo / PhysicalConstants.HoursToSecondsFactor),
			new ConversionDefinition("kph", PhysicalConstants.Kilo / PhysicalConstants.HoursToSecondsFactor),
			new ConversionDefinition("knots", PhysicalConstants.NauticalMilesToMetersFactor / PhysicalConstants.HoursToSecondsFactor),
			new ConversionDefinition("kt", PhysicalConstants.NauticalMilesToMetersFactor / PhysicalConstants.HoursToSecondsFactor),
		];
	}
}
