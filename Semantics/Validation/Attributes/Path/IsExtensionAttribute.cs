// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using FluentValidation;

/// <summary>
/// Validates that a string represents a valid file extension (starts with a period)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsExtensionAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for file extension validation.
	/// </summary>
	/// <returns>A FluentValidation validator for file extensions</returns>
	protected override FluentValidationAdapter CreateValidator() => new ExtensionValidator();

	/// <summary>
	/// FluentValidation validator for file extensions.
	/// </summary>
	private sealed class ExtensionValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the ExtensionValidator class.
		/// </summary>
		public ExtensionValidator()
		{
			RuleFor(value => value)
				.Must(BeValidExtension)
				.WithMessage("File extension must start with a period (.).")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string represents a valid file extension.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is a valid file extension, false otherwise</returns>
		private static bool BeValidExtension(string value) => string.IsNullOrEmpty(value) || value.StartsWith('.');
	}
}
