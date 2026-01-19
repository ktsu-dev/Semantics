// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

using System;
using System.IO;
using ktsu.Semantics.Strings;

/// <summary>
/// Validates that a string represents a valid directory name (a single directory component, not a path).
/// </summary>
/// <remarks>
/// This attribute enforces the following rules:
/// <list type="bullet">
/// <item><description>Directory name must not contain invalid filename characters (which includes path separators)</description></item>
/// <item><description>Empty or null strings are considered valid</description></item>
/// </list>
/// Note: <see cref="Path.GetInvalidFileNameChars"/> includes both <see cref="Path.DirectorySeparatorChar"/>
/// and <see cref="Path.AltDirectorySeparatorChar"/>, so path separators are inherently rejected.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsDirectoryNameAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for directory name validation.
	/// </summary>
	/// <returns>A validation adapter for directory name strings</returns>
	protected override ValidationAdapter CreateValidator() => new DirectoryNameValidator();

	/// <summary>
	/// Validation adapter for directory name strings.
	/// </summary>
	private sealed class DirectoryNameValidator : ValidationAdapter
	{
		private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

		/// <summary>
		/// Validates that a directory name string contains only valid characters.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		/// <remarks>
		/// Since <see cref="Path.GetInvalidFileNameChars"/> includes path separators,
		/// this validation inherently prevents directory paths from being passed as directory names.
		/// </remarks>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			// Check for invalid filename characters (includes path separators)
#if NETSTANDARD2_0
			bool hasInvalidChars = value.IndexOfAny(InvalidFileNameChars) != -1;
#else
			ReadOnlySpan<char> valueSpan = value.AsSpan();
			bool hasInvalidChars = valueSpan.IndexOfAny(InvalidFileNameChars) != -1;
#endif
			return hasInvalidChars
				? ValidationResult.Failure("The directory name contains invalid characters.")
				: ValidationResult.Success();
		}
	}
}
