// Copyright (c) 2023-2026 ktsu-dev contributors

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

	/// <summary>
	/// The sRGB primaries land on the published Oklab reference values (Ottosson's matrices, as tabulated
	/// for <c>red</c>, <c>lime</c> and <c>blue</c> in CSS Color 4), so a mistyped coefficient fails here
	/// although white and every round trip still pass (#556).
	/// </summary>
	/// <param name="r">The sRGB red channel.</param>
	/// <param name="g">The sRGB green channel.</param>
	/// <param name="b">The sRGB blue channel.</param>
	/// <param name="expectedL">The reference lightness.</param>
	/// <param name="expectedA">The reference green–red axis.</param>
	/// <param name="expectedB">The reference blue–yellow axis.</param>
	[TestMethod]
	[DataRow(1.0, 0.0, 0.0, 0.62796, 0.22486, 0.12585, DisplayName = "red")]
	[DataRow(0.0, 1.0, 0.0, 0.86644, -0.23389, 0.17950, DisplayName = "lime")]
	[DataRow(0.0, 0.0, 1.0, 0.45201, -0.03246, -0.31153, DisplayName = "blue")]
	public void SrgbPrimaries_MatchPublishedOklabValues(double r, double g, double b, double expectedL, double expectedA, double expectedB)
	{
		Oklab lab = Color.FromSrgb(r, g, b).ToOklab();
		Assert.AreEqual(expectedL, lab.L, 1e-4);
		Assert.AreEqual(expectedA, lab.A, 1e-4);
		Assert.AreEqual(expectedB, lab.B, 1e-4);
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
		Assert.IsGreaterThan(0.1, lch.C, $"expected chroma > 0.1 but was {lch.C}");
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
