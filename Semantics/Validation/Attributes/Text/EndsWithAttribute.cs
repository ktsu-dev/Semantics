// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string ends with the specified suffix
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class EndsWithAttribute(string suffix, StringComparison comparison = StringComparison.Ordinal) : SemanticStringValidationAttribute
{
	/// <summary>
	/// Gets the suffix that the string must end with.
	/// </summary>
	public string Suffix => suffix;

	/// <summary>
	/// Gets the comparison type used for matching.
	/// </summary>
	public StringComparison Comparison => comparison;

	/// <summary>
	/// Validates that the SemanticString ends with the specified suffix.
	/// </summary>
	/// <param name="semanticString">The SemanticString to validate</param>
	/// <returns>True if the string ends with the suffix, false otherwise</returns>
	public override bool Validate(ISemanticString semanticString) => semanticString.EndsWith(suffix, comparison);
}
