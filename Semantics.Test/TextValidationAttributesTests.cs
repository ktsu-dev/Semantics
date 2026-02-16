// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;
nusing ktsu.Semantics.Strings;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class TextValidationAttributesTests
{
	// Test semantic string type for validation
	[StartsWith("test")]
	[EndsWith("end")]
	[Contains("middle")]
	[RegexMatch(@"^test.*middle.*end$")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
	private sealed partial record TestValidatedString : SemanticString<TestValidatedString> { }

	[IsEmailAddress]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
	private sealed partial record EmailString : SemanticString<EmailString> { }

	[IsBase64]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
	private sealed partial record Base64String : SemanticString<Base64String> { }

	[TestMethod]
	public void StartsWithAttribute_ValidString_ShouldPass()
	{
		TestValidatedString validString = SemanticString<TestValidatedString>.Create<TestValidatedString>("testmiddleend");
		Assert.IsNotNull(validString);
		Assert.AreEqual("testmiddleend", validString.WeakString);
	}

	[TestMethod]
	public void StartsWithAttribute_InvalidString_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<TestValidatedString>.Create<TestValidatedString>("invalidmiddleend"));
	}

	[TestMethod]
	public void EndsWithAttribute_ValidString_ShouldPass()
	{
		TestValidatedString validString = SemanticString<TestValidatedString>.Create<TestValidatedString>("testmiddleend");
		Assert.IsNotNull(validString);
		Assert.AreEqual("testmiddleend", validString.WeakString);
	}

	[TestMethod]
	public void EndsWithAttribute_InvalidString_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<TestValidatedString>.Create<TestValidatedString>("testmiddleinvalid"));
	}

	[TestMethod]
	public void ContainsAttribute_ValidString_ShouldPass()
	{
		TestValidatedString validString = SemanticString<TestValidatedString>.Create<TestValidatedString>("testmiddleend");
		Assert.IsNotNull(validString);
		Assert.AreEqual("testmiddleend", validString.WeakString);
	}

	[TestMethod]
	public void ContainsAttribute_InvalidString_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<TestValidatedString>.Create<TestValidatedString>("testinvalidend"));
	}

	[TestMethod]
	public void RegexMatchAttribute_ValidString_ShouldPass()
	{
		TestValidatedString validString = SemanticString<TestValidatedString>.Create<TestValidatedString>("testmiddleend");
		Assert.IsNotNull(validString);
		Assert.AreEqual("testmiddleend", validString.WeakString);
	}

	[TestMethod]
	public void RegexMatchAttribute_InvalidString_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<TestValidatedString>.Create<TestValidatedString>("invalid"));
	}

	[TestMethod]
	public void IsEmailAddressAttribute_ValidEmails_ShouldPass()
	{
		string[] validEmails =
		[
			"test@example.com",
			"user.name@domain.co.uk",
			"firstname+lastname@example.org",
			"test123@test-domain.com"
		];

		foreach (string? email in validEmails)
		{
			EmailString emailString = SemanticString<EmailString>.Create<EmailString>(email);
			Assert.IsNotNull(emailString);
			Assert.AreEqual(email, emailString.WeakString);
		}
	}

	[TestMethod]
	public void IsEmailAddressAttribute_InvalidEmail_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<EmailString>.Create<EmailString>("invalid-email"));
	}

	[TestMethod]
	public void IsEmailAddressAttribute_EmailWithoutAt_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<EmailString>.Create<EmailString>("test.example.com"));
	}

	[TestMethod]
	public void IsEmailAddressAttribute_EmailWithoutDomain_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<EmailString>.Create<EmailString>("test@"));
	}

	[TestMethod]
	public void IsBase64Attribute_ValidBase64_ShouldPass()
	{
		string[] validBase64Strings =
		[
			"SGVsbG8gV29ybGQ=", // "Hello World"
			"VGVzdA==", // "Test"
			"YWJjZGVmZw==", // "abcdefg"
			"", // Empty string is valid base64
			"QQ==", // "A"
		];

		foreach (string? base64 in validBase64Strings)
		{
			Base64String base64String = SemanticString<Base64String>.Create<Base64String>(base64);
			Assert.IsNotNull(base64String);
			Assert.AreEqual(base64, base64String.WeakString);
		}
	}

	[TestMethod]
	public void IsBase64Attribute_InvalidBase64_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<Base64String>.Create<Base64String>("InvalidBase64!"));
	}

	[TestMethod]
	public void IsBase64Attribute_Base64WithSpaces_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<Base64String>.Create<Base64String>("SGVs bG8g V29y bGQ="));
	}

	[TestMethod]
	public void IsBase64Attribute_IncorrectPadding_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<Base64String>.Create<Base64String>("SGVsbG8gV29ybGQ"));
	}

	[TestMethod]
	public void PrefixAndSuffixAttribute_ValidString_ShouldPass()
	{
		// This test would need a separate test string class with PrefixAndSuffixAttribute
		Type testClass = typeof(PrefixSuffixTestString);
	}

	[PrefixAndSuffix("pre", "suf")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
	private sealed partial record PrefixSuffixTestString : SemanticString<PrefixSuffixTestString> { }

	[TestMethod]
	public void PrefixAndSuffixAttribute_WithPrefixAndSuffix_ShouldPass()
	{
		PrefixSuffixTestString validString = SemanticString<PrefixSuffixTestString>.Create<PrefixSuffixTestString>("premiddlesuf");
		Assert.IsNotNull(validString);
		Assert.AreEqual("premiddlesuf", validString.WeakString);
	}

	[TestMethod]
	public void PrefixAndSuffixAttribute_MissingPrefix_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<PrefixSuffixTestString>.Create<PrefixSuffixTestString>("middlesuf"));
	}

	[TestMethod]
	public void PrefixAndSuffixAttribute_MissingSuffix_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<PrefixSuffixTestString>.Create<PrefixSuffixTestString>("premiddle"));
	}

	[TestMethod]
	public void PrefixAndSuffixAttribute_MissingBoth_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<PrefixSuffixTestString>.Create<PrefixSuffixTestString>("middle"));
	}

	// Test multiple validation attributes on empty strings
	[TestMethod]
	public void ValidationAttributes_EmptyString_ShouldHandleCorrectly()
	{
		try
		{
			SemanticString<TestValidatedString>.Create<TestValidatedString>("");
			Assert.Fail("Should have thrown for empty string");
		}
		catch (ArgumentException)
		{
			// Expected behavior
		}
	}

	// Test null input handling
	[TestMethod]
	public void ValidationAttributes_NullString_ShouldThrow()
	{
		string? nullString = null;
		Assert.ThrowsExactly<ArgumentNullException>(() => SemanticString<TestValidatedString>.Create<TestValidatedString>(nullString!));
	}

	// Test case sensitivity
	[StartsWith("TEST")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
	private sealed partial record CaseSensitiveString : SemanticString<CaseSensitiveString> { }

	[TestMethod]
	public void StartsWithAttribute_CaseSensitive_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<CaseSensitiveString>.Create<CaseSensitiveString>("test"));
	}

	[TestMethod]
	public void StartsWithAttribute_CaseMatch_ShouldPass()
	{
		CaseSensitiveString validString = SemanticString<CaseSensitiveString>.Create<CaseSensitiveString>("TEST");
		Assert.IsNotNull(validString);
	}

	// Test regex edge cases
	[RegexMatch(@"^\d{3}-\d{2}-\d{4}$")] // Social Security Number format
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
	private sealed partial record SSNString : SemanticString<SSNString> { }

	[TestMethod]
	public void RegexMatchAttribute_SSNFormat_ShouldPass()
	{
		SSNString ssnString = SemanticString<SSNString>.Create<SSNString>("123-45-6789");
		Assert.IsNotNull(ssnString);
		Assert.AreEqual("123-45-6789", ssnString.WeakString);
	}

	[TestMethod]
	public void RegexMatchAttribute_InvalidSSNFormat_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<SSNString>.Create<SSNString>("12345-6789"));
	}

	// Test very long strings
	[TestMethod]
	public void ValidationAttributes_LongStrings_ShouldWork()
	{
		string longMiddle = new('x', 1000);
		string longValidString = $"test{longMiddle}middleend";

		TestValidatedString result = SemanticString<TestValidatedString>.Create<TestValidatedString>(longValidString);
		Assert.IsNotNull(result);
		Assert.AreEqual(longValidString, result.WeakString);
	}

	// Test special characters in validation
	[Contains("@#$%")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
	private sealed partial record SpecialCharString : SemanticString<SpecialCharString> { }

	[TestMethod]
	public void ContainsAttribute_SpecialCharacters_ShouldPass()
	{
		SpecialCharString specialString = SemanticString<SpecialCharString>.Create<SpecialCharString>("test@#$%test");
		Assert.IsNotNull(specialString);
		Assert.AreEqual("test@#$%test", specialString.WeakString);
	}

	[TestMethod]
	public void ContainsAttribute_MissingSpecialCharacters_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<SpecialCharString>.Create<SpecialCharString>("testtest"));
	}
}
