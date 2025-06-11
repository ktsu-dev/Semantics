// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

/// <summary>
/// Conversion calculator for Force quantities.
/// </summary>
public sealed class ForceConversionCalculator : BaseConversionCalculator<Force>
{
	/// <summary>
	/// Gets the shared instance of the Force conversion calculator.
	/// </summary>
	public static ForceConversionCalculator Instance { get; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ForceConversionCalculator"/> class.
	/// </summary>
	private ForceConversionCalculator() : base("newtons", GetConversions())
	{
	}

	private static IEnumerable<ConversionDefinition> GetConversions()
	{
		return
		[
			// SI prefixes
			new ConversionDefinition("kilonewtons", PhysicalConstants.Kilo),
			new ConversionDefinition("kn", PhysicalConstants.Kilo),
			new ConversionDefinition("millinewtons", PhysicalConstants.Milli),
			new ConversionDefinition("mn", PhysicalConstants.Milli),
			new ConversionDefinition("micronewtons", PhysicalConstants.Micro),
			new ConversionDefinition("μn", PhysicalConstants.Micro),

			// Imperial/US force units
			new ConversionDefinition("pounds_force", PhysicalConstants.PoundsForceToNewtonsFactor),
			new ConversionDefinition("lbf", PhysicalConstants.PoundsForceToNewtonsFactor),
			new ConversionDefinition("pound_force", PhysicalConstants.PoundsForceToNewtonsFactor),
		];
	}
}
