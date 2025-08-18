// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

using System;

/// <summary>
/// Validates that the string is a properly formatted boolean value (true/false).
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.Boolean directly:
/// - Boolean.Parse() / Boolean.TryParse() for parsing
/// - Boolean.ToString() for string representation
/// - Built-in logical operators (&amp;&amp;, ||, !, etc.)
/// - Direct conditional evaluation without parsing
/// - Better performance for logical operations
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.Boolean directly instead of semantic string types. Boolean provides better type safety, performance, built-in logical operations, and direct conditional evaluation.")]
public sealed class IsBooleanAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for Boolean validation.
	/// </summary>
	/// <returns>A validation adapter for Boolean strings</returns>
	protected override ValidationAdapter CreateValidator() => new BooleanValidator();

	/// <summary>
	/// validation adapter for Boolean strings.
	/// </summary>
	private sealed class BooleanValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is a valid boolean value.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			bool isValid = bool.TryParse(value, out _);
			return isValid
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be a valid boolean (true/false).");
		}
	}
}
