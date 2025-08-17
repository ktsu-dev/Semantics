// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string has both the specified prefix and suffix
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class PrefixAndSuffixAttribute(string prefix, string suffix, StringComparison comparison = StringComparison.Ordinal) : NativeSemanticStringValidationAttribute
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
	/// Creates the validation adapter for prefix and suffix validation.
	/// </summary>
	/// <returns>A validation adapter for prefix and suffix validation</returns>
	protected override ValidationAdapter CreateValidator() => new PrefixAndSuffixValidator(prefix, suffix, comparison);

	/// <summary>
	/// validation adapter for prefix and suffix validation.
	/// </summary>
	/// <remarks>
	/// Initializes a new instance of the PrefixAndSuffixValidator class.
	/// </remarks>
	/// <param name="prefix">The prefix that the string must start with</param>
	/// <param name="suffix">The suffix that the string must end with</param>
	/// <param name="comparison">The comparison type</param>
	private sealed class PrefixAndSuffixValidator(string prefix, string suffix, StringComparison comparison) : ValidationAdapter
	{
		private readonly string _prefix = prefix;
		private readonly string _suffix = suffix;
		private readonly StringComparison _comparison = comparison;

		/// <summary>
		/// Validates that a string has both the specified prefix and suffix.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			// Check prefix
			if (!value.StartsWith(_prefix, _comparison))
			{
				return ValidationResult.Failure($"The value must start with '{_prefix}'.");
			}

			// Check suffix
			if (!value.EndsWith(_suffix, _comparison))
			{
				return ValidationResult.Failure($"The value must end with '{_suffix}'.");
			}

			return ValidationResult.Success();
		}
	}
}
