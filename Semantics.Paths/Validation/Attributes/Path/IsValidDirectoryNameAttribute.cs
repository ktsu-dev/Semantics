// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

using System;
using System.IO;
using ktsu.Semantics.Strings;

/// <summary>
/// Validates that a path string contains valid directory name characters (no path separators) using span semantics.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsValidDirectoryNameAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for valid directory name validation.
	/// </summary>
	/// <returns>A validation adapter for valid directory name strings</returns>
	protected override ValidationAdapter CreateValidator() => new ValidDirectoryNameValidator();

	/// <summary>
	/// validation adapter for valid directory name strings.
	/// </summary>
	private sealed class ValidDirectoryNameValidator : ValidationAdapter
	{
		private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

		/// <summary>
		/// Validates that a directory name string contains only valid characters and no path separators.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			// Check for invalid filename characters
#if NETSTANDARD2_0
			bool hasInvalidChars = value.IndexOfAny(InvalidFileNameChars) != -1;
#else
			ReadOnlySpan<char> valueSpan = value.AsSpan();
			bool hasInvalidChars = valueSpan.IndexOfAny(InvalidFileNameChars) != -1;
#endif
			if (hasInvalidChars)
			{
				return ValidationResult.Failure("The directory name contains invalid characters.");
			}

			// Check for path separators (directory names shouldn't contain path separators)
			if (value.Contains(Path.DirectorySeparatorChar) || value.Contains(Path.AltDirectorySeparatorChar))
			{
				return ValidationResult.Failure("The directory name contains path separators.");
			}

			return ValidationResult.Success();
		}
	}
}
