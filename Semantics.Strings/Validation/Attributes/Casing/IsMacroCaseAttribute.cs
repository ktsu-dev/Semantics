// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

using System;
using System.Linq;

/// <summary>
/// Validates that a string is in MACRO_CASE (uppercase words separated by underscores)
/// </summary>
/// <remarks>
/// MACRO_CASE uses uppercase letters with words separated by underscores.
/// Examples: "MACRO_CASE", "HELLO_WORLD", "THE_QUICK_BROWN_FOX"
/// No spaces, hyphens, or lowercase letters are allowed.
/// Also known as SCREAMING_SNAKE_CASE or CONSTANT_CASE.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsMacroCaseAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for MACRO_CASE validation.
	/// </summary>
	/// <returns>A validation adapter for MACRO_CASE strings</returns>
	protected override ValidationAdapter CreateValidator() => new MacroCaseValidator();

	/// <summary>
	/// validation adapter for MACRO_CASE strings.
	/// </summary>
	private sealed class MacroCaseValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is in MACRO_CASE.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			// Cannot start or end with underscore
			if (value.StartsWith('_') || value.EndsWith('_'))
			{
				return ValidationResult.Failure("The value must be in MACRO_CASE format.");
			}

			// Cannot have consecutive underscores
			if (value.Contains("__"))
			{
				return ValidationResult.Failure("The value must be in MACRO_CASE format.");
			}

			// No spaces, hyphens, or other separators allowed (except underscores)
			if (value.Any(c => char.IsWhiteSpace(c) || c == '-'))
			{
				return ValidationResult.Failure("The value must be in MACRO_CASE format.");
			}

			// All characters must be uppercase letters, digits, or underscores
			if (!value.All(c => char.IsUpper(c) || char.IsDigit(c) || c == '_'))
			{
				return ValidationResult.Failure("The value must be in MACRO_CASE format.");
			}

			return ValidationResult.Success();
		}
	}
}
