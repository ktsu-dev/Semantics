// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

using System;

/// <summary>
/// A color in the gamma-encoded sRGB space, each channel 0..1. This is the only color space that
/// crosses the gamma boundary to and from the linear <see cref="Color"/>.
/// </summary>
/// <param name="R">Gamma-encoded red channel.</param>
/// <param name="G">Gamma-encoded green channel.</param>
/// <param name="B">Gamma-encoded blue channel.</param>
public readonly record struct Srgb(double R, double G, double B)
{
	/// <summary>Converts this sRGB color to a linear <see cref="Color"/>.</summary>
	/// <param name="a">Straight alpha for the resulting color (default 1.0).</param>
	/// <returns>The linear-RGB equivalent.</returns>
	public Color ToLinear(double a = 1.0) => new(DecodeChannel(R), DecodeChannel(G), DecodeChannel(B), a);

	/// <summary>Converts a linear <see cref="Color"/> to gamma-encoded sRGB (alpha dropped).</summary>
	/// <param name="color">The linear color.</param>
	/// <returns>The sRGB equivalent.</returns>
	public static Srgb FromLinear(Color color) =>
		new(EncodeChannel(color.R), EncodeChannel(color.G), EncodeChannel(color.B));

	private static double DecodeChannel(double s) =>
		s <= 0.04045 ? s / 12.92 : Math.Pow((s + 0.055) / 1.055, 2.4);

	private static double EncodeChannel(double linear) =>
		linear <= 0.0031308 ? 12.92 * linear : (1.055 * Math.Pow(linear, 1.0 / 2.4)) - 0.055;
}
