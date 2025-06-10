// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that a string contains at least some non-whitespace content
/// </summary>
/// <remarks>
/// This attribute validates that the string has meaningful content beyond whitespace.
/// Examples of valid strings: "Hello", " a ", "text", "a\n"
/// Examples of invalid strings: "", "   ", "\t\n\r", null
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class HasNonWhitespaceContentAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string contains non-whitespace content.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string contains non-whitespace content; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		return !string.IsNullOrWhiteSpace(value);
	}
}
