// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

[TestClass]
public class IDerivativeOperatorsTests
{
	public record MockQuantity : PhysicalQuantity<MockQuantity>, IDerivativeOperators<MockQuantity, MockQuantity, MockQuantity>
	{
		public static MockQuantity Derive(MockQuantity left, MockQuantity right)
		{
			return Divide<MockQuantity>(left, right);
		}

		public static MockQuantity operator /(MockQuantity left, MockQuantity right) => Derive(left, right);
	}

	[TestMethod]
	public void Derive_ShouldReturnCorrectResult()
	{
		// Arrange
		MockQuantity left = MockQuantity.Create(double.CreateChecked(10.0));
		MockQuantity right = MockQuantity.Create(double.CreateChecked(2.0));

		// Act
		MockQuantity result = MockQuantity.Derive(left, right);

		// Assert
		Assert.AreEqual(5.0, result.Quantity<double>());
	}

	[TestMethod]
	public void Derive_WithZeroRightOperand_ShouldThrowException()
	{
		// Arrange
		MockQuantity left = MockQuantity.Create(double.CreateChecked(10.0));
		MockQuantity right = MockQuantity.Create(double.CreateChecked(0.0));

		// Act & Assert
		Assert.ThrowsException<DivideByZeroException>(() => MockQuantity.Derive(left, right));
	}

	[TestMethod]
	public void Derive_WithNegativeValues_ShouldReturnCorrectResult()
	{
		// Arrange
		MockQuantity left = MockQuantity.Create(double.CreateChecked(-10.0));
		MockQuantity right = MockQuantity.Create(double.CreateChecked(2.0));

		// Act
		MockQuantity result = MockQuantity.Derive(left, right);

		// Assert
		Assert.AreEqual(-5.0, result.Quantity<double>());
	}
}
