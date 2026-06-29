// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

using System;

public readonly partial record struct Color
{
	/// <summary>Gets the WCAG relative luminance of this color (computed on the linear channels).</summary>
	public double RelativeLuminance => (0.2126 * R) + (0.7152 * G) + (0.0722 * B);

	/// <summary>Computes the WCAG contrast ratio (1..21) between this color and another.</summary>
	/// <param name="other">The other color.</param>
	/// <returns>The contrast ratio, from 1 (identical luminance) to 21 (black vs white).</returns>
	public double ContrastRatio(Color other)
	{
		double l1 = RelativeLuminance;
		double l2 = other.RelativeLuminance;
		double lighter = Math.Max(l1, l2);
		double darker = Math.Min(l1, l2);
		return (lighter + 0.05) / (darker + 0.05);
	}

	/// <summary>Rates the contrast of this color against a background per WCAG.</summary>
	/// <param name="background">The background color.</param>
	/// <param name="largeText">True for large text (lower thresholds).</param>
	/// <returns>The highest <see cref="AccessibilityLevel"/> the pair satisfies.</returns>
	public AccessibilityLevel AccessibilityLevelAgainst(Color background, bool largeText = false)
	{
		double contrast = ContrastRatio(background);
		if (contrast >= (largeText ? 4.5 : 7.0))
		{
			return AccessibilityLevel.AAA;
		}

		return contrast >= (largeText ? 3.0 : 4.5) ? AccessibilityLevel.AA : AccessibilityLevel.Fail;
	}

	/// <summary>
	/// Adjusts this color's Oklab lightness (preserving hue and chroma) until it meets the requested
	/// contrast level against a background. Returns this color unchanged if already sufficient or if
	/// no adjustment can reach the target.
	/// </summary>
	/// <param name="background">The background color.</param>
	/// <param name="target">The desired conformance level.</param>
	/// <param name="largeText">True for large text (lower thresholds).</param>
	/// <returns>An adjusted color, clamped to gamut.</returns>
	public Color AdjustForContrast(Color background, AccessibilityLevel target, bool largeText = false)
	{
		double required = target switch
		{
			AccessibilityLevel.AAA => largeText ? 4.5 : 7.0,
			AccessibilityLevel.AA => largeText ? 3.0 : 4.5,
			_ => 1.0,
		};

		if (ContrastRatio(background) >= required)
		{
			return this;
		}

		Oklab lab = ToOklab();
		double alpha = A;
		bool goLighter = background.RelativeLuminance < 0.5;
		double lo = goLighter ? lab.L : 0.0;
		double hi = goLighter ? 1.0 : lab.L;

		// Contrast increases monotonically as L moves toward the chosen extreme; binary-search the
		// smallest movement that meets the requirement.
		for (int i = 0; i < 30; i++)
		{
			double mid = (lo + hi) / 2.0;
			Color candidate = Candidate(lab, mid);
			bool meets = candidate.ContrastRatio(background) >= required;
			if (goLighter)
			{
				if (meets)
				{
					hi = mid;
				}
				else
				{
					lo = mid;
				}
			}
			else if (meets)
			{
				lo = mid;
			}
			else
			{
				hi = mid;
			}
		}

		Color result = Candidate(lab, goLighter ? hi : lo);
		return result.ContrastRatio(background) >= required ? result : this;

		Color Candidate(Oklab source, double lightness) =>
			FromOklab(new Oklab(lightness, source.A, source.B), alpha).Clamp();
	}
}
