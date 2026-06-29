// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

using System;

/// <summary>
/// A color in the Oklab perceptual color space (Björn Ottosson, 2020), derived from linear RGB.
/// </summary>
/// <param name="L">Perceived lightness.</param>
/// <param name="A">Green–red axis (negative green, positive red/magenta).</param>
/// <param name="B">Blue–yellow axis (negative blue, positive yellow).</param>
public readonly record struct Oklab(double L, double A, double B)
{
	/// <summary>Converts a linear <see cref="Color"/> to Oklab.</summary>
	/// <param name="color">The linear color.</param>
	/// <returns>The Oklab equivalent.</returns>
	public static Oklab FromColor(Color color)
	{
		double l = (0.4122214708 * color.R) + (0.5363325363 * color.G) + (0.0514459929 * color.B);
		double m = (0.2119034982 * color.R) + (0.6806995451 * color.G) + (0.1073969566 * color.B);
		double s = (0.0883024619 * color.R) + (0.2817188376 * color.G) + (0.6299787005 * color.B);

		double l_ = Cbrt(l);
		double m_ = Cbrt(m);
		double s_ = Cbrt(s);

		return new Oklab(
			(0.2104542553 * l_) + (0.7936177850 * m_) - (0.0040720468 * s_),
			(1.9779984951 * l_) - (2.4285922050 * m_) + (0.4505937099 * s_),
			(0.0259040371 * l_) + (0.7827717662 * m_) - (0.8086757660 * s_));
	}

	/// <summary>Converts this Oklab color to a linear <see cref="Color"/>.</summary>
	/// <param name="a">Straight alpha for the result (default 1.0).</param>
	/// <returns>The linear-RGB equivalent.</returns>
	public Color ToColor(double a = 1.0)
	{
		double l_ = L + (0.3963377774 * A) + (0.2158037573 * B);
		double m_ = L - (0.1055613458 * A) - (0.0638541728 * B);
		double s_ = L - (0.0894841775 * A) - (1.2914855480 * B);

		double l = l_ * l_ * l_;
		double m = m_ * m_ * m_;
		double s = s_ * s_ * s_;

		return new Color(
			(+4.0767416621 * l) - (3.3077115913 * m) + (0.2309699292 * s),
			(-1.2684380046 * l) + (2.6097574011 * m) - (0.3413193965 * s),
			(-0.0041960863 * l) - (0.7034186147 * m) + (1.7076147010 * s),
			a);
	}

	/// <summary>Converts this Oklab color to its polar <see cref="Oklch"/> form.</summary>
	/// <returns>The Oklch equivalent.</returns>
	public Oklch ToOklch()
	{
		double c = Math.Sqrt((A * A) + (B * B));
		double h = Math.Atan2(B, A) * (180.0 / Math.PI);
		if (h < 0.0)
		{
			h += 360.0;
		}

		return new Oklch(L, c, h);
	}

	// netstandard2.0 lacks Math.Cbrt; one Newton-Raphson refinement after a
	// sign-aware Pow gives a correctly-rounded result on all target frameworks.
	private static double Cbrt(double value)
	{
		if (value == 0.0)
		{
			return 0.0;
		}

		double x = Math.Sign(value) * Math.Pow(Math.Abs(value), 1.0 / 3.0);
		return ((2.0 * x) + (value / (x * x))) / 3.0;
	}
}
