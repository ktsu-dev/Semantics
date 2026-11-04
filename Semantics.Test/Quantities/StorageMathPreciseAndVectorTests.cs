// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System.Numerics;
using ktsu.PreciseNumber;
using ktsu.Semantics.Quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Higher roots that finish for a type with no fixed precision (#457), and vector lengths that do not
/// square their components outright (#327).
/// </summary>
[TestClass]
public sealed class StorageMathPreciseAndVectorTests
{
	private static PreciseNumber P(double value) => Of<PreciseNumber>(value);

	private static T Of<T>(double value)
		where T : INumber<T> => T.CreateChecked(value);

	private static T Abs<T>(T value)
		where T : INumber<T> => T.Abs(value);

	[TestMethod]
	[Timeout(20_000, CooperativeCancellation = true)]
	public void PreciseCubeRootOfAPerfectCubeIsExact()
	{
		PreciseNumber root = StorageMath.Cbrt(P(27));
		Assert.IsLessThan(P(1e-30), Abs(root - P(3)));
	}

	[TestMethod]
	[Timeout(20_000, CooperativeCancellation = true)]
	public void PreciseCubeRootOfTwoHasThirtyDigits()
	{
		PreciseNumber root = StorageMath.Cbrt(P(2));
		PreciseNumber error = Abs((root * root * root) - P(2));
		Assert.IsLessThan(P(1e-30), error);
	}

	[TestMethod]
	[Timeout(20_000, CooperativeCancellation = true)]
	public void PreciseFourthAndFifthRootsFinish()
	{
		PreciseNumber fourth = StorageMath.RootN(P(3), 4);
		PreciseNumber fifth = StorageMath.RootN(P(3), 5);

		Assert.IsLessThan(P(1e-30), Abs((fourth * fourth * fourth * fourth) - P(3)));
		Assert.IsLessThan(P(1e-30), Abs((fifth * fifth * fifth * fifth * fifth) - P(3)));
	}

	[TestMethod]
	public void IntegerVectorLengthsDoNotWrap()
	{
		Assert.AreEqual(65536, Position3D<int>.FromMeter(65536, 0, 0).Length());
		Assert.AreEqual(65536, Displacement2D<int>.FromMeter(65536, 0).Length());
		Assert.AreEqual(1L << 32, Position3D<long>.FromMeter(1L << 32, 0, 0).Length());
		Assert.AreEqual(50000, Position3D<int>.FromMeter(50000, 0, 0).Length());
	}

	[TestMethod]
	public void DecimalVectorLengthPastTheSquareOfItsRange()
	{
		Position3D<decimal> far = Position3D<decimal>.FromMeter(1e15m, 0m, 0m);

		Assert.AreEqual(1e15m, far.Length());
		Assert.AreEqual(1e15m, far.Magnitude().Value);
		Assert.AreEqual(1e15m, far.Distance(Position3D<decimal>.Zero));
	}

	[TestMethod]
	public void FloatVectorLengthAndNormalizePastTheSquareOfItsRange()
	{
		Position3D<float> far = Position3D<float>.FromMeter(1e20f, 0f, 0f);

		Assert.AreEqual(1e20f, far.Length());
		Assert.AreEqual(1f, far.Normalize().X);
	}

	[TestMethod]
	public void FourComponentLengthIsScaled()
	{
		Assert.AreEqual(2m, StorageMath.Hypot(1m, 1m, 1m, 1m));
		Assert.AreEqual(2e20m, StorageMath.Hypot(1e20m, 1e20m, 1e20m, 1e20m));
		Assert.AreEqual(13d, StorageMath.Hypot(3d, 4d, 12d));
	}
}
