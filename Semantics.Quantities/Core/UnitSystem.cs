// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents a unit system classification.
/// </summary>
public enum UnitSystem
{
	/// <summary>SI base units (the 7 fundamental units: meter, kilogram, second, ampere, kelvin, mole, candela).</summary>
	SIBase,
	/// <summary>SI derived units (units derived from SI base units like m², m/s, Newton, Joule).</summary>
	SIDerived,
	/// <summary>Metric units that are not part of SI (like liter, bar, hectare).</summary>
	Metric,
	/// <summary>Imperial units (British Imperial system).</summary>
	Imperial,
	/// <summary>US Customary units (may differ from Imperial in some cases).</summary>
	USCustomary,
	/// <summary>Centimeter-Gram-Second system.</summary>
	CGS,
	/// <summary>Atomic units (used in atomic physics).</summary>
	Atomic,
	/// <summary>Natural units (used in particle physics, ℏ = c = 1).</summary>
	Natural,
	/// <summary>Planck units (fundamental units based on physical constants).</summary>
	Planck,
	/// <summary>Other unit systems not covered above.</summary>
	Other
}
