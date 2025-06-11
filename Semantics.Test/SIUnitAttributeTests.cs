// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

[TestClass]
public class SIUnitAttributeTests
{
	[TestMethod]
	public void Constructor_ShouldSetPropertiesCorrectly()
	{
		// Arrange
		string expectedSymbol = "m";
		string expectedSingular = "meter";
		string expectedPlural = "meters";

		// Act
		var attribute = new SIUnitAttribute(expectedSymbol, expectedSingular, expectedPlural);

		// Assert
		Assert.AreEqual(expectedSymbol, attribute.Symbol);
		Assert.AreEqual(expectedSingular, attribute.Singular);
		Assert.AreEqual(expectedPlural, attribute.Plural);
	}
}
