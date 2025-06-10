// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;

/// <summary>
/// Validates that a string is in MACRO_CASE (uppercase words separated by underscores)
/// </summary>
/// <remarks>
/// MACRO_CASE uses uppercase letters with words separated by underscores.
/// Examples: "MACRO_CASE", "HELLO_WORLD", "THE_QUICK_BROWN_FOX"
/// No spaces, hyphens, or lowercase letters are allowed.
/// Also known as SCREAMING_SNAKE_CASE or CONSTANT_CASE.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsMacroCaseAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is in MACRO_CASE.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is in MACRO_CASE; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		if (string.IsNullOrEmpty(value))
		{
			return true;
		}

		// Cannot start or end with underscore
		if (value.StartsWith('_') || value.EndsWith('_'))
		{
			return false;
		}

		// Cannot have consecutive underscores
		if (value.Contains("__"))
		{
			return false;
		}

		// No spaces, hyphens, or other separators allowed (except underscores)
		if (value.Any(c => char.IsWhiteSpace(c) || c == '-'))
		{
			return false;
		}

		// All characters must be uppercase letters, digits, or underscores
		return value.All(c => char.IsUpper(c) || char.IsDigit(c) || c == '_');
	}
}
