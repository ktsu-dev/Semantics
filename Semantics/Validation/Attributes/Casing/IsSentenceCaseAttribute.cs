// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

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
public sealed class IsSentenceCaseAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is in sentence case.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is in sentence case; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
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
