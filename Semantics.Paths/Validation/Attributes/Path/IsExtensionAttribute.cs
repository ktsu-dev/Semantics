// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

using System;
using ktsu.Semantics.Strings;

/// <summary>
/// Validates that a string represents a valid file extension (starts with a period)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsExtensionAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for file extension validation.
	/// </summary>
	/// <returns>A validation adapter for file extensions</returns>
	protected override ValidationAdapter CreateValidator() => new ExtensionValidator();

	/// <summary>
	/// validation adapter for file extensions.
	/// </summary>
	private sealed class ExtensionValidator : ValidationAdapter
	{
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

#if NETSTANDARD2_0
			bool isValidExtension = value.StartsWith(".");
#else
			bool isValidExtension = value.StartsWith('.');
#endif
			return isValidExtension
				? ValidationResult.Success()
				: ValidationResult.Failure("File extension must start with a period (.).");
		}
	}
}
