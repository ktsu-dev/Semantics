// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string is a properly formatted 32-bit integer.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.Int32 directly:
/// - Int32.Parse() / Int32.TryParse() for parsing
/// - Int32.ToString() for string representation with format control
/// - Built-in comparison operators (&gt;, &lt;, ==, etc.)
/// - Rich API for mathematical operations
/// - Better performance for numerical operations
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.Int32 directly instead of semantic string types. Int32 provides better type safety, performance, built-in mathematical operations, and efficient numerical computations.")]
public sealed class IsInt32Attribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is a valid 32-bit integer.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>True if the string is a valid Int32, false otherwise.</returns>
	public override bool Validate(ISemanticString semanticString) => int.TryParse(semanticString.WeakString, out _);
}
