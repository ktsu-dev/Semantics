// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Colors;

using ktsu.Semantics.Color;

[TestClass]
public class AccessibilityTests
{
	[TestMethod]
	public void BlackOnWhite_HasMaximumContrast()
	{
		Color black = Color.FromSrgb(0.0, 0.0, 0.0);
		Color white = Color.FromSrgb(1.0, 1.0, 1.0);
		Assert.AreEqual(21.0, black.ContrastRatio(white), 1e-2);
	}

	[TestMethod]
	public void SameColor_HasUnitContrast()
	{
		Color gray = Color.FromSrgb(0.5, 0.5, 0.5);
		Assert.AreEqual(1.0, gray.ContrastRatio(gray), 1e-9);
	}

	[TestMethod]
	public void BlackOnWhite_RatesAaa()
	{
		Color black = Color.FromSrgb(0.0, 0.0, 0.0);
		Color white = Color.FromSrgb(1.0, 1.0, 1.0);
		Assert.AreEqual(AccessibilityLevel.AAA, black.AccessibilityLevelAgainst(white));
	}

	[TestMethod]
	public void AdjustForContrast_ReachesRequestedLevel()
	{
		Color background = Color.FromSrgb(1.0, 1.0, 1.0);
		Color faint = Color.FromSrgb(0.85, 0.85, 0.2);
		Color adjusted = faint.AdjustForContrast(background, AccessibilityLevel.AA);
		Assert.IsTrue(
			adjusted.AccessibilityLevelAgainst(background) >= AccessibilityLevel.AA,
			$"contrast was {adjusted.ContrastRatio(background)}");
	}
}
