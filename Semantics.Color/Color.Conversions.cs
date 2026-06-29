// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

using System.Numerics;

public readonly partial record struct Color
{
	/// <summary>Creates a linear color from a gamma-encoded <see cref="Srgb"/>.</summary>
	/// <param name="srgb">The sRGB color.</param>
	/// <param name="a">Straight alpha (default 1.0).</param>
	/// <returns>The linear-RGB color.</returns>
	public static Color FromSrgb(Srgb srgb, double a = 1.0) => srgb.ToLinear(a);

	/// <summary>Creates a linear color from gamma-encoded sRGB channels.</summary>
	/// <param name="r">sRGB red channel (0..1).</param>
	/// <param name="g">sRGB green channel (0..1).</param>
	/// <param name="b">sRGB blue channel (0..1).</param>
	/// <param name="a">Straight alpha (default 1.0).</param>
	/// <returns>The linear-RGB color.</returns>
	public static Color FromSrgb(double r, double g, double b, double a = 1.0) => new Srgb(r, g, b).ToLinear(a);

	/// <summary>Converts this linear color to gamma-encoded <see cref="Srgb"/>.</summary>
	/// <returns>The sRGB equivalent (alpha dropped).</returns>
	public Srgb ToSrgb() => Srgb.FromLinear(this);

	/// <summary>Converts to a gamma-encoded sRGB <see cref="Vector4"/> (float) — the value ImGui expects.</summary>
	/// <returns>A float vector of sRGB RGB plus alpha.</returns>
	public Vector4 ToSrgbVector4()
	{
		Srgb s = ToSrgb();
		return new Vector4((float)s.R, (float)s.G, (float)s.B, (float)A);
	}

	/// <summary>Converts to a gamma-encoded sRGB <see cref="Vector3"/> (float), dropping alpha.</summary>
	/// <returns>A float vector of sRGB RGB.</returns>
	public Vector3 ToSrgbVector3()
	{
		Srgb s = ToSrgb();
		return new Vector3((float)s.R, (float)s.G, (float)s.B);
	}
}
