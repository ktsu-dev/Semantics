// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Colors;

using ktsu.Semantics.Color;

[TestClass]
public class GammaRegressionTests
{
	[TestMethod]
	public void HexDisplayIsStable_AcrossLinearRoundTrip()
	{
		// Proves the fix is display-safe: hex -> linear Color -> hex returns the same hex.
		string[] samples = ["#000000", "#FFFFFF", "#3A7BD5", "#7F7F7F", "#FFA500"];
		foreach (string hex in samples)
		{
			Assert.AreEqual(hex, Color.FromHex(hex).ToHex());
		}
	}

	[TestMethod]
	public void OklabFromLinear_DiffersFromNaiveSrgbAsLinear()
	{
		// The old bug fed sRGB values straight into the Oklab matrix. Confirm the correct (linear)
		// computation produces a different lightness for mid-gray.
		Color correct = Color.FromHex("#7F7F7F");
		double correctL = correct.ToOklab().L;
		double naiveL = Color.FromLinear(127.0 / 255.0, 127.0 / 255.0, 127.0 / 255.0).ToOklab().L;
		Assert.AreNotEqual(naiveL, correctL, 1e-3);
	}
}
