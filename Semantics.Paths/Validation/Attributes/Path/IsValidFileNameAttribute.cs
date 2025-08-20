// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

using System;
using System.IO;
using ktsu.Semantics.Strings;

/// <summary>
/// Validates that a path string contains valid filename characters using span semantics.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsValidFileNameAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for valid filename validation.
	/// </summary>
	/// <returns>A validation adapter for valid filename strings</returns>
	protected override ValidationAdapter CreateValidator() => new ValidFileNameValidator();

	/// <summary>
	/// validation adapter for valid filename strings.
	/// </summary>
	private sealed class ValidFileNameValidator : ValidationAdapter
	{
		private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

		/// <summary>
		/// Validates that a filename string contains only valid filename characters.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			// Use span-based search for invalid characters
			ReadOnlySpan<char> valueSpan = value.AsSpan();
			bool hasInvalidChars = valueSpan.IndexOfAny(InvalidFileNameChars) != -1;
			return hasInvalidChars
				? ValidationResult.Failure("The filename contains invalid characters.")
				: ValidationResult.Success();
		}
	}
}
