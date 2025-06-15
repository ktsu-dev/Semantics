// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ValidationStrategyTests
{
	[TestMethod]
	public void RegexMatchAttribute_WithOptions_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithRegexOptions testString = SemanticString<TestStringWithRegexOptions>.Create<TestStringWithRegexOptions>("ABC123");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void RegexMatchAttribute_WithOptions_InvalidString_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
			SemanticString<TestStringWithRegexOptions>.Create<TestStringWithRegexOptions>("abc123"));
	}

	[TestMethod]
	public void StringComparison_IgnoreCase_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithCaseInsensitivePrefix testString = SemanticString<TestStringWithCaseInsensitivePrefix>.Create<TestStringWithCaseInsensitivePrefix>("PREFIX_test");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void StringComparison_IgnoreCase_DifferentCase_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithCaseInsensitivePrefix testString = SemanticString<TestStringWithCaseInsensitivePrefix>.Create<TestStringWithCaseInsensitivePrefix>("prefix_test");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void StringComparison_IgnoreCase_WrongPrefix_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
			SemanticString<TestStringWithCaseInsensitivePrefix>.Create<TestStringWithCaseInsensitivePrefix>("wrong_test"));
	}

	[TestMethod]
	public void MultipleAttributes_AllMustPass_ValidString_Succeeds()
	{
		// Arrange
		TestStringWithBasicMultipleValidations testString = SemanticString<TestStringWithBasicMultipleValidations>.Create<TestStringWithBasicMultipleValidations>("PREFIX_contains_SUFFIX");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void MultipleAttributes_OneFails_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
			SemanticString<TestStringWithBasicMultipleValidations>.Create<TestStringWithBasicMultipleValidations>("PREFIX_no_suffix"));
	}

	[TestMethod]
	public void ContainsAttribute_WithStringComparison_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithCaseInsensitiveContains testString = SemanticString<TestStringWithCaseInsensitiveContains>.Create<TestStringWithCaseInsensitiveContains>("test MIDDLE end");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void EndsWith_WithStringComparison_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithCaseInsensitiveEnding testString = SemanticString<TestStringWithCaseInsensitiveEnding>.Create<TestStringWithCaseInsensitiveEnding>("test END");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void EndsWith_WithStringComparison_DifferentCase_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithCaseInsensitiveEnding testString = SemanticString<TestStringWithCaseInsensitiveEnding>.Create<TestStringWithCaseInsensitiveEnding>("test end");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void PrefixAndSuffix_WithStringComparison_ValidatesCorrectly()
	{
		// Arrange
		TestStringWithCaseInsensitivePrefixSuffix testString = SemanticString<TestStringWithCaseInsensitivePrefixSuffix>.Create<TestStringWithCaseInsensitivePrefixSuffix>("START_middle_end");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void EmptyString_WithValidations_HandledCorrectly()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
			SemanticString<TestStringWithPrefix>.Create<TestStringWithPrefix>(""));
	}

	[TestMethod]
	public void SingleCharString_WithValidations_HandledCorrectly()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
			SemanticString<TestStringWithPrefix>.Create<TestStringWithPrefix>("P"));
	}

	[TestMethod]
	public void LongString_WithValidations_HandledCorrectly()
	{
		// Arrange
		string longString = "Prefix" + new string('a', 1000) + "Suffix";

		// Act
		TestStringWithPrefixAndSuffix testString = SemanticString<TestStringWithPrefixAndSuffix>.Create<TestStringWithPrefixAndSuffix>(longString);

		// Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void SpecialCharacters_InValidations_HandledCorrectly()
	{
		// Arrange
		TestStringWithSpecialChars testString = SemanticString<TestStringWithSpecialChars>.Create<TestStringWithSpecialChars>("test@#$%^&*()middle!@#$end");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void Unicode_InValidations_HandledCorrectly()
	{
		// Arrange
		TestStringWithUnicode testString = SemanticString<TestStringWithUnicode>.Create<TestStringWithUnicode>("🚀test emoji🌟");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void ValidateAny_AllAttributesFail_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
			SemanticString<TestStringWithStrictAnyValidation>.Create<TestStringWithStrictAnyValidation>("none"));
	}

	[TestMethod]
	public void NoValidationAttributes_AlwaysValid()
	{
		// Arrange
		TestStringWithNoValidation testString = SemanticString<TestStringWithNoValidation>.Create<TestStringWithNoValidation>("any string");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}
}

// Test types for validation scenarios

[StartsWith("PREFIX", StringComparison.OrdinalIgnoreCase)]
public record TestStringWithCaseInsensitivePrefix : SemanticString<TestStringWithCaseInsensitivePrefix> { }

[RegexMatch("^[A-Z]+[0-9]+$", System.Text.RegularExpressions.RegexOptions.None)]
public record TestStringWithRegexOptions : SemanticString<TestStringWithRegexOptions> { }

[ValidateAll]
[StartsWith("PREFIX")]
[Contains("_contains_")]
[EndsWith("SUFFIX")]
public record TestStringWithBasicMultipleValidations : SemanticString<TestStringWithBasicMultipleValidations> { }

[Contains("middle", StringComparison.OrdinalIgnoreCase)]
public record TestStringWithCaseInsensitiveContains : SemanticString<TestStringWithCaseInsensitiveContains> { }

[EndsWith("end", StringComparison.OrdinalIgnoreCase)]
public record TestStringWithCaseInsensitiveEnding : SemanticString<TestStringWithCaseInsensitiveEnding> { }

[PrefixAndSuffix("start", "end", StringComparison.OrdinalIgnoreCase)]
public record TestStringWithCaseInsensitivePrefixSuffix : SemanticString<TestStringWithCaseInsensitivePrefixSuffix> { }

[Contains("@#$%^&*()")]
[Contains("!@#$")]
public record TestStringWithSpecialChars : SemanticString<TestStringWithSpecialChars> { }

[Contains("🚀")]
[Contains("🌟")]
public record TestStringWithUnicode : SemanticString<TestStringWithUnicode> { }

[ValidateAny]
[StartsWith("ALPHA")]
[StartsWith("BETA")]
[StartsWith("GAMMA")]
public record TestStringWithStrictAnyValidation : SemanticString<TestStringWithStrictAnyValidation> { }

public record TestStringWithNoValidation : SemanticString<TestStringWithNoValidation> { }
