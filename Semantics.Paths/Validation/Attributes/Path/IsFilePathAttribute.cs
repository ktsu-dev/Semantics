// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

using System;
using System.IO;
using ktsu.Semantics.Strings;

/// <summary>
/// Validates that a path represents a file (not an existing directory)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsFilePathAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for file path validation.
	/// </summary>
	/// <returns>A validation adapter for file paths</returns>
	protected override ValidationAdapter CreateValidator() => new FilePathValidator();

	/// <summary>
	/// validation adapter for file paths.
	/// </summary>
	private sealed class FilePathValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a path represents a file by ensuring it's not an existing directory.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		/// <remarks>
		/// This validation passes if the path doesn't exist as a directory, allowing for non-existent files
		/// and existing files. It only fails if the path exists and is specifically a directory.
		/// </remarks>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			bool isValidFilePath = !Directory.Exists(value);
			return isValidFilePath
				? ValidationResult.Success()
				: ValidationResult.Failure("The path must not be an existing directory.");
		}
	}
}
