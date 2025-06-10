// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string is a properly formatted boolean value (true/false).
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.Boolean directly:
/// - Boolean.Parse() / Boolean.TryParse() for parsing
/// - Boolean.ToString() for string representation
/// - Built-in logical operators (&amp;&amp;, ||, !, etc.)
/// - Direct conditional evaluation without parsing
/// - Better performance for logical operations
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.Boolean directly instead of semantic string types. Boolean provides better type safety, performance, built-in logical operations, and direct conditional evaluation.")]
public sealed class IsBooleanAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is a valid boolean value.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>True if the string is a valid boolean, false otherwise.</returns>
	public override bool Validate(ISemanticString semanticString) => bool.TryParse(semanticString.WeakString, out _);
}
