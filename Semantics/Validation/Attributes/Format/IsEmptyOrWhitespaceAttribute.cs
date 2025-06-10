// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that a string is either empty, null, or contains only whitespace characters
/// </summary>
/// <remarks>
/// This attribute validates that the string has no meaningful content.
/// Examples of valid strings: "", "   ", "\t\n\r", null
/// Examples of invalid strings: "Hello", " a ", "text"
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsEmptyOrWhitespaceAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is empty or contains only whitespace.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is null, empty, or whitespace only; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		return string.IsNullOrWhiteSpace(value);
	}
}
