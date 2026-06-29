// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Colors;

using System;

using ktsu.Semantics.Color;

[TestClass]
public class HexConversionTests
{
	[TestMethod]
	public void FromHex_ParsesSixDigitAsOpaqueSrgb()
	{
		Color red = Color.FromHex("#FF0000");
		Assert.AreEqual(1.0, red.A, 1e-12);
		(byte r, byte g, byte b, byte a) = red.ToBytes();
		Assert.AreEqual((byte)255, r);
		Assert.AreEqual((byte)0, g);
		Assert.AreEqual((byte)0, b);
		Assert.AreEqual((byte)255, a);
	}

	[TestMethod]
	public void FromHex_ParsesEightDigitAlpha()
	{
		Color half = Color.FromHex("#00000080");
		Assert.AreEqual(128.0 / 255.0, half.A, 1e-6);
	}

	[TestMethod]
	public void FromHex_ParsesThreeDigitShorthand()
	{
		Color a = Color.FromHex("#F00");
		Color b = Color.FromHex("#FF0000");
		Assert.AreEqual(b.R, a.R, 1e-12);
	}

	[TestMethod]
	public void FromHex_AcceptsNoLeadingHash()
	{
		(byte r, _, _, _) = Color.FromHex("00FF00").ToBytes();
		Assert.AreEqual((byte)0, r);
	}

	[TestMethod]
	public void HexRoundTrip_IsStable()
	{
		Assert.AreEqual("#3A7BD5", Color.FromHex("#3A7BD5").ToHex());
	}

	[TestMethod]
	public void ToHex_EmitsAlphaWhenNotOpaque()
	{
		Assert.AreEqual("#3A7BD580", Color.FromHex("#3A7BD580").ToHex());
	}

	[TestMethod]
	public void FromHex_InvalidLength_Throws() =>
		Assert.ThrowsExactly<ArgumentException>(() => Color.FromHex("#12345"));
}
