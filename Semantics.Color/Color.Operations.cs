// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Color;

using System;
using System.Collections.Generic;

public readonly partial record struct Color
{
	/// <summary>Gets the WCAG relative luminance of this color (computed on the linear channels).</summary>
	public double RelativeLuminance => (0.2126 * R) + (0.7152 * G) + (0.0722 * B);

	/// <summary>
	/// Composites this color over a background with the source-over operator, the way CSS and every browser
	/// blend a translucent color onto what is beneath it.
	/// </summary>
	/// <param name="background">The color underneath.</param>
	/// <returns>
	/// The blended color, with alpha <c>a + b·(1 − a)</c> for this color's alpha <c>a</c> and the background's
	/// <c>b</c>. An opaque background gives an opaque result.
	/// </returns>
	/// <remarks>
	/// The blend is computed on the gamma-encoded sRGB channels rather than the linear ones, because that is
	/// what browsers do and what WCAG contrast checkers such as axe and Chrome DevTools measure, so a contrast
	/// ratio taken from the result agrees with theirs. Blending in linear light would be physically closer to
	/// mixing light, and would disagree with every tool a reader checks the ratio against.
	/// </remarks>
	public Color CompositeOver(Color background)
	{
		double a = Clamp01(A);
		double b = Clamp01(background.A) * (1.0 - a);
		double alpha = a + b;
		if (alpha <= 0.0)
		{
			return new Color(0.0, 0.0, 0.0, 0.0);
		}

		Srgb fg = ToSrgb();
		Srgb bg = background.ToSrgb();
		Srgb blended = new(
			((fg.R * a) + (bg.R * b)) / alpha,
			((fg.G * a) + (bg.G * b)) / alpha,
			((fg.B * a) + (bg.B * b)) / alpha);
		return FromSrgb(blended, alpha);
	}

	/// <summary>Computes the WCAG contrast ratio (1..21) between this color and a background.</summary>
	/// <param name="other">The background color. Its own alpha is ignored: it is treated as opaque.</param>
	/// <returns>The contrast ratio, from 1 (identical luminance) to 21 (black vs white).</returns>
	/// <remarks>
	/// A translucent color is first composited over the background with <see cref="CompositeOver"/>, as WCAG
	/// checkers do, so the ratio measures what is actually seen: fully transparent text rates 1:1 against
	/// anything. For two opaque colors the ratio is symmetric; when this color is translucent it is not, and
	/// <paramref name="other"/> is the one underneath.
	/// </remarks>
	public double ContrastRatio(Color other)
	{
		Color background = other.WithAlpha(1.0);
		Color seen = A >= 1.0 ? this : CompositeOver(background);
		double l1 = seen.RelativeLuminance;
		double l2 = background.RelativeLuminance;
		double lighter = Math.Max(l1, l2);
		double darker = Math.Min(l1, l2);
		return (lighter + 0.05) / (darker + 0.05);
	}

	/// <summary>Rates the contrast of this color against a background per WCAG.</summary>
	/// <param name="background">The background color, treated as opaque.</param>
	/// <param name="largeText">True for large text (lower thresholds).</param>
	/// <returns>The highest <see cref="AccessibilityLevel"/> the pair satisfies.</returns>
	/// <remarks>A translucent color is rated as it looks over the background; see <see cref="ContrastRatio"/>.</remarks>
	public AccessibilityLevel AccessibilityLevelAgainst(Color background, bool largeText = false)
	{
		double contrast = ContrastRatio(background);

		if (contrast >= RequiredContrast(AccessibilityLevel.AAA, largeText))
		{
			return AccessibilityLevel.AAA;
		}

		return contrast >= RequiredContrast(AccessibilityLevel.AA, largeText)
			? AccessibilityLevel.AA
			: AccessibilityLevel.Fail;
	}

	/// <summary>
	/// Adjusts this color until it meets the requested contrast level against a background, changing it as
	/// little as it can.
	/// </summary>
	/// <param name="background">The background color, treated as opaque.</param>
	/// <param name="target">The desired conformance level.</param>
	/// <param name="largeText">True for large text (lower thresholds).</param>
	/// <returns>An adjusted color inside the sRGB gamut, with this color's alpha.</returns>
	/// <remarks>
	/// The search first moves Oklab lightness, keeping hue, and keeping chroma wherever the sRGB gamut allows it:
	/// near black and white a saturated color has no in-gamut form at its own chroma, so chroma is reduced there
	/// rather than the channels being clipped, which would shift the hue and the luminance. If no lightness
	/// reaches the target, the color is mixed towards black or white instead, so whenever black or white meets
	/// the level against this background the result does too. Alpha is kept, and the contrast is measured on the
	/// color as it looks over the background (see <see cref="ContrastRatio"/>), so a color too transparent to
	/// reach the target even at black or white is returned unchanged; that, and an already sufficient color,
	/// are the only cases that return this color as it is.
	/// </remarks>
	public Color AdjustForContrast(Color background, AccessibilityLevel target, bool largeText = false)
	{
		double required = RequiredContrast(target, largeText);

		if (ContrastRatio(background) >= required)
		{
			return this;
		}

		Oklab lab = ToOklab();
		double alpha = A;
		Color self = this;

		// Move lightness in whichever direction reaches the target. When both do, keep the one closer to
		// this color: which extreme (white or black) has more headroom against the background says
		// nothing about where this color starts, so on a background just below luminance 0.179 a dark
		// color that needs only a nudge darker would otherwise be inverted to a light one. If neither
		// direction reaches it, mix towards black or white instead, trying first the extreme that gives
		// the higher contrast against this background, before giving up.
		Color? byLightness = NearerOf(Search(goLighter: true, LightnessCandidate), Search(goLighter: false, LightnessCandidate));

		double bgLum = background.WithAlpha(1.0).RelativeLuminance;
		bool preferLighter = (1.05 / (bgLum + 0.05)) >= ((bgLum + 0.05) / 0.05);

		return byLightness
			?? Search(preferLighter, MixCandidate)
			?? Search(!preferLighter, MixCandidate)
			?? this;

		Color? Search(bool goLighter, Func<bool, double, Color> candidateAt)
		{
			// t runs from 0 (this color) to 1 (the extreme in the chosen direction), and contrast grows
			// as t does: binary-search the smallest movement that meets the requirement.
			Color extreme = candidateAt(goLighter, 1.0);
			if (extreme.ContrastRatio(background) < required)
			{
				return null;
			}

			double lo = 0.0;
			double hi = 1.0;
			for (int i = 0; i < 30; i++)
			{
				double mid = (lo + hi) / 2.0;
				if (candidateAt(goLighter, mid).ContrastRatio(background) >= required)
				{
					hi = mid;
				}
				else
				{
					lo = mid;
				}
			}

			Color result = candidateAt(goLighter, hi);
			return result.ContrastRatio(background) >= required ? result : extreme;
		}

		Color LightnessCandidate(bool goLighter, double t)
		{
			double end = goLighter ? 1.0 : 0.0;
			return GamutMapped(new Oklab(lab.L + ((end - lab.L) * t), lab.A, lab.B), alpha);
		}

		Color MixCandidate(bool goLighter, double t)
		{
			Color end = goLighter ? new Color(1.0, 1.0, 1.0, alpha) : new Color(0.0, 0.0, 0.0, alpha);
			return self.Clamp().Lerp(end, t).WithAlpha(alpha);
		}
	}

	/// <summary>Returns whichever of two candidates is closer to this color, or the one that exists.</summary>
	/// <param name="first">The first candidate, or <c>null</c>.</param>
	/// <param name="second">The second candidate, or <c>null</c>.</param>
	/// <returns>The nearer candidate; <paramref name="first"/> on a tie; <c>null</c> if neither exists.</returns>
	private Color? NearerOf(Color? first, Color? second) =>
		first is Color a && second is Color b
			? (DistanceTo(a) <= DistanceTo(b) ? a : b)
			: first ?? second;

	/// <summary>
	/// Converts an Oklab color to the nearest in-gamut linear color of the same lightness and hue, reducing
	/// chroma until it fits, as CSS Color 4 gamut-maps. Lightness at or past 0 or 1 gives black or white.
	/// </summary>
	/// <param name="lab">The Oklab color.</param>
	/// <param name="alpha">The straight alpha for the result.</param>
	/// <returns>A color with every channel in 0..1.</returns>
	internal static Color GamutMapped(Oklab lab, double alpha)
	{
		const double tolerance = 1e-6;

		if (lab.L >= 1.0)
		{
			return new Color(1.0, 1.0, 1.0, Clamp01(alpha));
		}

		if (lab.L <= 0.0)
		{
			return new Color(0.0, 0.0, 0.0, Clamp01(alpha));
		}

		Color direct = lab.ToColor(alpha);
		if (IsInGamut(direct, tolerance))
		{
			return direct.Clamp();
		}

		double lo = 0.0;
		double hi = 1.0;
		for (int i = 0; i < 30; i++)
		{
			double mid = (lo + hi) / 2.0;
			if (IsInGamut(new Oklab(lab.L, lab.A * mid, lab.B * mid).ToColor(alpha), tolerance))
			{
				lo = mid;
			}
			else
			{
				hi = mid;
			}
		}

		return new Oklab(lab.L, lab.A * lo, lab.B * lo).ToColor(alpha).Clamp();
	}

	private static bool IsInGamut(Color color, double tolerance) =>
		color.R >= -tolerance && color.R <= 1.0 + tolerance
		&& color.G >= -tolerance && color.G <= 1.0 + tolerance
		&& color.B >= -tolerance && color.B <= 1.0 + tolerance;

	/// <summary>
	/// The WCAG contrast ratio a foreground/background pair must reach to satisfy
	/// <paramref name="level"/>.
	/// </summary>
	/// <param name="level">The conformance level.</param>
	/// <param name="largeText">True for large text, which has lower thresholds.</param>
	/// <returns>The required contrast ratio; 1.0 for levels with no requirement.</returns>
	private static double RequiredContrast(AccessibilityLevel level, bool largeText) => level switch
	{
		AccessibilityLevel.AAA => largeText ? 4.5 : 7.0,
		AccessibilityLevel.AA => largeText ? 3.0 : 4.5,
		_ => 1.0,
	};

	/// <summary>Computes the perceptual (Oklab Euclidean) distance to another color.</summary>
	/// <param name="other">The other color.</param>
	/// <returns>The Oklab distance.</returns>
	public double DistanceTo(Color other)
	{
		Oklab a = ToOklab();
		Oklab b = other.ToOklab();
		double dl = a.L - b.L;
		double da = a.A - b.A;
		double db = a.B - b.B;
		return Math.Sqrt((dl * dl) + (da * da) + (db * db));
	}

	/// <summary>Mixes this color with another in Oklab space (perceptually uniform).</summary>
	/// <param name="other">The other color.</param>
	/// <param name="t">The interpolation factor, 0 = this, 1 = other.</param>
	/// <returns>The mixed color, inside the sRGB gamut.</returns>
	/// <remarks>
	/// A straight line in Oklab between two in-gamut colors can leave the sRGB gamut, as cyan to blue does, so
	/// the mix is gamut-mapped by reducing its chroma at the same lightness and hue. Without that, every HSL
	/// adjustment made to the result afterwards would start from channels outside 0..1.
	/// </remarks>
	public Color MixOklab(Color other, double t)
	{
		Oklab a = ToOklab();
		Oklab b = other.ToOklab();
		double inv = 1.0 - t;
		Oklab mixed = new(
			(a.L * inv) + (b.L * t),
			(a.A * inv) + (b.A * t),
			(a.B * inv) + (b.B * t));
		return GamutMapped(mixed, (A * inv) + (other.A * t));
	}

	/// <summary>Linearly interpolates this color with another in linear-RGB space.</summary>
	/// <param name="other">The other color.</param>
	/// <param name="t">The interpolation factor, 0 = this, 1 = other.</param>
	/// <returns>The interpolated color.</returns>
	public Color Lerp(Color other, double t)
	{
		double inv = 1.0 - t;
		return new Color(
			(R * inv) + (other.R * t),
			(G * inv) + (other.G * t),
			(B * inv) + (other.B * t),
			(A * inv) + (other.A * t));
	}

	/// <summary>Builds a perceptually-uniform (Oklab) gradient from this color to another.</summary>
	/// <param name="to">The end color.</param>
	/// <param name="steps">The number of colors to produce (at least 2).</param>
	/// <returns>The gradient, inclusive of both endpoints; every interior stop is inside the sRGB gamut (see <see cref="MixOklab"/>).</returns>
	/// <exception cref="ArgumentException">Thrown when <paramref name="steps"/> is less than 2.</exception>
	public IReadOnlyList<Color> Gradient(Color to, int steps)
	{
		if (steps < 2)
		{
			throw new ArgumentException("A gradient needs at least 2 steps.", nameof(steps));
		}

		Color[] result = new Color[steps];
		result[0] = this;
		result[steps - 1] = to;
		for (int i = 1; i < steps - 1; i++)
		{
			result[i] = MixOklab(to, i / (double)(steps - 1));
		}

		return result;
	}

	// Convenience adjustments. The cylindrical ops forward to the canonical HSL operations on Hsl
	// (round-tripping through the gamma boundary and preserving alpha); reach for ToOklch() when you
	// want perceptually-uniform lightness/chroma instead. Invert is a per-channel negative in sRGB.

	/// <summary>Returns a copy with its HSL saturation replaced (clamped to 0..1).</summary>
	/// <param name="saturation">The new saturation.</param>
	/// <returns>The adjusted color.</returns>
	public Color WithSaturation(double saturation) => FromHsl(ToHsl().WithSaturation(saturation), A);

	/// <summary>Returns a copy with its HSL saturation increased by <paramref name="amount"/> (clamped to 0..1).</summary>
	/// <param name="amount">The amount to add to saturation.</param>
	/// <returns>The adjusted color.</returns>
	public Color SaturateBy(double amount) => FromHsl(ToHsl().SaturateBy(amount), A);

	/// <summary>Returns a copy with its HSL saturation decreased by <paramref name="amount"/> (clamped to 0..1).</summary>
	/// <param name="amount">The amount to subtract from saturation.</param>
	/// <returns>The adjusted color.</returns>
	public Color DesaturateBy(double amount) => FromHsl(ToHsl().DesaturateBy(amount), A);

	/// <summary>Returns a copy with its HSL saturation multiplied by <paramref name="factor"/> (clamped to 0..1).</summary>
	/// <param name="factor">The saturation multiplier.</param>
	/// <returns>The adjusted color.</returns>
	public Color MultiplySaturation(double factor) => FromHsl(ToHsl().MultiplySaturation(factor), A);

	/// <summary>Returns a fully desaturated (grayscale) copy, preserving lightness and alpha.</summary>
	/// <returns>The grayscale color.</returns>
	public Color ToGrayscale() => FromHsl(ToHsl().ToGrayscale(), A);

	/// <summary>Returns a copy with its HSL lightness replaced (clamped to 0..1).</summary>
	/// <param name="lightness">The new lightness.</param>
	/// <returns>The adjusted color.</returns>
	public Color WithLightness(double lightness) => FromHsl(ToHsl().WithLightness(lightness), A);

	/// <summary>Returns a copy with its HSL lightness increased by <paramref name="amount"/> (clamped to 0..1).</summary>
	/// <param name="amount">The amount to add to lightness.</param>
	/// <returns>The adjusted color.</returns>
	public Color LightenBy(double amount) => FromHsl(ToHsl().LightenBy(amount), A);

	/// <summary>Returns a copy with its HSL lightness decreased by <paramref name="amount"/> (clamped to 0..1).</summary>
	/// <param name="amount">The amount to subtract from lightness.</param>
	/// <returns>The adjusted color.</returns>
	public Color DarkenBy(double amount) => FromHsl(ToHsl().DarkenBy(amount), A);

	/// <summary>Returns a copy with its HSL lightness multiplied by <paramref name="factor"/> (clamped to 0..1).</summary>
	/// <param name="factor">The lightness multiplier.</param>
	/// <returns>The adjusted color.</returns>
	public Color MultiplyLightness(double factor) => FromHsl(ToHsl().MultiplyLightness(factor), A);

	/// <summary>Returns a copy with its hue offset by <paramref name="degrees"/> around the color wheel (wraps at 360).</summary>
	/// <param name="degrees">The hue offset in degrees.</param>
	/// <returns>The adjusted color.</returns>
	public Color OffsetHue(double degrees) => FromHsl(ToHsl().OffsetHue(degrees), A);

	/// <summary>Returns the per-channel inverse (photographic negative), computed in gamma-encoded sRGB and preserving alpha.</summary>
	/// <returns>The inverted color.</returns>
	public Color Invert() => FromSrgb(ToSrgb().Invert(), A);

	/// <summary>
	/// Simulates how this color looks to a viewer with a color vision deficiency, by applying
	/// <see cref="ColorVision.Matrix"/> to the linear channels and clamping each to 0..1. Alpha is kept.
	/// </summary>
	/// <param name="deficiency">The kind of deficiency.</param>
	/// <param name="severity">The severity, from 0 (normal vision) to 1 (dichromacy).</param>
	/// <returns>The color as the viewer perceives it.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="severity"/> is outside 0..1 or not a number, or <paramref name="deficiency"/>
	/// is not a defined value.
	/// </exception>
	public Color Simulate(ColorVisionDeficiency deficiency, double severity) =>
		ColorVision.Matrix(deficiency, severity).Apply(this);
}
