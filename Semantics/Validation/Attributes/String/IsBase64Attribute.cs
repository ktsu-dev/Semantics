// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Text.RegularExpressions;

/// <summary>
/// Validates that the string has proper Base64 format (valid characters and padding).
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed partial class IsBase64Attribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string has valid Base64 format.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>True if the string has valid Base64 format, false otherwise.</returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		if (string.IsNullOrEmpty(value))
		{
			return true;
		}

		// Check length is multiple of 4 (Base64 requirement)
		if (value.Length % 4 != 0)
		{
			return false;
		}

		// Check for valid Base64 characters and proper padding
		return MyRegex().IsMatch(value);
	}

	[GeneratedRegex(@"^[A-Za-z0-9+/]*={0,2}$")]
	private static partial Regex MyRegex();
}
