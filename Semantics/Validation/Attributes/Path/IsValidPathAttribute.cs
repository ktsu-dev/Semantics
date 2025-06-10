// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;
using FluentValidation;

/// <summary>
/// Validates that a path string contains valid path characters using span semantics.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsValidPathAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for valid path validation.
	/// </summary>
	/// <returns>A FluentValidation validator for valid path strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new ValidPathValidator();

	/// <summary>
	/// FluentValidation validator for valid path strings.
	/// </summary>
	private sealed class ValidPathValidator : FluentValidationAdapter
	{
		private static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();

		/// <summary>
		/// Initializes a new instance of the ValidPathValidator class.
		/// </summary>
		public ValidPathValidator()
		{
			RuleFor(value => value)
				.Must(BeValidPath)
				.WithMessage("The path contains invalid characters.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a path string contains only valid path characters.
		/// </summary>
		/// <param name="value">The path string to validate</param>
		/// <returns>True if the path contains only valid characters, false otherwise</returns>
		private static bool BeValidPath(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}

			// Use span-based search for invalid characters
			ReadOnlySpan<char> valueSpan = value.AsSpan();
			return valueSpan.IndexOfAny(InvalidPathChars) == -1;
		}
	}
}
