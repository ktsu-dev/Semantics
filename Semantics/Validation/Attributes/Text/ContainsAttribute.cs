// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string contains the specified substring
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class ContainsAttribute(string substring, StringComparison comparison = StringComparison.Ordinal) : SemanticStringValidationAttribute
{
	/// <summary>
	/// Gets the substring that the string must contain.
	/// </summary>
	public string Substring => substring;

	/// <summary>
	/// Gets the comparison type used for matching.
	/// </summary>
	public StringComparison Comparison => comparison;

	/// <summary>
	/// Validates that the SemanticString contains the specified substring.
	/// </summary>
	/// <param name="semanticString">The SemanticString to validate</param>
	/// <returns>True if the string contains the substring, false otherwise</returns>
	public override bool Validate(ISemanticString semanticString) => semanticString.Contains(substring, comparison);
}
