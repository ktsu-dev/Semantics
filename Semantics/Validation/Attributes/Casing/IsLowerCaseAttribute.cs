// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;

/// <summary>
/// Validates that a string is in lower case (all lowercase letters)
/// </summary>
/// <remarks>
/// Lower case uses all lowercase letters with spaces between words preserved.
/// Examples: "lower case", "hello world", "the quick brown fox"
/// All alphabetic characters must be lowercase. Spaces, digits, and punctuation are allowed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsLowerCaseAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is in lower case.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is in lower case; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		if (string.IsNullOrEmpty(value))
		{
			return true;
		}

		// All letters must be lowercase
		return value.All(c => !char.IsLetter(c) || char.IsLower(c));
	}
}
