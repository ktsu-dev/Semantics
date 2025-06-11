// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents a base SI unit with its symbol and names.
/// </summary>
public record SIUnit(string Symbol, string Singular, string Plural)
{
	/// <summary>
	/// Gets the symbol of the SI unit (e.g., "m" for meters).
	/// </summary>
	public string Symbol { get; } = Symbol;

	/// <summary>
	/// Gets the singular name of the SI unit (e.g., "meter").
	/// </summary>
	public string Singular { get; } = Singular;

	/// <summary>
	/// Gets the plural name of the SI unit (e.g., "meters").
	/// </summary>
	public string Plural { get; } = Plural;

	/// <summary>
	/// Returns a string representation of the unit.
	/// </summary>
	/// <returns>The symbol of the unit.</returns>
	public override string ToString() => Symbol;
}

/// <summary>
/// Represents a derived SI unit with conversion factors.
/// </summary>
public record DerivedSIUnit : SIUnit
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DerivedSIUnit"/> class.
	/// </summary>
	/// <param name="symbol">The unit symbol.</param>
	/// <param name="singular">The singular name.</param>
	/// <param name="plural">The plural name.</param>
	/// <param name="baseUnit">The base SI unit this is derived from.</param>
	/// <param name="conversionFactor">The factor to convert from this unit to the base unit.</param>
	/// <param name="conversionOffset">The offset to apply during conversion (default is 0).</param>
	public DerivedSIUnit(string symbol, string singular, string plural, SIUnit baseUnit, double conversionFactor, double conversionOffset = 0)
		: base(symbol, singular, plural)
	{
		BaseUnit = baseUnit;
		ConversionFactor = conversionFactor;
		ConversionOffset = conversionOffset;
	}

	/// <summary>
	/// Gets the base SI unit this unit is derived from.
	/// </summary>
	public SIUnit BaseUnit { get; }

	/// <summary>
	/// Gets the conversion factor to convert from this unit to the base unit.
	/// </summary>
	public double ConversionFactor { get; }

	/// <summary>
	/// Gets the conversion offset to apply during conversion.
	/// </summary>
	public double ConversionOffset { get; }

	/// <summary>
	/// Converts a value from this unit to the base unit.
	/// </summary>
	/// <param name="value">The value in this unit.</param>
	/// <returns>The value in the base unit.</returns>
	public double ToBase(double value) => (value * ConversionFactor) + ConversionOffset;

	/// <summary>
	/// Converts a value from the base unit to this unit.
	/// </summary>
	/// <param name="baseValue">The value in the base unit.</param>
	/// <returns>The value in this unit.</returns>
	public double FromBase(double baseValue) => (baseValue - ConversionOffset) / ConversionFactor;
}
