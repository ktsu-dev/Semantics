// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using FluentValidation;

/// <summary>
/// Validates that the string is a properly formatted decimal number.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.Decimal directly:
/// - Decimal.Parse() / Decimal.TryParse() for parsing
/// - Decimal.ToString() for string representation with format control
/// - Built-in comparison operators (&gt;, &lt;, ==, etc.)
/// - Rich API for mathematical operations
/// - High precision for financial calculations
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.Decimal directly instead of semantic string types. Decimal provides better type safety, performance, built-in mathematical operations, and high precision for calculations.")]
public sealed class IsDecimalAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for Decimal validation.
	/// </summary>
	/// <returns>A FluentValidation validator for Decimal strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new DecimalValidator();

	/// <summary>
	/// FluentValidation validator for Decimal strings.
	/// </summary>
	private sealed class DecimalValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the DecimalValidator class.
		/// </summary>
		public DecimalValidator()
		{
			RuleFor(value => value)
				.Must(BeValidDecimal)
				.WithMessage("The value must be a valid decimal number.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is a valid decimal number.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is a valid decimal, false otherwise</returns>
		private static bool BeValidDecimal(string value) => decimal.TryParse(value, out _);
	}
}
