// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

/// <summary>
/// Conversion calculator for Temperature quantities.
/// </summary>
public sealed class TemperatureConversionCalculator : BaseConversionCalculator<Temperature>
{
	/// <summary>
	/// Gets the shared instance of the Temperature conversion calculator.
	/// </summary>
	public static TemperatureConversionCalculator Instance { get; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="TemperatureConversionCalculator"/> class.
	/// </summary>
	private TemperatureConversionCalculator() : base("kelvin", GetConversions())
	{
	}

	private static IEnumerable<ConversionDefinition> GetConversions()
	{
		return
		[
			// Temperature scales with offsets
			new ConversionDefinition("celsius", PhysicalConstants.CelsiusToKelvinFactor, PhysicalConstants.CelsiusToKelvinOffset),
			new ConversionDefinition("c", PhysicalConstants.CelsiusToKelvinFactor, PhysicalConstants.CelsiusToKelvinOffset),
			new ConversionDefinition("°c", PhysicalConstants.CelsiusToKelvinFactor, PhysicalConstants.CelsiusToKelvinOffset),

			// Fahrenheit requires special handling due to different conversion formula
			// F = (C * 9/5) + 32, so K = (F - 32) * 5/9 + 273.15
			// This gives us: K = F * (5/9) + (273.15 - 32 * 5/9)
			new ConversionDefinition("fahrenheit",
				5.0 / 9.0,
				PhysicalConstants.CelsiusToKelvinOffset - (PhysicalConstants.FahrenheitToCelsiusOffset * 5.0 / 9.0)),
			new ConversionDefinition("f",
				5.0 / 9.0,
				PhysicalConstants.CelsiusToKelvinOffset - (PhysicalConstants.FahrenheitToCelsiusOffset * 5.0 / 9.0)),
			new ConversionDefinition("°f",
				5.0 / 9.0,
				PhysicalConstants.CelsiusToKelvinOffset - (PhysicalConstants.FahrenheitToCelsiusOffset * 5.0 / 9.0)),
		];
	}
}
