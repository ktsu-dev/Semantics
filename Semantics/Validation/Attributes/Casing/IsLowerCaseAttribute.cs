// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;
using FluentValidation;

/// <summary>
/// Validates that a string is in lower case (all lowercase letters)
/// </summary>
/// <remarks>
/// Lower case uses all lowercase letters with spaces between words preserved.
/// Examples: "lower case", "hello world", "the quick brown fox"
/// All alphabetic characters must be lowercase. Spaces, digits, and punctuation are allowed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsLowerCaseAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for lowercase validation.
	/// </summary>
	/// <returns>A FluentValidation validator for lowercase strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new LowerCaseValidator();

	/// <summary>
	/// FluentValidation validator for lowercase strings.
	/// </summary>
	private sealed class LowerCaseValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the LowerCaseValidator class.
		/// </summary>
		public LowerCaseValidator()
		{
			RuleFor(value => value)
				.Must(BeValidLowerCase)
				.WithMessage("All alphabetic characters must be lowercase.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is in lower case.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if all alphabetic characters are lowercase, false otherwise</returns>
		private static bool BeValidLowerCase(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}

			// All letters must be lowercase
			return value.All(c => !char.IsLetter(c) || char.IsLower(c));
		}
	}
}
