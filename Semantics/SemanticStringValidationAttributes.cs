// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Text.RegularExpressions;

/// <summary>
/// Base attribute for semantic string validation
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public abstract class SemanticStringValidationAttribute : Attribute
{
	/// <summary>
	/// Validates a SemanticString against the criteria defined by this attribute.
	/// </summary>
	/// <param name="semanticString">The SemanticString to validate</param>
	/// <returns>True if the string passes validation, false otherwise</returns>
	public abstract bool Validate(ISemanticString semanticString);
}

/// <summary>
/// Specifies that all validation attributes should pass (logical AND)
/// This is the default behavior, but can be used for clarity
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class ValidateAllAttribute : Attribute
{
}

/// <summary>
/// Specifies that any validation attribute can pass (logical OR)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class ValidateAnyAttribute : Attribute
{
}

/// <summary>
/// Validates that the string starts with the specified prefix
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class StartsWithAttribute(string prefix, StringComparison comparison = StringComparison.Ordinal) : SemanticStringValidationAttribute
{
	/// <summary>
	/// Gets the prefix that the string must start with.
	/// </summary>
	public string Prefix => prefix;

	/// <summary>
	/// Gets the comparison type used for matching.
	/// </summary>
	public StringComparison Comparison => comparison;

	/// <summary>
	/// Validates that the SemanticString starts with the specified prefix.
	/// </summary>
	/// <param name="semanticString">The SemanticString to validate</param>
	/// <returns>True if the string starts with the prefix, false otherwise</returns>
	public override bool Validate(ISemanticString semanticString) => semanticString.StartsWith(prefix, comparison);
}

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
	public override bool Validate(ISemanticString semanticString) => Regex.IsMatch(semanticString.ToString(), Pattern, Options);
}

/// <summary>
/// Helper class for validating using attributes
/// </summary>
internal static class AttributeValidation
{
	/// <summary>
	/// Validates a SemanticString against all validation attributes defined on its type.
	/// </summary>
	/// <param name="semanticString">The SemanticString to validate</param>
	/// <param name="type">The type of the SemanticString, which contains the validation attributes</param>
	/// <returns>True if the string passes all validation according to the rules, false otherwise</returns>
	/// <remarks>
	/// Uses the Strategy pattern to determine the appropriate validation approach based on type attributes.
	/// This eliminates code duplication and improves maintainability.
	/// </remarks>
	public static bool ValidateAttributes(ISemanticString semanticString, Type type)
	{
		IValidationStrategy strategy = ValidationStrategyFactory.CreateStrategy(type);
		return strategy.Validate(semanticString, type);
	}
}
