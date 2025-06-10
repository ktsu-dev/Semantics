// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;
using FluentValidation;

/// <summary>
/// Validates that a string is in PascalCase (no spaces, each word starts with uppercase)
/// </summary>
/// <remarks>
/// PascalCase concatenates words without spaces, capitalizing the first letter of each word.
/// Examples: "PascalCase", "HelloWorld", "TheQuickBrownFox"
/// No spaces, underscores, or hyphens are allowed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsPascalCaseAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for PascalCase validation.
	/// </summary>
	/// <returns>A FluentValidation validator for PascalCase strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new PascalCaseValidator();

	/// <summary>
	/// FluentValidation validator for PascalCase strings.
	/// </summary>
	private sealed class PascalCaseValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the PascalCaseValidator class.
		/// </summary>
		public PascalCaseValidator()
		{
			RuleFor(value => value)
				.Must(BeValidPascalCase)
				.WithMessage("The value must be in PascalCase format.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is in PascalCase.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is in PascalCase, false otherwise</returns>
		private static bool BeValidPascalCase(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}

			// Must start with uppercase letter
			if (!char.IsUpper(value[0]))
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
