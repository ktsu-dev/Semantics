// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;
using FluentValidation;

/// <summary>
/// Validates that a path is relative (not fully qualified), meaning it does not start from a root directory.
/// </summary>
/// <remarks>
/// A relative path is one that specifies a location relative to the current working directory or another specified directory.
/// Examples of relative paths:
/// <list type="bullet">
/// <item><description><c>file.txt</c> - file in current directory</description></item>
/// <item><description><c>folder/file.txt</c> - file in subdirectory</description></item>
/// <item><description><c>../file.txt</c> - file in parent directory</description></item>
/// <item><description><c>./folder/file.txt</c> - file in subdirectory (explicit current directory)</description></item>
/// </list>
/// This validation uses <see cref="Path.IsPathFullyQualified(string)"/> to determine if a path is absolute.
/// Empty or null strings are considered valid relative paths.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsRelativePathAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for relative path validation.
	/// </summary>
	/// <returns>A FluentValidation validator for relative paths</returns>
	protected override FluentValidationAdapter CreateValidator() => new RelativePathValidator();

	/// <summary>
	/// FluentValidation validator for relative paths.
	/// </summary>
	private sealed class RelativePathValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the RelativePathValidator class.
		/// </summary>
		public RelativePathValidator()
		{
			RuleFor(value => value)
				.Must(BeValidRelativePath)
				.WithMessage("The path must be relative (not fully qualified).")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a path is relative.
		/// </summary>
		/// <param name="value">The path to validate</param>
		/// <returns>True if the path is relative, false otherwise</returns>
		private static bool BeValidRelativePath(string value) =>
			string.IsNullOrEmpty(value) || !Path.IsPathFullyQualified(value);
	}
}
