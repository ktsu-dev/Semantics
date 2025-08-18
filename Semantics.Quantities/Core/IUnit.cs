// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Interface for all physical units.
/// </summary>
public interface IUnit
{
	/// <summary>Gets the full name of the unit.</summary>
	public string Name { get; }

	/// <summary>Gets the symbol/abbreviation of the unit.</summary>
	public string Symbol { get; }

	/// <summary>Gets the physical dimension this unit measures.</summary>
	public PhysicalDimension Dimension { get; }

	/// <summary>Gets the unit system this unit belongs to.</summary>
	public UnitSystem System { get; }

	/// <summary>Gets the multiplication factor to convert to the base unit.</summary>
	public double ToBaseFactor { get; }

	/// <summary>Gets the offset to add when converting to the base unit (0.0 for linear units).</summary>
	public double ToBaseOffset { get; }
}
