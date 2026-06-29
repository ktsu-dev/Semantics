// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

using System;
using System.Globalization;
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

	/// <summary>Creates a linear color from a hex string: <c>#RGB</c>, <c>#RRGGBB</c>, or <c>#RRGGBBAA</c> (leading '#' optional). Channels are interpreted as sRGB.</summary>
	/// <param name="hex">The hex color string.</param>
	/// <returns>The linear-RGB color.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="hex"/> is null.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="hex"/> is not a recognised hex length.</exception>
	public static Color FromHex(string hex)
	{
		Ensure.NotNull(hex);

		string h = hex.StartsWith('#') ? hex[1..] : hex;

		if (h.Length == 3)
		{
			h = new string([h[0], h[0], h[1], h[1], h[2], h[2]]);
		}

		if (h.Length is not (6 or 8))
		{
			throw new ArgumentException("Hex color must be #RGB, #RRGGBB, or #RRGGBBAA.", nameof(hex));
		}

		byte r = ParseByte(h, 0);
		byte g = ParseByte(h, 2);
		byte b = ParseByte(h, 4);
		byte a = h.Length == 8 ? ParseByte(h, 6) : (byte)255;
		return FromBytes(r, g, b, a);
	}

	/// <summary>Converts to an uppercase hex string: <c>#RRGGBB</c>, or <c>#RRGGBBAA</c> when alpha is not fully opaque.</summary>
	/// <returns>The hex string.</returns>
	public string ToHex()
	{
		(byte r, byte g, byte b, byte a) = ToBytes();
		return a == 255
			? $"#{r:X2}{g:X2}{b:X2}"
			: $"#{r:X2}{g:X2}{b:X2}{a:X2}";
	}

	/// <summary>Creates a linear color from 8-bit sRGB channels.</summary>
	/// <param name="r">sRGB red byte.</param>
	/// <param name="g">sRGB green byte.</param>
	/// <param name="b">sRGB blue byte.</param>
	/// <param name="a">Alpha byte (default 255).</param>
	/// <returns>The linear-RGB color.</returns>
	public static Color FromBytes(byte r, byte g, byte b, byte a = 255) =>
		FromSrgb(r / 255.0, g / 255.0, b / 255.0, a / 255.0);

	/// <summary>Converts to 8-bit sRGB channels plus an alpha byte.</summary>
	/// <returns>The rounded sRGB byte tuple.</returns>
	public (byte R, byte G, byte B, byte A) ToBytes()
	{
		Srgb s = ToSrgb();
		return (ToByte(s.R), ToByte(s.G), ToByte(s.B), ToByte(A));
	}

	private static byte ParseByte(string hex, int index) =>
		byte.Parse(hex.AsSpan(index, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

	private static byte ToByte(double channel)
	{
		double scaled = Math.Round(Clamp01(channel) * 255.0);
		return (byte)scaled;
	}
}
