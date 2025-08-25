// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class PhysicalDimensionTests
{
	[TestMethod]
	public void Equality_And_HashCode_Work()
	{
		PhysicalDimension a = new(baseUnit: BootstrapUnits.Meter, length: 1, time: -1);
		PhysicalDimension b = new(baseUnit: BootstrapUnits.Meter, length: 1, time: -1);
		PhysicalDimension c = new(baseUnit: BootstrapUnits.Meter, length: 2, time: -2);

		Assert.IsTrue(a == b);
		Assert.IsFalse(a == c);
		Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
	}

	[TestMethod]
	public void Multiply_And_Divide_ComposeDimensions()
	{
		PhysicalDimension length = PhysicalDimensions.Length;
		PhysicalDimension time = PhysicalDimensions.Time;
		PhysicalDimension velocity = length / time;
		PhysicalDimension acceleration = velocity / time;

		Assert.AreEqual(PhysicalDimensions.Velocity, velocity);
		Assert.AreEqual(PhysicalDimensions.Acceleration, acceleration);
	}

	[TestMethod]
	public void Pow_RaisesDimension()
	{
		PhysicalDimension length = PhysicalDimensions.Length;
		PhysicalDimension area = PhysicalDimension.Pow(length, 2);
		PhysicalDimension volume = PhysicalDimension.Pow(length, 3);

		Assert.AreEqual(PhysicalDimensions.Area, area);
		Assert.AreEqual(PhysicalDimensions.Volume, volume);
	}

	[TestMethod]
	public void ToString_FormatsCorrectly()
	{
		Assert.AreEqual("1", PhysicalDimensions.Dimensionless.ToString());
		Assert.AreEqual("L", PhysicalDimensions.Length.ToString());
		Assert.AreEqual("L²", PhysicalDimensions.Area.ToString());
		Assert.AreEqual("L T⁻¹", PhysicalDimensions.Velocity.ToString());
		// Order in ToString follows insertion order; verify content rather than exact order
		string z = PhysicalDimensions.AcousticImpedance.ToString();
		StringAssert.Contains(z, "M");
		StringAssert.Contains(z, "L⁻²");
		StringAssert.Contains(z, "T⁻¹");
	}
}

