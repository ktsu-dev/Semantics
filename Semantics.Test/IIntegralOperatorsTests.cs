// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

[TestClass]
public class IIntegralOperatorsTests
{
	public record MockQuantity : PhysicalQuantity<MockQuantity>, IIntegralOperators<MockQuantity, MockQuantity, MockQuantity>
	{
		public static MockQuantity Integrate(MockQuantity left, MockQuantity right)
		{
			return Multiply<MockQuantity>(left, right);
		}

		public static MockQuantity operator *(MockQuantity left, MockQuantity right) => Integrate(left, right);
	}

	[TestMethod]
	public void Integrate_ShouldReturnCorrectResult()
	{
		// Arrange
		MockQuantity left = MockQuantity.Create(double.CreateChecked(5.0));
		MockQuantity right = MockQuantity.Create(double.CreateChecked(3.0));

		// Act
		MockQuantity result = MockQuantity.Integrate(left, right);

		// Assert
		Assert.AreEqual(15.0, result.Quantity<double>());
	}
}
