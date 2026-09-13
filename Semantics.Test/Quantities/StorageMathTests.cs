// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System;
using System.Numerics;
using ktsu.Semantics.Quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// The square root every generated <c>Length()</c> and <c>Distance()</c> takes.
/// </summary>
/// <remarks>
/// The primitives are asserted against the expression the generated code used to inline, so a change
/// to their results fails here. The other types are asserted against the answer at their own
/// precision, which the old round trip through <see cref="double"/> could not reach.
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
}
