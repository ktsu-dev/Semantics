// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Strings;

using System;

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
public sealed class IsMacroCaseAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for MACRO_CASE validation.
	/// </summary>
	/// <returns>A validation adapter for MACRO_CASE strings</returns>
	protected override ValidationAdapter CreateValidator() => new MacroCaseValidator();

	/// <summary>
	/// validation adapter for MACRO_CASE strings.
	/// </summary>
	private sealed class MacroCaseValidator : ValidationAdapter
	{
		private const string FailureMessage = "The value must be in MACRO_CASE format.";

		/// <summary>
		/// Validates that a string is in MACRO_CASE.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value) =>
			DelimitedCaseValidation.Validate(value, '_', '-', char.IsUpper, FailureMessage);
	}
}
