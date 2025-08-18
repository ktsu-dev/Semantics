// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;
using System.Linq;

/// <summary>
/// Validates that a string represents a valid path with no invalid path characters and a reasonable length.
/// </summary>
/// <remarks>
/// This attribute enforces the following rules:
/// <list type="bullet">
/// <item><description>Path length must not exceed 256 characters</description></item>
/// <item><description>Path must not contain any characters returned by <see cref="Path.GetInvalidPathChars()"/></description></item>
/// <item><description>Empty or null strings are considered valid</description></item>
/// </list>
/// The 256-character limit provides a reasonable balance between compatibility and practical usage,
/// while being more restrictive than the maximum path lengths supported by most file systems.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsPathAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for path validation.
	/// </summary>
	/// <returns>A validation adapter for path strings</returns>
	protected override ValidationAdapter CreateValidator() => new PathValidator();

	/// <summary>
	/// validation adapter for path strings.
	/// </summary>
	private sealed class PathValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string represents a valid path.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			// Check path length
			if (value.Length > 256)
			{
				return ValidationResult.Failure("Path length cannot exceed 256 characters.");
			}

			// Check for invalid characters
			char[] invalidChars = [.. Path.GetInvalidPathChars(), '<', '>', '|'];
			if (value.Intersect(invalidChars).Any())
			{
				return ValidationResult.Failure("Path contains invalid characters.");
			}

			return ValidationResult.Success();
		}
	}
}
