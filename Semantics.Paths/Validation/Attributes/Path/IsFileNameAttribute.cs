// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

using System;
using System.IO;
using System.Linq;
using ktsu.Semantics.Strings;

/// <summary>
/// Validates that a string represents a valid filename (no invalid filename characters, not a directory)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsFileNameAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for filename validation.
	/// </summary>
	/// <returns>A validation adapter for filenames</returns>
	protected override ValidationAdapter CreateValidator() => new FileNameValidator();

	/// <summary>
	/// validation adapter for filenames.
	/// </summary>
	private sealed class FileNameValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string represents a valid filename.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		/// <remarks>
		/// A valid filename must meet the following criteria:
		/// <list type="bullet">
		/// <item><description>Must not contain any characters from <see cref="Path.GetInvalidFileNameChars()"/></description></item>
		/// <item><description>Must not be an existing directory path</description></item>
		/// <item><description>Empty or null strings are considered valid</description></item>
		/// </list>
		/// </remarks>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			bool isValidFileName = !Directory.Exists(value) && !value.Intersect(Path.GetInvalidFileNameChars()).Any();
			return isValidFileName
				? ValidationResult.Success()
				: ValidationResult.Failure("The filename contains invalid characters or is an existing directory.");
		}
	}
}
