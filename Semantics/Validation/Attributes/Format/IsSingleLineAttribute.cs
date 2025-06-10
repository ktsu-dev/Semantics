// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;

/// <summary>
/// Validates that a string contains no line breaks (single line)
/// </summary>
/// <remarks>
/// A single line string contains no carriage return (\r), line feed (\n), or other line separator characters.
/// Examples of valid single line strings: "Hello World", "This is a single line", "No line breaks here"
/// Examples of invalid strings: "Line 1\nLine 2", "Text with\r\nline breaks"
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsSingleLineAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string contains no line breaks.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string contains no line breaks; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		if (string.IsNullOrEmpty(value))
		{
			return true;
		}

		// Check for any line break characters
		return !value.Any(c => c == '\n' || c == '\r' || char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.LineSeparator);
	}
}
