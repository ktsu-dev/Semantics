// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;

/// <summary>
/// Validates that a string is in PascalCase (no spaces, each word starts with uppercase)
/// </summary>
/// <remarks>
/// PascalCase concatenates words without spaces, capitalizing the first letter of each word.
/// Examples: "PascalCase", "HelloWorld", "TheQuickBrownFox"
/// No spaces, underscores, or hyphens are allowed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsPascalCaseAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is in PascalCase.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is in PascalCase; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		if (string.IsNullOrEmpty(value))
		{
			return true;
		}

		// Must start with uppercase letter
		if (!char.IsUpper(value[0]))
		{
			return false;
		}

		// No spaces, underscores, hyphens, or other separators allowed
		if (value.Any(c => char.IsWhiteSpace(c) || c == '_' || c == '-'))
		{
			return false;
		}

		// All characters must be letters or digits
		return value.All(char.IsLetterOrDigit);
	}
}
