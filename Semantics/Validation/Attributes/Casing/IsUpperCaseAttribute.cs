// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;

/// <summary>
/// Validates that a string is in UPPER CASE (all uppercase letters)
/// </summary>
/// <remarks>
/// Upper case uses all uppercase letters with spaces between words preserved.
/// Examples: "UPPER CASE", "HELLO WORLD", "THE QUICK BROWN FOX"
/// All alphabetic characters must be uppercase. Spaces, digits, and punctuation are allowed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsUpperCaseAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is in UPPER CASE.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is in UPPER CASE; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		if (string.IsNullOrEmpty(value))
		{
			return true;
		}

		// All letters must be uppercase
		return value.All(c => !char.IsLetter(c) || char.IsUpper(c));
	}
}
