// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string is a properly formatted DateTime.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.DateTime directly:
/// - DateTime.Parse() / DateTime.TryParse() for parsing
/// - DateTime.ToString() for string representation with format control
/// - Built-in comparison operators (&gt;, &lt;, ==, etc.)
/// - Rich API for date/time operations (AddDays, AddHours, etc.)
/// - Culture-aware formatting and parsing
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.DateTime directly instead of semantic string types. DateTime provides better type safety, performance, built-in comparison operations, and rich API for date/time operations.")]
public sealed class IsDateTimeAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for DateTime validation.
	/// </summary>
	/// <returns>A validation adapter for DateTime strings</returns>
	protected override ValidationAdapter CreateValidator() => new DateTimeValidator();

	/// <summary>
	/// validation adapter for DateTime strings.
	/// </summary>
	private sealed class DateTimeValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is a valid DateTime.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			bool isValid = DateTime.TryParse(value, out _);
			return isValid
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be a valid DateTime.");
		}
	}
}
