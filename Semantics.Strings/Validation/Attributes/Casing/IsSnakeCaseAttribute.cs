// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Strings;

using System;

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
		protected override ValidationResult ValidateValue(string value) =>
			DelimitedCaseValidation.Validate(value, '_', '-', char.IsLower, FailureMessage);
	}
}
