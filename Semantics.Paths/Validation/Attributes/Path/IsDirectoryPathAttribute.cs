// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

using System;
using System.IO;
using ktsu.Semantics.Strings;

/// <summary>
/// Validates that a path represents a directory (not an existing file)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsDirectoryPathAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for directory path validation.
	/// </summary>
	/// <returns>A validation adapter for directory paths</returns>
	protected override ValidationAdapter CreateValidator() => new DirectoryPathValidator();

	/// <summary>
	/// validation adapter for directory paths.
	/// </summary>
	private sealed class DirectoryPathValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a path represents a directory by ensuring it's not an existing file.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		/// <remarks>
		/// <para>
		/// This validation passes if the path doesn't exist as a file, allowing for non-existent directories
		/// and existing directories. It only fails if the path exists and is specifically a file.
		/// </para>
		/// <para>
		/// The existence check applies only to fully qualified paths. A path that is not fully qualified
		/// names no particular location on disk until a caller supplies a base directory, so probing the
		/// filesystem for it would resolve it against the process's current working directory — making the
		/// same string valid in one process and invalid in another depending on what files happen to sit
		/// beside them. Such paths are validated by shape alone.
		/// </para>
		/// </remarks>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

#if NETSTANDARD2_0
			bool isFullyQualified = PathPolyfill.IsPathFullyQualified(value);
#else
			bool isFullyQualified = Path.IsPathFullyQualified(value);
#endif
			if (!isFullyQualified)
			{
				return ValidationResult.Success();
			}

			bool isNotFile = !File.Exists(value);
			return isNotFile
				? ValidationResult.Success()
				: ValidationResult.Failure("The path must not be an existing file.");
		}
	}
}
