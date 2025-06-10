// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Text.RegularExpressions;

/// <summary>
/// Validates that the string matches the specified regex pattern
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class RegexMatchAttribute(string pattern, RegexOptions options = RegexOptions.None) : SemanticStringValidationAttribute
{
	/// <summary>
	/// Gets the regex pattern that the string must match.
	/// </summary>
	public string Pattern => pattern;

	/// <summary>
	/// Gets the regex options used for matching.
	/// </summary>
	public RegexOptions Options => options;

	/// <summary>
	/// Validates that the SemanticString matches the specified regex pattern.
	/// </summary>
	/// <param name="semanticString">The SemanticString to validate</param>
	/// <returns>True if the string matches the pattern, false otherwise</returns>
	/// <inheritdoc/>
	public override bool Validate(ISemanticString semanticString) => Regex.IsMatch(semanticString.WeakString, Pattern, Options);
}
