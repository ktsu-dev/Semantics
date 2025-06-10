// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;
using FluentValidation;

/// <summary>
/// Validates that a path exists on the filesystem
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class DoesExistAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for path existence validation.
	/// </summary>
	/// <returns>A FluentValidation validator for path existence</returns>
	protected override FluentValidationAdapter CreateValidator() => new ExistenceValidator();

	/// <summary>
	/// FluentValidation validator for path existence.
	/// </summary>
	private sealed class ExistenceValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the ExistenceValidator class.
		/// </summary>
		public ExistenceValidator()
		{
			RuleFor(value => value)
				.NotEmpty()
				.WithMessage("Path cannot be empty or null.")
				.Must(PathExists)
				.WithMessage("The specified path does not exist.");
		}

		/// <summary>
		/// Validates that a path exists on the filesystem.
		/// </summary>
		/// <param name="value">The path to validate</param>
		/// <returns>True if the path exists as either a file or directory, false otherwise</returns>
		/// <remarks>
		/// This validation requires the path to actually exist on the filesystem as either a file or directory.
		/// The validation uses both <see cref="File.Exists(string)"/> and <see cref="Directory.Exists(string)"/>
		/// to check for existence.
		/// </remarks>
		private static bool PathExists(string value) => !string.IsNullOrEmpty(value) && (File.Exists(value) || Directory.Exists(value));
	}
}
