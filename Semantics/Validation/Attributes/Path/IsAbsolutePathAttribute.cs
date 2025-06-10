// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;
using FluentValidation;

/// <summary>
/// Validates that a path is absolute (fully qualified), meaning it specifies a complete path from the root of the file system.
/// </summary>
/// <remarks>
/// An absolute path provides the complete location of a file or directory from the root directory.
/// Examples of absolute paths:
/// <list type="bullet">
/// <item><description><c>C:\Windows\System32</c> - Windows absolute path</description></item>
/// <item><description><c>/usr/local/bin</c> - Unix/Linux absolute path</description></item>
/// <item><description><c>\\server\share\file.txt</c> - UNC path</description></item>
/// </list>
/// This validation uses <see cref="Path.IsPathFullyQualified(string)"/> with a directory separator appended
/// to handle edge cases where the path might be interpreted differently.
/// Empty or null strings are considered valid for flexibility in initialization scenarios.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsAbsolutePathAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for absolute path validation.
	/// </summary>
	/// <returns>A FluentValidation validator for absolute paths</returns>
	protected override FluentValidationAdapter CreateValidator() => new AbsolutePathValidator();

	/// <summary>
	/// FluentValidation validator for absolute paths.
	/// </summary>
	private sealed class AbsolutePathValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the AbsolutePathValidator class.
		/// </summary>
		public AbsolutePathValidator()
		{
			RuleFor(value => value)
				.Must(BeValidAbsolutePath)
				.WithMessage("The path must be absolute (fully qualified).")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a path is absolute.
		/// </summary>
		/// <param name="value">The path to validate</param>
		/// <returns>True if the path is absolute, false otherwise</returns>
		private static bool BeValidAbsolutePath(string value) =>
			string.IsNullOrEmpty(value) || Path.IsPathFullyQualified(value + Path.DirectorySeparatorChar);
	}
}
