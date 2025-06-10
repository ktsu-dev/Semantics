// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;
using FluentValidation;

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
public sealed class IsMacroCaseAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for MACRO_CASE validation.
	/// </summary>
	/// <returns>A FluentValidation validator for MACRO_CASE strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new MacroCaseValidator();

	/// <summary>
	/// FluentValidation validator for MACRO_CASE strings.
	/// </summary>
	private sealed class MacroCaseValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the MacroCaseValidator class.
		/// </summary>
		public MacroCaseValidator()
		{
			RuleFor(value => value)
				.Must(BeValidMacroCase)
				.WithMessage("The value must be in MACRO_CASE format.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is in MACRO_CASE.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is in MACRO_CASE, false otherwise</returns>
		private static bool BeValidMacroCase(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}

			// Cannot start or end with underscore
			if (value.StartsWith('_') || value.EndsWith('_'))
			{
				return false;
			}

			// Cannot have consecutive underscores
			if (value.Contains("__"))
			{
				return false;
			}

			// No spaces, hyphens, or other separators allowed (except underscores)
			if (value.Any(c => char.IsWhiteSpace(c) || c == '-'))
			{
				return false;
			}

			// All characters must be uppercase letters, digits, or underscores
			return value.All(c => char.IsUpper(c) || char.IsDigit(c) || c == '_');
		}
	}
}
