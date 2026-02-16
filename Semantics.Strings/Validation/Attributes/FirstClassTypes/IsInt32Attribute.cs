// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

using System;

/// <summary>
/// Validates that the string is a properly formatted 32-bit integer.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.Int32 directly:
/// - Int32.Parse() / Int32.TryParse() for parsing
/// - Int32.ToString() for string representation with format control
/// - Built-in comparison operators (&gt;, &lt;, ==, etc.)
/// - Rich API for mathematical operations
/// - Better performance for numerical operations
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.Int32 directly instead of semantic string types. Int32 provides better type safety, performance, built-in mathematical operations, and efficient numerical computations.")]
public sealed class IsInt32Attribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for Int32 validation.
	/// </summary>
	/// <returns>A validation adapter for Int32 strings</returns>
	protected override ValidationAdapter CreateValidator() => new Int32Validator();

	/// <summary>
	/// validation adapter for Int32 strings.
	/// </summary>
	private sealed class Int32Validator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is a valid 32-bit integer.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			bool isValid = int.TryParse(value, out _);
			return isValid
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be a valid 32-bit integer.");
		}
	}
}
