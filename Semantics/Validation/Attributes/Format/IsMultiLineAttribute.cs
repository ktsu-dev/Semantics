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
public sealed class IsMultiLineAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for multi-line validation.
	/// </summary>
	/// <returns>A validation adapter for multi-line strings</returns>
	protected override ValidationAdapter CreateValidator() => new MultiLineValidator();

	/// <summary>
	/// validation adapter for multi-line strings.
	/// </summary>
	private sealed class MultiLineValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string contains line breaks.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Failure("Multi-line strings cannot be empty.");
			}

			// Check for any line break characters
			bool hasLineBreaks = value.Any(c => c == '\n' || c == '\r' || char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.LineSeparator);
			return hasLineBreaks
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must contain line breaks.");
		}
	}
}
