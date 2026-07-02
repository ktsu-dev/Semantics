// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class DurationTests
{
	[TestMethod]
	public void Create_ReducesToLowestTerms()
	{
		Duration d = Duration.Create(2, 8);
		Assert.AreEqual(1, d.Numerator);
		Assert.AreEqual(4, d.Denominator);
	}

	[TestMethod]
	public void CommonValues_AreCorrect()
	{
		Assert.AreEqual(Duration.Create(1, 4), Duration.Quarter);
		Assert.AreEqual(0.25, Duration.Quarter.AsWholeNotes, 1e-9);
	}

	[TestMethod]
	public void Triplet_Eighth_IsOneTwelfth()
	{
		// three triplet-eighths fill a quarter note
		Duration tripletEighth = Duration.Create(1, 12);
		Assert.AreEqual(Duration.Quarter, tripletEighth.Multiply(3));
	}

	[TestMethod]
	public void Dotted_QuarterIsThreeEighths()
	{
		Assert.AreEqual(Duration.Create(3, 8), Duration.Quarter.Dotted());
	}

	[TestMethod]
	public void Add_SumsFractions()
	{
		Assert.AreEqual(Duration.Create(3, 4), Duration.Half.Add(Duration.Quarter));
	}

	[TestMethod]
	public void Create_RejectsZeroDenominator()
	{
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Duration.Create(1, 0));
	}

	[TestMethod]
	public void ToStringIsReducedFraction()
	{
		Assert.AreEqual("1/4", Duration.Quarter.ToString());
		Assert.AreEqual("1/4", Duration.Create(2, 8).ToString());
	}

	[TestMethod]
	public void RoundTripOfCanonicalOutput()
	{
		Duration d = Duration.Create(2, 8);
		Assert.AreEqual(d, Duration.Parse(d.ToString()));
	}

	[TestMethod]
	public void TryParseFailsOnMissingSlashOrZeroDenominator()
	{
		Assert.IsFalse(Duration.TryParse("4", out Duration? a));
		Assert.IsNull(a);
		Assert.IsFalse(Duration.TryParse("1/0", out Duration? b));
		Assert.IsNull(b);
	}
}
