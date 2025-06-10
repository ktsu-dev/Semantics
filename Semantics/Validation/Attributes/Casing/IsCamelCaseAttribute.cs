// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;
using FluentValidation;

/// <summary>
/// Validates that a string is in camelCase (first word lowercase, subsequent words start with uppercase)
/// </summary>
/// <remarks>
/// CamelCase concatenates words without spaces, with the first word in lowercase and subsequent words capitalized.
/// Examples: "camelCase", "helloWorld", "theQuickBrownFox"
/// No spaces, underscores, or hyphens are allowed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsCamelCaseAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for camelCase validation.
	/// </summary>
	/// <returns>A FluentValidation validator for camelCase strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new CamelCaseValidator();

	/// <summary>
	/// FluentValidation validator for camelCase strings.
	/// </summary>
	private sealed class CamelCaseValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the CamelCaseValidator class.
		/// </summary>
		public CamelCaseValidator()
		{
			RuleFor(value => value)
				.Must(BeValidCamelCase)
				.WithMessage("The value must be in camelCase format.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is in camelCase.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is in camelCase, false otherwise</returns>
		private static bool BeValidCamelCase(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}

			// Must start with lowercase letter
			if (!char.IsLower(value[0]))
			{
				return false;
			}

			// No spaces, underscores, hyphens, or other separators allowed
			if (value.Any(c => char.IsWhiteSpace(c) || c == '_' || c == '-'))
			{
				return false;
			}

			// All characters must be letters or digits
			return value.All(char.IsLetterOrDigit);
		}
	}
}
