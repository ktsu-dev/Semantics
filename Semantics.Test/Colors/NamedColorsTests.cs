// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Colors;

using ktsu.Semantics.Color;

[TestClass]
public class NamedColorsTests
{
	[TestMethod]
	public void Red_MatchesPureRedHex() =>
		Assert.AreEqual("#FF0000", NamedColors.Red.ToHex());

	[TestMethod]
	public void Transparent_HasZeroAlpha() =>
		Assert.AreEqual(0.0, NamedColors.Transparent.A, 1e-12);

	[TestMethod]
	public void TryGet_IsCaseInsensitive()
	{
		bool found = NamedColors.TryGet("ReD", out Color color);
		Assert.IsTrue(found);
		Assert.AreEqual("#FF0000", color.ToHex());
	}

	[TestMethod]
	public void TryGet_UnknownName_ReturnsFalse() =>
		Assert.IsFalse(NamedColors.TryGet("notacolor", out _));
}
