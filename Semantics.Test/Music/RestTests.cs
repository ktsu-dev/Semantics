// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class RestTests
{
	[TestMethod]
	public void ToStringIsRColonDuration()
	{
		Assert.AreEqual("R:1/4", Rest.Create(Duration.Quarter).ToString());
	}

	[TestMethod]
	public void RoundTrip()
	{
		Rest r = Rest.Create(Duration.Half);
		Assert.AreEqual(r, Rest.Parse(r.ToString()));
	}

	[TestMethod]
	public void TryParseFailsWithoutRPrefix()
	{
		Assert.IsFalse(Rest.TryParse("1/4", out Rest? result));
		Assert.IsNull(result);
	}
}
