// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

/// <summary>
/// Conversion calculator for Power quantities.
/// </summary>
public sealed class PowerConversionCalculator : BaseConversionCalculator<Power>
{
	/// <summary>
	/// Gets the shared instance of the Power conversion calculator.
	/// </summary>
	public static PowerConversionCalculator Instance { get; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="PowerConversionCalculator"/> class.
	/// </summary>
	private PowerConversionCalculator() : base("watts", GetConversions())
	{
	}

	private static IEnumerable<ConversionDefinition> GetConversions()
	{
		return
		[
			// SI prefixes
			new ConversionDefinition("kilowatts", PhysicalConstants.Kilo),
			new ConversionDefinition("kw", PhysicalConstants.Kilo),
			new ConversionDefinition("megawatts", PhysicalConstants.Mega),
			new ConversionDefinition("mw", PhysicalConstants.Mega),
			new ConversionDefinition("gigawatts", PhysicalConstants.Giga),
			new ConversionDefinition("gw", PhysicalConstants.Giga),

			// Horsepower units
			new ConversionDefinition("horsepower", PhysicalConstants.HorsepowerToWattsFactor),
			new ConversionDefinition("hp", PhysicalConstants.HorsepowerToWattsFactor),
			new ConversionDefinition("metric_horsepower", PhysicalConstants.MetricHorsePowerToWattsFactor),
			new ConversionDefinition("ps", PhysicalConstants.MetricHorsePowerToWattsFactor),
		];
	}
}
