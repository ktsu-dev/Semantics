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

	[TestMethod]
	public void RegexMatch_NumberPattern_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithNumberPattern testString = SemanticString<TestStringWithNumberPattern>.FromString<TestStringWithNumberPattern>("12345");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void RegexMatch_NumberPattern_InvalidString_ThrowsFormatException()
	{
		// Act & Assert
		Assert.ThrowsException<FormatException>(() => SemanticString<TestStringWithNumberPattern>.FromString<TestStringWithNumberPattern>("abc123"));
	}

	[TestMethod]
	public void MultipleContains_AllMatch_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithMultipleContains testString = SemanticString<TestStringWithMultipleContains>.FromString<TestStringWithMultipleContains>("abcABC123xyz");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void MultipleContains_OneMissing_ThrowsFormatException()
	{
		// Act & Assert
		Assert.ThrowsException<FormatException>(() => SemanticString<TestStringWithMultipleContains>.FromString<TestStringWithMultipleContains>("abc123xyz"));
	}

	[TestMethod]
	public void EmptyStringRegex_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithEmptyRegex testString = SemanticString<TestStringWithEmptyRegex>.FromString<TestStringWithEmptyRegex>("");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void ComplexRegex_UrlPattern_ValidatesCorrectly()
	{
		// Arrange
		TestUrlString testString = SemanticString<TestUrlString>.FromString<TestUrlString>("https://example.com");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void ComplexRegex_UrlPattern_InvalidFormat_ThrowsFormatException()
	{
		// Act & Assert
		Assert.ThrowsException<FormatException>(() =>
			SemanticString<TestUrlString>.FromString<TestUrlString>("not-a-url"));
	}

	// Additional comprehensive tests for edge cases

	[TestMethod]
	public void RegexMatchAttribute_WithIgnoreCase_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithRegexIgnoreCase testString = SemanticString<TestStringWithRegexIgnoreCase>.FromString<TestStringWithRegexIgnoreCase>("ABC123");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void RegexMatchAttribute_WithIgnoreCase_DifferentCase_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithRegexIgnoreCase testString = SemanticString<TestStringWithRegexIgnoreCase>.FromString<TestStringWithRegexIgnoreCase>("abc123");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void StringComparison_OrdinalIgnoreCase_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithCaseInsensitive testString = SemanticString<TestStringWithCaseInsensitive>.FromString<TestStringWithCaseInsensitive>("PREFIX_test_SUFFIX");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void StringComparison_OrdinalIgnoreCase_DifferentCase_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithCaseInsensitive testString = SemanticString<TestStringWithCaseInsensitive>.FromString<TestStringWithCaseInsensitive>("prefix_test_suffix");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void EmptyString_Validation_HandledCorrectly()
	{
		// Arrange
		TestEmptyStringValidation testString = SemanticString<TestEmptyStringValidation>.FromString<TestEmptyStringValidation>("");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void MultipleIdenticalAttributes_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithMultipleIdentical testString = SemanticString<TestStringWithMultipleIdentical>.FromString<TestStringWithMultipleIdentical>("PREFIX_test_PREFIX");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void UnicodeCharacters_InValidation_HandledCorrectly()
	{
		// Arrange
		TestStringWithUnicodeValidation testString = SemanticString<TestStringWithUnicodeValidation>.FromString<TestStringWithUnicodeValidation>("🚀Hello World🌟");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void ComplexRegex_Email_ValidatesCorrectly()
	{
		// Arrange
		TestEmailString testString = SemanticString<TestEmailString>.FromString<TestEmailString>("user@example.com");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void ComplexRegex_Email_InvalidFormat_ThrowsFormatException()
	{
		// Act & Assert
		Assert.ThrowsException<FormatException>(() =>
			SemanticString<TestEmailString>.FromString<TestEmailString>("invalid-email"));
	}

	[TestMethod]
	public void ValidateAll_WithManyAttributes_ValidString_Succeeds()
	{
		// Arrange
		TestStringWithManyValidations testString = SemanticString<TestStringWithManyValidations>.FromString<TestStringWithManyValidations>("PREFIX_middle_content_SUFFIX");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void ValidateAll_WithManyAttributes_OneFails_ThrowsFormatException()
	{
		// Act & Assert
		Assert.ThrowsException<FormatException>(() =>
			SemanticString<TestStringWithManyValidations>.FromString<TestStringWithManyValidations>("PREFIX_content_SUFFIX"));
	}

	[TestMethod]
	public void ValidateAny_WithManyAttributes_OneSucceeds_Passes()
	{
		// Arrange
		TestStringWithManyAnyValidations testString = SemanticString<TestStringWithManyAnyValidations>.FromString<TestStringWithManyAnyValidations>("OPTION1_test");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void ValidateAny_WithManyAttributes_AllFail_ThrowsFormatException()
	{
		// Act & Assert
		Assert.ThrowsException<FormatException>(() =>
			SemanticString<TestStringWithManyAnyValidations>.FromString<TestStringWithManyAnyValidations>("none_match"));
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
