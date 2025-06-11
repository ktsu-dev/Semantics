// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

/// <summary>
/// Conversion calculator for Energy quantities.
/// </summary>
public sealed class EnergyConversionCalculator : BaseConversionCalculator<Energy>
{
	/// <summary>
	/// Gets the shared instance of the Energy conversion calculator.
	/// </summary>
	public static EnergyConversionCalculator Instance { get; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="EnergyConversionCalculator"/> class.
	/// </summary>
	private EnergyConversionCalculator() : base("joules", GetConversions())
	{
	}

	private static IEnumerable<ConversionDefinition> GetConversions()
	{
		return
		[
			// SI prefixes
			new ConversionDefinition("millijoules", PhysicalConstants.Milli),
			new ConversionDefinition("mj", PhysicalConstants.Milli),
			new ConversionDefinition("microjoules", PhysicalConstants.Micro),
			new ConversionDefinition("μj", PhysicalConstants.Micro),
			new ConversionDefinition("nanojoules", PhysicalConstants.Nano),
			new ConversionDefinition("nj", PhysicalConstants.Nano),
			new ConversionDefinition("kilojoules", PhysicalConstants.Kilo),
			new ConversionDefinition("kj", PhysicalConstants.Kilo),
			new ConversionDefinition("megajoules", PhysicalConstants.Mega),
			new ConversionDefinition("mj_large", PhysicalConstants.Mega), // Avoiding conflict with millijoules
			new ConversionDefinition("gigajoules", PhysicalConstants.Giga),
			new ConversionDefinition("gj", PhysicalConstants.Giga),

			// Other energy units
			new ConversionDefinition("calories", PhysicalConstants.CaloriesToJoulesFactor),
			new ConversionDefinition("cal", PhysicalConstants.CaloriesToJoulesFactor),
			new ConversionDefinition("kilocalories", PhysicalConstants.CaloriesToJoulesFactor * PhysicalConstants.Kilo),
			new ConversionDefinition("kcal", PhysicalConstants.CaloriesToJoulesFactor * PhysicalConstants.Kilo),
			new ConversionDefinition("btus", PhysicalConstants.BTUsToJoulesFactor),
			new ConversionDefinition("btu", PhysicalConstants.BTUsToJoulesFactor),
			new ConversionDefinition("watt_hours", 3600.0), // 1 Wh = 3600 J
			new ConversionDefinition("wh", 3600.0),
			new ConversionDefinition("kilowatt_hours", 3600.0 * PhysicalConstants.Kilo),
			new ConversionDefinition("kwh", 3600.0 * PhysicalConstants.Kilo),
		];
	}
}
