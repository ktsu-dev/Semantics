// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using FluentValidation;

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
public sealed class IsUriAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for URI validation.
	/// </summary>
	/// <returns>A FluentValidation validator for URI strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new UriValidator();

	/// <summary>
	/// FluentValidation validator for URI strings.
	/// </summary>
	private sealed class UriValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the UriValidator class.
		/// </summary>
		public UriValidator()
		{
			RuleFor(value => value)
				.Must(BeValidUri)
				.WithMessage("The value must be a valid absolute URI.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is a valid URI.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is a valid URI, false otherwise</returns>
		private static bool BeValidUri(string value) => Uri.TryCreate(value, UriKind.Absolute, out _);
	}
}
