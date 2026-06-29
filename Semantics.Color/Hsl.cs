// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

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
