// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

using System;
using System.IO;
using ktsu.Semantics.Strings;

/// <summary>
/// Validates that a string represents a valid path with no invalid path characters and a reasonable length.
/// </summary>
/// <remarks>
/// This attribute enforces the following rules:
/// <list type="bullet">
/// <item><description>Path length must not exceed 256 characters</description></item>
/// <item><description>Path must not contain any characters returned by <see cref="Path.GetInvalidPathChars()"/></description></item>
/// <item><description>Path must not contain reserved characters: &lt;, &gt;, |</description></item>
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
	/// Validation adapter for path strings using efficient span-based character validation.
	/// </summary>
	private sealed class PathValidator : ValidationAdapter
	{
		// Combine system invalid path chars with reserved characters (<, >, |)
		// These characters are technically returned by GetInvalidPathChars on Windows but not on Unix,
		// so we explicitly include them for cross-platform consistency.
		private static readonly char[] InvalidPathChars = BuildInvalidCharArray();

		private static char[] BuildInvalidCharArray()
		{
			char[] systemInvalid = Path.GetInvalidPathChars();
			char[] reservedChars = ['<', '>', '|'];

			// Combine arrays, using HashSet to deduplicate in case system already includes reserved chars
			System.Collections.Generic.HashSet<char> charSet = [.. systemInvalid, .. reservedChars];

			char[] result = new char[charSet.Count];
			charSet.CopyTo(result);
			return result;
		}

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

			// Use efficient span-based search for invalid characters
#if NETSTANDARD2_0
			bool hasInvalidChars = value.IndexOfAny(InvalidPathChars) != -1;
#else
			ReadOnlySpan<char> valueSpan = value.AsSpan();
			bool hasInvalidChars = valueSpan.IndexOfAny(InvalidPathChars) != -1;
#endif
			return hasInvalidChars
				? ValidationResult.Failure("Path contains invalid characters.")
				: ValidationResult.Success();
		}
	}
}
