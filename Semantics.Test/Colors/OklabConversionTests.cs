// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Colors;

using ktsu.Semantics.Color;

[TestClass]
public class OklabConversionTests
{
	[TestMethod]
	public void White_HasLightnessOneAndNoChroma()
	{
		// Reference (Ottosson): linear white -> Oklab L=1, a=0, b=0.
		Oklab lab = Color.FromLinear(1.0, 1.0, 1.0).ToOklab();
		Assert.AreEqual(1.0, lab.L, 1e-4);
		Assert.AreEqual(0.0, lab.A, 1e-4);
		Assert.AreEqual(0.0, lab.B, 1e-4);
	}

	[TestMethod]
	public void OklabRoundTrip_IsIdentity()
	{
		Color original = Color.FromSrgb(0.2, 0.6, 0.9);
		Color roundTripped = Color.FromOklab(original.ToOklab());
		// Tolerance is 1e-6: the 10-significant-digit Ottosson matrix constants limit
		// round-trip precision to ~1e-7 for colours with small linear channel values.
		Assert.AreEqual(original.R, roundTripped.R, 1e-6);
		Assert.AreEqual(original.G, roundTripped.G, 1e-6);
		Assert.AreEqual(original.B, roundTripped.B, 1e-6);
	}

	[TestMethod]
	public void Oklch_RedHasPositiveChroma()
	{
		Oklch lch = Color.FromSrgb(1.0, 0.0, 0.0).ToOklch();
		Assert.IsTrue(lch.C > 0.1, $"expected chroma > 0.1 but was {lch.C}");
	}

	[TestMethod]
	public void OklchRoundTrip_IsIdentity()
	{
		Color original = Color.FromSrgb(0.2, 0.6, 0.9);
		Color roundTripped = Color.FromOklch(original.ToOklch());
		// Tolerance is 1e-6: same constraint as OklabRoundTrip (Oklch passes through Oklab).
		Assert.AreEqual(original.R, roundTripped.R, 1e-6);
		Assert.AreEqual(original.G, roundTripped.G, 1e-6);
		Assert.AreEqual(original.B, roundTripped.B, 1e-6);
	}
}
