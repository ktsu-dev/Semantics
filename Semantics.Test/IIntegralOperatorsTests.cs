// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

[TestClass]
public class IIntegralOperatorsTests
{
	public record MockQuantity : SemanticQuantity<MockQuantity, double>, IIntegralOperators<MockQuantity, MockQuantity, MockQuantity>
	{
		public static MockQuantity Integrate(MockQuantity left, MockQuantity right)
		{
			return Create(left.Quantity * right.Quantity);
		}

		public static MockQuantity operator *(MockQuantity left, MockQuantity right) => Integrate(left, right);
	}

	[TestMethod]
	public void Integrate_ShouldReturnCorrectResult()
	{
		// Arrange
		var left = MockQuantity.Create(5.0);
		var right = MockQuantity.Create(3.0);

		// Act
		var result = MockQuantity.Integrate(left, right);

		// Assert
		Assert.AreEqual(15.0, result.Quantity);
	}
}
