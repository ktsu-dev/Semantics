// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;
using FluentValidation;

/// <summary>
/// Validates that a string is in sentence case (first letter uppercase, rest lowercase)
/// </summary>
/// <remarks>
/// Sentence case capitalizes only the first letter of the first word, with the rest in lowercase.
/// Examples: "This is sentence case.", "Hello world", "The quick brown fox"
/// Proper nouns and other capitalization rules are not enforced - only the first letter rule.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsSentenceCaseAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for sentence case validation.
	/// </summary>
	/// <returns>A FluentValidation validator for sentence case strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new SentenceCaseValidator();

	/// <summary>
	/// FluentValidation validator for sentence case strings.
	/// </summary>
	private sealed class SentenceCaseValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the SentenceCaseValidator class.
		/// </summary>
		public SentenceCaseValidator()
		{
			RuleFor(value => value)
				.Must(BeValidSentenceCase)
				.WithMessage("The value must be in sentence case format.")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is in sentence case.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is in sentence case, false otherwise</returns>
		private static bool BeValidSentenceCase(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}

			// Find the first letter in the string
			char? firstLetter = value.FirstOrDefault(char.IsLetter);
			if (firstLetter.HasValue && !char.IsUpper(firstLetter.Value))
			{
				return false;
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
						return false; // Found uppercase letter after the first
					}
				}
			}

			return true;
		}
	}
}
