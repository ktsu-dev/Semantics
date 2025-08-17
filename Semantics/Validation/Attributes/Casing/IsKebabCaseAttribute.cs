// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;

/// <summary>
/// Validates that a string is in kebab-case (lowercase words separated by hyphens)
/// </summary>
/// <remarks>
/// Kebab-case uses lowercase letters with words separated by hyphens.
/// Examples: "kebab-case", "hello-world", "the-quick-brown-fox"
/// No spaces, underscores, or uppercase letters are allowed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsKebabCaseAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for kebab-case validation.
	/// </summary>
	/// <returns>A validation adapter for kebab-case strings</returns>
	protected override ValidationAdapter CreateValidator() => new KebabCaseValidator();

	/// <summary>
	/// validation adapter for kebab-case strings.
	/// </summary>
	private sealed class KebabCaseValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is in kebab-case.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			// Cannot start or end with hyphen
			if (value.StartsWith('-') || value.EndsWith('-'))
			{
				return ValidationResult.Failure("The value must be in kebab-case format.");
			}

			// Cannot have consecutive hyphens
			if (value.Contains("--"))
			{
				return ValidationResult.Failure("The value must be in kebab-case format.");
			}

			// No spaces, underscores, or other separators allowed (except hyphens)
			if (value.Any(c => char.IsWhiteSpace(c) || c == '_'))
			{
				return ValidationResult.Failure("The value must be in kebab-case format.");
			}

			// All characters must be lowercase letters, digits, or hyphens
			if (!value.All(c => char.IsLower(c) || char.IsDigit(c) || c == '-'))
			{
				return ValidationResult.Failure("The value must be in kebab-case format.");
			}

			return ValidationResult.Success();
		}
	}
}
