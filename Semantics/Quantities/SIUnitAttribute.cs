// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// An attribute to define the SI unit for a class.
/// </summary>
/// <remarks>
/// This attribute is intended to be applied to classes that represent physical quantities
/// and their associated SI units.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public sealed class SIUnitAttribute(string symbol, string singular, string plural) : Attribute
// TODO: separate into SIUnit and attribute
// TODO: define derived SIUnits for each unit type
// TODO: define attributes for each derived SIUnit
// TODO: move conversions to each derived SIUnit
// TODO: streamline SI to imperial conversions
// TODO: streamline metric magnitude conversions
{
	/// <summary>
	/// Gets the symbol of the SI unit (e.g., "m" for meters).
	/// </summary>
	public string Symbol { get; } = symbol;

	/// <summary>
	/// Gets the singular name of the SI unit (e.g., "meter").
	/// </summary>
	public string Singular { get; } = singular;

	/// <summary>
	/// Gets the plural name of the SI unit (e.g., "meters").
	/// </summary>
	public string Plural { get; } = plural;
}
