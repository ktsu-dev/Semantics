// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string is a properly formatted decimal number.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.Decimal directly:
/// - Decimal.Parse() / Decimal.TryParse() for parsing
/// - Decimal.ToString() for string representation with format control
/// - Built-in comparison operators (&gt;, &lt;, ==, etc.)
/// - Rich API for mathematical operations
/// - High precision for financial calculations
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.Decimal directly instead of semantic string types. Decimal provides better type safety, performance, built-in mathematical operations, and high precision for calculations.")]
public sealed class IsDecimalAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for Decimal validation.
	/// </summary>
	/// <returns>A validation adapter for Decimal strings</returns>
	protected override ValidationAdapter CreateValidator() => new DecimalValidator();

	/// <summary>
	/// validation adapter for Decimal strings.
	/// </summary>
	private sealed class DecimalValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is a valid decimal number.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			bool isValid = decimal.TryParse(value, out _);
			return isValid
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be a valid decimal number.");
		}
	}
}
