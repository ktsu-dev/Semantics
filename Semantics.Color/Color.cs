// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

using System.Numerics;

/// <summary>
/// A color stored as linear (not gamma-encoded) RGB plus straight alpha, each in the range 0..1.
/// This is the canonical color type; conversions to and from other color spaces live in the
/// <c>Color.Conversions</c> partial, and color-science operations in <c>Color.Operations</c>.
/// </summary>
/// <param name="R">Linear red channel.</param>
/// <param name="G">Linear green channel.</param>
/// <param name="B">Linear blue channel.</param>
/// <param name="A">Straight (non-premultiplied) alpha.</param>
public readonly partial record struct Color(double R, double G, double B, double A)
{
	/// <summary>Creates a color from linear RGB channels, defaulting alpha to fully opaque.</summary>
	/// <param name="r">Linear red channel.</param>
	/// <param name="g">Linear green channel.</param>
	/// <param name="b">Linear blue channel.</param>
	/// <param name="a">Straight alpha (default 1.0).</param>
	/// <returns>A linear-RGB color.</returns>
	public static Color FromLinear(double r, double g, double b, double a = 1.0) => new(r, g, b, a);

	/// <summary>Returns a copy of this color with a replaced alpha.</summary>
	/// <param name="a">The new straight alpha.</param>
	/// <returns>A color with the same RGB and the given alpha.</returns>
	public Color WithAlpha(double a) => new(R, G, B, a);

	/// <summary>Returns a copy with every channel clamped to the 0..1 range.</summary>
	/// <returns>A gamut- and alpha-clamped color.</returns>
	public Color Clamp() => new(Clamp01(R), Clamp01(G), Clamp01(B), Clamp01(A));

	/// <summary>Converts to a linear-RGBA <see cref="Vector4"/> (float).</summary>
	/// <returns>A float vector of the linear channels.</returns>
	public Vector4 ToLinearVector4() => new((float)R, (float)G, (float)B, (float)A);

	/// <summary>Converts to a linear-RGB <see cref="Vector3"/> (float), dropping alpha.</summary>
	/// <returns>A float vector of the linear RGB channels.</returns>
	public Vector3 ToLinearVector3() => new((float)R, (float)G, (float)B);

	internal static double Clamp01(double value) => value < 0.0 ? 0.0 : value > 1.0 ? 1.0 : value;
}
