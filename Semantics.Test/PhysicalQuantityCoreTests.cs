// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using ktsu.Semantics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class PhysicalQuantityCoreTests
{
	[TestMethod]
	public void In_LinearUnit_And_OffsetUnit()
	{
		Length<double> tenMeters = Length<double>.Create(10.0);
		double inKilometers = tenMeters.In(Units.Kilometer);
		Assert.AreEqual(0.01, inKilometers, 1e-12);

		Temperature<double> kelvin300 = Temperature<double>.Create(300.0);
		double celsius = kelvin300.In(Units.Celsius);
		Assert.AreEqual(26.85, celsius, 1e-2);
	}

	[TestMethod]
	public void In_InvalidDimension_ShouldThrowUnitConversionException()
	{
		Length<double> length = Length<double>.Create(1.0);
		Assert.ThrowsExactly<UnitConversionException>(() => length.In(Units.Kilogram));
	}

	[TestMethod]
	public void CompareTo_And_Equals_Behavior()
	{
		Length<double> a = Length<double>.Create(1.0);
		Length<double> b = Length<double>.Create(2.0);
		Assert.IsLessThan(0, a.CompareTo(b));
		Assert.IsGreaterThan(0, b.CompareTo(a));
		Assert.IsTrue(a.Equals(Length<double>.Create(1.0)));

		IPhysicalQuantity<double> other = new FakeQuantity();
		Assert.ThrowsExactly<ArgumentException>(() => a.CompareTo(other));
	}

	private sealed class FakeQuantity : IPhysicalQuantity<double>
	{
		public PhysicalDimension Dimension => PhysicalDimensions.Mass;
		public bool IsPhysicallyValid => true;
		public double Quantity => 0.0;
		public double In(IUnit targetUnit) => 0.0;
		public int CompareTo(IPhysicalQuantity<double>? other) => 0;
		public bool Equals(IPhysicalQuantity<double>? other) => false;
	}
}
