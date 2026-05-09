// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

using System;
using System.IO;
using ktsu.Semantics.Strings;

/// <summary>
/// Validates that a string represents a valid file extension.
/// </summary>
/// <remarks>
/// This attribute enforces the following rules:
/// <list type="bullet">
/// <item><description>Extension must start with a period (.)</description></item>
/// <item><description>Extension must not contain invalid filename characters</description></item>
/// <item><description>Empty or null strings are considered valid</description></item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsFileExtensionAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for file extension validation.
	/// </summary>
	/// <returns>A validation adapter for file extensions</returns>
	protected override ValidationAdapter CreateValidator() => new FileExtensionValidator();

	/// <summary>
	/// Validation adapter for file extensions.
	/// </summary>
	private sealed class FileExtensionValidator : ValidationAdapter
	{
		private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

		/// <summary>
		/// Validates that a string represents a valid file extension.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			// Check that it starts with a period
#if NETSTANDARD2_0
			bool startsWithPeriod = value.StartsWith(".");
#else
			bool startsWithPeriod = value.StartsWith('.');
#endif
			if (!startsWithPeriod)
			{
				return ValidationResult.Failure("File extension must start with a period (.).");
			}

			// Check for invalid filename characters in the extension
#if NETSTANDARD2_0
			bool hasInvalidChars = value.IndexOfAny(InvalidFileNameChars) != -1;
#else
			ReadOnlySpan<char> valueSpan = value.AsSpan();
			bool hasInvalidChars = valueSpan.IndexOfAny(InvalidFileNameChars) != -1;
#endif
			return hasInvalidChars
				? ValidationResult.Failure("File extension contains invalid characters.")
				: ValidationResult.Success();
		}
	}
}
