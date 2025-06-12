// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Extension methods for units.
/// </summary>
public static class UnitExtensions
{
	/// <summary>
	/// Determines if this unit is a base unit (fundamental unit in any system).
	/// </summary>
	/// <param name="unit">The unit to check.</param>
	/// <returns>True if this is a base unit in its system, false otherwise.</returns>
	public static bool IsBaseUnit(this IUnit unit)
	{
		return unit.System switch
		{
			UnitSystem.SIBase => true,
			UnitSystem.CGS => Math.Abs(unit.ToBaseFactor - 1.0) < 1e-10,
			UnitSystem.SIDerived => throw new NotImplementedException(),
			UnitSystem.Metric => throw new NotImplementedException(),
			UnitSystem.Imperial => throw new NotImplementedException(),
			UnitSystem.USCustomary => throw new NotImplementedException(),
			UnitSystem.Atomic => throw new NotImplementedException(),
			UnitSystem.Natural => throw new NotImplementedException(),
			UnitSystem.Planck => throw new NotImplementedException(),
			UnitSystem.Other => throw new NotImplementedException(),
			_ => false
		};
	}

	/// <summary>
	/// Determines if this unit belongs to the SI system (either base or derived).
	/// </summary>
	/// <param name="unit">The unit to check.</param>
	/// <returns>True if this is an SI unit, false otherwise.</returns>
	public static bool IsSI(this IUnit unit) => unit.System is UnitSystem.SIBase or UnitSystem.SIDerived;

	/// <summary>
	/// Determines if this unit is metric (SI or other metric systems).
	/// </summary>
	/// <param name="unit">The unit to check.</param>
	/// <returns>True if this is a metric unit, false otherwise.</returns>
	public static bool IsMetric(this IUnit unit) => unit.System is UnitSystem.SIBase or UnitSystem.SIDerived or UnitSystem.Metric or UnitSystem.CGS;

	public static bool IsImperial(this IUnit unit) => unit.System is UnitSystem.Imperial or UnitSystem.USCustomary;
}
