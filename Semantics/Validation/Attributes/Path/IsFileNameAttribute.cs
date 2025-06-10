// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;
using System.Linq;
using FluentValidation;

/// <summary>
/// Validates that a string represents a valid filename (no invalid filename characters, not a directory)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsFileNameAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for filename validation.
	/// </summary>
	/// <returns>A FluentValidation validator for filenames</returns>
	protected override FluentValidationAdapter CreateValidator() => new FileNameValidator();

	/// <summary>
	/// FluentValidation validator for filenames.
	/// </summary>
	private sealed class FileNameValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the FileNameValidator class.
		/// </summary>
		public FileNameValidator()
		{
			RuleFor(value => value)
				.Must(BeValidFileName)
				.WithMessage("The filename contains invalid characters or is an existing directory.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string represents a valid filename.
		/// </summary>
		/// <param name="value">The filename to validate</param>
		/// <returns>True if the filename is valid, false otherwise</returns>
		/// <remarks>
		/// A valid filename must meet the following criteria:
		/// <list type="bullet">
		/// <item><description>Must not contain any characters from <see cref="Path.GetInvalidFileNameChars()"/></description></item>
		/// <item><description>Must not be an existing directory path</description></item>
		/// <item><description>Empty or null strings are considered valid</description></item>
		/// </list>
		/// </remarks>
		private static bool BeValidFileName(string value) =>
			string.IsNullOrEmpty(value) || (!Directory.Exists(value) && !value.Intersect(Path.GetInvalidFileNameChars()).Any());
	}
}
