// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Globalization;

/// <summary>
/// Validates that a string is in title case (each word starts with an uppercase letter)
/// </summary>
/// <remarks>
/// Title case capitalizes the first letter of each word, with the rest of the letters in lowercase.
/// Examples: "This Is Title Case", "Hello World", "The Quick Brown Fox"
/// Whitespace and punctuation are preserved.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsTitleCaseAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for title case validation.
	/// </summary>
	/// <returns>A validation adapter for title case strings</returns>
	protected override ValidationAdapter CreateValidator() => new TitleCaseValidator();

	/// <summary>
	/// validation adapter for title case strings.
	/// </summary>
	private sealed class TitleCaseValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is in title case.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			// Use TextInfo.ToTitleCase and compare with original
			string titleCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLowerInvariant());
			bool isValid = string.Equals(value, titleCase, StringComparison.Ordinal);
			return isValid
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be in title case format.");
		}
	}
}
