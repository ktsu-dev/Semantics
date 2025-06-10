// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using FluentValidation;

/// <summary>
/// Validates that the string is a properly formatted DateTime.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.DateTime directly:
/// - DateTime.Parse() / DateTime.TryParse() for parsing
/// - DateTime.ToString() for string representation with format control
/// - Built-in comparison operators (&gt;, &lt;, ==, etc.)
/// - Rich API for date/time operations (AddDays, AddHours, etc.)
/// - Culture-aware formatting and parsing
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.DateTime directly instead of semantic string types. DateTime provides better type safety, performance, built-in comparison operations, and rich API for date/time operations.")]
public sealed class IsDateTimeAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for DateTime validation.
	/// </summary>
	/// <returns>A FluentValidation validator for DateTime strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new DateTimeValidator();

	/// <summary>
	/// FluentValidation validator for DateTime strings.
	/// </summary>
	private sealed class DateTimeValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the DateTimeValidator class.
		/// </summary>
		public DateTimeValidator()
		{
			RuleFor(value => value)
				.Must(BeValidDateTime)
				.WithMessage("The value must be a valid DateTime.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is a valid DateTime.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is a valid DateTime, false otherwise</returns>
		private static bool BeValidDateTime(string value) => DateTime.TryParse(value, out _);
	}
}
