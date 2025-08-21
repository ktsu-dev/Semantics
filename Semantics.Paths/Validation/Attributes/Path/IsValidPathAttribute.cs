// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

using System;
using System.IO;
using ktsu.Semantics.Strings;

/// <summary>
/// Validates that a path string contains valid path characters using span semantics.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsValidPathAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for valid path validation.
	/// </summary>
	/// <returns>A validation adapter for valid path strings</returns>
	protected override ValidationAdapter CreateValidator() => new ValidPathValidator();

	/// <summary>
	/// validation adapter for valid path strings.
	/// </summary>
	private sealed class ValidPathValidator : ValidationAdapter
	{
		private static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();

		/// <summary>
		/// Validates that a path string contains only valid path characters.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			// Use span-based search for invalid characters
#if NETSTANDARD2_0
			bool hasInvalidChars = value.IndexOfAny(InvalidPathChars) != -1;
#else
			ReadOnlySpan<char> valueSpan = value.AsSpan();
			bool hasInvalidChars = valueSpan.IndexOfAny(InvalidPathChars) != -1;
#endif
			return hasInvalidChars
				? ValidationResult.Failure("The path contains invalid characters.")
				: ValidationResult.Success();
		}
	}
}
