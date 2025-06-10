// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using FluentValidation;

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
public sealed class IsVersionAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for Version validation.
	/// </summary>
	/// <returns>A FluentValidation validator for Version strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new VersionValidator();

	/// <summary>
	/// FluentValidation validator for Version strings.
	/// </summary>
	private sealed class VersionValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the VersionValidator class.
		/// </summary>
		public VersionValidator()
		{
			RuleFor(value => value)
				.Must(BeValidVersion)
				.WithMessage("The value must be a valid .NET Version.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is a valid .NET Version.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is a valid Version, false otherwise</returns>
		private static bool BeValidVersion(string value) => Version.TryParse(value, out _);
	}
}
