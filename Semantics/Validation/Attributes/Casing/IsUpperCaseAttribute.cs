// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;
using FluentValidation;

/// <summary>
/// Validates that a string is in UPPER CASE (all uppercase letters)
/// </summary>
/// <remarks>
/// Upper case uses all uppercase letters with spaces between words preserved.
/// Examples: "UPPER CASE", "HELLO WORLD", "THE QUICK BROWN FOX"
/// All alphabetic characters must be uppercase. Spaces, digits, and punctuation are allowed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsUpperCaseAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for uppercase validation.
	/// </summary>
	/// <returns>A FluentValidation validator for uppercase strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new UpperCaseValidator();

	/// <summary>
	/// FluentValidation validator for uppercase strings.
	/// </summary>
	private sealed class UpperCaseValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the UpperCaseValidator class.
		/// </summary>
		public UpperCaseValidator()
		{
			RuleFor(value => value)
				.Must(BeValidUpperCase)
				.WithMessage("All alphabetic characters must be uppercase.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is in upper case.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if all alphabetic characters are uppercase, false otherwise</returns>
		private static bool BeValidUpperCase(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}

			// All letters must be uppercase
			return value.All(c => !char.IsLetter(c) || char.IsUpper(c));
		}
	}
}
