// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

/// <summary>
/// Represents a unit conversion definition with factor and optional offset.
/// </summary>
/// <param name="Unit">The unit name.</param>
/// <param name="Factor">The conversion factor to multiply by when converting from this unit to base unit.</param>
/// <param name="Offset">The conversion offset to add when converting from this unit to base unit (default is 0).</param>
public record ConversionDefinition(string Unit, double Factor, double Offset = 0) // TODO: these should be between SIUnits and some kind of ImperialUnit
{
	/// <summary>
	/// Converts a value from this unit to the base unit.
	/// </summary>
	/// <param name="value">The value in this unit.</param>
	/// <returns>The value in the base unit.</returns>
	public double ToBase(double value) => (value * Factor) + Offset;

	/// <summary>
	/// Converts a value from the base unit to this unit.
	/// </summary>
	/// <param name="baseValue">The value in the base unit.</param>
	/// <returns>The value in this unit.</returns>
	public double FromBase(double baseValue) => (baseValue - Offset) / Factor;
}
