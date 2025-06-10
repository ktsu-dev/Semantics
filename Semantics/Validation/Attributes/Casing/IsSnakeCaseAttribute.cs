// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;
using FluentValidation;

/// <summary>
/// Validates that a string is in snake_case (lowercase words separated by underscores)
/// </summary>
/// <remarks>
/// Snake_case uses lowercase letters with words separated by underscores.
/// Examples: "snake_case", "hello_world", "the_quick_brown_fox"
/// No spaces, hyphens, or uppercase letters are allowed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsSnakeCaseAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for snake_case validation.
	/// </summary>
	/// <returns>A FluentValidation validator for snake_case strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new SnakeCaseValidator();

	/// <summary>
	/// FluentValidation validator for snake_case strings.
	/// </summary>
	private sealed class SnakeCaseValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the SnakeCaseValidator class.
		/// </summary>
		public SnakeCaseValidator()
		{
			RuleFor(value => value)
				.Must(BeValidSnakeCase)
				.WithMessage("The value must be in snake_case format.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is in snake_case.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is in snake_case, false otherwise</returns>
		private static bool BeValidSnakeCase(string value)
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

			// All characters must be lowercase letters, digits, or underscores
			return value.All(c => char.IsLower(c) || char.IsDigit(c) || c == '_');
		}
	}
}
