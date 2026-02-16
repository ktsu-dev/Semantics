// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

using System;

/// <summary>
/// Validates that the string is a properly formatted URI.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.Uri directly:
/// - Uri.TryCreate() for parsing and validation
/// - Uri.ToString() for string representation
/// - Built-in properties for components (Scheme, Host, Port, Path, Query, etc.)
/// - Rich API for URI operations and manipulation
/// - Automatic URL encoding/decoding support
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.Uri directly instead of semantic string types. Uri provides better type safety, performance, built-in component access, and rich API for URI operations.")]
public sealed class IsUriAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for URI validation.
	/// </summary>
	/// <returns>A validation adapter for URI strings</returns>
	protected override ValidationAdapter CreateValidator() => new UriValidator();

	/// <summary>
	/// validation adapter for URI strings.
	/// </summary>
	private sealed class UriValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is a valid URI.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			bool isValid = Uri.TryCreate(value, UriKind.Absolute, out _);
			return isValid
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be a valid absolute URI.");
		}
	}
}
