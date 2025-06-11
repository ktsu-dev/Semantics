// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

[TestClass]
public class PhysicalQuantityEdgeCaseTests
{
	[SIUnit("test", "test unit", "test units")]
	[Obsolete]
	public record TestQuantity : PhysicalQuantity<TestQuantity> { }

	[TestMethod]
	public void Create_WithZeroValue_ShouldWorkCorrectly()
	{
		// Arrange & Act
		TestQuantity quantity = TestQuantity.Create(0.0);

		// Assert
		Assert.IsNotNull(quantity);
		Assert.AreEqual(0.0, quantity.Quantity);
	}

	[TestMethod]
	public void CompareTo_WithNull_ShouldReturnPositive()
	{
		// Arrange
		TestQuantity quantity = TestQuantity.Create(10.0);

		// Act
		int result = quantity.CompareTo(null);

		// Assert
		Assert.IsTrue(result > 0);
	}

	[TestMethod]
	public void Pow_WithZeroPower_ShouldReturnOne()
	{
		// Arrange
		TestQuantity quantity = TestQuantity.Create(10.0);

		// Act
		TestQuantity result = quantity.Pow(0);

		// Assert
		Assert.AreEqual(1.0, result.Quantity);
	}

	[TestMethod]
	public void Pow_WithNegativePower_ShouldReturnCorrectResult()
	{
		// Arrange
		TestQuantity quantity = TestQuantity.Create(2.0);

		// Act
		TestQuantity result = quantity.Pow(-2);

		// Assert
		Assert.AreEqual(0.25, result.Quantity, 1e-10);
	}

	[TestMethod]
	public void Clamp_WithValueBelowMin_ShouldReturnMin()
	{
		// Arrange
		TestQuantity quantity = TestQuantity.Create(5.0);

		// Act
		TestQuantity result = quantity.Clamp(10.0, 20.0);

		// Assert
		Assert.AreEqual(10.0, result.Quantity);
	}

	[TestMethod]
	public void Clamp_WithValueAboveMax_ShouldReturnMax()
	{
		// Arrange
		TestQuantity quantity = TestQuantity.Create(25.0);

		// Act
		TestQuantity result = quantity.Clamp(10.0, 20.0);

		// Assert
		Assert.AreEqual(20.0, result.Quantity);
	}

	[TestMethod]
	public void ToString_WithCustomFormatting_ShouldReturnCorrectString()
	{
		// Arrange
		TestQuantity quantity = TestQuantity.Create(1234.567);

		// Act
		string result = quantity.ToString();

		// Assert
		Assert.IsFalse(string.IsNullOrEmpty(result));
		Assert.IsTrue(result.Contains("1234.567"));
	}

	[TestMethod]
	public void Create_WithMaxValue_ShouldWorkCorrectly()
	{
		// Arrange & Act
		TestQuantity quantity = TestQuantity.Create(double.MaxValue);

		// Assert
		Assert.IsNotNull(quantity);
		Assert.AreEqual(double.MaxValue, quantity.Quantity);
	}

	[TestMethod]
	public void Create_WithMinValue_ShouldWorkCorrectly()
	{
		// Arrange & Act
		TestQuantity quantity = TestQuantity.Create(double.MinValue);

		// Assert
		Assert.IsNotNull(quantity);
		Assert.AreEqual(double.MinValue, quantity.Quantity);
	}

	[TestMethod]
	public void Abs_WithNegativeValue_ShouldReturnPositive()
	{
		// Arrange
		TestQuantity quantity = TestQuantity.Create(-10.5);

		// Act
		TestQuantity result = quantity.Abs();

		// Assert
		Assert.AreEqual(10.5, result.Quantity);
	}

	[TestMethod]
	public void Abs_WithPositiveValue_ShouldReturnSameValue()
	{
		// Arrange
		TestQuantity quantity = TestQuantity.Create(10.5);

		// Act
		TestQuantity result = quantity.Abs();

		// Assert
		Assert.AreEqual(10.5, result.Quantity);
	}

	[TestMethod]
	public void Min_ShouldReturnSmallerValue()
	{
		// Arrange
		TestQuantity quantity1 = TestQuantity.Create(5.0);
		TestQuantity quantity2 = TestQuantity.Create(10.0);

		// Act
		TestQuantity result = quantity1.Min(quantity2);

		// Assert
		Assert.AreEqual(5.0, result.Quantity);
	}

	[TestMethod]
	public void Max_ShouldReturnLargerValue()
	{
		// Arrange
		TestQuantity quantity1 = TestQuantity.Create(5.0);
		TestQuantity quantity2 = TestQuantity.Create(10.0);

		// Act
		TestQuantity result = quantity1.Max(quantity2);

		// Assert
		Assert.AreEqual(10.0, result.Quantity);
	}

	[TestMethod]
	public void Equality_WithSameValues_ShouldReturnTrue()
	{
		// Arrange
		TestQuantity quantity1 = TestQuantity.Create(10.5);
		TestQuantity quantity2 = TestQuantity.Create(10.5);

		// Act & Assert
		Assert.AreEqual(quantity1, quantity2);
		Assert.IsTrue(quantity1 == quantity2);
		Assert.IsFalse(quantity1 != quantity2);
	}

	[TestMethod]
	public void Equality_WithDifferentValues_ShouldReturnFalse()
	{
		// Arrange
		TestQuantity quantity1 = TestQuantity.Create(10.5);
		TestQuantity quantity2 = TestQuantity.Create(15.5);

		// Act & Assert
		Assert.AreNotEqual(quantity1, quantity2);
		Assert.IsFalse(quantity1 == quantity2);
		Assert.IsTrue(quantity1 != quantity2);
	}
}
