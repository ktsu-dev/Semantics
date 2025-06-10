// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;
using FluentValidation;

/// <summary>
/// Validates that a string contains line breaks (multiple lines)
/// </summary>
/// <remarks>
/// A multi-line string contains at least one carriage return (\r), line feed (\n), or other line separator character.
/// Examples of valid multi-line strings: "Line 1\nLine 2", "Text with\r\nline breaks", "Multi\nLine\nText"
/// Examples of invalid strings: "Hello World", "This is a single line", "No line breaks here"
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsMultiLineAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for multi-line validation.
	/// </summary>
	/// <returns>A FluentValidation validator for multi-line strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new MultiLineValidator();

	/// <summary>
	/// FluentValidation validator for multi-line strings.
	/// </summary>
	private sealed class MultiLineValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the MultiLineValidator class.
		/// </summary>
		public MultiLineValidator()
		{
			RuleFor(value => value)
				.NotEmpty()
				.WithMessage("Multi-line strings cannot be empty.")
				.Must(BeValidMultiLine)
				.WithMessage("The value must contain line breaks.");
		}

		/// <summary>
		/// Validates that a string contains line breaks.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string contains line breaks, false otherwise</returns>
		private static bool BeValidMultiLine(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return false; // Empty strings are not multi-line
			}

			// Check for any line break characters
			return value.Any(c => c == '\n' || c == '\r' || char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.LineSeparator);
		}
	}
}
