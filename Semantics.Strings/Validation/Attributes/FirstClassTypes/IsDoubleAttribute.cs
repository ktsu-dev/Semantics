// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

using System;

/// <summary>
/// Validates that the string is a properly formatted double-precision floating-point number.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.Double directly:
/// - Double.Parse() / Double.TryParse() for parsing
/// - Double.ToString() for string representation with format control
/// - Built-in comparison operators (&gt;, &lt;, ==, etc.)
/// - Rich API for mathematical operations
/// - Support for special values (NaN, Infinity, etc.)
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.Double directly instead of semantic string types. Double provides better type safety, performance, built-in mathematical operations, and support for special floating-point values.")]
public sealed class IsDoubleAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for Double validation.
	/// </summary>
	/// <returns>A validation adapter for Double strings</returns>
	protected override ValidationAdapter CreateValidator() => new DoubleValidator();

	/// <summary>
	/// validation adapter for Double strings.
	/// </summary>
	private sealed class DoubleValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is a valid double-precision floating-point number.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			bool isValid = double.TryParse(value, out _);
			return isValid
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be a valid double-precision floating-point number.");
		}
	}
}
