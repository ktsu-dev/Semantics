// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Strings;

using System;
using System.Linq;

/// <summary>
/// The shared rules for the delimiter-separated casing conventions — kebab-case, snake_case and
/// MACRO_CASE — which differ only in the delimiter they use, the delimiter they forbid, and the
/// letter case they require.
/// </summary>
internal static class DelimitedCaseValidation
{
	/// <summary>
	/// Validates a delimiter-separated casing convention.
	/// </summary>
	/// <param name="value">The string value to validate.</param>
	/// <param name="delimiter">The delimiter the convention separates words with.</param>
	/// <param name="foreignDelimiter">The delimiter belonging to a different convention, which must not appear.</param>
	/// <param name="isExpectedCase">Predicate matching the letter case the convention requires.</param>
	/// <param name="failureMessage">The message reported for every way a value can fail.</param>
	/// <returns>A validation result indicating success or failure.</returns>
	internal static ValidationResult Validate(
		string value,
		char delimiter,
		char foreignDelimiter,
		Func<char, bool> isExpectedCase,
		string failureMessage)
	{
		if (string.IsNullOrEmpty(value))
		{
			return ValidationResult.Success();
		}

		// Cannot start or end with the delimiter.
		if (value.StartsWith(delimiter) || value.EndsWith(delimiter))
		{
			return ValidationResult.Failure(failureMessage);
		}

		// Cannot have consecutive delimiters.
		for (int i = 1; i < value.Length; i++)
		{
			if (value[i] == delimiter && value[i - 1] == delimiter)
			{
				return ValidationResult.Failure(failureMessage);
			}
		}

		// No whitespace, and no delimiter belonging to a different convention.
		if (value.Any(c => char.IsWhiteSpace(c) || c == foreignDelimiter))
		{
			return ValidationResult.Failure(failureMessage);
		}

		// Every character must be an expected-case letter, a digit, or the delimiter.
		return value.All(c => isExpectedCase(c) || char.IsDigit(c) || c == delimiter)
			? ValidationResult.Success()
			: ValidationResult.Failure(failureMessage);
	}
}
