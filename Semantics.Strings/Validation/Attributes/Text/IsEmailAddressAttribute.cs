// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

using System;
using System.Linq;

/// <summary>
/// Validates that the string has basic email address format (contains @ with valid characters).
/// For full RFC compliance, use MailAddress.TryCreate() in your application code.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsEmailAddressAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for email address validation.
	/// </summary>
	/// <returns>A validation adapter for email addresses</returns>
	protected override ValidationAdapter CreateValidator() => new EmailValidator();

	/// <summary>
	/// validation adapter for email addresses.
	/// </summary>
	private sealed class EmailValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is a basic email address format.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			// Check length
			if (value.Length > 254)
			{
				return ValidationResult.Failure("Email address cannot exceed 254 characters.");
			}

			// Basic email validation: must contain @ with characters before and after
			int atIndex = value.IndexOf('@');
			if (atIndex <= 0 || atIndex >= value.Length - 1)
			{
				return ValidationResult.Failure("The value must be a valid email address.");
			}

			// Check for multiple @ symbols
			if (value.IndexOf('@', atIndex + 1) != -1)
			{
				return ValidationResult.Failure("The value must be a valid email address.");
			}

			// Basic character validation - no spaces or control characters
			if (value.Any(c => char.IsWhiteSpace(c) || char.IsControl(c)))
			{
				return ValidationResult.Failure("The value must be a valid email address.");
			}

			return ValidationResult.Success();
		}
	}
}
