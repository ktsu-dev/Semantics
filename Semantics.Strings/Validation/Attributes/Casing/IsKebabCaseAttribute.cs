// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Strings;

using System;

/// <summary>
/// Validates that a string is in kebab-case (lowercase words separated by hyphens)
/// </summary>
/// <remarks>
/// Kebab-case uses lowercase letters with words separated by hyphens.
/// Examples: "kebab-case", "hello-world", "the-quick-brown-fox"
/// No spaces, underscores, or uppercase letters are allowed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsKebabCaseAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for kebab-case validation.
	/// </summary>
	/// <returns>A validation adapter for kebab-case strings</returns>
	protected override ValidationAdapter CreateValidator() => new KebabCaseValidator();

	/// <summary>
	/// validation adapter for kebab-case strings.
	/// </summary>
	private sealed class KebabCaseValidator : ValidationAdapter
	{
		private const string FailureMessage = "The value must be in kebab-case format.";

		/// <summary>
		/// Validates that a string is in kebab-case.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value) =>
			DelimitedCaseValidation.Validate(value, '-', '_', char.IsLower, FailureMessage);
	}
}
