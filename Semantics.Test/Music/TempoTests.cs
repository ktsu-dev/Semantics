// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

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
}
