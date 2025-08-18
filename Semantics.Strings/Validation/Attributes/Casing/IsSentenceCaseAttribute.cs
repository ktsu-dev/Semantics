// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

using System;
using System.Linq;

/// <summary>
/// Validates that a string is in sentence case (first letter uppercase, rest lowercase)
/// </summary>
/// <remarks>
/// Sentence case capitalizes only the first letter of the first word, with the rest in lowercase.
/// Examples: "This is sentence case.", "Hello world", "The quick brown fox"
/// Proper nouns and other capitalization rules are not enforced - only the first letter rule.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsSentenceCaseAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for sentence case validation.
	/// </summary>
	/// <returns>A validation adapter for sentence case strings</returns>
	protected override ValidationAdapter CreateValidator() => new SentenceCaseValidator();

	/// <summary>
	/// validation adapter for sentence case strings.
	/// </summary>
	private sealed class SentenceCaseValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is in sentence case.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			// Find the first letter in the string
			char? firstLetter = value.FirstOrDefault(char.IsLetter);
			if (firstLetter.HasValue && !char.IsUpper(firstLetter.Value))
			{
				return ValidationResult.Failure("The value must be in sentence case format.");
			}

			// Check that all other letters after the first are lowercase
			bool foundFirstLetter = false;
			foreach (char c in value)
			{
				if (char.IsLetter(c))
				{
					if (!foundFirstLetter)
					{
						foundFirstLetter = true; // Skip the first letter
						continue;
					}

					if (char.IsUpper(c))
					{
						return ValidationResult.Failure("The value must be in sentence case format.");
					}
				}
			}

			return ValidationResult.Success();
		}
	}
}
