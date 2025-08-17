// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;

/// <summary>
/// Validates that a string is in lower case (all lowercase letters)
/// </summary>
/// <remarks>
/// Lower case uses all lowercase letters with spaces between words preserved.
/// Examples: "lower case", "hello world", "the quick brown fox"
/// All alphabetic characters must be lowercase. Spaces, digits, and punctuation are allowed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsLowerCaseAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for lowercase validation.
	/// </summary>
	/// <returns>A validation adapter for lowercase strings</returns>
	protected override ValidationAdapter CreateValidator() => new LowerCaseValidator();

	/// <summary>
	/// Validation adapter for lowercase strings.
	/// </summary>
	private sealed class LowerCaseValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is in lower case.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			// All letters must be lowercase
			bool isValid = value.All(c => !char.IsLetter(c) || char.IsLower(c));

			return isValid
				? ValidationResult.Success()
				: ValidationResult.Failure("All alphabetic characters must be lowercase.");
		}
	}
}
