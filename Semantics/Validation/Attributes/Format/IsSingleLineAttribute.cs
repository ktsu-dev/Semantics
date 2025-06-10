// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;
using FluentValidation;

/// <summary>
/// Validates that a string contains no line breaks (single line)
/// </summary>
/// <remarks>
/// A single line string contains no carriage return (\r), line feed (\n), or other line separator characters.
/// Examples of valid single line strings: "Hello World", "This is a single line", "No line breaks here"
/// Examples of invalid strings: "Line 1\nLine 2", "Text with\r\nline breaks"
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsSingleLineAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for single line validation.
	/// </summary>
	/// <returns>A FluentValidation validator for single line strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new SingleLineValidator();

	/// <summary>
	/// FluentValidation validator for single line strings.
	/// </summary>
	private sealed class SingleLineValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the SingleLineValidator class.
		/// </summary>
		public SingleLineValidator()
		{
			RuleFor(value => value)
				.Must(BeValidSingleLine)
				.WithMessage("The value must not contain line breaks.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string contains no line breaks.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string contains no line breaks, false otherwise</returns>
		private static bool BeValidSingleLine(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}

			// Check for any line break characters
			return !value.Any(c => c == '\n' || c == '\r' || char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.LineSeparator);
		}
	}
}
