// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;

/// <summary>
/// Validates that a string is in snake_case (lowercase words separated by underscores)
/// </summary>
/// <remarks>
/// Snake_case uses lowercase letters with words separated by underscores.
/// Examples: "snake_case", "hello_world", "the_quick_brown_fox"
/// No spaces, hyphens, or uppercase letters are allowed.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsSnakeCaseAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is in snake_case.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is in snake_case; otherwise, <see langword="false"/>.
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

		// All characters must be lowercase letters, digits, or underscores
		return value.All(c => char.IsLower(c) || char.IsDigit(c) || c == '_');
	}
}
