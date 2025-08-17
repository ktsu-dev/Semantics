// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;

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
		/// Initializes a new instance of the ValidFileNameValidator class.
		/// </summary>
		public ValidFileNameValidator()
		{
			RuleFor(value => value)
				.Must(BeValidFileName)
				.WithMessage("The filename contains invalid characters.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a filename string contains only valid filename characters.
		/// </summary>
		/// <param name="value">The filename string to validate</param>
		/// <returns>True if the filename contains only valid characters, false otherwise</returns>
		private static bool BeValidFileName(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}

			// Use span-based search for invalid characters
			ReadOnlySpan<char> valueSpan = value.AsSpan();
			return valueSpan.IndexOfAny(InvalidFileNameChars) == -1;
		}
	}
}
