// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;

/// <summary>
/// Validates that a path string contains valid path characters using span semantics.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsValidPathAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for valid path validation.
	/// </summary>
	/// <returns>A validation adapter for valid path strings</returns>
	protected override ValidationAdapter CreateValidator() => new ValidPathValidator();

	/// <summary>
	/// validation adapter for valid path strings.
	/// </summary>
	private sealed class ValidPathValidator : ValidationAdapter
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
