// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;

/// <summary>
/// Validates that a string is in camelCase (first word lowercase, subsequent words start with uppercase)
/// </summary>
/// <remarks>
/// CamelCase concatenates words without spaces, with the first word in lowercase and subsequent words capitalized.
/// Examples: "camelCase", "helloWorld", "theQuickBrownFox"
/// No spaces, underscores, or hyphens are allowed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsCamelCaseAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is in camelCase.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is in camelCase; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		if (string.IsNullOrEmpty(value))
		{
			return true;
		}

		// Must start with lowercase letter
		if (!char.IsLower(value[0]))
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
