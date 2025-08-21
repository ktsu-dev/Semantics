// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

using System;
using System.IO;
using ktsu.Semantics.Strings;

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
/// This validation uses the system's path qualification logic with a directory separator appended
/// to handle edge cases where the path might be interpreted differently.
/// Empty or null strings are considered valid for flexibility in initialization scenarios.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsAbsolutePathAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for absolute path validation.
	/// </summary>
	/// <returns>A validation adapter for absolute paths</returns>
	protected override ValidationAdapter CreateValidator() => new AbsolutePathValidator();

	/// <summary>
	/// validation adapter for absolute paths.
	/// </summary>
	private sealed class AbsolutePathValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a path is absolute.
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
			bool isAbsolute = PathPolyfill.IsPathFullyQualified(value + Path.DirectorySeparatorChar);
#else
			bool isAbsolute = Path.IsPathFullyQualified(value + Path.DirectorySeparatorChar);
#endif
			return isAbsolute
					? ValidationResult.Success()
					: ValidationResult.Failure("The path must be absolute (fully qualified).");
		}
	}
}
