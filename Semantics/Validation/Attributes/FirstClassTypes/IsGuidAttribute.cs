// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string is a properly formatted GUID/UUID.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.Guid directly:
/// - Guid.Parse() / Guid.TryParse() for parsing
/// - Guid.ToString() for string representation
/// - Guid.NewGuid() for generating new GUIDs
/// - Built-in comparison and equality operations
/// - More efficient memory usage (16 bytes vs string overhead)
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.Guid directly instead of semantic string types. Guid provides better type safety, performance, built-in GUID operations, and efficient memory usage.")]
public sealed class IsGuidAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for GUID validation.
	/// </summary>
	/// <returns>A validation adapter for GUID strings</returns>
	protected override ValidationAdapter CreateValidator() => new GuidValidator();

	/// <summary>
	/// validation adapter for GUID strings.
	/// </summary>
	private sealed class GuidValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is a valid GUID.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			bool isValid = Guid.TryParse(value, out _);
			return isValid
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be a valid GUID/UUID.");
		}
	}
}
