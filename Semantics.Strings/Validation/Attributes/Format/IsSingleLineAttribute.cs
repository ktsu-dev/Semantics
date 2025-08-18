// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

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
public sealed class IsSingleLineAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for single line validation.
	/// </summary>
	/// <returns>A validation adapter for single line strings</returns>
	protected override ValidationAdapter CreateValidator() => new SingleLineValidator();

	/// <summary>
	/// Validation adapter for single line strings.
	/// </summary>
	private sealed class SingleLineValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string contains no line breaks.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			// Check for any line break characters
			bool isValid = !value.Any(c => c == '\n' || c == '\r' || char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.LineSeparator);

			return isValid
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must not contain line breaks.");
		}
	}
}
