// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

/// <summary>
/// Conversion calculator for Pressure quantities.
/// </summary>
public sealed class PressureConversionCalculator : BaseConversionCalculator<Pressure>
{
	/// <summary>
	/// Gets the shared instance of the Pressure conversion calculator.
	/// </summary>
	public static PressureConversionCalculator Instance { get; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="PressureConversionCalculator"/> class.
	/// </summary>
	private PressureConversionCalculator() : base("pascals", GetConversions())
	{
	}

	private static IEnumerable<ConversionDefinition> GetConversions()
	{
		return
		[
			// SI pressure units
			new ConversionDefinition("kilopascals", PhysicalConstants.Kilo),
			new ConversionDefinition("megapascals", PhysicalConstants.Mega),
			new ConversionDefinition("gigapascals", PhysicalConstants.Giga),

			// Common pressure units
			new ConversionDefinition("bar", 100000.0), // 1 bar = 100,000 Pa
						new ConversionDefinition("atmospheres", PhysicalConstants.AtmToPascalsFactor),
			new ConversionDefinition("torr", PhysicalConstants.TorrToPascalsFactor),
			new ConversionDefinition("mmhg", PhysicalConstants.TorrToPascalsFactor), // mmHg is equivalent to Torr
			new ConversionDefinition("psi", PhysicalConstants.PsiToPascalsFactor),

			// Alternative names
			new ConversionDefinition("atm", PhysicalConstants.AtmToPascalsFactor),
			new ConversionDefinition("millibar", 100.0), // 1 mbar = 100 Pa
			new ConversionDefinition("pounds_per_square_inch", PhysicalConstants.PsiToPascalsFactor),
		];
	}
}
