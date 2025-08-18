// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;

/// <summary>
/// Validates that a path exists on the filesystem
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class DoesExistAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for path existence validation.
	/// </summary>
	/// <returns>A validation adapter for path existence</returns>
	protected override ValidationAdapter CreateValidator() => new ExistenceValidator();

	/// <summary>
	/// validation adapter for path existence.
	/// </summary>
	private sealed class ExistenceValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a path exists on the filesystem.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		/// <remarks>
		/// This validation requires the path to actually exist on the filesystem as either a file or directory.
		/// The validation uses both <see cref="File.Exists(string)"/> and <see cref="Directory.Exists(string)"/>
		/// to check for existence.
		/// </remarks>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Failure("Path cannot be empty or null.");
			}

			bool pathExists = File.Exists(value) || Directory.Exists(value);
			return pathExists
				? ValidationResult.Success()
				: ValidationResult.Failure("The specified path does not exist.");
		}
	}
}
