// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

/// <summary>WCAG 2.x contrast conformance levels, ordered so that higher is stricter.</summary>
public enum AccessibilityLevel
{
	/// <summary>Does not meet the AA contrast threshold.</summary>
	Fail = 0,

	/// <summary>Meets the WCAG AA contrast threshold.</summary>
	AA = 1,

	/// <summary>Meets the WCAG AAA contrast threshold.</summary>
	AAA = 2,
}
