// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using FluentValidation;

/// <summary>
/// Validates that the string is a properly formatted TimeSpan.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.TimeSpan directly:
/// - TimeSpan.Parse() / TimeSpan.TryParse() for parsing
/// - TimeSpan.ToString() for string representation with format control
/// - Built-in comparison operators (&gt;, &lt;, ==, etc.)
/// - Rich API for time operations (Add, Subtract, Multiply, etc.)
/// - Static factory methods (FromDays, FromHours, FromMinutes, etc.)
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.TimeSpan directly instead of semantic string types. TimeSpan provides better type safety, performance, built-in comparison operations, and rich API for time operations.")]
public sealed class IsTimeSpanAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for TimeSpan validation.
	/// </summary>
	/// <returns>A FluentValidation validator for TimeSpan strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new TimeSpanValidator();

	/// <summary>
	/// FluentValidation validator for TimeSpan strings.
	/// </summary>
	private sealed class TimeSpanValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the TimeSpanValidator class.
		/// </summary>
		public TimeSpanValidator()
		{
			RuleFor(value => value)
				.Must(BeValidTimeSpan)
				.WithMessage("The value must be a valid TimeSpan.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is a valid TimeSpan.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is a valid TimeSpan, false otherwise</returns>
		private static bool BeValidTimeSpan(string value) => TimeSpan.TryParse(value, out _);
	}
}
