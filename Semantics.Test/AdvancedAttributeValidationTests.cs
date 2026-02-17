// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using ktsu.Semantics.Strings;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class AdvancedAttributeValidationTests
{
	[TestMethod]
	public void MultipleValidationAttributes_AllValid_ReturnsTrue()
	{
		// Arrange
		TestStringWithMultipleValidations testString = SemanticString<TestStringWithMultipleValidations>.Create<TestStringWithMultipleValidations>("prefix-abc123-suffix");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void MultipleValidationAttributes_OneInvalid_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
			SemanticString<TestStringWithMultipleValidations>.Create<TestStringWithMultipleValidations>("prefix-123abc-suffix"));
	}

	[TestMethod]
	public void ComplexRegexPattern_ValidInput_ReturnsTrue()
	{
		// Arrange
		TestStringWithComplexRegex testString = SemanticString<TestStringWithComplexRegex>.Create<TestStringWithComplexRegex>("user@example.com");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void ComplexRegexPattern_InvalidInput_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
			SemanticString<TestStringWithComplexRegex>.Create<TestStringWithComplexRegex>("invalid-email"));
	}

	[TestMethod]
	public void CombinedValidateAnyWithMultipleAttributes_OneValid_ReturnsTrue()
	{
		// These strings should pass with ValidateAny attribute
		TestStringWithAnyOfThreeValidations prefixOnlyString = SemanticString<TestStringWithAnyOfThreeValidations>.Create<TestStringWithAnyOfThreeValidations>("prefix-content");
		TestStringWithAnyOfThreeValidations containsOnlyString = SemanticString<TestStringWithAnyOfThreeValidations>.Create<TestStringWithAnyOfThreeValidations>("has-special-content");
		TestStringWithAnyOfThreeValidations suffixOnlyString = SemanticString<TestStringWithAnyOfThreeValidations>.Create<TestStringWithAnyOfThreeValidations>("content-suffix");

		// All should be valid as each satisfies at least one condition
		Assert.IsTrue(prefixOnlyString.IsValid());
		Assert.IsTrue(containsOnlyString.IsValid());
		Assert.IsTrue(suffixOnlyString.IsValid());
	}

	[TestMethod]
	public void CombinedValidateAnyWithMultipleAttributes_NoneValid_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
			SemanticString<TestStringWithAnyOfThreeValidations>.Create<TestStringWithAnyOfThreeValidations>("regular-content"));
	}

	[TestMethod]
	public void EmptyString_WithValidationAttributes_ThrowsArgumentException()
	{
		// Act & Assert - Empty string shouldn't match prefix/suffix requirements
		Assert.ThrowsExactly<ArgumentException>(() =>
			SemanticString<TestStringWithPrefix>.Create<TestStringWithPrefix>(""));
	}

	[TestMethod]
	public void RegexMatch_NumberPattern_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithNumberPattern testString = SemanticString<TestStringWithNumberPattern>.Create<TestStringWithNumberPattern>("12345");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void RegexMatch_NumberPattern_InvalidString_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<TestStringWithNumberPattern>.Create<TestStringWithNumberPattern>("abc123"));
	}

	[TestMethod]
	public void MultipleContains_AllMatch_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithMultipleContains testString = SemanticString<TestStringWithMultipleContains>.Create<TestStringWithMultipleContains>("abcABC123xyz");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void MultipleContains_OneMissing_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<TestStringWithMultipleContains>.Create<TestStringWithMultipleContains>("abc123xyz"));
	}

	[TestMethod]
	public void EmptyStringRegex_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithEmptyRegex testString = SemanticString<TestStringWithEmptyRegex>.Create<TestStringWithEmptyRegex>("");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void ComplexRegex_UrlPattern_ValidatesCorrectly()
	{
		// Arrange
		TestUrlString testString = SemanticString<TestUrlString>.Create<TestUrlString>("https://example.com");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void ComplexRegex_UrlPattern_InvalidFormat_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
			SemanticString<TestUrlString>.Create<TestUrlString>("not-a-url"));
	}

	// Additional comprehensive tests for edge cases

	[TestMethod]
	public void RegexMatchAttribute_WithIgnoreCase_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithRegexIgnoreCase testString = SemanticString<TestStringWithRegexIgnoreCase>.Create<TestStringWithRegexIgnoreCase>("ABC123");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void RegexMatchAttribute_WithIgnoreCase_DifferentCase_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithRegexIgnoreCase testString = SemanticString<TestStringWithRegexIgnoreCase>.Create<TestStringWithRegexIgnoreCase>("abc123");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void StringComparison_OrdinalIgnoreCase_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithCaseInsensitive testString = SemanticString<TestStringWithCaseInsensitive>.Create<TestStringWithCaseInsensitive>("PREFIX_test_SUFFIX");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void StringComparison_OrdinalIgnoreCase_DifferentCase_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithCaseInsensitive testString = SemanticString<TestStringWithCaseInsensitive>.Create<TestStringWithCaseInsensitive>("prefix_test_suffix");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void EmptyString_Validation_HandledCorrectly()
	{
		// Arrange
		TestEmptyStringValidation testString = SemanticString<TestEmptyStringValidation>.Create<TestEmptyStringValidation>("");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void MultipleIdenticalAttributes_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithMultipleIdentical testString = SemanticString<TestStringWithMultipleIdentical>.Create<TestStringWithMultipleIdentical>("PREFIX_test_PREFIX");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void UnicodeCharacters_InValidation_HandledCorrectly()
	{
		// Arrange
		TestStringWithUnicodeValidation testString = SemanticString<TestStringWithUnicodeValidation>.Create<TestStringWithUnicodeValidation>("🚀Hello World🌟");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void ComplexRegex_Email_ValidatesCorrectly()
	{
		// Arrange
		TestEmailString testString = SemanticString<TestEmailString>.Create<TestEmailString>("user@example.com");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void ComplexRegex_Email_InvalidFormat_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
			SemanticString<TestEmailString>.Create<TestEmailString>("not-an-email"));
	}

	[TestMethod]
	public void ValidateAll_WithManyAttributes_ValidString_Succeeds()
	{
		// Arrange
		TestStringWithManyValidations testString = SemanticString<TestStringWithManyValidations>.Create<TestStringWithManyValidations>("PREFIX_middle_content_SUFFIX");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void ValidateAll_WithManyAttributes_OneFails_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
			SemanticString<TestStringWithManyValidations>.Create<TestStringWithManyValidations>("missing-middle-content-SUFFIX"));
	}

	[TestMethod]
	public void ValidateAny_WithManyAttributes_OneSucceeds_Passes()
	{
		// Arrange
		TestStringWithManyAnyValidations testString = SemanticString<TestStringWithManyAnyValidations>.Create<TestStringWithManyAnyValidations>("OPTION1_test");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void ValidateAny_WithManyAttributes_AllFail_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
			SemanticString<TestStringWithManyAnyValidations>.Create<TestStringWithManyAnyValidations>("none_match"));
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

[RegexMatch("^[0-9]+$")]
public record TestStringWithNumberPattern : SemanticString<TestStringWithNumberPattern> { }

[Contains("abc")]
[Contains("ABC")]
[Contains("123")]
public record TestStringWithMultipleContains : SemanticString<TestStringWithMultipleContains> { }

[RegexMatch("^$")]
public record TestStringWithEmptyRegex : SemanticString<TestStringWithEmptyRegex> { }

[RegexMatch(@"^https?://[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}")]
public record TestUrlString : SemanticString<TestUrlString> { }

[RegexMatch("^[a-z]+[0-9]+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)]
public record TestStringWithRegexIgnoreCase : SemanticString<TestStringWithRegexIgnoreCase> { }

[PrefixAndSuffix("prefix", "suffix", StringComparison.OrdinalIgnoreCase)]
public record TestStringWithCaseInsensitive : SemanticString<TestStringWithCaseInsensitive> { }

public record TestEmptyStringValidation : SemanticString<TestEmptyStringValidation> { }

[StartsWith("PREFIX")]
[StartsWith("PREFIX")]
public record TestStringWithMultipleIdentical : SemanticString<TestStringWithMultipleIdentical> { }

[StartsWith("🚀")]
[EndsWith("🌟")]
public record TestStringWithUnicodeValidation : SemanticString<TestStringWithUnicodeValidation> { }

[RegexMatch(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
public record TestEmailString : SemanticString<TestEmailString> { }

[ValidateAll]
[StartsWith("PREFIX")]
[EndsWith("SUFFIX")]
[Contains("middle")]
[Contains("content")]
public record TestStringWithManyValidations : SemanticString<TestStringWithManyValidations> { }

[ValidateAny]
[StartsWith("OPTION1")]
[StartsWith("OPTION2")]
[StartsWith("OPTION3")]
[EndsWith("ALT1")]
[EndsWith("ALT2")]
public record TestStringWithManyAnyValidations : SemanticString<TestStringWithManyAnyValidations> { }
