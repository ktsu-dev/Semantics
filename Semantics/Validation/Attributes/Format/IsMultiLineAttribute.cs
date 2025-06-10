// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;

/// <summary>
/// Validates that a string contains line breaks (multiple lines)
/// </summary>
/// <remarks>
/// A multi-line string contains at least one carriage return (\r), line feed (\n), or other line separator character.
/// Examples of valid multi-line strings: "Line 1\nLine 2", "Text with\r\nline breaks", "Multi\nLine\nText"
/// Examples of invalid strings: "Hello World", "This is a single line", "No line breaks here"
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsMultiLineAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string contains line breaks.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string contains line breaks; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		if (string.IsNullOrEmpty(value))
		{
			return false; // Empty strings are not multi-line
		}

		// Check for any line break characters
		return value.Any(c => c == '\n' || c == '\r' || char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.LineSeparator);
	}
}
