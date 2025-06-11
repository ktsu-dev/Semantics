// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

/// <summary>
/// Conversion calculator for Time quantities.
/// </summary>
public sealed class TimeConversionCalculator : BaseConversionCalculator<Time>
{
	/// <summary>
	/// Gets the shared instance of the Time conversion calculator.
	/// </summary>
	public static TimeConversionCalculator Instance { get; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="TimeConversionCalculator"/> class.
	/// </summary>
	private TimeConversionCalculator() : base("seconds", GetConversions())
	{
	}

	private static IEnumerable<ConversionDefinition> GetConversions()
	{
		return
		[
			// Sub-second units
			new ConversionDefinition("milliseconds", PhysicalConstants.Milli),
			new ConversionDefinition("ms", PhysicalConstants.Milli),
			new ConversionDefinition("microseconds", PhysicalConstants.Micro),
			new ConversionDefinition("μs", PhysicalConstants.Micro),
			new ConversionDefinition("nanoseconds", PhysicalConstants.Nano),
			new ConversionDefinition("ns", PhysicalConstants.Nano),

			// Larger units
			new ConversionDefinition("minutes", PhysicalConstants.MinutesToSecondsFactor),
			new ConversionDefinition("min", PhysicalConstants.MinutesToSecondsFactor),
			new ConversionDefinition("hours", PhysicalConstants.HoursToSecondsFactor),
			new ConversionDefinition("h", PhysicalConstants.HoursToSecondsFactor),
			new ConversionDefinition("days", PhysicalConstants.DaysToSecondsFactor),
			new ConversionDefinition("d", PhysicalConstants.DaysToSecondsFactor),
			new ConversionDefinition("years", PhysicalConstants.YearsToSecondsFactor),
			new ConversionDefinition("y", PhysicalConstants.YearsToSecondsFactor),
		];
	}
}
