// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;
nusing ktsu.Semantics.Strings;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class AttributeValidationTests
{
	[TestMethod]
	public void StartsWith_ValidString_ReturnsTrue()
	{
		// Arrange
		TestStringWithPrefix testString = SemanticString<TestStringWithPrefix>.Create<TestStringWithPrefix>("PrefixTestString");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void StartsWith_InvalidString_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<TestStringWithPrefix>.Create<TestStringWithPrefix>("NoPrefixString"));
	}

	[TestMethod]
	public void EndsWith_ValidString_ReturnsTrue()
	{
		// Arrange
		TestStringWithSuffix testString = SemanticString<TestStringWithSuffix>.Create<TestStringWithSuffix>("TestStringSuffix");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void EndsWith_InvalidString_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<TestStringWithSuffix>.Create<TestStringWithSuffix>("NoSuffixString"));
	}

	[TestMethod]
	public void Contains_ValidString_ReturnsTrue()
	{
		// Arrange
		TestStringWithSubstring testString = SemanticString<TestStringWithSubstring>.Create<TestStringWithSubstring>("Test_Contains_String");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void Contains_InvalidString_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<TestStringWithSubstring>.Create<TestStringWithSubstring>("TestString"));
	}

	[TestMethod]
	public void PrefixAndSuffix_ValidString_ReturnsTrue()
	{
		// Arrange
		TestStringWithPrefixAndSuffix testString = SemanticString<TestStringWithPrefixAndSuffix>.Create<TestStringWithPrefixAndSuffix>("PrefixTestSuffix");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void PrefixAndSuffix_MissingPrefix_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<TestStringWithPrefixAndSuffix>.Create<TestStringWithPrefixAndSuffix>("TestSuffix"));
	}

	[TestMethod]
	public void PrefixAndSuffix_MissingSuffix_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<TestStringWithPrefixAndSuffix>.Create<TestStringWithPrefixAndSuffix>("PrefixTest"));
	}

	[TestMethod]
	public void RegexMatch_ValidString_ReturnsTrue()
	{
		// Arrange
		TestStringWithRegex testString = SemanticString<TestStringWithRegex>.Create<TestStringWithRegex>("abc123");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void RegexMatch_InvalidInput_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<TestStringWithRegex>.Create<TestStringWithRegex>("123abc"));
	}

	[TestMethod]
	public void ValidateAny_OneValidAttribute_ReturnsTrue()
	{
		// Arrange
		TestStringWithAnyValidation testString = SemanticString<TestStringWithAnyValidation>.Create<TestStringWithAnyValidation>("PrefixTest");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void ValidateAny_AnotherValidAttribute_ReturnsTrue()
	{
		// Arrange
		TestStringWithAnyValidation testString = SemanticString<TestStringWithAnyValidation>.Create<TestStringWithAnyValidation>("TestSuffix");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void ValidateAny_AllInvalid_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<TestStringWithAnyValidation>.Create<TestStringWithAnyValidation>("JustATest"));
	}

	[TestMethod]
	public void ValidateAll_AllValidAttributes_ReturnsTrue()
	{
		// Arrange
		TestStringWithAllValidation testString = SemanticString<TestStringWithAllValidation>.Create<TestStringWithAllValidation>("PrefixTestSuffix");

		// Act & Assert
		Assert.IsTrue(testString.IsValid());
	}

	[TestMethod]
	public void ValidateAll_OneInvalid_ThrowsArgumentException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<TestStringWithAllValidation>.Create<TestStringWithAllValidation>("TestSuffix"));
	}
}

[StartsWith("Prefix")]
public record TestStringWithPrefix : SemanticString<TestStringWithPrefix> { }

[EndsWith("Suffix")]
public record TestStringWithSuffix : SemanticString<TestStringWithSuffix> { }

[Contains("_Contains_")]
public record TestStringWithSubstring : SemanticString<TestStringWithSubstring> { }

[PrefixAndSuffix("Prefix", "Suffix")]
public record TestStringWithPrefixAndSuffix : SemanticString<TestStringWithPrefixAndSuffix> { }

[RegexMatch("^[a-z]+[0-9]+$")]
public record TestStringWithRegex : SemanticString<TestStringWithRegex> { }

[ValidateAny]
[StartsWith("Prefix")]
[EndsWith("Suffix")]
public record TestStringWithAnyValidation : SemanticString<TestStringWithAnyValidation> { }

[ValidateAll]
[StartsWith("Prefix")]
[EndsWith("Suffix")]
public record TestStringWithAllValidation : SemanticString<TestStringWithAllValidation> { }
