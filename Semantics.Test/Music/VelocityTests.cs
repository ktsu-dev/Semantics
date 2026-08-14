// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class VelocityTests
{
	[TestMethod]
	public void ToStringIsInteger()
	{
		Assert.AreEqual("96", Velocity.Create(96).ToString());
	}

	[TestMethod]
	public void RoundTrip()
	{
		Velocity v = Velocity.Create(100);
		Assert.AreEqual(v, Velocity.Parse(v.ToString()));
	}

	[TestMethod]
	public void TryParseFailsOutOfRange()
	{
		Assert.IsFalse(Velocity.TryParse("200", out Velocity? result));
		Assert.IsNull(result);
	}
}
