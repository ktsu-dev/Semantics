// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Strings;

using System;
using System.Linq;

/// <summary>
/// Validates that a string is in snake_case (lowercase words separated by underscores)
/// </summary>
/// <remarks>
/// Snake_case uses lowercase letters with words separated by underscores.
/// Examples: "snake_case", "hello_world", "the_quick_brown_fox"
/// No spaces, hyphens, or uppercase letters are allowed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsSnakeCaseAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for snake_case validation.
	/// </summary>
	/// <returns>A validation adapter for snake_case strings</returns>
	protected override ValidationAdapter CreateValidator() => new SnakeCaseValidator();

	/// <summary>
	/// validation adapter for snake_case strings.
	/// </summary>
	private sealed class SnakeCaseValidator : ValidationAdapter
	{
		private const string FailureMessage = "The value must be in snake_case format.";

		/// <summary>
		/// Validates that a string is in snake_case.
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
				return ValidationResult.Failure(FailureMessage);
			}

			// Cannot have consecutive underscores
			if (value.Contains("__"))
			{
				return ValidationResult.Failure(FailureMessage);
			}

			// No spaces, hyphens, or other separators allowed (except underscores)
			if (value.Any(c => char.IsWhiteSpace(c) || c == '-'))
			{
				return ValidationResult.Failure(FailureMessage);
			}

			// All characters must be lowercase letters, digits, or underscores
			if (!value.All(c => char.IsLower(c) || char.IsDigit(c) || c == '_'))
			{
				return ValidationResult.Failure(FailureMessage);
			}

			return ValidationResult.Success();
		}
	}
}
