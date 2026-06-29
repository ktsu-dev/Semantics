// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

using System;

/// <summary>
/// A color in HSV (hue, saturation, value), defined over the gamma-encoded sRGB channels.
/// Hue is in degrees, 0..360; saturation and value are 0..1.
/// </summary>
/// <param name="H">Hue angle in degrees, 0..360.</param>
/// <param name="S">Saturation, 0..1.</param>
/// <param name="V">Value (brightness), 0..1.</param>
public readonly record struct Hsv(double H, double S, double V)
{
	/// <summary>Converts a gamma-encoded <see cref="Srgb"/> color to HSV.</summary>
	/// <param name="srgb">The sRGB color.</param>
	/// <returns>The HSV equivalent.</returns>
	public static Hsv FromSrgb(Srgb srgb)
	{
		double max = Math.Max(srgb.R, Math.Max(srgb.G, srgb.B));
		double min = Math.Min(srgb.R, Math.Min(srgb.G, srgb.B));
		double d = max - min;
		double h = d > 0.0 ? Hsl.HueDegrees(srgb, max, d) : 0.0;
		double s = max > 0.0 ? d / max : 0.0;
		return new Hsv(h, s, max);
	}

	/// <summary>Converts this HSV color to a gamma-encoded <see cref="Srgb"/>.</summary>
	/// <returns>The sRGB equivalent.</returns>
	public Srgb ToSrgb()
	{
		double h = Hsl.NormalizeHue(H) / 60.0;
		double c = V * S;
		double x = c * (1.0 - Math.Abs((h % 2.0) - 1.0));
		double m = V - c;

		(double r, double g, double b) = h switch
		{
			< 1.0 => (c, x, 0.0),
			< 2.0 => (x, c, 0.0),
			< 3.0 => (0.0, c, x),
			< 4.0 => (0.0, x, c),
			< 5.0 => (x, 0.0, c),
			_ => (c, 0.0, x),
		};

		return new Srgb(r + m, g + m, b + m);
	}
}
