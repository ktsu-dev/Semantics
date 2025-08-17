// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string is a properly formatted TimeSpan.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.TimeSpan directly:
/// - TimeSpan.Parse() / TimeSpan.TryParse() for parsing
/// - TimeSpan.ToString() for string representation with format control
/// - Built-in comparison operators (&gt;, &lt;, ==, etc.)
/// - Rich API for time operations (Add, Subtract, Multiply, etc.)
/// - Static factory methods (FromDays, FromHours, FromMinutes, etc.)
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.TimeSpan directly instead of semantic string types. TimeSpan provides better type safety, performance, built-in comparison operations, and rich API for time operations.")]
public sealed class IsTimeSpanAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for TimeSpan validation.
	/// </summary>
	/// <returns>A validation adapter for TimeSpan strings</returns>
	protected override ValidationAdapter CreateValidator() => new TimeSpanValidator();

	/// <summary>
	/// validation adapter for TimeSpan strings.
	/// </summary>
	private sealed class TimeSpanValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is a valid TimeSpan.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			bool isValid = TimeSpan.TryParse(value, out _);
			return isValid
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be a valid TimeSpan.");
		}
	}
}
