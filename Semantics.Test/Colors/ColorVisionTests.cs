// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Colors;

using System;
using ktsu.Semantics.Color;

[TestClass]
public class ColorVisionTests
{
	private static readonly ColorVisionDeficiency[] Deficiencies =
		[ColorVisionDeficiency.Protan, ColorVisionDeficiency.Deutan, ColorVisionDeficiency.Tritan];

	[TestMethod]
	public void SeverityZero_IsIdentity()
	{
		foreach (ColorVisionDeficiency deficiency in Deficiencies)
		{
			AssertMatrix(ColorVisionMatrix.Identity, ColorVision.Matrix(deficiency, 0.0), 1e-12);
		}
	}

	[TestMethod]
	public void SeverityOne_IsThePublishedDichromacyMatrix()
	{
		// Machado, Oliveira and Fernandes (2009), supplementary tables, severity 1.0.
		AssertMatrix(
			new(0.152286, 1.052583, -0.204868, 0.114503, 0.786281, 0.099216, -0.003882, -0.048116, 1.051998),
			ColorVision.Matrix(ColorVisionDeficiency.Protan, 1.0),
			1e-6);
		AssertMatrix(
			new(0.367322, 0.860646, -0.227968, 0.280085, 0.672501, 0.047413, -0.011820, 0.042940, 0.968881),
			ColorVision.Matrix(ColorVisionDeficiency.Deutan, 1.0),
			1e-6);
		AssertMatrix(
			new(1.255528, -0.076749, -0.178779, -0.078411, 0.930809, 0.147602, 0.004733, 0.691367, 0.303900),
			ColorVision.Matrix(ColorVisionDeficiency.Tritan, 1.0),
			1e-6);
	}

	[TestMethod]
	public void APublishedStep_IsReturnedWithoutInterpolationError()
	{
		// 0.3 / 0.1 is 2.9999999999999996 in binary floating point; the step must still be exact.
		AssertMatrix(
			new(0.630323, 0.465641, -0.095964, 0.069181, 0.890046, 0.040773, -0.006308, -0.007724, 1.014032),
			ColorVision.Matrix(ColorVisionDeficiency.Protan, 0.3),
			1e-12);
	}

	[TestMethod]
	public void ASeverityBetweenSteps_IsTheLinearInterpolation()
	{
		foreach (ColorVisionDeficiency deficiency in Deficiencies)
		{
			ColorVisionMatrix low = ColorVision.Matrix(deficiency, 0.5);
			ColorVisionMatrix high = ColorVision.Matrix(deficiency, 0.6);
			ColorVisionMatrix mid = ColorVision.Matrix(deficiency, 0.55);
			AssertMatrix(
				new(
					(low.M11 + high.M11) / 2, (low.M12 + high.M12) / 2, (low.M13 + high.M13) / 2,
					(low.M21 + high.M21) / 2, (low.M22 + high.M22) / 2, (low.M23 + high.M23) / 2,
					(low.M31 + high.M31) / 2, (low.M32 + high.M32) / 2, (low.M33 + high.M33) / 2),
				mid,
				1e-9);
		}
	}

	[TestMethod]
	public void EveryRowOfEveryMatrix_SumsToOne()
	{
		// Each matrix keeps neutral greys neutral, so every row sums to one within the tables' rounding.
		foreach (ColorVisionDeficiency deficiency in Deficiencies)
		{
			for (int step = 0; step <= 20; step++)
			{
				ColorVisionMatrix m = ColorVision.Matrix(deficiency, step / 20.0);
				string where = $"{deficiency} at {step / 20.0}";
				Assert.AreEqual(1.0, m.M11 + m.M12 + m.M13, 1e-3, where);
				Assert.AreEqual(1.0, m.M21 + m.M22 + m.M23, 1e-3, where);
				Assert.AreEqual(1.0, m.M31 + m.M32 + m.M33, 1e-3, where);
			}
		}
	}

	[TestMethod]
	public void Simulate_AppliesTheMatrixToLinearChannelsAndKeepsAlpha()
	{
		Color color = Color.FromLinear(0.2, 0.4, 0.6, 0.5);
		Color simulated = color.Simulate(ColorVisionDeficiency.Deutan, 1.0);
		Assert.AreEqual((0.367322 * 0.2) + (0.860646 * 0.4) + (-0.227968 * 0.6), simulated.R, 1e-9);
		Assert.AreEqual((0.280085 * 0.2) + (0.672501 * 0.4) + (0.047413 * 0.6), simulated.G, 1e-9);
		Assert.AreEqual((-0.011820 * 0.2) + (0.042940 * 0.4) + (0.968881 * 0.6), simulated.B, 1e-9);
		Assert.AreEqual(0.5, simulated.A);
	}

	[TestMethod]
	public void Simulate_ClampsToTheUnitRange()
	{
		// Pure green under protanopia: the red row weights green by 1.052583, so red is clamped to one.
		Color green = Color.FromLinear(0.0, 1.0, 0.0);
		Color protan = green.Simulate(ColorVisionDeficiency.Protan, 1.0);
		Assert.AreEqual(1.0, protan.R);
		Assert.AreEqual(0.786281, protan.G, 1e-9);

		// Pure blue under tritanopia: the red row weights blue by -0.178779, so red is clamped to zero.
		Color blue = Color.FromLinear(0.0, 0.0, 1.0);
		Color tritan = blue.Simulate(ColorVisionDeficiency.Tritan, 1.0);
		Assert.AreEqual(0.0, tritan.R);
	}

	[TestMethod]
	public void Simulate_LeavesGreysGrey()
	{
		Color grey = Color.FromLinear(0.5, 0.5, 0.5);
		foreach (ColorVisionDeficiency deficiency in Deficiencies)
		{
			Color simulated = grey.Simulate(deficiency, 0.7);
			Assert.AreEqual(0.5, simulated.R, 1e-3, deficiency.ToString());
			Assert.AreEqual(0.5, simulated.G, 1e-3, deficiency.ToString());
			Assert.AreEqual(0.5, simulated.B, 1e-3, deficiency.ToString());
		}
	}

	[TestMethod]
	public void SeverityOutsideTheUnitRange_Throws()
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ColorVision.Matrix(ColorVisionDeficiency.Protan, -0.01));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ColorVision.Matrix(ColorVisionDeficiency.Protan, 1.01));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ColorVision.Matrix(ColorVisionDeficiency.Protan, double.NaN));
	}

	[TestMethod]
	public void AnUndefinedDeficiency_Throws() =>
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ColorVision.Matrix((ColorVisionDeficiency)3, 0.5));

	private static void AssertMatrix(ColorVisionMatrix expected, ColorVisionMatrix actual, double tolerance)
	{
		Assert.AreEqual(expected.M11, actual.M11, tolerance, nameof(actual.M11));
		Assert.AreEqual(expected.M12, actual.M12, tolerance, nameof(actual.M12));
		Assert.AreEqual(expected.M13, actual.M13, tolerance, nameof(actual.M13));
		Assert.AreEqual(expected.M21, actual.M21, tolerance, nameof(actual.M21));
		Assert.AreEqual(expected.M22, actual.M22, tolerance, nameof(actual.M22));
		Assert.AreEqual(expected.M23, actual.M23, tolerance, nameof(actual.M23));
		Assert.AreEqual(expected.M31, actual.M31, tolerance, nameof(actual.M31));
		Assert.AreEqual(expected.M32, actual.M32, tolerance, nameof(actual.M32));
		Assert.AreEqual(expected.M33, actual.M33, tolerance, nameof(actual.M33));
	}
}
