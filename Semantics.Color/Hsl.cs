// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Color;

using System;

/// <summary>
/// A color in HSL (hue, saturation, lightness), defined over the gamma-encoded sRGB channels.
/// Hue is in degrees, 0..360; saturation and lightness are 0..1.
/// </summary>
/// <param name="H">Hue angle in degrees, 0..360.</param>
/// <param name="S">Saturation, 0..1.</param>
/// <param name="L">Lightness, 0..1.</param>
public readonly record struct Hsl(double H, double S, double L)
{
	/// <summary>Converts a gamma-encoded <see cref="Srgb"/> color to HSL.</summary>
	/// <param name="srgb">The sRGB color.</param>
	/// <returns>The HSL equivalent.</returns>
	public static Hsl FromSrgb(Srgb srgb)
	{
		double max = Math.Max(srgb.R, Math.Max(srgb.G, srgb.B));
		double min = Math.Min(srgb.R, Math.Min(srgb.G, srgb.B));
		double l = (max + min) / 2.0;
		double h = 0.0;
		double s = 0.0;

		if (max > min)
		{
			double d = max - min;
			s = l > 0.5 ? d / (2.0 - max - min) : d / (max + min);
			h = HueDegrees(srgb, max, d);
		}

		return new Hsl(h, s, l);
	}

	/// <summary>Converts this HSL color to a gamma-encoded <see cref="Srgb"/>.</summary>
	/// <returns>The sRGB equivalent.</returns>
	public Srgb ToSrgb()
	{
		double h = NormalizeHue(H) / 360.0;
		if (S <= 0.0)
		{
			return new Srgb(L, L, L);
		}

		double q = L < 0.5 ? L * (1.0 + S) : L + S - (L * S);
		double p = (2.0 * L) - q;
		return new Srgb(
			HueToChannel(p, q, h + (1.0 / 3.0)),
			HueToChannel(p, q, h),
			HueToChannel(p, q, h - (1.0 / 3.0)));
	}

	// Conversions to the other color-space types. HSL is defined over sRGB, so hops to sRGB and
	// HSV stay within the family (no gamma round-trip). Reaching the linear Color hub and the
	// Oklab family crosses the gamma boundary once.

	/// <summary>Converts this HSL color to a linear <see cref="Color"/> (via <see cref="Srgb"/>).</summary>
	/// <param name="a">Straight alpha for the result (default 1.0).</param>
	/// <returns>The linear-RGB equivalent.</returns>
	public Color ToColor(double a = 1.0) => ToSrgb().ToLinear(a);

	/// <summary>Creates an HSL color from a linear <see cref="Color"/> (via <see cref="Srgb"/>).</summary>
	/// <param name="color">The linear color.</param>
	/// <returns>The HSL equivalent.</returns>
	public static Hsl FromColor(Color color) => FromSrgb(Srgb.FromLinear(color));

	/// <summary>Converts this HSL color to <see cref="Hsv"/> (via <see cref="Srgb"/>).</summary>
	/// <returns>The HSV equivalent.</returns>
	public Hsv ToHsv() => Hsv.FromSrgb(ToSrgb());

	/// <summary>Creates an HSL color from an <see cref="Hsv"/> value (via <see cref="Srgb"/>).</summary>
	/// <param name="hsv">The HSV color.</param>
	/// <returns>The HSL equivalent.</returns>
	public static Hsl FromHsv(Hsv hsv) => FromSrgb(hsv.ToSrgb());

	/// <summary>Converts this HSL color to <see cref="Oklab"/> (via the linear <see cref="Color"/> hub).</summary>
	/// <returns>The Oklab equivalent.</returns>
	public Oklab ToOklab() => Oklab.FromColor(ToColor());

	/// <summary>Creates an HSL color from an <see cref="Oklab"/> value (via the linear <see cref="Color"/> hub).</summary>
	/// <param name="oklab">The Oklab color.</param>
	/// <returns>The HSL equivalent.</returns>
	public static Hsl FromOklab(Oklab oklab) => FromColor(oklab.ToColor());

	/// <summary>Converts this HSL color to <see cref="Oklch"/> (via <see cref="Oklab"/>).</summary>
	/// <returns>The Oklch equivalent.</returns>
	public Oklch ToOklch() => ToOklab().ToOklch();

	/// <summary>Creates an HSL color from an <see cref="Oklch"/> value (via <see cref="Oklab"/>).</summary>
	/// <param name="oklch">The Oklch color.</param>
	/// <returns>The HSL equivalent.</returns>
	public static Hsl FromOklch(Oklch oklch) => FromOklab(oklch.ToOklab());

	// Adjustments. Saturation and lightness are clamped to 0..1; hue is a free angle in degrees
	// (normalized when converting out). These are the canonical HSL operations that the linear
	// Color and gamma Srgb convenience wrappers delegate to.

	/// <summary>Returns a copy with saturation replaced (clamped to 0..1).</summary>
	/// <param name="saturation">The new saturation.</param>
	/// <returns>The adjusted color.</returns>
	public Hsl WithSaturation(double saturation) => this with { S = Clamp01(saturation) };

	/// <summary>Returns a copy with saturation increased by <paramref name="amount"/> (clamped to 0..1).</summary>
	/// <param name="amount">The amount to add.</param>
	/// <returns>The adjusted color.</returns>
	public Hsl SaturateBy(double amount) => this with { S = Clamp01(S + amount) };

	/// <summary>Returns a copy with saturation decreased by <paramref name="amount"/> (clamped to 0..1).</summary>
	/// <param name="amount">The amount to subtract.</param>
	/// <returns>The adjusted color.</returns>
	public Hsl DesaturateBy(double amount) => this with { S = Clamp01(S - amount) };

	/// <summary>Returns a copy with saturation multiplied by <paramref name="factor"/> (clamped to 0..1).</summary>
	/// <param name="factor">The multiplier.</param>
	/// <returns>The adjusted color.</returns>
	public Hsl MultiplySaturation(double factor) => this with { S = Clamp01(S * factor) };

	/// <summary>Returns a fully desaturated (grayscale) copy, preserving lightness.</summary>
	/// <returns>The grayscale color.</returns>
	public Hsl ToGrayscale() => this with { S = 0.0 };

	/// <summary>Returns a copy with lightness replaced (clamped to 0..1).</summary>
	/// <param name="lightness">The new lightness.</param>
	/// <returns>The adjusted color.</returns>
	public Hsl WithLightness(double lightness) => this with { L = Clamp01(lightness) };

	/// <summary>Returns a copy with lightness increased by <paramref name="amount"/> (clamped to 0..1).</summary>
	/// <param name="amount">The amount to add.</param>
	/// <returns>The adjusted color.</returns>
	public Hsl LightenBy(double amount) => this with { L = Clamp01(L + amount) };

	/// <summary>Returns a copy with lightness decreased by <paramref name="amount"/> (clamped to 0..1).</summary>
	/// <param name="amount">The amount to subtract.</param>
	/// <returns>The adjusted color.</returns>
	public Hsl DarkenBy(double amount) => this with { L = Clamp01(L - amount) };

	/// <summary>Returns a copy with lightness multiplied by <paramref name="factor"/> (clamped to 0..1).</summary>
	/// <param name="factor">The multiplier.</param>
	/// <returns>The adjusted color.</returns>
	public Hsl MultiplyLightness(double factor) => this with { L = Clamp01(L * factor) };

	/// <summary>Returns a copy with its hue offset by <paramref name="degrees"/> around the wheel (wraps at 360).</summary>
	/// <param name="degrees">The hue offset in degrees.</param>
	/// <returns>The adjusted color.</returns>
	public Hsl OffsetHue(double degrees) => this with { H = NormalizeHue(H + degrees) };

	private static double Clamp01(double value) => Math.Clamp(value, 0.0, 1.0);

	internal static double NormalizeHue(double h)
	{
		double r = h % 360.0;
		return r < 0.0 ? r + 360.0 : r;
	}

	internal static double HueDegrees(Srgb srgb, double max, double d)
	{
		double h;
		if (max == srgb.R)
		{
			h = ((srgb.G - srgb.B) / d) + (srgb.G < srgb.B ? 6.0 : 0.0);
		}
		else if (max == srgb.G)
		{
			h = ((srgb.B - srgb.R) / d) + 2.0;
		}
		else
		{
			h = ((srgb.R - srgb.G) / d) + 4.0;
		}

		return h * 60.0;
	}

	private static double HueToChannel(double p, double q, double t)
	{
		if (t < 0.0)
		{
			t += 1.0;
		}

		if (t > 1.0)
		{
			t -= 1.0;
		}

		if (t < 1.0 / 6.0)
		{
			return p + ((q - p) * 6.0 * t);
		}

		if (t < 1.0 / 2.0)
		{
			return q;
		}

		if (t < 2.0 / 3.0)
		{
			return p + ((q - p) * ((2.0 / 3.0) - t) * 6.0);
		}

		return p;
	}
}
