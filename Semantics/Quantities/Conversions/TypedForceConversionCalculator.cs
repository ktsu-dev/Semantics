// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

/// <summary>
/// Strongly-typed conversion calculator for Force quantities.
/// </summary>
public sealed class TypedForceConversionCalculator : TypedConversionCalculator<Force>
{
	/// <summary>
	/// Gets the shared instance of the typed Force conversion calculator.
	/// </summary>
	public static TypedForceConversionCalculator Instance { get; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="TypedForceConversionCalculator"/> class.
	/// </summary>
	private TypedForceConversionCalculator() : base(SIUnits.Newton, GetForceAvailableSIUnits(), GetForceAvailableImperialUnits())
	{
	}

	private static IEnumerable<SIUnit> GetForceAvailableSIUnits()
	{
		return
		[
			SIUnits.Millinewton,
			SIUnits.Micronewton,
			SIUnits.Kilonewton,
		];
	}

	private static IEnumerable<ImperialUnit> GetForceAvailableImperialUnits()
	{
		return
		[
			ImperialUnits.PoundForce,
		];
	}
}
