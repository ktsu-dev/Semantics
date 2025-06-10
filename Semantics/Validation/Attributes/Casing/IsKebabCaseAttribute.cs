// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;

/// <summary>
/// Validates that a string is in kebab-case (lowercase words separated by hyphens)
/// </summary>
/// <remarks>
/// Kebab-case uses lowercase letters with words separated by hyphens.
/// Examples: "kebab-case", "hello-world", "the-quick-brown-fox"
/// No spaces, underscores, or uppercase letters are allowed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsKebabCaseAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is in kebab-case.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is in kebab-case; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		if (string.IsNullOrEmpty(value))
		{
			return true;
		}

		// Cannot start or end with hyphen
		if (value.StartsWith('-') || value.EndsWith('-'))
		{
			return false;
		}

		// Cannot have consecutive hyphens
		if (value.Contains("--"))
		{
			return false;
		}

		// No spaces, underscores, or other separators allowed (except hyphens)
		if (value.Any(c => char.IsWhiteSpace(c) || c == '_'))
		{
			return false;
		}

		// All characters must be lowercase letters, digits, or hyphens
		return value.All(c => char.IsLower(c) || char.IsDigit(c) || c == '-');
	}
}
