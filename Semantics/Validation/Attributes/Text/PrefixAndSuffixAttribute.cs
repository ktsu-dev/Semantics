// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string has both the specified prefix and suffix
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class PrefixAndSuffixAttribute(string prefix, string suffix, StringComparison comparison = StringComparison.Ordinal) : SemanticStringValidationAttribute
{
	/// <summary>
	/// Gets the prefix that the string must start with.
	/// </summary>
	public string Prefix => prefix;

	/// <summary>
	/// Gets the suffix that the string must end with.
	/// </summary>
	public string Suffix => suffix;

	/// <summary>
	/// Gets the comparison type used for matching.
	/// </summary>
	public StringComparison Comparison => comparison;

	/// <summary>
	/// Validates that the SemanticString starts with the specified prefix and ends with the specified suffix.
	/// </summary>
	/// <param name="semanticString">The SemanticString to validate</param>
	/// <returns>True if the string starts with the prefix and ends with the suffix, false otherwise</returns>
	public override bool Validate(ISemanticString semanticString) => semanticString.StartsWith(Prefix, Comparison) && semanticString.EndsWith(Suffix, Comparison);
}
