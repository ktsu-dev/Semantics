// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Text.RegularExpressions;

/// <summary>
/// Validates that the string has basic email address format (contains @ with valid characters).
/// For full RFC compliance, use MailAddress.TryCreate() in your application code.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed partial class IsEmailAddressAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string has basic email format.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>True if the string has basic email format, false otherwise.</returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		if (string.IsNullOrEmpty(value))
		{
			return true;
		}

		// Basic email format: localpart@domain with reasonable length limits
		return MyRegex().IsMatch(value) && value.Length <= 254;
	}

	[GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
	private static partial Regex MyRegex();
}
