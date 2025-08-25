// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

using System;
using System.Linq;

/// <summary>
/// Validates that a string is in camelCase (first word lowercase, subsequent words start with uppercase)
/// </summary>
/// <remarks>
/// CamelCase concatenates words without spaces, with the first word in lowercase and subsequent words capitalized.
/// Examples: "camelCase", "helloWorld", "theQuickBrownFox"
/// No spaces, underscores, or hyphens are allowed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsCamelCaseAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for camelCase validation.
	/// </summary>
	/// <returns>A validation adapter for camelCase strings</returns>
	protected override ValidationAdapter CreateValidator() => new CamelCaseValidator();

	/// <summary>
	/// validation adapter for camelCase strings.
	/// </summary>
	private sealed class CamelCaseValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is in camelCase.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			// Must start with lowercase letter
			if (!char.IsLower(value[0]))
			{
				return ValidationResult.Failure("The value must be in camelCase format.");
			}

			// No spaces, underscores, hyphens, or other separators allowed
			if (value.Any(c => char.IsWhiteSpace(c) || c == '_' || c == '-'))
			{
				return ValidationResult.Failure("The value must be in camelCase format.");
			}

			// All characters must be letters or digits
			if (!value.All(char.IsLetterOrDigit))
			{
				return ValidationResult.Failure("The value must be in camelCase format.");
			}

			return ValidationResult.Success();
		}
	}
}
