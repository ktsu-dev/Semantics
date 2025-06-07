// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Examples;

/// <summary>
/// Example of a semantic string that must start with "http://" or "https://"
/// By default, both StartsWith attributes must pass (using OR logic within the attribute itself)
/// </summary>
[StartsWith("http://", StringComparison.OrdinalIgnoreCase)]
[StartsWith("https://", StringComparison.OrdinalIgnoreCase)]
public record class UrlString : SemanticString<UrlString> { }

/// <summary>
/// Example of a semantic string that must end with ".com"
/// </summary>
[EndsWith(".com", StringComparison.OrdinalIgnoreCase)]
public record class DotComDomain : SemanticString<DotComDomain> { }

/// <summary>
/// Example of a semantic string that must contain "@" (email address)
/// </summary>
[Contains("@")]
public record class EmailAddressString : SemanticString<EmailAddressString> { }

/// <summary>
/// Example of a semantic string that must start with "http://" or "https://" and end with ".com"
/// </summary>
[PrefixAndSuffix("http://", ".com", StringComparison.OrdinalIgnoreCase)]
[PrefixAndSuffix("https://", ".com", StringComparison.OrdinalIgnoreCase)]
public record class DotComUrl : SemanticString<DotComUrl> { }

/// <summary>
/// Example of a semantic string that must match a US phone number pattern
/// </summary>
[RegexMatch(@"^\(\d{3}\) \d{3}-\d{4}$")]
public record class USPhoneNumber : SemanticString<USPhoneNumber> { }

/// <summary>
/// Example of a semantic string that combines multiple validation attributes.
/// By default, ALL validation attributes must pass.
/// </summary>
[ValidateAll]
[StartsWith("ID-")]
[RegexMatch(@"^ID-\d{6}$")]
public record class IdentifierString : SemanticString<IdentifierString> { }

/// <summary>
/// Example of using ValidateAny to allow ANY of the validation attributes to pass
/// </summary>
[ValidateAny]
[EndsWith(".com", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".org", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".net", StringComparison.OrdinalIgnoreCase)]
public record class TopLevelDomain : SemanticString<TopLevelDomain> { }

/// <summary>
/// Example of combining logical operations for complex validation
/// This string must start with "http" AND THEN any of the domain extensions can pass
/// </summary>
[StartsWith("http", StringComparison.OrdinalIgnoreCase)]
[ValidateAny] // Applies only to the next group of validators
[EndsWith(".com", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".org", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".net", StringComparison.OrdinalIgnoreCase)]
public record class WebsiteUrl : SemanticString<WebsiteUrl> { }

/// <summary>
/// Example usage demonstrations
/// </summary>
public static class SemanticStringValidationExamples
{
	/// <summary>
	/// Demonstrates various validation examples using attribute-based validation on SemanticString types.
	/// </summary>
	public static void DemonstrateValidations()
	{
		// Basic validation examples
		_ = "https://example.org".As<UrlString>();

		_ = "example.com".As<DotComDomain>();

		_ = "user@example.com".As<EmailAddressString>();

		_ = "https://example.com".As<DotComUrl>();

		_ = "(123) 456-7890".As<USPhoneNumber>();

		_ = "ID-123456".As<IdentifierString>();

		// ValidateAny example (can be .com OR .org OR .net)
		_ = "example.com".As<TopLevelDomain>();

		_ = "example.org".As<TopLevelDomain>();

		_ = "example.net".As<TopLevelDomain>();

		// This would fail: var invalidDomain = "example.io".As<TopLevelDomain>();

		// Combined example (must start with "http" AND have one of the allowed TLDs)
		_ = "http://example.com".As<WebsiteUrl>();

		_ = "https://example.org".As<WebsiteUrl>();

		_ = "http://example.net".As<WebsiteUrl>();
		// This would fail (wrong protocol): var invalidWebsiteUrl1 = "ftp://example.com".As<WebsiteUrl>();
		// This would fail (wrong TLD): var invalidWebsiteUrl2 = "http://example.io".As<WebsiteUrl>();
	}
}
