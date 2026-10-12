// Copyright (c) 2023-2026 ktsu-dev contributors

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
	public void Add_TenTupletDurations_ThrowsOverflowInsteadOfWrapping()
	{
		// 1/3 + 1/5 + ... + 1/31 is exactly n/100280245065, which no int fraction can hold. It used
		// to wrap to -505842482/1495997257 and report a negative total.
		int[] denominators = [3, 5, 7, 11, 13, 17, 19, 23, 29, 31];
		Duration total = Duration.Create(0, 1);
		_ = Assert.ThrowsExactly<OverflowException>(() =>
		{
			foreach (int denominator in denominators)
			{
				total = total.Add(Duration.Create(1, denominator));
			}
		});
		Assert.IsTrue(total.Numerator > 0, $"A partial sum wrapped to {total}");
	}

	[TestMethod]
	public void Add_WhoseCommonDenominatorFits_SumsExactly()
	{
		// The product of the denominators passes int.MaxValue but their lcm does not, so the sum is
		// representable. Multiplying the denominators first wrapped these.
		Assert.AreEqual(Duration.Create(1, 32768), Duration.Create(1, 65536).Add(Duration.Create(1, 65536)));

		Duration eightTuplets = Duration.Create(0, 1);
		foreach (int denominator in new[] { 3, 5, 7, 11, 13, 17, 19, 23 })
		{
			eightTuplets = eightTuplets.Add(Duration.Create(1, denominator));
		}

		Duration total = eightTuplets.Add(Duration.Create(1, 3 * 5 * 7 * 11 * 13 * 17 * 19));
		Assert.AreEqual(0, 111546435 % total.Denominator, $"{total} should be over a divisor of the lcm");
		Assert.AreEqual(eightTuplets.AsWholeNotes + (1.0 / 4849845), total.AsWholeNotes, 1e-12);
	}

	[TestMethod]
	public void Add_SharedDenominators_StayReduced()
	{
		// Mixing triplets and plain values reuses denominators, so the sum stays over their lcm.
		Duration total = Duration.Create(1, 12).Add(Duration.Create(1, 6)).Add(Duration.Create(1, 4)).Add(Duration.Create(1, 12));
		Assert.AreEqual(Duration.Create(7, 12), total);
	}

	[TestMethod]
	public void Arithmetic_ThatDoesNotFitInInt_ThrowsOverflow()
	{
		_ = Assert.ThrowsExactly<OverflowException>(() => Duration.Create(2, 1).Multiply(int.MaxValue));
		_ = Assert.ThrowsExactly<OverflowException>(() => Duration.Create(int.MaxValue, 1).Dotted());
		_ = Assert.ThrowsExactly<OverflowException>(() => Duration.Create(1, int.MaxValue).Add(Duration.Create(1, int.MaxValue - 1)));
		_ = Assert.ThrowsExactly<OverflowException>(() => Duration.Create(int.MinValue, -1));
	}

	[TestMethod]
	public void Arithmetic_WhoseReducedResultFits_DoesNotThrow()
	{
		// The unreduced product passes int.MaxValue, but the reduced result does not.
		Assert.AreEqual(Duration.Create(1, 1), Duration.Create(1, int.MaxValue).Multiply(int.MaxValue));
		Assert.AreEqual(Duration.Create(int.MinValue, 1), Duration.Create(int.MinValue, 1));
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
