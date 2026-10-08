// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Music;

using System;

using ktsu.Semantics.Music;

[TestClass]
public class TempoTests
{
	[TestMethod]
	public void ToStringEncodesBpmAndBeat()
	{
		Assert.AreEqual("120bpm@1/4", Tempo.Create(120).ToString());
		Assert.AreEqual("90bpm@1/8", Tempo.Create(90, Duration.Eighth).ToString());
	}

	[TestMethod]
	public void RoundTrip()
	{
		Tempo t = Tempo.Create(132.5, Duration.Eighth);
		Assert.AreEqual(t, Tempo.Parse(t.ToString()));
	}

	[TestMethod]
	public void TryParseFailsWhenMarkerMissing()
	{
		Assert.IsFalse(Tempo.TryParse("120", out Tempo? result));
		Assert.IsNull(result);
	}

	[TestMethod]
	[DataRow("NaNbpm@1/4")]
	[DataRow("-NaNbpm@1/4")]
	[DataRow("Infinitybpm@1/4")]
	[DataRow("-Infinitybpm@1/4")]
	[DataRow("0bpm@1/4")]
	[DataRow("-120bpm@1/4")]
	[DataRow("120bpm@0/4")]
	[DataRow("120bpm@-1/4")]
	[DataRow("120bpm@1/-4")]
	public void TryParseReturnsFalseForInvalidTempo(string text)
	{
		Assert.IsFalse(Tempo.TryParse(text, out Tempo? result));
		Assert.IsNull(result);
	}

	[TestMethod]
	public void TryParseAcceptsValidTempo()
	{
		Assert.IsTrue(Tempo.TryParse("120bpm@1/4", out Tempo? result));
		Assert.AreEqual(Tempo.Create(120), result);
		Assert.AreEqual("120bpm@1/4", result.ToString());
	}

	[TestMethod]
	[DataRow(double.NaN)]
	[DataRow(double.PositiveInfinity)]
	[DataRow(double.NegativeInfinity)]
	[DataRow(0.0)]
	[DataRow(-1.0)]
	public void CreateThrowsForInvalidBeatsPerMinute(double beatsPerMinute) =>
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Tempo.Create(beatsPerMinute, Duration.Quarter));

	[TestMethod]
	[DataRow(0, 1)]
	[DataRow(0, 4)]
	[DataRow(-1, 4)]
	[DataRow(1, -4)]
	public void CreateThrowsForNonPositiveBeat(int numerator, int denominator) =>
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Tempo.Create(120, Duration.Create(numerator, denominator)));
}
