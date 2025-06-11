// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

[TestClass]
public class IDerivativeOperatorsTests
{
	public record MockQuantity : SemanticQuantity<MockQuantity, double>, IDerivativeOperators<MockQuantity, MockQuantity, MockQuantity>
	{
		public static MockQuantity Derive(MockQuantity left, MockQuantity right)
		{
			return Create(left.Quantity / right.Quantity);
		}

		public static MockQuantity operator /(MockQuantity left, MockQuantity right) => Derive(left, right);
	}

	[TestMethod]
	public void Derive_ShouldReturnCorrectResult()
	{
		// Arrange
		var left = MockQuantity.Create(10.0);
		var right = MockQuantity.Create(2.0);

		// Act
		var result = MockQuantity.Derive(left, right);

		// Assert
		Assert.AreEqual(5.0, result.Quantity);
	}

	[TestMethod]
	public void Derive_WithZeroRightOperand_ShouldThrowException()
	{
		// Arrange
		var left = MockQuantity.Create(10.0);
		var right = MockQuantity.Create(0.0);

		// Act & Assert
		Assert.ThrowsException<DivideByZeroException>(() => MockQuantity.Derive(left, right));
	}

	[TestMethod]
	public void Derive_WithNegativeValues_ShouldReturnCorrectResult()
	{
		// Arrange
		var left = MockQuantity.Create(-10.0);
		var right = MockQuantity.Create(2.0);

		// Act
		var result = MockQuantity.Derive(left, right);

		// Assert
		Assert.AreEqual(-5.0, result.Quantity);
	}
}
