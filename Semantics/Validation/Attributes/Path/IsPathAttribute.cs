// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;
using System.Linq;
using FluentValidation;

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
public sealed class IsPathAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for path validation.
	/// </summary>
	/// <returns>A FluentValidation validator for path strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new PathValidator();

	/// <summary>
	/// FluentValidation validator for path strings.
	/// </summary>
	private sealed class PathValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the PathValidator class.
		/// </summary>
		public PathValidator()
		{
			RuleFor(value => value)
				.MaximumLength(256)
				.WithMessage("Path length cannot exceed 256 characters.")
				.When(value => !string.IsNullOrEmpty(value));

			RuleFor(value => value)
				.Must(BeValidPath)
				.WithMessage("Path contains invalid characters.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string represents a valid path.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is a valid path, false otherwise</returns>
		private static bool BeValidPath(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}

			// Check for characters from GetInvalidPathChars() and additional problematic characters
			// In .NET Core+, GetInvalidPathChars() doesn't include all characters that can cause issues in paths
			char[] invalidChars = [.. Path.GetInvalidPathChars(), '<', '>', '|'];
			return !value.Intersect(invalidChars).Any();
		}
	}
}
