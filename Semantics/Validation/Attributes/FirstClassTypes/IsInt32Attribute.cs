// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using FluentValidation;

/// <summary>
/// Validates that the string is a properly formatted 32-bit integer.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.Int32 directly:
/// - Int32.Parse() / Int32.TryParse() for parsing
/// - Int32.ToString() for string representation with format control
/// - Built-in comparison operators (&gt;, &lt;, ==, etc.)
/// - Rich API for mathematical operations
/// - Better performance for numerical operations
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.Int32 directly instead of semantic string types. Int32 provides better type safety, performance, built-in mathematical operations, and efficient numerical computations.")]
public sealed class IsInt32Attribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for Int32 validation.
	/// </summary>
	/// <returns>A FluentValidation validator for Int32 strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new Int32Validator();

	/// <summary>
	/// FluentValidation validator for Int32 strings.
	/// </summary>
	private sealed class Int32Validator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the Int32Validator class.
		/// </summary>
		public Int32Validator()
		{
			RuleFor(value => value)
				.Must(BeValidInt32)
				.WithMessage("The value must be a valid 32-bit integer.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is a valid 32-bit integer.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is a valid Int32, false otherwise</returns>
		private static bool BeValidInt32(string value) => int.TryParse(value, out _);
	}
}
