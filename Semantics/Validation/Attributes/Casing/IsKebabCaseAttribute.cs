// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;
using FluentValidation;

/// <summary>
/// Validates that a string is in kebab-case (lowercase words separated by hyphens)
/// </summary>
/// <remarks>
/// Kebab-case uses lowercase letters with words separated by hyphens.
/// Examples: "kebab-case", "hello-world", "the-quick-brown-fox"
/// No spaces, underscores, or uppercase letters are allowed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsKebabCaseAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for kebab-case validation.
	/// </summary>
	/// <returns>A FluentValidation validator for kebab-case strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new KebabCaseValidator();

	/// <summary>
	/// FluentValidation validator for kebab-case strings.
	/// </summary>
	private sealed class KebabCaseValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the KebabCaseValidator class.
		/// </summary>
		public KebabCaseValidator()
		{
			RuleFor(value => value)
				.Must(BeValidKebabCase)
				.WithMessage("The value must be in kebab-case format.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is in kebab-case.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is in kebab-case, false otherwise</returns>
		private static bool BeValidKebabCase(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}

			// Cannot start or end with hyphen
			if (value.StartsWith('-') || value.EndsWith('-'))
			{
				return false;
			}

			// Cannot have consecutive hyphens
			if (value.Contains("--"))
			{
				return false;
			}

			// No spaces, underscores, or other separators allowed (except hyphens)
			if (value.Any(c => char.IsWhiteSpace(c) || c == '_'))
			{
				return false;
			}

			// All characters must be lowercase letters, digits, or hyphens
			return value.All(c => char.IsLower(c) || char.IsDigit(c) || c == '-');
		}
	}
}
