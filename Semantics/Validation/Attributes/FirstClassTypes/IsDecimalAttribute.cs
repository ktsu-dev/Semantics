// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string is a properly formatted decimal number.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.Decimal directly:
/// - Decimal.Parse() / Decimal.TryParse() for parsing
/// - Decimal.ToString() for string representation with format control
/// - Built-in comparison operators (&gt;, &lt;, ==, etc.)
/// - Rich API for mathematical operations
/// - High precision for financial calculations
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.Decimal directly instead of semantic string types. Decimal provides better type safety, performance, built-in mathematical operations, and high precision for calculations.")]
public sealed class IsDecimalAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is a valid decimal number.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>True if the string is a valid decimal, false otherwise.</returns>
	public override bool Validate(ISemanticString semanticString) => decimal.TryParse(semanticString.WeakString, out _);
}
