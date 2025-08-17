// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;

/// <summary>
/// Validates that a string is in UPPER CASE (all uppercase letters)
/// </summary>
/// <remarks>
/// Upper case uses all uppercase letters with spaces between words preserved.
/// Examples: "UPPER CASE", "HELLO WORLD", "THE QUICK BROWN FOX"
/// All alphabetic characters must be uppercase. Spaces, digits, and punctuation are allowed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsUpperCaseAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for uppercase validation.
	/// </summary>
	/// <returns>A validation adapter for uppercase strings</returns>
	protected override ValidationAdapter CreateValidator() => new UpperCaseValidator();

	/// <summary>
	/// validation adapter for uppercase strings.
	/// </summary>
	private sealed class UpperCaseValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is in upper case.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			// All letters must be uppercase
			bool isValid = value.All(c => !char.IsLetter(c) || char.IsUpper(c));
			return isValid
				? ValidationResult.Success()
				: ValidationResult.Failure("All alphabetic characters must be uppercase.");
		}
	}
}
