// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using FluentValidation;

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
public sealed class IsGuidAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for GUID validation.
	/// </summary>
	/// <returns>A FluentValidation validator for GUID strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new GuidValidator();

	/// <summary>
	/// FluentValidation validator for GUID strings.
	/// </summary>
	private sealed class GuidValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the GuidValidator class.
		/// </summary>
		public GuidValidator()
		{
			RuleFor(value => value)
				.Must(BeValidGuid)
				.WithMessage("The value must be a valid GUID/UUID.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is a valid GUID.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is a valid GUID, false otherwise</returns>
		private static bool BeValidGuid(string value) => Guid.TryParse(value, out _);
	}
}
