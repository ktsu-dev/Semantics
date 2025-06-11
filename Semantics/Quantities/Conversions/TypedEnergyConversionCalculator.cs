// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

/// <summary>
/// Strongly-typed conversion calculator for Energy quantities.
/// </summary>
public sealed class TypedEnergyConversionCalculator : TypedConversionCalculator<Energy>
{
	/// <summary>
	/// Gets the shared instance of the typed Energy conversion calculator.
	/// </summary>
	public static TypedEnergyConversionCalculator Instance { get; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="TypedEnergyConversionCalculator"/> class.
	/// </summary>
	private TypedEnergyConversionCalculator() : base(SIUnits.Joule, GetEnergyAvailableSIUnits(), GetEnergyAvailableImperialUnits())
	{
	}

	private static IEnumerable<SIUnit> GetEnergyAvailableSIUnits()
	{
		return
		[
			SIUnits.Millijoule,
			SIUnits.Microjoule,
			SIUnits.Nanojoule,
			SIUnits.Kilojoule,
			SIUnits.Megajoule,
			SIUnits.Gigajoule,
		];
	}

	private static IEnumerable<ImperialUnit> GetEnergyAvailableImperialUnits()
	{
		return
		[
			ImperialUnits.Calorie,
			ImperialUnits.Kilocalorie,
			ImperialUnits.BTU,
			ImperialUnits.WattHour,
			ImperialUnits.KilowattHour,
		];
	}
}
