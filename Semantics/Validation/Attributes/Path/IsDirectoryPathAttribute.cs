// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;
using FluentValidation;

/// <summary>
/// Validates that a path represents a directory (not an existing file)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsDirectoryPathAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for directory path validation.
	/// </summary>
	/// <returns>A FluentValidation validator for directory paths</returns>
	protected override FluentValidationAdapter CreateValidator() => new DirectoryPathValidator();

	/// <summary>
	/// FluentValidation validator for directory paths.
	/// </summary>
	private sealed class DirectoryPathValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the DirectoryPathValidator class.
		/// </summary>
		public DirectoryPathValidator()
		{
			RuleFor(value => value)
				.Must(BeValidDirectoryPath)
				.WithMessage("The path must not be an existing file.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a path represents a directory by ensuring it's not an existing file.
		/// </summary>
		/// <param name="value">The path to validate</param>
		/// <returns>True if the path is not an existing file, false otherwise</returns>
		/// <remarks>
		/// This validation passes if the path doesn't exist as a file, allowing for non-existent directories
		/// and existing directories. It only fails if the path exists and is specifically a file.
		/// </remarks>
		private static bool BeValidDirectoryPath(string value) =>
			string.IsNullOrEmpty(value) || !File.Exists(value);
	}
}
