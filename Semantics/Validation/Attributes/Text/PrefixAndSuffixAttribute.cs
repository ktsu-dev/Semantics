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
	private sealed class PrefixAndSuffixValidator : ValidationAdapter
	{
		private readonly string _prefix;
		private readonly string _suffix;
		private readonly StringComparison _comparison;

		/// <summary>
		/// Initializes a new instance of the PrefixAndSuffixValidator class.
		/// </summary>
		/// <param name="prefix">The prefix that the string must start with</param>
		/// <param name="suffix">The suffix that the string must end with</param>
		/// <param name="comparison">The comparison type</param>
		public PrefixAndSuffixValidator(string prefix, string suffix, StringComparison comparison)
		{
			_prefix = prefix;
			_suffix = suffix;
			_comparison = comparison;

			RuleFor(value => value)
				.Must(value => value?.StartsWith(_prefix, _comparison) == true)
				.WithMessage($"The value must start with '{_prefix}'.");

			RuleFor(value => value)
				.Must(value => value?.EndsWith(_suffix, _comparison) == true)
				.WithMessage($"The value must end with '{_suffix}'.");
		}
	}
}
