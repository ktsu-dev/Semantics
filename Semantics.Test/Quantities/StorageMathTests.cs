// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System;
using System.Numerics;
using System.Reflection;
using ktsu.Semantics.Quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// The square root every generated <c>Length()</c> and <c>Distance()</c> takes, and the higher roots
/// and hypotenuse that ship beside it.
/// </summary>
/// <remarks>
/// The primitives are asserted against the expression the generated code used to inline, so a change
/// to their results fails here. The other types are asserted against the answer at their own
/// precision, which the old round trip through <see cref="double"/> could not reach. The surface
/// itself is asserted too, since these are public API on a package with a compatibility baseline.
/// </remarks>
[TestClass]
public sealed class StorageMathTests
{
	/// <summary>
	/// A <see cref="decimal"/> root has every digit the type holds, where converting the
	/// <see cref="double"/> root kept 15.
	/// </summary>
	[TestMethod]
	public void DecimalSquareRootOfTwoHasEveryDigitTheTypeHolds()
	{
		decimal expected = 1.4142135623730950488016887242m;
		decimal throughDouble = decimal.CreateChecked(Math.Sqrt(2d));

		Assert.AreEqual(expected, StorageMath.Sqrt(2m));
		Assert.AreNotEqual(expected, throughDouble, "The old route should have been the less precise one, or this test proves nothing.");
	}

	/// <summary>
	/// A perfect square comes back exactly, across the range of <see cref="decimal"/>.
	/// </summary>
	/// <param name="square">The value to take the root of, as a literal.</param>
	/// <param name="root">Its exact root, as a literal.</param>
	[TestMethod]
	[DataRow("144", "12")]
	[DataRow("25", "5")]
	[DataRow("0.0000000000000000000001", "0.00000000001")]
	[DataRow("10000000000000000000000000000", "100000000000000")]
	[DataRow("1", "1")]
	public void DecimalPerfectSquareIsExact(string square, string root)
	{
		decimal value = decimal.Parse(square, System.Globalization.CultureInfo.InvariantCulture);
		decimal expected = decimal.Parse(root, System.Globalization.CultureInfo.InvariantCulture);

		Assert.AreEqual(expected, StorageMath.Sqrt(value));
	}

	[TestMethod]
	public void DecimalZeroIsZero() => Assert.AreEqual(0m, StorageMath.Sqrt(0m));

	/// <summary>
	/// A negative <see cref="decimal"/> throws exactly as the old round trip did, since <see cref="decimal"/> has no NaN to answer with.
	/// </summary>
	[TestMethod]
	public void DecimalNegativeThrowsAsTheDoubleRoundTripDid()
	{
		Assert.ThrowsExactly<OverflowException>(static () => decimal.CreateChecked(Math.Sqrt(double.CreateChecked(-4m))));
		Assert.ThrowsExactly<OverflowException>(static () => StorageMath.Sqrt(-4m));
	}

	/// <summary>
	/// <see cref="double"/> goes through <see cref="Math.Sqrt(double)"/> unchanged, including NaN for a negative input.
	/// </summary>
	/// <param name="value">The value to take the root of.</param>
	[TestMethod]
	[DataRow(2d)]
	[DataRow(0.5d)]
	[DataRow(1e300)]
	[DataRow(3d)]
	[DataRow(-1d)]
	public void DoubleIsUnchanged(double value) => Assert.AreEqual(Math.Sqrt(value), StorageMath.Sqrt(value));

	[TestMethod]
	public void FloatIsUnchanged() => Assert.AreEqual(float.CreateChecked(Math.Sqrt(2d)), StorageMath.Sqrt(2f));

	[TestMethod]
	public void IntegerIsUnchanged() => Assert.AreEqual(int.CreateChecked(Math.Sqrt(26d)), StorageMath.Sqrt(26));

	/// <summary>
	/// An integer type outside the primitives is refined in integer arithmetic and settles on the
	/// floor of the root, which is what truncating the <see cref="double"/> root gave for values small
	/// enough to fit one.
	/// </summary>
	[TestMethod]
	public void BigIntegerSettlesOnTheFloorOfTheRoot()
	{
		BigInteger tenToTheForty = BigInteger.Pow(10, 40);
		BigInteger tenToTheTwenty = BigInteger.Pow(10, 20);

		Assert.AreEqual(new BigInteger(5), StorageMath.Sqrt(new BigInteger(26)));
		Assert.AreEqual(tenToTheTwenty, StorageMath.Sqrt(tenToTheForty));
		Assert.AreEqual(tenToTheTwenty, StorageMath.Sqrt(tenToTheForty + BigInteger.One));
	}

	/// <summary>
	/// A <see cref="BigInteger"/> too large for <see cref="double"/> still gets its exact root, where a
	/// start from the value itself used to run out of Newton steps and return a far larger number.
	/// </summary>
	[TestMethod]
	public void BigIntegerBeyondTheRangeOfDoubleHasItsExactRoot()
	{
		BigInteger tenToTheTwoHundred = BigInteger.Pow(10, 200);

		Assert.AreEqual(BigInteger.Pow(2, 1024), StorageMath.Sqrt(BigInteger.Pow(2, 2048)));
		Assert.AreEqual(tenToTheTwoHundred, StorageMath.Sqrt(BigInteger.Pow(10, 400)));
		Assert.AreEqual(tenToTheTwoHundred, StorageMath.Sqrt(BigInteger.Pow(10, 400) + BigInteger.One));
		Assert.AreEqual(tenToTheTwoHundred - BigInteger.One, StorageMath.Sqrt(BigInteger.Pow(10, 400) - BigInteger.One));
	}

	[TestMethod]
	public void BigIntegerZeroIsZero() => Assert.AreEqual(BigInteger.Zero, StorageMath.Sqrt(BigInteger.Zero));

	/// <summary>
	/// The smallest positive <see cref="decimal"/> has an exact root, and the largest has one correct to the last place the type holds.
	/// </summary>
	[TestMethod]
	public void DecimalExtremesHaveTheirRoots()
	{
		// The root of decimal.MaxValue is 281474976710655.9999999999999982236..., within 2e-15 of this.
		const decimal RootOfMaxValue = 281474976710656m;

		Assert.AreEqual(0.00000000000001m, StorageMath.Sqrt(0.0000000000000000000000000001m));
		Assert.IsLessThanOrEqualTo(0.00000000000002m, Math.Abs(StorageMath.Sqrt(decimal.MaxValue) - RootOfMaxValue));
	}

	/// <summary>
	/// The roots are public, because an application computing a norm the library does not emit has to
	/// reach them or reimplement them. The seeding and the double round trip are not: they are how the
	/// roots are taken, not what they promise.
	/// </summary>
	[TestMethod]
	public void TheRootsArePublicAndTheirWorkingsAreNot()
	{
		Assert.IsTrue(typeof(StorageMath).IsPublic, "StorageMath itself has to be reachable from outside the assembly.");

		string[] contract = [nameof(StorageMath.Sqrt), nameof(StorageMath.Cbrt), nameof(StorageMath.RootN), nameof(StorageMath.Hypot)];
		string[] workings = ["IsRoundedThroughDouble", "IsBinaryFloatingPoint", "HasFloorDivision", "Seed", "SeedForRoot", "TryRootThroughDouble", "RootThroughDouble", "RootByNewton", "RootByDescent", "RootBySettling", "TryPower"];

		foreach (string name in contract)
		{
			MethodInfo? method = typeof(StorageMath).GetMethod(name, BindingFlags.Public | BindingFlags.Static);
			Assert.IsNotNull(method, $"{name} is part of the contract and has to be public.");
		}

		foreach (string name in workings)
		{
			MethodInfo? method = typeof(StorageMath).GetMethod(name, BindingFlags.Public | BindingFlags.Static);
			Assert.IsNull(method, $"{name} is implementation and should not be frozen into the public surface.");
		}
	}

	/// <summary>
	/// <see cref="double"/> and <see cref="float"/> take <see cref="Math.Cbrt(double)"/>, including the
	/// negative cube root it defines where <see cref="Math.Pow(double, double)"/> answers NaN.
	/// </summary>
	/// <param name="value">The value to take the root of.</param>
	[TestMethod]
	[DataRow(8d)]
	[DataRow(2d)]
	[DataRow(0d)]
	[DataRow(-27d)]
	[DataRow(1e300)]
	public void DoubleCubeRootIsUnchanged(double value) => Assert.AreEqual(Math.Cbrt(value), StorageMath.Cbrt(value));

	/// <summary>
	/// A <see cref="decimal"/> cube root is refined in <see cref="decimal"/>, so cubing it returns
	/// further into the type than cubing the root the <see cref="double"/> route gives.
	/// </summary>
	[TestMethod]
	public void DecimalCubeRootIsMorePreciseThanTheDoubleRoute()
	{
		decimal refined = StorageMath.Cbrt(2m);
		decimal throughDouble = decimal.CreateChecked(Math.Cbrt(2d));

		decimal refinedError = Math.Abs((refined * refined * refined) - 2m);
		decimal doubleError = Math.Abs((throughDouble * throughDouble * throughDouble) - 2m);

		Assert.IsLessThan(doubleError, refinedError, "The refinement should beat the route it is seeded from, or it is not earning its steps.");
		Assert.IsLessThan(0.0000000000000000000000001m, refinedError);
	}

	/// <summary>
	/// A perfect cube comes back exactly, and a negative one keeps its sign, which is the cube root's
	/// point of difference from the square root.
	/// </summary>
	/// <param name="cube">The value to take the root of, as a literal.</param>
	/// <param name="root">Its exact root, as a literal.</param>
	[TestMethod]
	[DataRow("8", "2")]
	[DataRow("-8", "-2")]
	[DataRow("1000000000000", "10000")]
	[DataRow("0.000000000001", "0.0001")]
	[DataRow("-27", "-3")]
	public void DecimalPerfectCubeIsExact(string cube, string root)
	{
		decimal value = decimal.Parse(cube, System.Globalization.CultureInfo.InvariantCulture);
		decimal expected = decimal.Parse(root, System.Globalization.CultureInfo.InvariantCulture);

		Assert.AreEqual(expected, StorageMath.Cbrt(value));
	}

	/// <summary>
	/// An integer type is refined in integer arithmetic for a cube root, so it lands on the exact floor
	/// rather than on whatever <see cref="Math.Pow(double, double)"/> rounded to.
	/// </summary>
	[TestMethod]
	public void IntegerCubeRootIsTheExactFloor()
	{
		Assert.AreEqual(2, StorageMath.Cbrt(26));
		Assert.AreEqual(3, StorageMath.Cbrt(27));
		Assert.AreEqual(3, StorageMath.Cbrt(28));
		Assert.AreEqual(-3, StorageMath.Cbrt(-27));
		Assert.AreEqual(1290, StorageMath.Cbrt(2147483647));
	}

	/// <summary>
	/// A <see cref="BigInteger"/> beyond the range of <see cref="double"/> gets its exact cube root,
	/// through the same scaling the square root uses.
	/// </summary>
	[TestMethod]
	public void BigIntegerCubeRootBeyondTheRangeOfDoubleIsExact()
	{
		BigInteger tenToTheHundred = BigInteger.Pow(10, 100);

		Assert.AreEqual(tenToTheHundred, StorageMath.Cbrt(BigInteger.Pow(10, 300)));
		Assert.AreEqual(tenToTheHundred, StorageMath.Cbrt(BigInteger.Pow(10, 300) + BigInteger.One));
		Assert.AreEqual(tenToTheHundred - BigInteger.One, StorageMath.Cbrt(BigInteger.Pow(10, 300) - BigInteger.One));
		Assert.AreEqual(BigInteger.Pow(2, 512), StorageMath.Cbrt(BigInteger.Pow(2, 1536)));
	}

	/// <summary>
	/// The first two degrees are the identity and <see cref="StorageMath.Sqrt{T}(T)"/>, so a caller
	/// parameterised on the degree does not have to special-case them.
	/// </summary>
	[TestMethod]
	public void TheFirstTwoDegreesAreTheIdentityAndTheSquareRoot()
	{
		Assert.AreEqual(7m, StorageMath.RootN(7m, 1));
		Assert.AreEqual(StorageMath.Sqrt(2m), StorageMath.RootN(2m, 2));
		Assert.AreEqual(StorageMath.Sqrt(26), StorageMath.RootN(26, 2));
	}

	/// <summary>
	/// A degree that is not a positive integer is a caller error rather than an answer.
	/// </summary>
	/// <param name="degree">The degree to ask for.</param>
	[TestMethod]
	[DataRow(0)]
	[DataRow(-1)]
	[DataRow(int.MinValue)]
	public void ADegreeBelowOneThrows(int degree)
		=> Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => StorageMath.RootN(8m, degree));

	/// <summary>
	/// A higher root is exact where it terminates in the type, for a fractional type and an integer one.
	/// </summary>
	[TestMethod]
	public void HigherRootsAreExactWhereTheyTerminate()
	{
		Assert.AreEqual(2m, StorageMath.RootN(16m, 4));
		Assert.AreEqual(3m, StorageMath.RootN(243m, 5));
		Assert.AreEqual(0.1m, StorageMath.RootN(0.00001m, 5));
		Assert.AreEqual(-2m, StorageMath.RootN(-32m, 5));
		Assert.AreEqual(new BigInteger(10), StorageMath.RootN(BigInteger.Pow(10, 7), 7));
		Assert.AreEqual(BigInteger.Pow(10, 100), StorageMath.RootN(BigInteger.Pow(10, 500), 5));
	}

	/// <summary>
	/// A degree large enough that the estimate's own power leaves the type still answers, because the
	/// power is taken checked and an estimate that overflows it is known to be above the root.
	/// </summary>
	[TestMethod]
	public void ADegreeTooLargeForTheTypeToSquareStillAnswers()
	{
		Assert.AreEqual(1, StorageMath.RootN(100, 20));
		Assert.AreEqual(2, StorageMath.RootN(1048576, 20));
		Assert.AreEqual(1, StorageMath.RootN(int.MaxValue, 40));
	}

	/// <summary>
	/// An even root of a negative value has no real answer, and says so exactly as the square root does.
	/// </summary>
	[TestMethod]
	public void AnEvenRootOfANegativeValueHasNoAnswer()
	{
		Assert.IsTrue(double.IsNaN(StorageMath.RootN(-16d, 4)));
		Assert.ThrowsExactly<OverflowException>(static () => StorageMath.RootN(-16m, 4));
	}

	/// <summary>
	/// The primitives take <see cref="double.Hypot(double, double)"/>, which is the answer at their precision.
	/// </summary>
	[TestMethod]
	public void PrimitiveHypotenuseIsTheDoubleOne()
	{
		Assert.AreEqual(double.Hypot(3d, 4d), StorageMath.Hypot(3d, 4d));
		Assert.AreEqual(double.Hypot(-3d, 4d), StorageMath.Hypot(-3d, 4d));
		Assert.AreEqual(5, StorageMath.Hypot(3, 4));
		Assert.AreEqual(5f, StorageMath.Hypot(3f, 4f));
	}

	/// <summary>
	/// A pair whose squares leave the range of the type still has its hypotenuse, because a type with a
	/// fractional part is computed from the ratio of the two legs rather than from their squares.
	/// </summary>
	[TestMethod]
	public void DecimalHypotenuseSurvivesLegsWhoseSquaresDoNot()
	{
		Assert.AreEqual(5m, StorageMath.Hypot(3m, 4m));
		Assert.AreEqual(0m, StorageMath.Hypot(0m, 0m));
		Assert.AreEqual(4m, StorageMath.Hypot(0m, -4m));

		decimal leg = 1e20m;
		Assert.ThrowsExactly<OverflowException>(() => leg * leg);

		decimal hypotenuse = StorageMath.Hypot(leg, leg);
		decimal expected = leg * StorageMath.Sqrt(2m);

		Assert.IsLessThan(1e6m, Math.Abs(hypotenuse - expected));
	}

	/// <summary>
	/// An integer type squares and sums instead, since the ratio of two integers is not a ratio.
	/// </summary>
	[TestMethod]
	public void BigIntegerHypotenuseIsTheFloorOfTheAnswer()
	{
		Assert.AreEqual(new BigInteger(5), StorageMath.Hypot(new BigInteger(3), new BigInteger(-4)));
		Assert.AreEqual(new BigInteger(2), StorageMath.Hypot(new BigInteger(2), new BigInteger(1)));
		Assert.AreEqual(BigInteger.Pow(10, 200), StorageMath.Hypot(BigInteger.Pow(10, 200), BigInteger.Zero));
	}
}
