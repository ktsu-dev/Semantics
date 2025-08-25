// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

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
public sealed class HasNonWhitespaceContentAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for non-whitespace content validation.
	/// </summary>
	/// <returns>A validation adapter for non-whitespace content</returns>
	protected override ValidationAdapter CreateValidator() => new NonWhitespaceContentValidator();

	/// <summary>
	/// validation adapter for non-whitespace content.
	/// </summary>
	private sealed class NonWhitespaceContentValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string contains non-whitespace content.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			bool hasNonWhitespaceContent = !string.IsNullOrWhiteSpace(value);
			return hasNonWhitespaceContent
				? ValidationResult.Success()
				: ValidationResult.Failure("The text must contain at least some non-whitespace content.");
		}
	}
}
