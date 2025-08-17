// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Text.RegularExpressions;

/// <summary>
/// Validates that the string matches the specified regex pattern
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class RegexMatchAttribute(string pattern, RegexOptions options = RegexOptions.None) : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Gets the regex pattern that the string must match.
	/// </summary>
	public string Pattern => pattern;

	/// <summary>
	/// Gets the regex options used for matching.
	/// </summary>
	public RegexOptions Options => options;

	/// <summary>
	/// Creates the validation adapter for regex pattern matching.
	/// </summary>
	/// <returns>A validation adapter for regex pattern matching</returns>
	protected override ValidationAdapter CreateValidator() => new RegexValidator(pattern, options);

	/// <summary>
	/// validation adapter for regex pattern matching.
	/// </summary>
	/// <remarks>
	/// Initializes a new instance of the RegexValidator class.
	/// </remarks>
	/// <param name="pattern">The regex pattern to match</param>
	/// <param name="options">The regex options</param>
	private sealed class RegexValidator(string pattern, RegexOptions options) : ValidationAdapter
	{
		private readonly string _pattern = pattern;
		private readonly RegexOptions _options = options;

		/// <summary>
		/// Validates that a string matches the regex pattern.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			try
			{
				bool matches = Regex.IsMatch(value, _pattern, _options);
				return matches
					? ValidationResult.Success()
					: ValidationResult.Failure($"The value must match the pattern: {_pattern}");
			}
			catch (ArgumentException)
			{
				return ValidationResult.Failure($"Invalid regex pattern: {_pattern}");
			}
		}
	}
}
