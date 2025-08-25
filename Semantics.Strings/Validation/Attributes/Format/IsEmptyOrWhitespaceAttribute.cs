// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

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
public sealed class IsEmptyOrWhitespaceAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for empty or whitespace validation.
	/// </summary>
	/// <returns>A validation adapter for empty or whitespace strings</returns>
	protected override ValidationAdapter CreateValidator() => new EmptyOrWhitespaceValidator();

	/// <summary>
	/// validation adapter for empty or whitespace strings.
	/// </summary>
	private sealed class EmptyOrWhitespaceValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is empty or contains only whitespace characters.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			bool isEmptyOrWhitespace = string.IsNullOrWhiteSpace(value);
			return isEmptyOrWhitespace
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be empty or contain only whitespace characters.");
		}
	}
}
