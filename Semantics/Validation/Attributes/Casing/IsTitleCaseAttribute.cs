// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Globalization;

/// <summary>
/// Validates that a string is in title case (each word starts with an uppercase letter)
/// </summary>
/// <remarks>
/// Title case capitalizes the first letter of each word, with the rest of the letters in lowercase.
/// Examples: "This Is Title Case", "Hello World", "The Quick Brown Fox"
/// Whitespace and punctuation are preserved.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsTitleCaseAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is in title case.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is in title case; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		if (string.IsNullOrEmpty(value))
		{
			return true;
		}

		// Use TextInfo.ToTitleCase and compare with original
		string titleCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLowerInvariant());
		return string.Equals(value, titleCase, StringComparison.Ordinal);
	}
}
