// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class AdvancedAttributeValidationTests
{
	[TestMethod]
	public void MultipleValidationAttributes_AllValid_ReturnsTrue()
	{
		// Arrange
		TestStringWithMultipleValidations testString = SemanticString<TestStringWithMultipleValidations>.FromString<TestStringWithMultipleValidations>("prefix-abc123-suffix");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void MultipleValidationAttributes_OneInvalid_ThrowsFormatException()
	{
		// Act & Assert
		Assert.ThrowsException<FormatException>(() =>
			SemanticString<TestStringWithMultipleValidations>.FromString<TestStringWithMultipleValidations>("prefix-123abc-suffix"));
	}

	[TestMethod]
	public void ComplexRegexPattern_ValidInput_ReturnsTrue()
	{
		// Arrange
		TestStringWithComplexRegex testString = SemanticString<TestStringWithComplexRegex>.FromString<TestStringWithComplexRegex>("user@example.com");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void ComplexRegexPattern_InvalidInput_ThrowsFormatException()
	{
		// Act & Assert
		Assert.ThrowsException<FormatException>(() =>
			SemanticString<TestStringWithComplexRegex>.FromString<TestStringWithComplexRegex>("invalid-email"));
	}

	[TestMethod]
	public void CombinedValidateAnyWithMultipleAttributes_OneValid_ReturnsTrue()
	{
		// These strings should pass with ValidateAny attribute
		TestStringWithAnyOfThreeValidations prefixOnlyString = SemanticString<TestStringWithAnyOfThreeValidations>.FromString<TestStringWithAnyOfThreeValidations>("prefix-content");
		TestStringWithAnyOfThreeValidations containsOnlyString = SemanticString<TestStringWithAnyOfThreeValidations>.FromString<TestStringWithAnyOfThreeValidations>("has-special-content");
		TestStringWithAnyOfThreeValidations suffixOnlyString = SemanticString<TestStringWithAnyOfThreeValidations>.FromString<TestStringWithAnyOfThreeValidations>("content-suffix");

		// All should be valid as each satisfies at least one condition
		Assert.IsTrue(prefixOnlyString.IsValid());
		Assert.IsTrue(containsOnlyString.IsValid());
		Assert.IsTrue(suffixOnlyString.IsValid());
	}

	[TestMethod]
	public void CombinedValidateAnyWithMultipleAttributes_NoneValid_ThrowsFormatException()
	{
		// Act & Assert
		Assert.ThrowsException<FormatException>(() =>
			SemanticString<TestStringWithAnyOfThreeValidations>.FromString<TestStringWithAnyOfThreeValidations>("regular-content"));
	}

	[TestMethod]
	public void EmptyString_WithValidationAttributes_ThrowsFormatException()
	{
		// Act & Assert - Empty string shouldn't match prefix/suffix requirements
		Assert.ThrowsException<FormatException>(() =>
			SemanticString<TestStringWithPrefix>.FromString<TestStringWithPrefix>(""));
	}
}

// Test classes for the advanced validation tests
[ValidateAll]
[StartsWith("prefix-")]
[EndsWith("-suffix")]
[RegexMatch("^prefix-[a-z]+[0-9]+-suffix$")]
public record TestStringWithMultipleValidations : SemanticString<TestStringWithMultipleValidations> { }

[RegexMatch(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
public record TestStringWithComplexRegex : SemanticString<TestStringWithComplexRegex> { }

[ValidateAny]
[StartsWith("prefix-")]
[Contains("special")]
[EndsWith("-suffix")]
public record TestStringWithAnyOfThreeValidations : SemanticString<TestStringWithAnyOfThreeValidations> { }

// Reusing TestStringWithPrefix from AttributeValidationTests.cs
