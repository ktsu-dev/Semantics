// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using FluentValidation;

/// <summary>
/// Validates that a string is either empty, null, or contains only whitespace characters
/// </summary>
/// <remarks>
/// This attribute validates that the string has no meaningful content.
/// Examples of valid strings: "", "   ", "\t\n\r", null
/// Examples of invalid strings: "Hello", " a ", "text"
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsEmptyOrWhitespaceAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for empty or whitespace validation.
	/// </summary>
	/// <returns>A FluentValidation validator for empty or whitespace strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new EmptyOrWhitespaceValidator();

	/// <summary>
	/// FluentValidation validator for empty or whitespace strings.
	/// </summary>
	private sealed class EmptyOrWhitespaceValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the EmptyOrWhitespaceValidator class.
		/// </summary>
		public EmptyOrWhitespaceValidator()
		{
			RuleFor(value => value)
				.Must(string.IsNullOrWhiteSpace)
				.WithMessage("The value must be empty or contain only whitespace characters.");
		}
	}
}
