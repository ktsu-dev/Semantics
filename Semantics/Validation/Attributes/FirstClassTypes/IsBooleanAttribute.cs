// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using FluentValidation;

/// <summary>
/// Validates that the string is a properly formatted boolean value (true/false).
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.Boolean directly:
/// - Boolean.Parse() / Boolean.TryParse() for parsing
/// - Boolean.ToString() for string representation
/// - Built-in logical operators (&amp;&amp;, ||, !, etc.)
/// - Direct conditional evaluation without parsing
/// - Better performance for logical operations
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.Boolean directly instead of semantic string types. Boolean provides better type safety, performance, built-in logical operations, and direct conditional evaluation.")]
public sealed class IsBooleanAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for Boolean validation.
	/// </summary>
	/// <returns>A FluentValidation validator for Boolean strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new BooleanValidator();

	/// <summary>
	/// FluentValidation validator for Boolean strings.
	/// </summary>
	private sealed class BooleanValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the BooleanValidator class.
		/// </summary>
		public BooleanValidator()
		{
			RuleFor(value => value)
				.Must(BeValidBoolean)
				.WithMessage("The value must be a valid boolean (true/false).")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is a valid boolean value.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is a valid boolean, false otherwise</returns>
		private static bool BeValidBoolean(string value) => bool.TryParse(value, out _);
	}
}
