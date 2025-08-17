// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string has proper Base64 format (valid characters and padding).
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsBase64Attribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for Base64 validation.
	/// </summary>
	/// <returns>A validation adapter for Base64 strings</returns>
	protected override ValidationAdapter CreateValidator() => new Base64Validator();

	/// <summary>
	/// Validation adapter for Base64 strings.
	/// </summary>
	private sealed class Base64Validator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is valid Base64.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			// Check length is multiple of 4 (Base64 requirement)
			if (value.Length % 4 != 0)
			{
				return ValidationResult.Failure("The value must be a valid Base64 string.");
			}

			// Check for valid Base64 characters and proper padding
			try
			{
				Convert.FromBase64String(value);
				return ValidationResult.Success();
			}
			catch (FormatException)
			{
				return ValidationResult.Failure("The value must be a valid Base64 string.");
			}
		}
	}
}
