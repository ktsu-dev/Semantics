// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string is a properly formatted TimeSpan.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.TimeSpan directly:
/// - TimeSpan.Parse() / TimeSpan.TryParse() for parsing
/// - TimeSpan.ToString() for string representation with format control
/// - Built-in comparison operators (&gt;, &lt;, ==, etc.)
/// - Rich API for time operations (Add, Subtract, Multiply, etc.)
/// - Static factory methods (FromDays, FromHours, FromMinutes, etc.)
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.TimeSpan directly instead of semantic string types. TimeSpan provides better type safety, performance, built-in comparison operations, and rich API for time operations.")]
public sealed class IsTimeSpanAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is a valid TimeSpan.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>True if the string is a valid TimeSpan, false otherwise.</returns>
	public override bool Validate(ISemanticString semanticString) => TimeSpan.TryParse(semanticString.WeakString, out _);
}
