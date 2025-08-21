// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

using System;
using System.IO;
using ktsu.Semantics.Strings;

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
public sealed class IsRelativePathAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for relative path validation.
	/// </summary>
	/// <returns>A validation adapter for relative paths</returns>
	protected override ValidationAdapter CreateValidator() => new RelativePathValidator();

	/// <summary>
	/// validation adapter for relative paths.
	/// </summary>
	private sealed class RelativePathValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a path is relative.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

#if NETSTANDARD2_0
			bool isRelative = !PathPolyfill.IsPathFullyQualified(value);
#else
			bool isRelative = !Path.IsPathFullyQualified(value);
#endif
			return isRelative
					? ValidationResult.Success()
					: ValidationResult.Failure("The path must be relative (not fully qualified).");
		}
	}
}
