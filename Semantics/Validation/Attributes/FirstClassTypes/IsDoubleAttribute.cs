// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string is a properly formatted double-precision floating-point number.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.Double directly:
/// - Double.Parse() / Double.TryParse() for parsing
/// - Double.ToString() for string representation with format control
/// - Built-in comparison operators (&gt;, &lt;, ==, etc.)
/// - Rich API for mathematical operations
/// - Support for special values (NaN, Infinity, etc.)
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.Double directly instead of semantic string types. Double provides better type safety, performance, built-in mathematical operations, and support for special floating-point values.")]
public sealed class IsDoubleAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is a valid double-precision floating-point number.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>True if the string is a valid double, false otherwise.</returns>
	public override bool Validate(ISemanticString semanticString) => double.TryParse(semanticString.WeakString, out _);
}
