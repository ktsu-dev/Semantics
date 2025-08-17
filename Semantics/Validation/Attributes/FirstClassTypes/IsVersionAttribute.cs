// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string is a properly formatted .NET Version.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.Version directly:
/// - Version.Parse() / Version.TryParse() for parsing
/// - Version.ToString() for string representation
/// - Built-in comparison operators (&gt;, &lt;, ==, etc.)
/// - Rich API for version operations (Major, Minor, Build, Revision)
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.Version directly instead of semantic string types. System.Version provides better type safety, performance, built-in comparison operations, and rich API for version operations.")]
public sealed class IsVersionAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for Version validation.
	/// </summary>
	/// <returns>A validation adapter for Version strings</returns>
	protected override ValidationAdapter CreateValidator() => new VersionValidator();

	/// <summary>
	/// validation adapter for Version strings.
	/// </summary>
	private sealed class VersionValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is a valid .NET Version.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			bool isValid = Version.TryParse(value, out _);
			return isValid
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be a valid .NET Version.");
		}
	}
}
