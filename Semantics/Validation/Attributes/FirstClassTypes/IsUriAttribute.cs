// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string is a properly formatted URI.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.Uri directly:
/// - Uri.TryCreate() for parsing and validation
/// - Uri.ToString() for string representation
/// - Built-in properties for components (Scheme, Host, Port, Path, Query, etc.)
/// - Rich API for URI operations and manipulation
/// - Automatic URL encoding/decoding support
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.Uri directly instead of semantic string types. Uri provides better type safety, performance, built-in component access, and rich API for URI operations.")]
public sealed class IsUriAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is a valid URI.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>True if the string is a valid URI, false otherwise.</returns>
	public override bool Validate(ISemanticString semanticString) => Uri.TryCreate(semanticString.WeakString, UriKind.Absolute, out _);
}
