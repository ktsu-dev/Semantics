// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

/// <summary>
/// Conversion calculator for Mass quantities.
/// </summary>
public sealed class MassConversionCalculator : BaseConversionCalculator<Mass>
{
	/// <summary>
	/// Gets the shared instance of the Mass conversion calculator.
	/// </summary>
	public static MassConversionCalculator Instance { get; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="MassConversionCalculator"/> class.
	/// </summary>
	private MassConversionCalculator() : base("kilograms", GetConversions())
	{
	}

	private static IEnumerable<ConversionDefinition> GetConversions()
	{
		return
		[
			// Metric units
			new ConversionDefinition("grams", PhysicalConstants.Milli),
			new ConversionDefinition("g", PhysicalConstants.Milli),
			new ConversionDefinition("milligrams", PhysicalConstants.Micro),
			new ConversionDefinition("mg", PhysicalConstants.Micro),
			new ConversionDefinition("metric_tons", PhysicalConstants.MetricTonsToKilogramsFactor),
			new ConversionDefinition("tonnes", PhysicalConstants.MetricTonsToKilogramsFactor),
			new ConversionDefinition("t", PhysicalConstants.MetricTonsToKilogramsFactor),

			// Imperial units
			new ConversionDefinition("pounds", PhysicalConstants.PoundsToKilogramsFactor),
			new ConversionDefinition("lbs", PhysicalConstants.PoundsToKilogramsFactor),
			new ConversionDefinition("ounces", PhysicalConstants.OuncesToKilogramsFactor),
			new ConversionDefinition("oz", PhysicalConstants.OuncesToKilogramsFactor),
			new ConversionDefinition("stones", PhysicalConstants.StonesToKilogramsFactor),
			new ConversionDefinition("st", PhysicalConstants.StonesToKilogramsFactor),
			new ConversionDefinition("imperial_tons", PhysicalConstants.ImperialTonsToKilogramsFactor),
			new ConversionDefinition("us_tons", PhysicalConstants.USTonsToKilogramsFactor),
		];
	}
}
