// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

using System;

/// <summary>
/// A color in the polar (lightness, chroma, hue) form of Oklab. Hue is in degrees, 0..360.
/// </summary>
/// <param name="L">Perceived lightness.</param>
/// <param name="C">Chroma (colourfulness).</param>
/// <param name="H">Hue angle in degrees, 0..360.</param>
public readonly record struct Oklch(double L, double C, double H)
{
	/// <summary>Converts this polar color back to Cartesian <see cref="Oklab"/>.</summary>
	/// <returns>The Oklab equivalent.</returns>
	public Oklab ToOklab()
	{
		double hRad = H * (Math.PI / 180.0);
		return new Oklab(L, C * Math.Cos(hRad), C * Math.Sin(hRad));
	}
}
