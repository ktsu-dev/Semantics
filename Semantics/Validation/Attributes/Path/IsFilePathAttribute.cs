// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;
using FluentValidation;

/// <summary>
/// Validates that a path represents a file (not an existing directory)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsFilePathAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for file path validation.
	/// </summary>
	/// <returns>A FluentValidation validator for file paths</returns>
	protected override FluentValidationAdapter CreateValidator() => new FilePathValidator();

	/// <summary>
	/// FluentValidation validator for file paths.
	/// </summary>
	private sealed class FilePathValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the FilePathValidator class.
		/// </summary>
		public FilePathValidator()
		{
			RuleFor(value => value)
				.Must(BeValidFilePath)
				.WithMessage("The path must not be an existing directory.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a path represents a file by ensuring it's not an existing directory.
		/// </summary>
		/// <param name="value">The path to validate</param>
		/// <returns>True if the path is not an existing directory, false otherwise</returns>
		/// <remarks>
		/// This validation passes if the path doesn't exist as a directory, allowing for non-existent files
		/// and existing files. It only fails if the path exists and is specifically a directory.
		/// </remarks>
		private static bool BeValidFilePath(string value) =>
			string.IsNullOrEmpty(value) || !Directory.Exists(value);
	}
}
