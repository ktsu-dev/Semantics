// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string is a properly formatted DateTime.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.DateTime directly:
/// - DateTime.Parse() / DateTime.TryParse() for parsing
/// - DateTime.ToString() for string representation with format control
/// - Built-in comparison operators (&gt;, &lt;, ==, etc.)
/// - Rich API for date/time operations (AddDays, AddHours, etc.)
/// - Culture-aware formatting and parsing
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.DateTime directly instead of semantic string types. DateTime provides better type safety, performance, built-in comparison operations, and rich API for date/time operations.")]
public sealed class IsDateTimeAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is a valid DateTime.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>True if the string is a valid DateTime, false otherwise.</returns>
	public override bool Validate(ISemanticString semanticString) => DateTime.TryParse(semanticString.WeakString, out _);
}
