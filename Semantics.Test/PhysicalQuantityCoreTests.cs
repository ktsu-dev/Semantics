// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using System;
using ktsu.Semantics.Quantities;
using ktsu.Semantics.Quantities.Units;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Exercises the canonical <see cref="IPhysicalQuantity{T}"/> surface from #59:
/// the <c>Dimension</c> accessor, dimensionally-typed <c>In(I&lt;Dim&gt;Unit)</c>
/// (compile-time safety), and same-dimension <c>CompareTo</c> / <c>Equals</c>.
/// </summary>
[TestClass]
public sealed class PhysicalQuantityCoreTests
{
	[TestMethod]
	public void In_LinearUnit()
	{
		Length<double> tenMeters = Length<double>.Create(10.0);
		double inKilometers = tenMeters.In(Units.Kilometer);
		Assert.AreEqual(0.01, inKilometers, 1e-12);
	}

	[TestMethod]
	public void In_OffsetUnit()
	{
		Temperature<double> kelvin300 = Temperature<double>.Create(300.0);
		double celsius = kelvin300.In(Units.Celsius);
		Assert.AreEqual(26.85, celsius, 1e-2);
	}

	// Note: the previous "should throw UnitConversionException on dimension mismatch"
	// test is now expressed at compile time via the typed I{Dim}Unit parameter on In().
	// E.g. `length.In(Units.Kilogram)` no longer compiles because Kilogram implements
	// IMassUnit, not ILengthUnit. The UnitConversionException type is kept for any
	// future runtime-dispatch path.

	[TestMethod]
	public void Dimension_ReportsDimensionForGeneratedQuantity()
	{
		Length<double> length = Length<double>.Create(1.0);
		Assert.AreEqual("Length", length.Dimension.Name);
	}

	[TestMethod]
	public void CompareTo_SameDimension()
	{
		Length<double> a = Length<double>.Create(1.0);
		Length<double> b = Length<double>.Create(2.0);
		Assert.IsLessThan(0, a.CompareTo(b));
		Assert.IsGreaterThan(0, b.CompareTo(a));
		Assert.AreEqual(0, a.CompareTo(Length<double>.Create(1.0)));
	}

	[TestMethod]
	public void CompareTo_CrossDimension_Throws()
	{
		Length<double> length = Length<double>.Create(1.0);
		IPhysicalQuantity<double> mass = Mass<double>.Create(1.0);
		Assert.ThrowsExactly<ArgumentException>(() => length.CompareTo(mass));
	}

	[TestMethod]
	public void Equals_SameDimensionAndValue()
	{
		Length<double> a = Length<double>.Create(1.0);
		Length<double> b = Length<double>.Create(1.0);
		Assert.IsTrue(a.Equals((IPhysicalQuantity<double>)b));
	}

	[TestMethod]
	public void Equals_CrossDimension_ReturnsFalse()
	{
		Length<double> length = Length<double>.Create(1.0);
		IPhysicalQuantity<double> mass = Mass<double>.Create(1.0);
		Assert.IsFalse(length.Equals(mass));
	}
}
