// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using FluentValidation;

/// <summary>
/// Validates that the string has proper Base64 format (valid characters and padding).
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsBase64Attribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for Base64 validation.
	/// </summary>
	/// <returns>A FluentValidation validator for Base64 strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new Base64Validator();

	/// <summary>
	/// FluentValidation validator for Base64 strings.
	/// </summary>
	private sealed class Base64Validator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the Base64Validator class.
		/// </summary>
		public Base64Validator()
		{
			RuleFor(value => value)
				.Must(BeValidBase64)
				.WithMessage("The value must be a valid Base64 string.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is valid Base64.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is valid Base64, false otherwise</returns>
		private static bool BeValidBase64(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}

			// Check length is multiple of 4 (Base64 requirement)
			if (value.Length % 4 != 0)
			{
				return false;
			}

			// Check for valid Base64 characters and proper padding
			try
			{
				Convert.FromBase64String(value);
				return true;
			}
			catch (FormatException)
			{
				return false;
			}
		}
	}
}
