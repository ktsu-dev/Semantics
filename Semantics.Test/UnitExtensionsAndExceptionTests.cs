// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using ktsu.Semantics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class UnitExtensionsAndExceptionTests
{
	[TestMethod]
	public void UnitExtensions_BasicChecks()
	{
		Assert.IsTrue(Units.Meter.IsBaseUnit());
		Assert.IsTrue(Units.Meter.IsSI());
		Assert.IsTrue(Units.Kilometer.IsSI());
		Assert.IsTrue(Units.Centimeter.IsMetric());
		Assert.IsTrue(Units.Foot.IsImperial());
		Assert.IsFalse(Units.NauticalMile.IsImperial());
	}

	[TestMethod]
	public void UnitConversionException_MessageAndProperties()
	{
		UnitConversionException ex = new(Units.Meter, Units.Second, "different dimensions");
		StringAssert.Contains(ex.Message, Units.Meter.Symbol);
		StringAssert.Contains(ex.Message, Units.Second.Symbol);
		Assert.AreEqual(Units.Meter, ex.SourceUnit);
		Assert.AreEqual(Units.Second, ex.TargetUnit);
	}
}

