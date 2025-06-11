// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

/// <summary>
/// Conversion calculator for Angle quantities.
/// </summary>
public sealed class AngleConversionCalculator : BaseConversionCalculator<Angle>
{
	/// <summary>
	/// Gets the shared instance of the Angle conversion calculator.
	/// </summary>
	public static AngleConversionCalculator Instance { get; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="AngleConversionCalculator"/> class.
	/// </summary>
	private AngleConversionCalculator() : base("radians", GetConversions())
	{
	}

	private static IEnumerable<ConversionDefinition> GetConversions()
	{
		return
		[
			// Angular units
			new ConversionDefinition("degrees", PhysicalConstants.DegreesToRadiansFactor),
			new ConversionDefinition("deg", PhysicalConstants.DegreesToRadiansFactor),
			new ConversionDefinition("°", PhysicalConstants.DegreesToRadiansFactor),
			new ConversionDefinition("gradians", PhysicalConstants.GradiansToRadiansFactor),
			new ConversionDefinition("grad", PhysicalConstants.GradiansToRadiansFactor),
			new ConversionDefinition("gon", PhysicalConstants.GradiansToRadiansFactor),
			new ConversionDefinition("arcminutes", PhysicalConstants.MinutesToRadiansFactor),
			new ConversionDefinition("arcmin", PhysicalConstants.MinutesToRadiansFactor),
			new ConversionDefinition("'", PhysicalConstants.MinutesToRadiansFactor),
			new ConversionDefinition("arcseconds", PhysicalConstants.SecondsToRadiansFactor),
			new ConversionDefinition("arcsec", PhysicalConstants.SecondsToRadiansFactor),
			new ConversionDefinition("\"", PhysicalConstants.SecondsToRadiansFactor),
			new ConversionDefinition("revolutions", PhysicalConstants.RevolutionsToRadiansFactor),
			new ConversionDefinition("rev", PhysicalConstants.RevolutionsToRadiansFactor),
			new ConversionDefinition("cycles", PhysicalConstants.CyclesToRadiansFactor),
			new ConversionDefinition("cyc", PhysicalConstants.CyclesToRadiansFactor),
			new ConversionDefinition("turns", PhysicalConstants.TurnsToRadiansFactor),
			new ConversionDefinition("tr", PhysicalConstants.TurnsToRadiansFactor),
		];
	}
}
