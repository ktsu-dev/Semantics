// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using FluentValidation;

/// <summary>
/// Validates that a string contains at least some non-whitespace content
/// </summary>
/// <remarks>
/// This attribute validates that the string has meaningful content beyond whitespace.
/// Examples of valid strings: "Hello", " a ", "text", "a\n"
/// Examples of invalid strings: "", "   ", "\t\n\r", null
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class HasNonWhitespaceContentAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for non-whitespace content validation.
	/// </summary>
	/// <returns>A FluentValidation validator for non-whitespace content</returns>
	protected override FluentValidationAdapter CreateValidator() => new NonWhitespaceContentValidator();

	/// <summary>
	/// FluentValidation validator for non-whitespace content.
	/// </summary>
	private sealed class NonWhitespaceContentValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the NonWhitespaceContentValidator class.
		/// </summary>
		public NonWhitespaceContentValidator()
		{
			RuleFor(value => value)
				.Must(HaveNonWhitespaceContent)
				.WithMessage("The text must contain at least some non-whitespace content.");
		}

		/// <summary>
		/// Validates that a string contains non-whitespace content.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string contains non-whitespace content, false otherwise</returns>
		private static bool HaveNonWhitespaceContent(string value) =>
			!string.IsNullOrWhiteSpace(value);
	}
}
