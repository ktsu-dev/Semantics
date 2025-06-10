// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Globalization;
using FluentValidation;

/// <summary>
/// Validates that a string is in title case (each word starts with an uppercase letter)
/// </summary>
/// <remarks>
/// Title case capitalizes the first letter of each word, with the rest of the letters in lowercase.
/// Examples: "This Is Title Case", "Hello World", "The Quick Brown Fox"
/// Whitespace and punctuation are preserved.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsTitleCaseAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for title case validation.
	/// </summary>
	/// <returns>A FluentValidation validator for title case strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new TitleCaseValidator();

	/// <summary>
	/// FluentValidation validator for title case strings.
	/// </summary>
	private sealed class TitleCaseValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the TitleCaseValidator class.
		/// </summary>
		public TitleCaseValidator()
		{
			RuleFor(value => value)
				.Must(BeValidTitleCase)
				.WithMessage("The value must be in title case format.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is in title case.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is in title case, false otherwise</returns>
		private static bool BeValidTitleCase(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}

			// Use TextInfo.ToTitleCase and compare with original
			string titleCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLowerInvariant());
			return string.Equals(value, titleCase, StringComparison.Ordinal);
		}
	}
}
